// -- Flag Response -- v2.1 ----------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
//
// *** Requires the Presto Pack v0.93 or later *** http://www.planetstarsiege.com/presto/
// *** Requires Zear's NewOpts v0.966 or later *** http://www.planetstarsiege.com/zear/
//
// This script has messaging that will improve team awareness of each flags status.  It is easy to miss flag
// messages and if you have taken the enemy flag you don't want to be fidling with the chat menu!
//
// If you take the enemy flag the script will say "I have the enemy flag!" and then "I need backup NOW!" for
// you.  The script is intelligent in that it waits a few seconds and then checks that you still have the
// flag.  A lot of grabs end in a drop withing a few seconds because of enemy defence / mines / turrets.
//
// When your flag is taken the script will say for you "<player> has taken our flag!".  To stop the whole team
// spamming the same message the script trys to ensure that only one person will say it.  The script staggers
// the messages by using a random delay.  The scripts then listen for other ppl saying the same thing and stop
// if someone else has said it.  This works most of the time, sometimes two scripts end up with the same
// deplay and they both say it.  However it works well enough to be useful and not a pain in the neck :).
//
// The script will center print all flag events at the bottom of the screen for a few seconds.  The chat box
// is often too busy to be able to see all the flag events so having the info presented elsewhere is very
// useful.  The script gives info of who took the flag etc.
//
// Finally you can get the script to say something when you cap, it's nice to celbrate your own caps :).  The
// 3 default messages are very boring, so edit the list in the file FlagResponseMessages.cs.
//
// Changes in v2.1:
//  + AutoLoad compatible, using a autoloading volume file.
//  + Added option to run off the second message ("I need backup NOW") that you say when you take the flag.
//  + Moved the cap messages into a seperate file to make editing possible.
//
// Changes in v2.0:
//	+ Complete rewrite, had some better ideas.
//	+ Added optional "<player> has taken our flag!" message when your flag is taken.  If other players are
//	  using the same script only one player will say this.
//  + Added second message when you take the flag, will say "I have the enemy flag!" and then "I need backup
//	  NOW!".
//	+ Turned off center prints for observer mode.
//  + NewOpts options menu.
//  + Toggle options on and off with a key in the game.
//
// Changes in v1.3
//  + Cap messages are added with a function and use the method from Presto's say.cs.
//  + Added banner to main screen (Presto Pack v0.93).
//  + Changed preference system to make it slightly easier.
//  + Added flood protect on flag take, can be useful.
//  = Fixed bug where centre prints would stay on screen, Thanks to Shaz for spotting that one :)
//
//
// -- Preferences --
//
// Preferences are now set using a NewOpts page!  Start Tribes then go to Options, Scripts and then select
// Flag Response from the pull down list.  Now you can change the options, get some help and then the options
// are saved so you only need to set all this once.
//

//
// -- Header --
//

$Flag::dir = "";
$Flag::GUIdir = "";

Include("Presto\\Events.cs");
Include("Presto\\TeamTrak.cs");
Include("Presto\\Say.cs");
if(isFile("config\\Crunchy\\Say.cs")) Include("Crunchy\\Say.cs");

// Look for a Schedule.cs
if (isFile("config\\Writer\\Schedule.cs"))
{
	Include("Writer\\Schedule.cs");
}
else
if (isFile("config\\Presto\\Schedule.cs"))
{
	Include("Presto\\Schedule.cs");
}
else
{
	Include($Flag::dir@"Schedule.cs");
}

function Flag::SetDefaults()
{
	$CrunchyPref::CenterPrintFlag = false;
	$CrunchyPref::CenterPrintLen = 5;
	$CrunchyPref::TakenEnemyResponse = true;
	$CrunchyPref::TakenEnemy2nd = true;
	$CrunchyPref::TakenResponse = true;
	$CrunchyPref::FlagResponseWait = 4;
	$CrunchyPref::CapResponse = false;
	$CrunchyPref::CapResponseWait = 2.2;
}
if (isFile("config\\CrunchyPrefs.cs"))
{
	Include("CrunchyPrefs.cs");  // We only want this run once, a script might want to override.
	if ($CrunchyPref::TakenEnemy2nd == "")
	{
		Flag::SetDefaults();
	}
}
else
{
	Flag::SetDefaults();
}

Event::Attach(eventFlagTaken, Flag::Taken);
Event::Attach(eventFlagCaptured, Flag::Captured);
Event::Attach(eventFlagDropped, Flag::Dropped);
Event::Attach(eventFlagReturned, Flag::Returned);
Event::Attach(eventClientMessage, Flag::onClientMessage);

$Flag::Center[taken, friendly]		= "<jc><f2>%p<f1> has taken your flag!";
$Flag::Center[taken, enemy]			= "<jc><f1>%p<f0> has taken the enemy's flag!";
$Flag::Center[captured, friendly]	= "<jc><f1>%p captured your flag!";
$Flag::Center[captured, enemy]		= "<jc><f0>%p captured the enemy's flag!";
$Flag::Center[dropped, friendly]	= "<jc><f2>Your flag was dropped!";
$Flag::Center[dropped, enemy]		= "<jc><f1>The enemy's flag was dropped!";
$Flag::Center[returned, friendly]	= "<jc><f1>Your flag was returned!";
$Flag::Center[returned, enemy]		= "<jc><f0>The enemy's flag was returned!";

$Flag::Response[taken, friendly]	= "%p has taken our flag!";
$Flag::Response[taken, 1, enemy]	= "I have the enemy flag!";
$Flag::Response[taken, 2, enemy]	= "I need backup NOW!";
$Flag::Sound[taken, friendly]		= flagTaken;
$Flag::Sound[taken, 1, enemy]		= flagHave;
$Flag::Sound[taken, 2, enemy]		= yellHelp;

$Flag::FloodProtect = 20;

//
// -- Begin code --
//

// Get the team for use in the message array.
function Flag::GetTeam(%team)
{
	if (%team == Team::Friendly())
	{
		return friendly;
	}
	else
	{
		return enemy;
	}
}

// When the enemy flag is taken say a message, check for flag drop or return.
function Flag::TakenEnemy(%msg) 
{
	%me = getManagerID();
	%location = Team::GetFlagLocation(Team::Enemy());
	if (%location == Client::GetName(%me))
	{
		Say::Team($Flag::Sound[taken, %msg, enemy], $Flag::Response[taken, %msg, enemy]);
	}
}

// Say a message when the friendly flag is taken, check for return or drop.
function Flag::TakenFriendly()
{
	%location = Team::GetFlagLocation(Team::Friendly());
	if (%location != $Trak::locationHome
	&&  %location != $Trak::locationField
	&&  %location != "")
	{
		%text = $Flag::Response[taken, friendly];
		%text = String::Replace(%text, "%p", %location);
		Say::Team($Flag::Sound[taken, friendly], %text);
	}
}

// Celebrate a cap with a message.
function Flag::CapResponse(%msg)
{
	if (%msg == "")
	{	
		%msg = floor($Flag::CapMessageNum * getRandom());
	}
	
	if ($Flag::CapMessage[%msg, who] == public)
	{
		Say::Public($Flag::CapMessage[%msg, say], $Flag::CapMessage[%msg, text]);
	}
	else
	{
		Say::Team($Flag::CapMessage[%msg, say], $Flag::CapMessage[%msg, text]);
	}
}

// Cancel the schedule to say returning a flag if we detect someone else announce it.
function Flag::onClientMessage(%client, %msg)
{
	if (%client != 0)
	{
		if (String::findSubStr(%msg, "has taken our flag") != -1
		||  String::findSubStr(%msg, "eturn our flag") != -1)
		{
			Schedule::Cancel("Flag::TakenFriendly();");
		}
	}
}

// Check what to do when a flag is taken.
function Flag::Taken(%team, %client)
{
	%me = getManagerID();
	if (%client == %me)
	{
		if ($CrunchyPref::TakenEnemyResponse
		&&  Flood::Protect(Flag::TakenEnemy, $Flag::FloodProtect))
		{
		    // Say::Local(sayBye);  // HH only! :)
			Schedule::Add("Flag::TakenEnemy(1);", $CrunchyPref::FlagResponseWait);
			if ($CrunchyPref::TakenEnemy2nd)
			{
				Schedule::Add("Flag::TakenEnemy(2);", $CrunchyPref::FlagResponseWait+3);
			}
		}
	}
	else
	{
		if (%team == Team::Friendly()
		&&  $CrunchyPref::TakenResponse
		&&  Flood::Protect(Flag::TakenFriendly, $Flag::FloodProtect))
		{
			%when = $CrunchyPref::FlagResponseWait * getRandom();
			Schedule::Add("Flag::TakenFriendly();", %when);
		}
		if ($CrunchyPref::CenterPrintFlag && Team::Friendly() != -1)
		{
			%text = $Flag::Center[taken, Flag::GetTeam(%team)];
			%text = String::Replace(%text, "%p", Client::GetName(%client));
			remoteBP(2048, %text, $CrunchyPref::CenterPrintLen);
		}
	}
}

// Check what to do when a flag is captured.
function Flag::Captured(%team, %client)
{
	%me = getManagerID();
	if (%client == %me)
	{
		if ($CrunchyPref::CapResponse
		&&  Flood::Protect(Flag::CapResponse, $Flag::FloodProtect))
		{
			Schedule::Add("Flag::CapResponse();", $CrunchyPref::CapResponseWait);
		}
	}
	else
	if ($CrunchyPref::CenterPrintFlag && Team::Friendly() != -1)
	{
		%text = String::Replace($Flag::Center[captured,Flag::GetTeam(%team)],"%p",Client::GetName(%client));
		remoteBP(2048, %text, $CrunchyPref::CenterPrintLen);
	}
}

// Center print a dropped flag.
function Flag::Dropped(%team, %client)
{
	%me = getManagerID();
	if (%client == %me)
	{
	}
	else
	if ($CrunchyPref::CenterPrintFlag && Team::Friendly() != -1)
	{
		%text = $Flag::Center[dropped, Flag::GetTeam(%team)];
		remoteBP(2048, %text, $CrunchyPref::CenterPrintLen);
	}
}

// Center print a returned flag.
function Flag::Returned(%team, %client)
{
	%me = getManagerID();
	if (%client == %me)
	{
	}
	else
	if ($CrunchyPref::CenterPrintFlag && Team::Friendly() != -1)
	{
		%text = $Flag::Center[returned, Flag::GetTeam(%team)];
		remoteBP(2048, %text, $CrunchyPref::CenterPrintLen);
	}
}

$Flag::CapMessageNum = 0;
function Flag::AddCapMessage(%who, %say, %text)
{
	$Flag::CapMessage[$Flag::CapMessageNum, who] = %who;
	$Flag::CapMessage[$Flag::CapMessageNum, say] = %say;
	$Flag::CapMessage[$Flag::CapMessageNum, text] = %text;
	$Flag::CapMessageNum++; // We've added a cap message.
}

//
// NewOpts
//

$Flag::[centerprint, var]	= "$CrunchyPref::CenterPrintFlag";
$Flag::[cap, var]			= "$CrunchyPref::CapResponse";
$Flag::[taken, var]			= "$CrunchyPref::TakenResponse";
$Flag::[takenenemy, var]	= "$CrunchyPref::TakenEnemyResponse";

$Flag::[centerprint, text]	= " Center print flag events is <f2>%b<f0>.";
$Flag::[cap, text]			= " Celebrate your caps is <f2>%b<f0>.";
$Flag::[taken, text]		= " Tell team when your flag is taken is <f2>%b<f0>.";
$Flag::[takenenemy, text]	= " Tell team when you have the enemy flag is <f2>%b<f0>.";

function Flag::PrintBool(%bool)
{
	if(%bool) return "on";
	else return "off";
}
function Flag::Toggle(%var)
{
	%varname = $Flag::[%var, var];								// Get variable name
	eval(%varname@" = !"@%varname@";");							// Negate variable
	%bool = eval("Flag::PrintBool("@%varname@");");
	%text = String::Replace($Flag::[%var, text], "%b", %bool);	// Insert "on" or "off"
	remoteBP(2048, %text, 5);
}

function Flag::onOpen()
{
}

function Flag::onClose()
{
	export("$CrunchyPref::*", "config\\CrunchyPrefs.cs", false);
}

// Options page for Zear's NewOpts
NewOpts::register("Flag Response",
				  $Flag::GUIdir@"FlagResponseoptions.gui",
				  "Flag::onOpen();",
				  "Flag::onClose();",
				  TRUE);
Include($Flag::dir@"FlagResponseHelp.cs");

// Include the cap messages.
Include("FlagResponseMessages.cs");

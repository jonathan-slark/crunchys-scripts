// -- Welcome message script v2.4.1 -------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Email: jonslark@barrysworld.co.uk
//
// *** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
// Unzip the script into the Tribes/config directory.  Include the Welcome.cs in your autoexec.cs and then 
// use the functions below to add messages for ppl you know.  There is a complete example below to show you 
// how to do this.  If you want to use sounds with your messages refer to the script say.cs in the Presto 
// directory.
//
// This script automatically welcomes people.  Who it welcomes depends on how you set it up, how to do this 
// is described below.
//
// Interface
// ---------
// Use these functions to add your own messages:
//
// Welcome::AddGreet(%name,%tribe,%say[,%text]);
//	%name = Persons name, could be part of their name.  Eg. "Wal" would match "Waldaize".
//	%tribe = Badge of the tribe of the person.
//	%say = Wav file and asociated text using the Say convention from the Presto pack.
//	%text = Optional overiding text.
//
//	Use this function to add the data for the ppl to greet. The tribe badge is optional, if the peron doesn't 
//	have a tribe then use "".  The badge is seperate from the name to allow powerful matching.  For example 
//	in my tribe some ppl use a space between the badge and their name and others don't.  It won't matter if 
//	the person is "|HH| Switch" or "|HH|Switch" or even "Switch|HH|".  The third and fourth arguments use the 
//	Say convention that comes with the Presto pack.
//
// Welcome::AddGreetTextOnly(%name,%tribe,%text);
//	%name = Persons name, could be part of their name.  Eg. "Wal" would match "Waldaize".
//	%tribe = Badge of the tribe of the person.
//	%text = Text message.
//
//	Like AddGreet() but only displays the text and doesn't play a wav file.
//
// Welcome::AddTribeGreet(%tribe,%say[,%text]);
//	%tribe = Badge of the tribe.
//	%say = Wav file and asociated text using the Say convention from the Presto pack.
//	%text = Optional overiding text.
//
//	Similar to AddGreet() this function is for generic messages to anyone from the same tribe.  All of 
//	specific greets using AddGreet() should come before the tribe ones added using AddTribeGreet().  This is  
//	to ensure that the people that have specific greets always get those in preference to the generic tribe 
//	ones.  Refer to the example below.
//
// Welcome::AddTribeGreetTextOnly(%tribe,%text);
//	%tribe = Badge of the tribe.
//	%text = Text message.
//
//	Like AddTribeGreet() but only displays the text and doesn't play a wav file.
//
//
// Example
// -------
// Add something like this to your autoexec.cs (without the //) after the line for Welcome.cs.
//
// -- Start Example --
// Include("Welcome.cs");
//
// Welcome::AddGreet("Rico", "|HH|", sayHi);
// Welcome::AddGreetTextOnly("Wal", "|HH|", "Hiya Wal!");
//
// Welcome::AddTribeGreet("|HH|", yellAllRight, "Head Hunters are in da House!");
// Welcome::AddTribeGreetTextOnly("[McM]", "Take cover! It's McMental!");
// -- End Example --
//
// In this example, specific ppl like Rico have personalized messages.  If someone from |HH| tribe doesn't 
// match one of the specific messages they will get a generic message.
//
//
// Changes in v2.4.1:
//	+ Reformatted v2.4 so that it is consistent with my other scripts.
//  + Used correct way to check that the client message was from yourself.
//
// Changes in v2.4:
//	+ You can enable the script to send a message when you join a team. Thanks GrymReaper for the idea :)
//
// Changes in v2.3:
//	+ Rewritten to use the Presto script pack.
//	+ The script will co-exist, with no editing, with other Presto pack scripts.
//	+ Uses the Say script from the Presto pack for wav files and ascociated text.
//	+ Interface split into four functions.
//	+ Various improvements to the code.
//
// Changes in v2.2:
//	+ Added random delay to messages.  This staggers greets to the same person made by different ppl.
//
// Changes in v2.1:
//	- Script disabled when the server is in Tournament mode.
//	+ You can use the function welcomeAddGreet() to add the data.
//	+ Script is now stand-alone, all data is passed to script with the welcomeAddGreet function.
//
// Changes in v2.0:
//	+ Complete rewrite.
//	+ Messages are no longer hard coded, messages stored in easy to edit tables.
//	- Person or tribe is greeted only once and not everytime they change teams.
//
//
// -- Preferences
//
$CrunchyPref::WelcomeDebug = false;
// Debug mode?  Usually you won't want to have the script greet yourself, for testing this is handy.  Set 
// this to true if you want to enable the debug version.  Also prints debuging information and will greet 
// someone on every team change.
//
$CrunchyPref::WelcomeSelf = false;
// Normally the script won't send any messages when you yourself join a team.  If you want to announce your 
// coming then set this preference :)  Unlike the debug mode, in this mode the script will not greet ppl on 
// every team change, only the first connection.


//
// -- Header --
//

Include("Presto\\Events.cs");
Include("Presto\\Match.cs");
Include("Presto\\Say.cs");

Event::Attach(eventClientChangeTeam, Welcome::ParseClient, WelcomeParse);

// Used to add data into the tables.
function Welcome::AddGreet(%name,%tribe,%say,%text)
{
	// Data tables
	$Welcome::Name[$Welcome::Num] = %name;
	$Welcome::Tribe[$Welcome::Num] = %tribe;
	$Welcome::Say[$Welcome::Num] = %say;
	$Welcome::Text[$Welcome::Num] = %text;

	// Initialise status
	$Welcome::Status[$Welcome::Num] = false;

	$Welcome::Num++;
	if($CrunchyPref::WelcomeDebug)
		echo("Messages added: " @ $Welcome::Num);
}

function Welcome::AddTribeGreet(%tribe,%say,%text)
{
	Welcome::AddGreet("",%tribe,%say,%text);
}

function Welcome::AddTribeGreetTextOnly(%tribe,%text)
{
	Welcome::AddGreet("",%tribe,"",%text);
}

function Welcome::AddGreetTextOnly(%name,%tribe,%text)
{
	Welcome::AddGreet(%name,%tribe,"",%text);
}

//
// -- Begin code --
//

$Welcome::Num = 0; // Counter used in adding data, after all data is added it equals number of messages.
$Welcome::MaxTime = 4; // Max number of seconds to delay message up to.

// Welcome::Greet looks up ppl and greets them.
function Welcome::Greet(%fullname,%index)
{
	// Check to see if this is the person.  Checks name and badge.
	// Empty names or badges are considered to be a match.
	if($Welcome::Name[%index]=="")
		%found = true;
	else
		%found = Match::String(%fullname, "*" @ $Welcome::Name[%index] @ "*");
	if($Welcome::Tribe[%index] != "")
		%found = %found && Match::String(%fullname, "*" @ $Welcome::Tribe[%index] @ "*");

	// If we greeted this person before, stop here.
	if($Welcome::Status[%index] && !$CrunchyPref::WelcomeDebug)
		return %found;

	if(%found)  // Only greet the person if its one we are looking for.
	{
		// Offset message by random time to stagger scripts greeting the same person.
		%timedelay = getRandom() * $Welcome::MaxTime;
		if($CrunchyPref::WelcomeDebug)
			echo("Waiting " @ %timedelay @ " seconds.");

		// Schedule the text message with delay set.
		if($Welcome::Say[%index]=="")
			schedule("say(0, \"" @ $Welcome::Text[%index] @ "\");", %timedelay);
		else if($Welcome::Text[%index]=="")
			schedule("Say::Public(" @ $Welcome::Say[%index] @ ");", %timedelay);
		else
			schedule("Say::Public(" @ $Welcome::Say[%index] @ ", \"" @ $Welcome::Text[%index] @ "\");", %timedelay);

		$Welcome::Status[%index] = true;  // Remeber we have greeted this person.
	}

	return %found;  // We're done.
}

// This function is automatically run whenever someone changes teams, it will then lookup ppl and greet them.
function Welcome::ParseClient(%client, %team)
{
	// If server is in tournament mode, don't bother.
	// In this mode everyone joins a team at the same time, greeting everyone is awful :)
	if($Server::TourneyMode)
		return;

	if(%client == getManagerID() && !$CrunchyPref::WelcomeDebug && !$CrunchyPref::WelcomeSelf)  // If its me, don't bother.
		return;

	// Look for everyone on the list, if found greet em.
	%name = Client::getName(%client);  // Lookup name of client.
	for(%loop=0; %loop < $Welcome::Num; %loop++)
		if(Welcome::Greet(%name,%loop)) return;
}

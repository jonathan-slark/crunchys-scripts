// -- Objective HUD -- v2.7 -----------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
//
// *** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
// *** Requires Zear's NewOpts v0.966 or later *** http://www.planetstarsiege.com/zear/
// *** Requires Writer's Support Pack v4.0.4 BETA or later *** http://www.planetstarsiege.com/lorne/
// *** Requires my ObjectiveTrak script v3.3 or later ***
//
// Objective HUD displays information about the objectives in the current mission.  It is similar to the
// objectives screen but it is in a compact HUD that stays on your screen at all times.  You can glance at the
// HUD and find out everything you need to know, without having to stop and bring up the objectives screen.
// The HUD has more features than the objectives screen, such as flag counters.
//
// The script gets it's information about the objectives in various ways.  You can run ObjectiveExtract.exe
// in your Tribes\base\missions\ directrory which makes info files the script can use.  However if you play on
// a mission you don't have the script learns about the mission and then saves what it knows about the mission
// when the map changes.  The next time you play that mission the HUD loads the data and away you go.  If you
// play on a BWAdmin mod server the script takes advantages of it's script support.  The mod lets the script
// get access to all the information about the objectives.  This means the mod will provide the HUD with info
// about all the objectives and their exact status.
//
// CTF
// (Redisigned in v2.5)
// Your flag is represented by a green flag and enemy flags are red.  The center square shows the status of
// the flag.  If the square is black then the flag is at its base.   When someone takes the flag the center
// colour relects which team has the flag, if it's green your team has the flag, if it's red an enemy team has
// that flag.  If the box is yellow then the status of that flag is unknown, this happens when you join a
// server and the match has already started.
//
// The text next to the icon is the text for that flag, "Your flag" if it's your flag, "Enemy flag" otherwise.
// There is the option to change this text to the team name.  Your flag is always the first flag in the list
// of flags, no matter what team you are on.
//
// If a flag is being carried the text changes to show which player has the flag.  If the flag is dropped the
// text changes to "Dropped! (48)".  The 48 is a countdown in seconds till the flag is returned.
//
// C&H
// These objectives have a switch symbol: red means enemy controled, green means your team has control and
// black means neutral (ie start of the game).  If you join mid-game then the script won't know the status of
// the objectives, which is indicated by a yellow colour.
//
// D&D
// These objectives appear as generator symbols.  Red means destroyed, green means safe.  Again yellow means
// unknown.  Both the enemy and friendly objectives start out green and will turn red once destroyed. If you
// set the abbreviate preference to true a green player symbol next to the status symbol means it is one of
// your objectives, defend it!  If the player symbol is red, this objective should be destroyed!
//
// New in v2.5:
// There is an option to use more specific icons for the objectives, instead of the generic generator symbol.
// The icons show you the difference between generators, radars, turrets and stations.
//
// F&R
// When a flag is at a stand it has a grey background, if a player is carrying it, it will have a yellow
// background. The center square indicates the location of the flag.  A black square means it is at it's
// initial position.  A flag at your base has a green square and any flags the enemy has have a red square.
// When the flag is being carried the square reflects the team of the carrier, green for friendly, red for
// enemy.
//
// New in v2.5:
// The text is normally the name of the flag.  When a player is carrying the flag it changes to the players
// name.  There is also a flag counter for when the flags are taken, eg: North Flag/129.
//
//
// Changes in v2.7:
//  + Optional timer showing how long someone has had the flag.  Useful for deciding when a flag stand off has
//    developed.
//  + Now uses Writer's support pack.
//
// Changes in v2.6:
//  + Added option to abbreviate team names.
//  + Added option to change the dropped flag value, useful for modded servers.
//  + Now gets the F&R flag timer value from the server messages, was hard coded before.
//
// Changes in v2.5:
//  + Autoloading volume file.
//	+ Makes use of extensions in ObjectiveTrak v3.0.
//  + Added a dropped flag countdown which starts blinking when within 10 secs of flag return.
//	+ Redesigned the CTF flag icons to be clearer and more consistent.
//	+ Your flag always appears on a line before the enemy flag now, making it easier to distinguish the
//    flags.
//  + Added option to display team names instead of "Your flag" and "Enemy flag".
//  + F&R flag timer.
//  + Displays player name when an F&R flag is being carried.
//	+ Presto showed me how to get the control characters to display properly :).  So "<oep>Nachoking" will
//	  now display properly as "<oep>Nachoking".  Without the fix it would look like "Nachoking".
//	+ The lines clip neatly at the edge, again thanks to Presto for the solution :).
//	+ You can now bind a key to reset the objectives, use it when you join mid-game and know the objectives
//	  are neutral.  It's impossible for the script to find this out for itself.
//  + Option to use specific icons for the different types of D&D objective.
//  + Works with Balanced and Open Call missions properly.
//  + Works with Multiple Team missions.
//  + Works 100% with HUD mover.
//  = Now uses Schedule.cs for safe scheduling.
//
// Changes in v2.4:
//  + HUDMover friendly (you CAN teach an old dog new tricks :).
//  + Rewritten update routine so that only the lines that need to be updated are updated.  This should help
//	  with CPU lag, especially with lots of objectives.
//	+ If ppl have control characters in their name this could mess up the display, these are now replaced
//	  with valid ones.  Eg: "<oep>Nachoking" becomes "[oep]Nachoking".  The name is also stripped of any
//	  leading or trailing spaces by using String::trim from NewOpts.
//  = Various bug fixes and changes, esp regarding events.
//
// Changes in v2.3.2:
//	+ Added eventObjectiveHUDResize (used by TeamScoreHUD).
//  = When observing, CTF flags are displayed with the team name (if known) instead of "Enemy flag" (change
//	  in ObjectiveTrak).
//  = Changed dropped flag icon to be clearer and more consistent with the other icons.
//
// Changes in v2.3.1:
//	= Fixed bug where options screen would mess up if used in a resolution other than 640x480.
//
// Changes in v2.3:
//	+ Added options page for Zear's NewOpts!  Thanks to Zear for NewOpts :)
//  + Saves and loads last preferences, so you set 'em once and they stick :)
//  + Options can be set on the fly and the HUD updates properly.
//  + Solved the "messed up HUD" bug by puting each line into a SimGui::Control so that they are clipped
//	  properly, Thanks to Cowboy for the idea!  Thanks to Zear for his gui docs!
//	= Moved location of the bmps to their own directory, if you installed a previous version you can remove
//	  the bmps in the Tribes\Crunchy dir, they are now in the bmp dir.
//
// Changes in v2.2.1:
//  = Client message parser didn't check to see if the message was from the server. D'oh :)
//    (this is actually a change in ObjectiveTrak but it's important it was fixed)
//
// Changes in v2.2:
//	+ Added optional CTF flag support!
//  = Fixed minor bug in blinking code (the code to make the names blink, didn't mean I was fed up with it :)
//  = Stopped two consecutive updates to same objective making the blinking schedules clash.
//
// Changes in v2.1.1:
//	= Fixed bug where HUD wouldn't be updated if no objectives loaded.
//
// Changes in v2.1:
//  + Overcome 8 bitmap limit by having a seperate object per line.  Thanks Cowboy for the suggestion!
//  + Added preference for user to decide if the HUD should always come on, or only when toggled with a key,
//	  or when there are objectives on a mission.
//  + Added option to abbreviate objective names and thus make the HUD _a lot_ smaller.
//  + Objectives blink for a few seconds when they have been updated.
//  + Preferences can be put in a seperate file and they will override preferences in this file.
//  = Some changes on what functions were attached to events and where.
//
// Changes in v2.0.1:
//	+ HUD comes on automatically at start of any mission type.  The HUD is useful for most missions.
//
// What no v1.0?
//  - Version 1 worked with ObjectiveTrak v1.0 which only tracked C&H objectives.  I scrapped this version as
//    I wanted to generalise the script to track any objectives.
//
//
// -- Preferences --
//
// Preferences are now set using a NewOpts page!  Start Tribes then go to Options, Scripts and then select
// Objective HUD from the pull down list.  Now you can change the options, get some help and then the options
// are saved so you only need to set all this once.
//

//
// -- Header --
//

$ObjectiveHUD::dir = "";
$ObjectiveHUD::BMPdir = "";
$ObjectiveHUD::GUIdir = "";

if($Presto::version >= 0.93)
{

include("support.acs.cs");
include("writer\\schedule.cs");
include("writer\\Event.cs");

include("Presto\\HUD.cs");
include("Presto\\TeamTrak.cs");
include("Crunchy\\ObjectiveTrak.cs");
include("Crunchy\\RegisterPref.cs");

Event::Attach(eventClientChangeTeam, ObjectiveHUD::onChangeTeam);
Event::Attach(eventObjectiveNew, ObjectiveHUD::onNew);
Event::Attach(eventObjectiveUpdated, ObjectiveHUD::onUpdate);
Event::Attach(eventObjectiveLoaded, ObjectiveHUD::onLoaded);
Event::Attach(eventObjectiveMadeNeutral, "ObjectiveHUD::UpdateAll();");

Include("CrunchyPrefs.cs");
RegisterPref("$CrunchyPref::ObjectiveHUDDisplay", true);
RegisterPref("$CrunchyPref::ObjectiveAbbreviate", true);
RegisterPref("$CrunchyPref::ObjectiveAbbreviateTeamNames", true);
RegisterPref("$CrunchyPref::ObjectiveCTF", true);
RegisterPref("$CrunchyPref::ObjectiveHUDPos", "100% 30% 126 14");
RegisterPref("$CrunchyPref::ObjectiveTeamNames", false);
RegisterPref("$CrunchyPref::ObjectiveNewDandDIcons", true);
RegisterPref("$CrunchyPref::ObjectiveDroppedTimer", 48);
RegisterPref("$CrunchyPref::ObjectiveCarryTimer", false);
//RegisterPref("$CrunchyPref::ObjectiveHUDSort", ifNew);

// Hand drawn and hand anti-aliased BMPs (you try and make pictures this small look cool ;)
// If ppl are using massive resolutions I may need to make these clearer?
// |HH|Rico assures me they are ok :)

// C&H
$ObjectiveHUD::[$Objective::Friendly,
				$Objective::CandH] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"candh_tower_friendly";

$ObjectiveHUD::[$Objective::Enemy,
				$Objective::CandH] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"candh_tower_enemy";

$ObjectiveHUD::[$Objective::Unknown,
				$Objective::CandH] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"candh_tower_unknown";

$ObjectiveHUD::[$Objective::Neutral,
				$Objective::CandH] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"candh_tower_neutral";

// D&D
$ObjectiveHUD::[$Objective::Neutral,
				$Objective::DandD] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"dandd_objective_safe";

$ObjectiveHUD::[$Objective::Destroyed,
				$Objective::DandD] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"dandd_objective_destroyed";

$ObjectiveHUD::[$Objective::Unknown,
				$Objective::DandD] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"dandd_objective_unknown";

$ObjectiveHUD::[$Objective::Neutral,
				$Objective::Generator] =
				"B2,4:M_generator_green";

$ObjectiveHUD::[$Objective::Destroyed,
				$Objective::Generator] =
				"B2,4:M_generator_red";

$ObjectiveHUD::[$Objective::Unknown,
				$Objective::Generator] =
				"B2,4:M_generator_green";

$ObjectiveHUD::[$Objective::Neutral,
				$Objective::PlasmaTurret] =
				"B0,4:M_turret_green";

$ObjectiveHUD::[$Objective::Destroyed,
				$Objective::PlasmaTurret] =
				"B0,4:M_turret_red";

$ObjectiveHUD::[$Objective::Unknown,
				$Objective::PlasmaTurret] =
				"B0,4:M_turret_green";

$ObjectiveHUD::[$Objective::Neutral,
				$Objective::CommandStation] =
				"B1,4:M_station_green";

$ObjectiveHUD::[$Objective::Destroyed,
				$Objective::CommandStation] =
				"B1,4:M_station_red";

$ObjectiveHUD::[$Objective::Unknown,
				$Objective::CommandStation] =
				"B1,4:M_station_green";

$ObjectiveHUD::[$Objective::Neutral,
				$Objective::PulseSensor] =
				"B3,4:M_radar_green";

$ObjectiveHUD::[$Objective::Destroyed,
				$Objective::PulseSensor] =
				"B3,4:M_radar_red";

$ObjectiveHUD::[$Objective::Unknown,
				$Objective::PulseSensor] =
				"B3,4:M_radar_green";

// CTF
$ObjectiveHUD::[$Objective::FriendlyNeutral,
				$Objective::CTF] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"ctf_flag_friendlyNeutral";

$ObjectiveHUD::[$Objective::EnemyNeutral,
				$Objective::CTF] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"ctf_flag_enemyNeutral";

$ObjectiveHUD::[$Objective::EnemyCarry,
				$Objective::CTF] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"ctf_flag_enemyCarry";

$ObjectiveHUD::[$Objective::FriendlyCarry,
				$Objective::CTF] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"ctf_flag_friendlyCarry";

$ObjectiveHUD::[$Objective::FriendlyDropped,
				$Objective::CTF] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"ctf_flag_friendlyUnknown";

$ObjectiveHUD::[$Objective::EnemyDropped,
				$Objective::CTF] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"ctf_flag_enemyUnknown";

$ObjectiveHUD::[$Objective::FriendlyUnknown,
				$Objective::CTF] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"ctf_flag_friendlyUnknown";

$ObjectiveHUD::[$Objective::EnemyUnknown,
				$Objective::CTF] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"ctf_flag_enemyUnknown";

// F&R
$ObjectiveHUD::[$Objective::Friendly,
				$Objective::FandR] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"fandr_flag_friendly";

$ObjectiveHUD::[$Objective::Enemy,
				$Objective::FandR] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"fandr_flag_enemy";

$ObjectiveHUD::[$Objective::FriendlyCarry,
				$Objective::FandR] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"fandr_flag_friendlyCarry";

$ObjectiveHUD::[$Objective::EnemyCarry,
				$Objective::FandR] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"fandr_flag_enemyCarry";

$ObjectiveHUD::[$Objective::Dropped,
				$Objective::FandR] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"fandr_flag_dropped";

$ObjectiveHUD::[$Objective::Unknown,
				$Objective::FandR] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"fandr_flag_unknown";

$ObjectiveHUD::[$Objective::Neutral,
				$Objective::FandR] =
				"B0,4:"@$ObjectiveHUD::BMPdir@"fandr_flag_neutral";

function ObjectiveHUD::AddBanner()
{
	Presto::AddScriptBanner(ObjectiveHUD,
		" <f2>Objective HUD <jr><f0>version 2.7 <jl>\n" @
		" <f0>Displays mission objectives.\n" @
		" \n" @
		" <f1>New:<f0>	Flag carry timer.\n" @
		" <f1>New:<f0>	Dropped flag timer.\n" @
		" <f1>New:<f0>	Better F&R support.\n" @
		" \n" @
		" <f0>Written by: <f1>|HH|Crunchy\n" @
		" <f1>crunchy@planetstarsiege.com");
}
ObjectiveHUD::AddBanner();

//
// -- Begin code --
//

// Taken from Presto's TextEdit.cs
function FilterFormattedText(%text)
{
	%nextFmt = String::FindSubStr(%text, "<");
	while (%nextFmt != -1)
	{
		%newtext = %newtext @ String::GetSubStr(%text, 0, %nextFmt) @ "<<<>";

		%text = String::GetSubStr(%text, %nextFmt+1, 255);
		%nextFmt = String::FindSubStr(%text, "<");
	}

	return %newtext@%text;
}

// Convert the extended type into groups of types.
function ObjectiveHUD::GetDandDType(%id)
{
	%type = Objective::GetTypeExt(%id);
	if (%type == $Objective::Generator
	||  %type == $Objective::PortableGenerator
	||  %type == $Objective::SolarPanel)
	{
		return $Objective::Generator;
	}
	else
	if (%type == $Objective::PlasmaTurret
	||  %type == $Objective::RocketTurret)
	{
		return $Objective::PlasmaTurret;
	}
	if (%type == $Objective::PulseSensor
	||  %type == $Objective::MediumPulseSensor)
	{
		return $Objective::PulseSensor;
	}
	else
	{
		return %type;
	}
}

// Update one line of the HUD.
function ObjectiveHUD::UpdateLine(%id)
{
	%type = Objective::GetType(%id);
	// DeMorgan's law used to simplify:
	// !(%type == $Objective::CTF && !$CrunchyPref::ObjectiveCTF)
	if (%type != $Objective::CTF || $CrunchyPref::ObjectiveCTF)
	{
		%status = Objective::GetStatusExt(%id);

		if (%type == $Objective::DandD && $CrunchyPref::ObjectiveNewDandDIcons)
		{
			%bmp = $ObjectiveHUD::[%status, ObjectiveHUD::GetDandDType(%id)];
		}
		else
		{
			%bmp = $ObjectiveHUD::[%status, %type];
		}

		if ($ObjectiveHUD::BlinkOn[%id])
		{
			%colour = "<f2>";
		}
		else
		{
			%colour = "<f1>";
		}

		if (%type == $Objective::CTF)
		{
			// If a flag is carried show the player's name rather than the flag's name.
			if (%status == $Objective::FriendlyCarry || %status == $Objective::EnemyCarry)
			{
 				%text = FilterFormattedText(Client::GetName(Objective::GetClient(%id)));

 				if ($CrunchyPref::ObjectiveCarryTimer)
 				{
	 				%time = floor(getSimTime()-Objective::GetTime(%id));
 					%text = %text@"/"@%time;
 				}
 			}
			else
			// Dropped flag timer.
			if (%status == $Objective::FriendlyDropped || %status == $Objective::EnemyDropped)
			{
				%time = $CrunchyPref::ObjectiveDroppedTimer-floor(getSimTime()-Objective::GetTime(%id));
				%text = "Dropped! ("@%time@"s)";
			}
			else
			// Show team names if the option is set or we are observing.
			if (Team::Friendly() == -1 || $CrunchyPref::ObjectiveTeamNames)
			{
				%o = Objective::GetName(%id);
				%team = String::getSubStr(%o, 1, 1);

				if ($CrunchyPref::ObjectiveAbbreviateTeamNames)
				{
					%text = Objective::GetTeamNameAbbreviated(%team, 1);
				}
				else
				{
					%text = Team::GetName(%team);
				}
			}
			// Show flag name.
			else
			{
				%o = Objective::GetName(%id);
				%text = Objective::InsertTeamName(%o);
			}
		}
		else
		// Show name for a carried F&R flag.
		if (%type == $Objective::FandR
		&& (%status == $Objective::FriendlyCarry || %status == $Objective::EnemyCarry))
		{
			%text = FilterFormattedText(Client::GetName(Objective::GetClient(%id)));
		}
		else
		// If the option is set show abbrevieated objective name.
		if ($CrunchyPref::ObjectiveAbbreviate)
		{
			%text = Objective::GetNameAbbreviated(%id);
			if (%type == $Objective::DandD)
			{
				%text = Objective::InsertTeamSymbol(%text);
			}
		}
		// Show stripped name.
		else
		{
			%text = Objective::GetNameStripped(%id);
			if (%type == $Objective::DandD)
			{
				%text = Objective::InsertTeamName(%text);
			}
		}

		// Add F&R flag timer to text line.
		if (%type == $Objective::FandR
		&& (%status == $Objective::FriendlyCarry || %status == $Objective::EnemyCarry
		    || %status == $Objective::Dropped))
		{
			%time = Objective::GetFandRTimer()-floor(getSimTime()-$ObjectiveHUD::TimeTaken[%id]);
			%text= %text@"/"@%time;
		}

		// Set the objective line's text.
		Control::SetValue(Object::GetName($ObjectiveHUD::Line[$ObjectiveHUD::LookupLine[%id]]),
						  %colour@"<"@%bmp@".bmp><L4>"@%text);
	}
}

// Not yet implemented.
function ObjectiveHUD::getSort()
{
	if ((%sort = $CrunchyPref::ObjectiveHUDSort) == ifNew)
	{
		if($Objective::New)
		{
			%sort = byAlpha;
		}
		else %sort = unsorted;
	}
	return %sort;
}

// Find out the number of non-CTF objectives.
function ObjectiveHUD::nonCTFNumber()
{
	return Objective::Number() - Objective::GetTypeNum($Objective::CTF);
}

// Update the HUD using ObjectiveTrak.
function ObjectiveHUD::UpdateAll()
{
	%num = Objective::Number();
	// Check to see if there are any objective to display.
	if (%num == 0 || !($CrunchyPref::ObjectiveCTF || ObjectiveHUD::nonCTFNumber()))
	{
		Control::SetValue(Object::GetName($ObjectiveHUD::Line[0]), "<jl><f1>No objectives");
	}
	// If there are then update the whole HUD.
	else
	{
		for (%id = 0; %id < %num; %id++)
		{
			ObjectiveHUD::UpdateLine(%id);
		}
	}
}

// If we find out about new objectives resize the HUD accordingly.
function ObjectiveHUD::Resize()
{
	// Get the number of lines to display.
	if ($CrunchyPref::ObjectiveCTF)
	{
		%numLines = Objective::Number();
	}
	else
	{
		%numLines = ObjectiveHUD::nonCTFNumber();
	}

	// We must have at least one line.
	if (%numLines < 1)
	{
		%numLines = 1;
	}
	%lines = HUD::GetGuiObjectCount(ObjectiveHUD); // Number of existing lines.
	%perLine = getWord($CrunchyPref::ObjectiveHUDPos, 3); // Height per line.

	// If we don't already have objects for the lines, make em.
	for (%i = %lines; %i < %numLines; %i++)
	{
		$ObjectiveHUD::Frame[%i] =
			HUD::AddObject(ObjectiveHUD,SimGui::Control,4,%i*%perLine-3,2000,%perLine+2);
		$ObjectiveHUD::Line[%i] =
			newobject("ObjectiveHUD::_Line"@%i,FearGuiFormattedText,0,0,2000,%perLine+2);
		addToSet($ObjectiveHUD::Frame[%i], Object::GetName($ObjectiveHUD::Line[%i]));
	}

	// Resize the HUD.
	HUD::SetCoord(ObjectiveHUD, height, %numLines*%perLine);
	ObjectiveHUD::AssignRows();
	ObjectiveHUD::UpdateAll();

	// Let other scripts know the HUD has been resized.
	Event::Trigger(eventObjectiveHUDResize);
}

// When the HUD is updated make sure it's the right size.
// This is mainly a work around for HUD Mover.
function ObjectiveHUD::onUpdateHUD(%hud)
{
	// Resize and update.
	ObjectiveHUD::Resize();

	// Only update when explicitly asked to.
	return 0;
}

// If we change teams HUD needs updating.
function ObjectiveHUD::onChangeTeam(%client, %team)
{
	if(%client == getManagerID())
	{
		ObjectiveHUD::AssignRows();
		ObjectiveHUD::UpdateAll();
	}
}

// If objectives were loaded or new ones found about resize the HUD.
// Also check to see if we should display the HUD.
function ObjectiveHUD::onLoaded(%num)
{
	ObjectiveHUD::Reset();
	ObjectiveHUD::Resize();

	if(((!$CrunchyPref::ObjectiveCTF && ObjectiveHUD::nonCTFNumber() == 0) || %num == 0)
	&&  $CrunchyPref::ObjectiveHUDDisplay == onLoaded)
	{
		HUD::Display(ObjectiveHUD, false);
	}
	else
	if($CrunchyPref::ObjectiveHUDDisplay == onLoaded)
	{
		HUD::Display(ObjectiveHUD, true);
	}
	else
	{
		HUD::Display(ObjectiveHUD, $CrunchyPref::ObjectiveHUDDisplay);
	}
}

// Work out which lines the objective id's are associated with.
// If we aren't displaying CTF flags this doens't match the objective list,
// plus we could change the order if we want.  When displaying CTF flags the
// friendly flag is moved to the top of the list.
function ObjectiveHUD::AssignRows()
{
	%friendlyFlag = "%"@Team::Friendly()@" flag";
	%enemy = 0;
	%row=0;
	for (%id = 0; %id < Objective::Number(); %id++)
	{
		if (Objective::GetType(%id) == $Objective::CTF)
		{
			if ($CrunchyPref::ObjectiveCTF)
			{
				if(Objective::GetName(%id) == %friendlyFlag)
				{
					$ObjectiveHUD::LookupLine[%id] = %row;
					%friendly = true;
					%row++;
					%totalEnemy = %enemy;
					for (%i = 0; %i < %totalEnemy; %i++)
					{
						$ObjectiveHUD::LookupLine[%enemyFlag[%i]] = %row;
						%row++;
						%enemy--;
					}
				}
				else
				{
					if (%friendly || Team::Friendly() == -1)
					{
						$ObjectiveHUD::LookupLine[%id] = %row;
						%row++;
					}
					else
					{
						%enemyFlag[%enemy] = %id;
						%enemy++;
					}
				}
			}
		}
		else
		{
			$ObjectiveHUD::LookupLine[%id] = %row;
			%row++;
		}
	}

	%totalEnemy = %enemy;
	// Assign any stored enemy flags left over.
	for (%i = 0; %i < %totalEnemy; %i++)
	{
		$ObjectiveHUD::LookupLine[%enemyFlag[%i]] = %row;
		%row++;
		%enemy--;
	}
}

// When an objective has been found out about, update HUD plus blink the name.
function ObjectiveHUD::onNew(%client, %id, %status, %type)
{
	if (%type == $Objective::CTF && !$CrunchyPref::ObjectiveCTF) return;

	ObjectiveHUD::onLoaded();
	ObjectiveHUD::onUpdate(%client, %id, %status, %type);
}

// When an objective has been updated, update HUD plus blink the name.
function ObjectiveHUD::onUpdate(%client, %id, %status, %type)
{
	// Don't bother if it's a CTF flag and we are displaying those.
	if (%type == $Objective::CTF && !$CrunchyPref::ObjectiveCTF) return;

	if (%type == $Objective::CTF)
	{
		// Carried flag.
		if ((%status == $Objective::FriendlyCarry || %status == $Objective::EnemyCarry)
		&&  $CrunchyPref::ObjectiveCarryTimer)
		{
			$ObjectiveHUD::BlinkOn[%id] = false;
			ObjectiveHUD::UpdateLine(%id);

			%tag = "ObjectiveHUD::Carry_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Add("ObjectiveHUD::Carry("@%id@",0);", 0.3, %tag);
			%tag = "ObjectiveHUD::Dropped_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Cancel(%tag);
			%tag = "ObjectiveHUD::Blink_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Cancel(%tag);
			$ObjectiveHUD::Blinking = false;
			return;
		}
		else
		// Setup dropped flag timer.
		if (%status == $Objective::Dropped)
		{
			$ObjectiveHUD::BlinkOn[%id] = false;
			ObjectiveHUD::UpdateLine(%id);

			// Calculate how many times the schedule will be called.
			$ObjectiveHUD::DroppedMaxIndex = floor($CrunchyPref::ObjectiveDroppedTimer / 0.3);

			%tag = "ObjectiveHUD::Dropped_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Add("ObjectiveHUD::Dropped("@%id@",0);", 0.3, %tag);
			%tag = "ObjectiveHUD::Blink_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Cancel(%tag);
			%tag = "ObjectiveHUD::Carry_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Cancel(%tag);
			$ObjectiveHUD::Blinking = false;
			return;
		}
		else
		{
			%tag = "ObjectiveHUD::Carry_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Cancel(%tag);
			%tag = "ObjectiveHUD::Dropped_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Cancel(%tag);
		}
	}
	else
	if (%type == $Objective::FandR)
	{
		// Setup F&R flag timer.
		if (%status == $Objective::FriendlyCarry || %status == $Objective::EnemyCarry)
		{
			// Update the time if we took the flag from a stand.
			%laststatus = Objective::GetLastStatus(%id);
			if (%laststatus != $Objective::Dropped)
			{
				$ObjectiveHUD::TimeTaken[%id] = Objective::GetTime(%id);
			}

			$ObjectiveHUD::BlinkOn[%id] = false;
			ObjectiveHUD::UpdateLine(%id);

			// Calculate how many times the schedule will be called.
			$ObjectiveHUD::TakenMaxIndex = floor(Objective::GetFandRTimer() / 0.3);

			%tag = "ObjectiveHUD::Taken_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Add("ObjectiveHUD::Taken("@%id@",0);", 0.3, %tag);
			%tag = "ObjectiveHUD::Blink_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Cancel(%tag);
			$ObjectiveHUD::Blinking = false;
			return;
		}
		else
		if (%status == $Objective::Dropped)
		{
		}
		else
		{
			%tag = "ObjectiveHUD::Taken_"@$ObjectiveHUD::LookupLine[%id];
			Schedule::Cancel(%tag);
		}
	}

	$ObjectiveHUD::BlinkOn[%id] = false;
	ObjectiveHUD::UpdateLine(%id);

	$ObjectiveHUD::Blinking = true;
	%tag = "ObjectiveHUD::Blink_"@$ObjectiveHUD::LookupLine[%id];
	Schedule::Add("ObjectiveHUD::Blink("@%id@",0);", 0.3, %tag);
}

// Flag carry timer.
function ObjectiveHUD::Carry(%id, %i)
{
	// Blink for a short time when the flag is picked up.
	if(%i < 10)
	{
		$ObjectiveHUD::BlinkOn[%id] = !$ObjectiveHUD::BlinkOn[%id];
	}
	else
	{
		$ObjectiveHUD::BlinkOn[%id] = false;
	}
	%tag = "ObjectiveHUD::Carry_"@$ObjectiveHUD::LookupLine[%id];
	Schedule::Add("ObjectiveHUD::Carry("@%id@","@%i+1@");", 0.3, %tag);

	ObjectiveHUD::UpdateLine(%id);
}


// Dropped flag timer.
function ObjectiveHUD::Dropped(%id, %i)
{
	if(%i > $ObjectiveHUD::DroppedMaxIndex)
	{
		$ObjectiveHUD::BlinkOn[%id] = false;
	}
	else
	{
		// Blink for a short time when the flag is dropped
		// and then for the last 10 seconds before returning.
		if(%i < 10 || %i > ($ObjectiveHUD::DroppedMaxIndex-34))
		{
			$ObjectiveHUD::BlinkOn[%id] = !$ObjectiveHUD::BlinkOn[%id];
		}
		else
		{
			$ObjectiveHUD::BlinkOn[%id] = false;
		}
		%tag = "ObjectiveHUD::Dropped_"@$ObjectiveHUD::LookupLine[%id];
		Schedule::Add("ObjectiveHUD::Dropped("@%id@","@%i+1@");", 0.3, %tag);
	}

	ObjectiveHUD::UpdateLine(%id);
}

// F&R flag timer.
function ObjectiveHUD::Taken(%id, %i)
{
	if(%i > $ObjectiveHUD::TakenMaxIndex)
	{
		$ObjectiveHUD::BlinkOn[%id] = false;
	}
	else
	{
		if (!$ObjectiveHUD::Blinking)
		{
			// Blink for a short time when the flag is dropped
			// and then for the last 10 seconds before returning.
			if(%i < 10 || %i > ($ObjectiveHUD::TakenMaxIndex-34))
			{
				$ObjectiveHUD::BlinkOn[%id] = !$ObjectiveHUD::BlinkOn[%id];
			}
			else
			{
				$ObjectiveHUD::BlinkOn[%id] = false;
			}
		}
		%tag = "ObjectiveHUD::Taken_"@$ObjectiveHUD::LookupLine[%id];
		Schedule::Add("ObjectiveHUD::Taken("@%id@","@%i+1@");", 0.3, %tag);
	}

	ObjectiveHUD::UpdateLine(%id);
}

// Blink the objective name on and off.
function ObjectiveHUD::Blink(%id, %i)
{
	if(%i > 10)
	{
		$ObjectiveHUD::BlinkOn[%id] = false;
		$ObjectiveHUD::Blinking = false;
	}
	else
	{
		$ObjectiveHUD::BlinkOn[%id] = !$ObjectiveHUD::BlinkOn[%id];
		%tag = "ObjectiveHUD::Blink_"@$ObjectiveHUD::LookupLine[%id];
		Schedule::Add("ObjectiveHUD::Blink("@%id@","@%i+1@");", 0.3, %tag);
	}

	ObjectiveHUD::UpdateLine(%id);
}

// Objective list has been reset.
function ObjectiveHUD::Reset()
{
	// Reset variables.
	deleteVariables("$ObjectiveHUD::BlinkOn*");
	for (%i = 0; %i < HUD::GetGuiObjectCount(ObjectiveHUD); %i++)
	{
		Schedule::Cancel("ObjectiveHUD::Blink_"@%i);
		Schedule::Cancel("ObjectiveHUD::Dropped_"@%i);
		Schedule::Cancel("ObjectiveHUD::Taken_"@%i);
	}
	$ObjectiveHUD::Blinking = false;
}

// Create the HUD.
if(!HUD::Exists(ObjectiveHUD))
{
	HUD::NewFrame(ObjectiveHUD, ObjectiveHUD::onUpdateHUD, $CrunchyPref::ObjectiveHUDPos);
}

// Setup HUD on connect or mission change.
function ObjectiveHUD::Setup()
{
	ObjectiveHUD::Resize();

	if($CrunchyPref::ObjectiveHUDDisplay == onLoaded)
	{
		HUD::Display(ObjectiveHUD, false);
	}
	else
	{
		HUD::Display(ObjectiveHUD, $CrunchyPref::ObjectiveHUDDisplay);
	}
}
ObjectiveHUD::Setup();

//
// NewOpts
//

$ObjectiveHUD::Display[0] = "true";
$ObjectiveHUD::Display[1] = "false";
$ObjectiveHUD::Display[2] = "onLoaded";
$ObjectiveHUD::Display["true"] = 0;
$ObjectiveHUD::Display["false"] = 1;
$ObjectiveHUD::Display["onLoaded"] = 2;

// Not yet implemented.
$ObjectiveHUD::Sort[0] = "byAlpha";
$ObjectiveHUD::Sort[1] = "unsorted";
$ObjectiveHUD::Sort[2] = "ifNew";
$ObjectiveHUD::Sort["byAlpha"] = 0;
$ObjectiveHUD::Sort["unsorted"] = 1;
$ObjectiveHUD::Sort["ifNew"] = 2;

// Setup options page
function ObjectiveHUD::onOpen()
{
	FGCombo::clear(ObjHUD::comboDisplay);
	FGCombo::addEntry(ObjHUD::comboDisplay,"on",0);
	FGCombo::addEntry(ObjHUD::comboDisplay,"off",1);
	FGCombo::addEntry(ObjHUD::comboDisplay,"with objectives",2);
	FGCombo::setSelected(ObjHUD::comboDisplay,$ObjectiveHUD::Display[$CrunchyPref::ObjectiveHUDDisplay]);

	Control::setActive(ObjHUD::checkCarry, $CrunchyPref::ObjectiveCTF);
	Control::setActive(ObjHUD::checkAbrvTeam, $CrunchyPref::ObjectiveTeamNames);

	// Not yet implemented.
	Control::setVisible(ObjHUD::comboSort, FALSE);
	//FGCombo::clear(ObjHUD::comboSort);
	//FGCombo::addEntry(ObjHUD::comboSort,"alphanumerically",0);
	//FGCombo::addEntry(ObjHUD::comboSort,"unsorted",1);
	//FGCombo::addEntry(ObjHUD::comboSort,"if new objectives",2);
	//FGCombo::setSelected(ObjHUD::comboSort,$ObjectiveHUD::Sort[$CrunchyPref::ObjectiveHUDSort]);
}

// Update preferences and HUD if needed.
function ObjectiveHUD::onClose()
{
	$CrunchyPref::ObjectiveHUDDisplay = $ObjectiveHUD::Display[FGCombo::getSelected(ObjHUD::comboDisplay)];
	//$CrunchyPref::ObjectiveHUDSort = $ObjectiveHUD::Sort[FGCombo::getSelected(ObjHUD::comboSort)];

	export("$CrunchyPref::*", "config\\CrunchyPrefs.cs", false);
	ObjectiveHUD::onLoaded(Objective::Number());
}

// Options page for Zear's NewOpts, <grin>
NewOpts::register("Objective HUD",
				  $ObjectiveHUD::GUIdir@"ObjectiveHUD.gui",
				  "ObjectiveHUD::onOpen();",
				  "ObjectiveHUD::onClose();",
				  TRUE);
Include($ObjectiveHUD::dir@"ObjectiveHUDHelp.cs");


} // Version check
else echo("Objective HUD: you need the latest versions of Presto Pack and ObjectiveTrak");
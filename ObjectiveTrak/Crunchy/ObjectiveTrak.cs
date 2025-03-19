// -- ObjectiveTrak -- v3.3 ------------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
//
// *** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
// *** Requires Writer's Support Pack v4.0.4 BETA or later *** http://www.planetstarsiege.com/lorne/
// *** Supports Poker's BWAdmin mod ***  http://www.pokerspockets.freeserve.co.uk/
//
// Scripting tool that tracks the status of the objectives in missions.  The script can track objectives in
// any of the missions, either base or custom including CTF, C&H, D&D, F&R, Balanced, Open Call and Multiple
// Team.  Take a look at the new events and functions ObjectiveTrak defines listed below.  For an example of
// the use of the script check out my Objective HUD.
//
// The script has the ability to learn about objectives from the messages sent to the client you see in the
// chat box.  The script also loads information about missions from the files created using
// ObjectiveExtract.exe.  There is no other way to find out about the objectives from a script, rather than 20
// scripts all having to do this hard work ObjectiveTrak does it all for you.
//
// The current implemetation assigns each objective an  unique ID.  The ID is an interger which is in the
// range from zero to one less than the number of objectives (see ObjectiveTrak_notes.txt).
//
// The script patches TeamTrak to bring it up to date.  It lets TeamTrak know about the team names as soon as
// you are connected.  Flag tracking has been extended to work with multiple teams.  The interface for
// TeamTrak has not been changed in any way.
//
//
// Changes in v3.3:
//	+ Autoloading volume.
//  + Uses Writers support pack.
//
// Changes in v3.2.1:
//  + Fixed problem with F&R timer going negative.  Thanks (RSV)MassMedia for spotting it.
//
// Changes in v3.2:
//  + Gets the winning score from BWAdmin mod v4.2 properly.
//  + Parses server message to get the start value of the F&R flag timer.
//  + Fixed double CTF flag return problem, the flag status is only changed once now.
//  + Added new functions:
//		Objective::GetFlagName()
//		Objective::GetFlagID()
//      Objective::GetTeamNameAbbreviated()
//		Objective::GetFandRTimer()
//		Objective::GetLocationX()
//		Objective::GetLocationY()
//
// Changes in v3.1:
//  + Complete Team Aerial Combat score support.
//
// Changes in v3.0:
//  + Files are stored in Tribes\base\missions.  No files are saved to the config dir now!
//  + Supports the latest BW Admin mod.  If you join a server running the latest BW Admin mod the script will
//    know the scores and the exact status of all the objectives on any mission! Woohoo :).
//  + ObjectiveExtract.exe, which can extract mission info from any mission files the user has.
//  + The guts have been rewritten to be lean and mean.  It is more efficient and should be easier to find and
//    potential bugs.
//  + Multiple team support, up to 8 teams if you wanted!
//  + Patches TeamTrak to support multiple team missions and updates TeamTrak to know the team names as soon
//    as you join the server or an admin changes the name.
//  + Uses mission type to cut down on message parsing.  Works out what type of mission a balanced or open
//    call mission is from the objective list.
//  + Extended status and type functions: Objective::GetStatusExt() and Objective::GetTypeExt().
//  + Moved the docs to ObjectiveTrak_notes.txt, also updated the docs.
//  + New functions (see ObjectiveTrak_notes.txt):
//      Objective::GetMissionType()
//      Objective::IsBWAdmin()
//      Objective::IsMatchStarted()
//      Objective::GetTeamNum()
//      Objective::GetStatusExt()
//      Objective::GetTypeExt()
//		Objective::GetTypeNum()
//		Objective::ConvertType()
//  - Removed key binding.
//
// Changes in v2.0 beta 7:
//  = Moved eventObjectiveLoaded so that scripts can avoid having to use eventObjectiveMadeNeutral as well.
//	  These events were very close together and so could cause problems.
//
// Changes in v2.0 beta 6:
//	+ Added support for scores in the mission info files.
//	+ Added score code to Objective::Set() and the plugins.
//	+ Added support functions Objective::GetTeamScore(), Objective::GetScore(), Objective::GetDeltaScore(),
//	  Objective::GetTime(), Objective::GetTimeScore(), Objective::GetWinScore().
//  = Changed F&R tracking to work with Tribes v1.6's new 180sec timelimit for conveying a flag.
//	= Objective::InsertTeamName() returns the team name (if known) instead of "Enemy ..." when observing.
//
// Changes in v2.0 beta 5:
//	= Client message parser didn't check to see if the message was from the server. D'oh :)
//
// Changes in v2.0 beta 4:
//	+ Added new events:
//		eventObjectiveTaken
//		eventObjectiveLost
//		eventObjectiveFlagTaken
//		eventObjectiveFlagDropped
//		eventObjectiveFlagReturned
//		eventObjectiveFlagCaptured
//		eventObjectiveFlagHolds
//  + CTF flags support!
//  + Added support function Objective::InsertTeamSymbol().
//	= Support function Objective::GetId() optimised.  Now executes in constant rather than linear time.
//  = Flag (F&R and CTF) carry state was being stored fixed, now changes correctly if you change teams.
//
// Changes in v2.0 beta 3:
//  = Fixed another CTF / F&R flag clash.
//	= Fixed bug where abbreviated names for new objectives weren't being stripped.
//	- Moved abbreviations to the MissionInfo Pack.
//
// Changes in v2.0 beta 2:
//	+ Added support function Objective::GetNameAbbreviated(%id).  Abbreviations are stored in a table for
//	  efficiency.
//  + Had to add toggle key to distinguish between CTF and F&R flags, $ServerMissionType isn't always set :/.
//
// Changes in v2.0 beta:
//	+ Compelete rewrite.  Generalised the script so that it could track any objective.
//  + Beta version released to get bug reports as testing is difficult.
//  + Used the logic and messages from scripts.vol to derive the messages and parsing functions.
//
// Changes in v1.0.1:
//	= Fixed bug where Objective::GetNameStripped(%id) would return a null value if there was no "the" at
//	  the start of the name (don't rename variables ;).
//
// What happend to v1.0?
//	+ I first wrote the script to track C&H mission objectives but Presto pointed out it could be
//	  generalised to track any objective.  Thanks for the idea Presto :).  Dunno if thanks are in order or not
//    as this script is over 2000 lines long, lol.
//
//

//
// --- Header ---
//

$ObjectiveTrak::dir = "";

include("writer\\version.cs");
version("Crunchy\\ObjectiveTrak.cs", "3.3", "|HH|Crunchy", "- June 26, 2000 - comprehensive objective tracking support script");

include("support.acs.cs");
include("writer\\event.cs");
include("writer\\schedule.cs");

Include("Presto\\Match.cs");
Include("Presto\\TeamTrak.cs");

Event::Attach(eventConnectionAccepted, "Objective::onConnectionAccepted();");
Event::Attach(eventChangeMission, "Objective::onChangeMission();");
Event::Attach(eventConnected, "Objective::onConnected();");
Event::Attach(eventClientTeamAdd, Objective::onTeamAdd);
Event::Attach(eventMissionInfo, Objective::onMissionInfo);
Event::Attach(eventClientMessage, Objective::onClientMessage);

if ($Presto::version == 0.93)
{
	// We're taking over some of TeamTrak's work.
	Event::Detach(eventClientMessage, Team::ParseClientMessage);
	Event::Detach(eventChangeMission, "Team::NewGame();");
	Event::Detach(eventConnectionAccepted, "Team::Reset();");
}

// Time between score updates on a C&H map.
$Objective::UpdateWait = 5;

// Possible states.
$Objective::Enemy		    = enemy;
$Objective::Friendly	    = friendly;
$Objective::Destroyed	    = destroyed;
$Objective::Neutral		    = neutral;
$Objective::Carry		    = carry; // This will be interpreted as one of the following two.
$Objective::FriendlyCarry   = carryFriendly;
$Objective::EnemyCarry	    = carryEnemy;
$Objective::Dropped		    = dropped;
$Objective::Unknown		    = unknown;

// Extended states.
$Objective::FriendlyNeutral = neutralFriendly;
$Objective::EnemyNeutral	= neutralEnemy;
$Objective::FriendlyDropped	= droppedFriendly;
$Objective::EnemyDropped	= droppedEnemy;
$Objective::FriendlyUnknown	= unknownFriendly;
$Objective::EnemyUnknown	= unknownEnemy;

// Objective types.
$Objective::Flag			  = flag;
$Objective::TowerSwitch		  = TowerSwitch;
$Objective::Generator		  = Generator;
$Objective::PortableGenerator = PortGenerator;
$Objective::PlasmaTurret      = PlasmaTurret;
$Objective::SolarPanel        = SolarPanel;
$Objective::CommandStation    = CommandStation;
$Objective::RocketTurret      = RocketTurret;
$Objective::PulseSensor       = PulseSensor;
$Objective::MediumPulseSensor = MediumPulseSensor;

// Generic objective types.
$Objective::CandH = candh;
$Objective::DandD = dandd;
$Objective::FandR = fandr;
$Objective::CTF   = ctf;

//
// --- Begin main ---
//

// Reset the script.
function Objective::Reset()
{
	deleteVariables("$ObjData::*");
	$ObjData::Num = 0;
	$ObjData::TeamNum = 0;
	$Objective::Loaded = false;
	$Objective::New = false;
	$Objective::MissionType = "";

	// Some servers don't set this so we need to know if it has been set.
	$ServerMissionType = "";

	// Maximum of 8 teams (from scripts.vol)
	for(%i = 0; %i < 8; %i++)
	{
		$ObjData::TeamScore[%i] = 0;

		Event::Trigger(eventTeamScoreUpdate, %i, $ObjData::TeamScore[%i]);
	}

	Schedule::Cancel("Objective::_TeamScoreUpdate();");

	Event::Trigger(eventObjectiveReset);
}

// We can use $ServerMissionType if it's one of the following.
function Objective::SetMissonType()
{
	if ($ServerMissionType == "Capture the Flag"
	||  $ServerMissionType == "Capture and Hold"
	||  $ServerMissionType == "Defend and Destroy"
	||  $ServerMissionType == "Find and Retrieve")
	{
		$Objective::MissionType = $ServerMissionType;

		$Objective::New = true;
	}
}

// Get the mod we are running.
function Objective::GetMod()
{
	// Check server mod.
	if (String::FindSubStr($ServerMod, "TAC") != -1)
	{
		return "TAC";
	}
	else
	{
		return "base";
	}
}

// Reset the script and we don't know if the match has started.
function Objective::onConnectionAccepted()
{
	Objective::Reset();

	// Reset TeamTrak.
	Team::Reset();

	$Objective::MatchStarted = false;
}

// Reset the script, we do know the match is about to start and we need to save the objective info.
function Objective::onChangeMission()
{
	// Save the objectives if there was any new ones.
	if (Objective::GetMod() == "base")
	{
		%file = "base\\missions\\"@$ServerMission@".cs";
		if ($Objective::New || ($Objective::BWAdmin && !isFile(%file)))
		{
			// Check for C&H map.
			if ($Objective::MissionType == "" && (Objective::GetTypeNum($Objective::TowerSwitch) > 0))
			{
				$Objective::MissionType = "Capture and Hold";

				for (%id = 0; %id < $ObjData::Num; %id++)
				{
					$ObjData::[%id, score] = "0 12";
				}
			}

			// Export the mission info.
			export("$ObjData::Num", %file, false);
			export("$ObjData::*_name", %file, true);
			export("$ObjData::*_type", %file, true);
			export("$ObjData::*_score", %file, true);
			export("$ObjData::*_waypoint", %file, true);
			export("$ObjData::WinScore", %file, true);

			$ObjData::MissionType = $Objective::MissionType;
			export("$ObjData::MissionType", %file, true);

			echo("ObjectiveTrak - Saved "@$ObjData::Num@" objectives to file: "@%file);
		}
	}

	// Reset team trak.
	deleteVariables("$TeamData::TeamNum*");

	Objective::Reset();
	$Objective::MatchStarted = true;
}

// Look for the BWAdmin mod and register for information.
function Objective::onConnected()
{
	Objective::DetectMod();

	if ($Objective::BWAdmin)
	{
		Objective::Register();
	}
}

// Check the mod.
function Objective::DetectMod()
{
	// Look for the bwadmin base mod.  If a demo was recorded on a BWAdmin mod server then the mod would be
	// found but we may not be running it locally.
	if (String::FindSubStr($ServerMod, "bwadmin") != -1
	&&  String::FindSubStr($ServerMod, "base") != -1
	&& !$PlayingDemo)
	{
		echo("ObjectiveTrak - Providing support for BWAdmin mod");
		$Objective::BWAdmin = true;
		$Objective::TAC = false;
	}
	else
	// Look for TAC base mod.
	if (String::FindSubStr($ServerMod, "TAC") != -1
	&&  String::FindSubStr($ServerMod, "base") != -1)
	{
		echo("ObjectiveTrak - Providing support for TAC base mod");
		$Objective::BWAdmin = false;
		$Objective::TAC = true;
	}
	else
	{
		echo("ObjectiveTrak - Providing support for base mod");
		$Objective::BWAdmin = false;
		$Objective::TAC = false;
	}
}

// Schedule loop to update team score on C&H missions.
function Objective::_TeamScoreUpdate()
{
	if ($Objective::BWAdmin)
	{
		Objective::TeamScoreUpdate();
	}
	else
	{
		for (%team = 0; %team < $ObjData::TeamNum; %team++)
		{
			Event::Trigger(eventTeamScoreUpdate, %team, Objective::GetTeamScore(%team));
		}
	}

	Schedule::Add("Objective::_TeamScoreUpdate();", $Objective::UpdateWait);
}

// Make ID table for quick retrieval.
function Objective::StoreIDs()
{
	for(%id = 0; %id < $ObjData::Num; %id++)
	{
		$ObjData::[$ObjData::[%id, name]] = %id;
	}
}

// If we are starting a mission where we know about some of the objectives we know they are neutral.
function Objective::MakeNeutral()
{
	for(%id = 0; %id < $ObjData::Num; %id++)
	{
		$ObjData::[%id, status] = $Objective::Neutral;
	}

	if ($Presto::version == 0.93)
	{
		// Update TeamTrak.
		for (%team = 0; %team < $ObjData::TeamNum; %team++)
		{
			Team::MoveFlag(%team, $Trak::locationHome);
		}
	}

	Event::Trigger(eventObjectiveMadeNeutral);
}

// If we couldn't load any objectives but it's a CTF mission assume there are two flags.
function Objective::AssumeFlag(%team)
{
	%o = "%"@%team@" flag";
	%id = $ObjData::Num;

	$ObjData::[%id, name] = %o;
	$ObjData::[%id, type] = $Objective::Flag;
	$ObjData::[%o] = %id;
	$ObjData::[%id, abrv] = Objective::Abbreviate(Objective::GetNameStripped(%id));
	$ObjData::[%id, score] = "1 0";
	$ObjData::TypeNum[$Objective::Flag]++;
	$ObjData::TypeNum[$Objective::CTF]++;

	$ObjData::Num++;
	$Objective::New = true;

	return %id;
}

// Set the number of each type of objective.
function Objective::StoreTypeNum()
{
	for (%id = 0; %id < $ObjData::Num; %id++)
	{
		$ObjData::TypeNum[Objective::GetType(%id)]++;
		$ObjData::TypeNum[Objective::GetTypeExt(%id)]++;
	}
}

// Load mission info and setup schedule to update score if the map is a C&H one.
function Objective::onMissionInfo(%server, %missionName, %missionType)
{
	if (%server == 2048)
	{
		%file = "missions\\"@%missionName@".cs";
		%mod = Objective::GetMod();

		if ($Objective::BWAdmin)
		{
			// Need to load the file as we need waypoint info.
			// Server will tell us about the objectives (see bottom of script).
			if (isFile(%mod@"\\"@%file))
			{
				exec(%file);
				$ObjData::Num = 0;
			}

			// Need to request a team score update when map changes to get the winning score.
			Objective::TeamScoreUpdate();
		}
		else
		{
			// Load mission info file.
			if (isFile(%mod@"\\"@%file))
			{
				exec(%file);
				$Objective::Loaded = true;
				Objective::MakeAbbreviations();
				Objective::StoreIDs();
				Objective::StoreTypeNum();

				echo("ObjectiveTrak - "@$ObjData::Num@" objectives loaded.");

				if ($ObjData::MissionType != "")
				{
					$Objective::MissionType = $ObjData::MissionType;
				}
				else
				{
					Objective::SetMissonType();
				}

				if ($Objective::MatchStarted)
				{
					Objective::MakeNeutral();
				}
			}
			else
			{
				Objective::SetMissonType();

				// If we couldn't load any objectives but it's a CTF mission assume two flags.
				if ($Objective::MissionType == "Capture the Flag")
				{
					Objective::AssumeFlag(0);
					Objective::AssumeFlag(1);

					if ($Objective::MatchStarted)
					{
						Objective::MakeNeutral();
					}
				}
			}

			Event::Trigger(eventObjectiveLoaded, $ObjData::Num);

			// If we're playing a C&H map then update the score periodically.
			if ($Objective::MissionType == "Capture and Hold")
			{
				Schedule::Add("Objective::_TeamScoreUpdate();", $Objective::UpdateWait);
			}
		}
	}
}

// Get the mission type.
function Objective::GetMissionType()
{
	return $Objective::MissionType;
}

// Returns true if the mod we are playing on is the BWAdmin mod.
function Objective::IsBWAdmin()
{
	return $Objective::BWAdmin;
}

// Returns true if the match has started.
function Objective::IsMatchStarted()
{
	return $Objective::MatchStarted;
}

// Get the number of teams (not including observers).
function Objective::GetTeamNum()
{
	return $ObjData::TeamNum;
}

// Get the basic status of an objective.
function Objective::_GetStatus(%id, %type, %status)
{
	if (%type == $Objective::CTF)
	{
		if (%status == $Objective::Carry)
		{
			if (Client::GetTeam($ObjData::[%id, client]) == Team::Friendly())
			{
				return $Objective::FriendlyCarry;
			}
			else
			{
				return $Objective::EnemyCarry;
			}
		}
		else
		if (%status == $Objective::Neutral)
		{
			if ($ObjData::[%id, name] == "%"@Team::Friendly()@" flag")
			{
				%status = $Objective::Friendly;
			}
			else
			{
				%status = $Objective::Enemy;
			}
		}
		else
		if (%status == "")
		{
			return $Objective::Unknown;
		}
		else
		{
			return %status;
		}
	}
	else
	if (%type == $Objective::CandH)
	{
		if (%status == Team::Friendly())
		{
			return $Objective::Friendly;
		}
		else
		if (%status == $Objective::Neutral)
		{
			return $Objective::Neutral;
		}
		else
		if (%status == "")
		{
			return $Objective::Unknown;
		}
		else
		{
			// Works with multiple teams.  Can't tell you exact enemy team though.
			return $Objective::Enemy;
		}
	}
	else
	if (%type == $Objective::DandD)
	{
		if(%status == "")
		{
			return $Objective::Unknown;
		}
		else
		{
			return %status;
		}
	}
	else
	if (%type == $Objective::FandR)
	{
		if(%status == $Objective::Carry)
		{
			if(Client::GetTeam($ObjData::[%id, client]) == Team::Friendly())
			{
				return $Objective::FriendlyCarry;
			}
			else
			{
				return $Objective::EnemyCarry;
			}
		}
		else
		if (%status == Team::Friendly())
		{
			return $Objective::Friendly;
		}
		else
		if (%status == $Objective::Dropped || %status == $Objective::Carry || %status == $Objective::Neutral)
		{
			return %status;
		}
		else
		if (%status == "")
		{
			return $Objective::Unknown;
		}
		else
		{
			return $Objective::Enemy;
		}
	}
	else
	{
		return %status;
	}
}

// Get the status of an objective.
function Objective::GetStatus(%id)
{
	%status = $ObjData::[%id, status];
	%type = Objective::GetType(%id);

	return Objective::_GetStatus(%id, %type, %status);
}

// Get the status before the last change.
function Objective::GetLastStatus(%id)
{
	%status = $ObjData::[%id, laststatus];
	%type = Objective::GetType(%id);

	return Objective::_GetStatus(%id, %type, %status);
}

// Extends status information for CTF flags.
function Objective::_GetStatusExt(%id, %type, %status)
{
	if (%type == $Objective::CTF)
	{
		if(%status == "")
		{
			%status = $Objective::Unknown;
		}

		if ($ObjData::[%id, name] == "%"@Team::Friendly()@" flag")
		{
			return %status@"Friendly";
		}
		else
		{
			return %status@"Enemy";
		}
	}
	else
	{
		return Objective::_GetStatus(%id, %type, %status);
	}
}

// Extended version of GetStatus.
function Objective::GetStatusExt(%id)
{
	%status = $ObjData::[%id, status];
	%type = Objective::GetType(%id);

	return Objective::_GetStatusExt(%id, %type, %status);
}

// Get the extended status from before the last change.
function Objective::GetLastStatusExt(%id)
{
	%status = $ObjData::[%id, laststatus];
	%type = Objective::GetType(%id);

	return Objective::_GetStatusExt(%id, %type, %status);
}

// Get the uninterpreted status, ie the status that is team independant.
// Use Objective::GetStatus() in preference.
function Objective::GetRawStatus(%id)
{
 	return $ObjData::[%id, status];
}

// Get the uninterpreted last status.
// Use Objective::GetLastStatus() in preference.
function Objective::GetRawLastStatus(%id)
{
 	return $ObjData::[%id, laststatus];
}

// Get the name of an objective.
function Objective::GetName(%id)
{
	return $ObjData::[%id, name];
}

// Get the name of a teams flag.
function Objective::GetFlagName(%team)
{
	return "%"@%team@" flag";
}

// Get the objective ID of a teams flag.
function Objective::GetFlagID(%team)
{
	%o = "%"@%team@" flag";
	return $ObjData::[%o];
}

// Get the name of an objective and strip the "the" if it exists.
function Objective::GetNameStripped(%id)
{
	%o = $ObjData::[%id, name];
	if (Match::ParamString(%o, "the %o"))
	{
		return Match::Result(o);
	}
	else
	{
		return %o;
	}
}

// Get the abbreviated version of the objective name.
function Objective::GetNameAbbreviated(%id)
{
	return $ObjData::[%id, abrv];
}

// Get the start value of the F&R countdown timer for this server.
function Objective::GetFandRTimer()
{
	// If the value hasn't been set assume it's 180 seconds.
	if ($ObjData::FandRTimer == "")
	{
		return 180;
	}
	else
	{
		return $ObjData::FandRTimer;
	}
}

// Get the x location of an objective on the pda map.
function Objective::GetLocationX(%id)
{
	return getWord($ObjData::[%id, waypoint], 0);
}

// Get the y location of an objective on the pda map.
function Objective::GetLocationY(%id)
{
	return getWord($ObjData::[%id, waypoint], 1);
}

// Returns the abbreviated version of this teams name.
// %level is the severness, 0 being no abbreviation, 1 being least severe and 3 being the most severe.
function Objective::GetTeamNameAbbreviated(%team, %level)
{
	if (%level == "") %level = 1;

	%name = $ObjData::TeamName[%team];
	if (%level == 0)
	{
		%abrv = %name;
	}
	else
	{
		%abrv = $ObjAbrv::[%name, level@%level];
	}

	if (%abrv == "")
	{
		return %name;
	}
	else
	{
		return %abrv;
	}
}

// Returns the number of objectives.
function Objective::Number()
{
	return $ObjData::Num;
}

// Get the id of an objective from it's name.
function Objective::GetID(%name)
{
	%id = $ObjData::[%name];
	if (%id == "") %id = -1;
	return %id;
}

// Get client number associated with the last status change.
function Objective::GetClient(%id)
{
	return $ObjData::[%id, client];
}

// Get the score amount this objective is valued at.
function Objective::GetScore(%id)
{
	%score = $ObjData::[%id, score];
	if (%score == "")
	{
		return 0;
	}
	else
	{
		return getWord(%score, 0);
	}
}

// Get the change in score over time that this objective is worth.
function Objective::GetDeltaScore(%id)
{
	%score = $ObjData::[%id, score];
	if(%score == "")
	{
		return 0;
	}
	else
	{
		return getWord(%score, 1);
	}
}

// Get the score that this team has gained since they took it.
// Returns 0 if the team isn't controlling it.
function Objective::GetTimeScore(%id, %team)
{
	%deltaScore = Objective::GetDeltaScore(%id);
	%timeUpdate = $ObjData::[%id, time];

	if (%deltaScore != ""
	&&  %timeUpdate != ""
	&&  $ObjData::[%id, status] == %team)
	{
		return ((getSimTime() - %timeUpdate) * %deltaScore) / 60;
	}
	else
	{
		return 0;
	}
}

// Get the score required for a team to win the mission.
function Objective::GetWinScore()
{
	return $ObjData::WinScore;
}

// Get a team's score.
function Objective::GetTeamScore(%team)
{
	if ($Objective::BWAdmin)
	{
		%score = $ObjData::TeamScore[%team];
	}
	else
	{
		%baseScore = $ObjData::TeamScore[%team];

		%timeScore = 0;
		for(%id = 0; %id < $ObjData::Num; %id++)
		{
			%timeScore += Objective::GetTimeScore(%id, %team);
		}

		%score = %baseScore + %timeScore;
	}

	return %score;
}

// Get the time of the last update on the objective
function Objective::GetTime(%id)
{
	return $ObjData::[%id, time];
}

// Get the number of a particular type of objective.
function Objective::GetTypeNum(%type)
{
	%num = $ObjData::TypeNum[%type];
	if (%num == "")
	{
		return 0;
	}
	else
	{
		return %num;
	}
}

// Convert to generic type.
function Objective::ConvertType(%type)
{
	if (%type == $Objective::Flag)
	{
		if ($Objective::MissionType == "Find and Retrieve")
		{
			return $Objective::FandR;
		}
		else
		{
			return $Objective::CTF;
		}
	}
	else
	if (%type == $Objective::TowerSwitch)
	{
		return $Objective::CandH;
	}
	else
	{
		return $Objective::DandD;
	}
}

// Get the type of the objective.
function Objective::GetType(%id)
{
	// Get the type.
	%type = $ObjData::[%id, type];

	// Convert to generic type.
	%type = Objective::ConvertType(%type);

	return %type;
}

// Get the type of the objective (extended - specific type).
function Objective::GetTypeExt(%id)
{
	return $ObjData::[%id, type];
}

// Where the name of an objective is dependant on the team, the team number is stored as "%0" etc.  When we
// display it we want to insert "Your" or "Enemy" in the place of these wildcards.
// If the player is in observer mode then the name of the team is returned instead of the normal name.
function Objective::InsertTeamName(%msg)
{
	while ((%pos = String::findSubStr(%msg, "%")) != -1)
	{
		%team = String::getSubStr(%msg, %pos+1, 1);  // Assumes not more than 10 teams :)

		if (Team::Friendly() == -1)
		{
			return $ObjData::TeamName[%team];
		}
		else
		if (%team == Team::Friendly())
		{
			if (%pos == 0)
			{
				%teamName = "Your";
			}
			else
			{
				%teamName = "your";
			}
		}
		else
		{
			if (%pos == 0)
			{
				%teamName = "Enemy";
			}
			else
			{
				%teamName = "enemy";
			}
		}
		%msg = String::getSubStr(%msg, 0, %pos) @ %teamName @ String::getSubStr(%msg, %pos+2, 256);
	}

	return %msg;
}

// Add team icon to objectives.
function Objective::InsertTeamSymbol(%msg)
{
	if ((%pos = String::findSubStr(%msg, "%")) != -1)
	{
		%team = String::getSubStr(%msg, %pos+1, 1);  // Theres an 8 team limit in scripts.vol
		if (%team == Team::Friendly())
		{
			%teamName = "<B-4,3:M_Player_green.bmp>";
		}
		else
		{
			%teamName = "<B-4,2:M_Player_red.bmp>";
		}
		%msg = String::getSubStr(%msg, 0, %pos) @ %teamName @ String::getSubStr(%msg, %pos+2, 256);
	}

	return %msg;
}

// Translate the person refered to in the text to a client number.
function Objective::GetClientFromMsg(%text)
{
	if (%text == "You")
	{
		return getManagerID();
	}
	else
	{
		return getClientbyName(%text);
	}
}

// Translate the team refered to in the text to a team number.
function Objective::GetTeamFromMsg(%text)
{
	if (%text == "Your" || %text == "your")
	{
		return Team::Friendly();
	}
	else
	if (Match::ParamString(%text, "The %t") || Match::ParamString(%text, "the %t"))
	{
		%t = Match::Result(t);
		return $ObjData::Team[%t];
	}
	else
	{
		return -1;
	}
}

// Get the type of the objective from the text message, only needed for D&D objectives atm.
function Objective::GetTypeFromMsg(%text)
{
	if (String::FindSubStr(%text, "Secondary") != -1)
	{
		return $Objective::PortableGenerator;
	}
	else
	if (String::FindSubStr(%text, "Small") != -1)
	{
		return $Objective::PortableGenerator;
	}
	else
	if (String::FindSubStr(%text, "Station") != -1)
	{
		return $Objective::PortGenerator;
	}
	else
	if (String::FindSubStr(%text, "Generator") != -1)
	{
		return $Objective::Generator;
	}
	else
	if (String::FindSubStr(%text, "Command") != -1)
	{
		return $Objective::CommandStation;
	}
	else
	if (String::FindSubStr(%text, "Plasma") != -1)
	{
		return $Objective::PlasmaTurret;
	}
	else
	if (String::FindSubStr(%text, "Solar") != -1)
	{
		return $Objective::SolarPanel;
	}
	else
	if (String::FindSubStr(%text, "Rocket") != -1)
	{
		return $Objective::RocketTurret;
	}
	else
	if (String::FindSubStr(%text, "Medium") != -1)
	{
		return $Objective::MediumPulseSensor;
	}
	else
	if (String::FindSubStr(%text, "Radar") != -1)
	{
		return $Objective::PulseSensor;
	}
	else
	if (String::FindSubStr(%text, "Pulse") != -1)
	{
		return $Objective::PulseSensor;
	}
	else
	{
		return $Objective::Generator;
	}
}

// Set the status of an objective, make a new one if it doesn't exist.
function Objective::Set(%o, %type, %status, %client)
{
	%new = false;
	%id = $ObjData::[%o];

	// New objective.
	if (%id == "")
	{
		%id = $ObjData::Num;

		$ObjData::[%id, name] = %o;
		$ObjData::[%id, type] = %type;
		$ObjData::[%o] = %id;
		$ObjData::[%id, abrv] = Objective::Abbreviate(Objective::GetNameStripped(%id));

		// Work out the score value for this new objective.
		if (%type == $Objective::Flag)
		{
			$ObjData::[%id, score] = "1 0";
		}
		else
		if (%type == $Objective::TowerSwitch)
		{
			if ($Objective::MissionType == "Capture and Hold")
			{
				$ObjData::[%id, score] = "0 12";
			}
			else
			{
				$ObjData::[%id, score] = "1 0";
			}
		}

		$ObjData::TypeNum[%type]++;
		$ObjData::TypeNum[Objective::ConvertType(%type)]++;

		$ObjData::Num++;
		%new = true;
	}

	// Upadte the status.
	$ObjData::[%id, client] = %client;
	$ObjData::[%id, laststatus] = $ObjData::[%id, status];
	$ObjData::[%id, status] = %status;
	$ObjData::[%id, time] = getSimTime();

	//echo("Objective: ", %o, ", ID: ", %id, ", Client: ", %client, ", Status: ", %status);

	if (%new)
	{
		$Objective::New = true;

		echo("ObjectiveTrak - *** New Objective ***");

		Event::Trigger(eventObjectiveNew,
					   %client,
					   %id,
					   Objective::GetStatus(%id),
					   Objective::GetType(%id));
	}
	else
	{
		Event::Trigger(eventObjectiveUpdated,
					   %client,
					   %id,
					   Objective::GetStatus(%id),
					   Objective::GetType(%id));
	}

	return %id;
}

// A tower was captured.
function Objective::CandH::onCaptured(%o, %p, %e)
{
	%client = Objective::GetClientFromMsg(%p);
	%team = Client::GetTeam(%client);

	%id = $ObjData::[%o];
	if (%e != "" && %id != "" && !$Objective::BWAdmin)
	{
		%enemy = $ObjData::Team[%e];
		// Cash-in the score accumalated over time for this objective.
		$ObjData::TeamScore[%enemy] += Objective::GetTimeScore(%id, %enemy);
	}

	%id = Objective::Set(%o, $Objective::TowerSwitch, %team, %client);

	// Request a score update from the server.
	if ($Objective::BWAdmin)
	{
		Objective::TeamScoreUpdate();
	}
	else
	{
		%deltaScore = Objective::GetScore(%id);
		if (%deltaScore > 0)
		{
			$ObjData::TeamScore[%team] += %deltaScore;
			Event::Trigger(eventTeamScoreUpdate, %team, $ObjData::TeamScore[%team]);

			if (%e != "")
			{
				// If the enemy team was holding the objective we should adjust their score.
				if ($ObjData::[%id, laststatus] == %enemy)
				{
					$ObjData::TeamScore[%enemy] -= %deltaScore;
					Event::Trigger(eventTeamScoreUpdate, %enemy, $ObjData::TeamScore[%enemy]);
				}
			}
		}
	}

	if(Team::Friendly() == -1 || %team == Team::Friendly())
	{
		Event::Trigger(eventObjectiveTaken, %client, %id, %team, Objective::GetType(%id));
	}
	else
	{
		Event::Trigger(eventObjectiveLost, %client, %id, %team, Objective::GetType(%id));
	}
}

// Store player name for later.
function Objective::DandD::onPlayerDestroyed(%p)
{
	$Objective::MissionType = "Defend and Destroy";

	$ObjData::Client = Objective::GetClientFromMsg(%p);
}

// An objective was destroyed.
function Objective::DandD::onDestroyed(%o, %t)
{
	$Objective::MissionType = "Defend and Destroy";

	%team = $ObjData::Team[%t];
	%o = "%"@%team@" "@%o;
	%client = $ObjData::Client;
	%enemy = Client::GetTeam(%client);
	%type = Objective::GetTypeFromMsg(%o);

	%id = Objective::Set(%o, %type, $Objective::Destroyed, %client);

	// Request a score update from the server.
	if ($Objective::BWAdmin)
	{
		Objective::TeamScoreUpdate();
	}
	else
	{
		%deltaScore = Objective::GetScore(%id);
		$ObjData::TeamScore[%enemy] += %deltaScore;

		Event::Trigger(eventTeamScoreUpdate, %enemy, $ObjData::TeamScore[%enemy]);
	}

	if(Team::Friendly() == -1 || %enemy == Team::Friendly())
	{
		Event::Trigger(eventObjectiveTaken, %client, %id, %enemy, Objective::GetType(%id));
	}
	else
	{
		Event::Trigger(eventObjectiveLost, %client, %id, %enemy, Objective::GetType(%id));
	}
}

// An F&R flag was taken.
function Objective::FandR::onTaken(%o, %p)
{
	$Objective::MissionType = "Find and Retrieve";

	%client = Objective::GetClientFromMsg(%p);
	%team = Client::GetTeam(%client);

	// Check for case when the other team had the flag.
	if(Match::ParamString(%o, "%o from the %t team"))
	{
		%o = Match::Result(o);	// Objective name.
		%t = Match::Result(t);	// Enemy team.

		// Remember the objective taken, if the player leaves the mission area we need this.
		$ObjData::Taken[%client] = %o;

		%enemy = $ObjData::Team[%t];

		%id = Objective::Set(%o, $Objective::Flag, $Objective::Carry, %client);

		// Request a score update from the server.
		if ($Objective::BWAdmin)
		{
			Objective::TeamScoreUpdate();
		}
		else
		// If the enemy team was holding the objective we should adjust their score.
		if ($ObjData::[%id, laststatus] == %enemy)
		{
			%deltaScore = Objective::GetScore(%id);
			$ObjData::TeamScore[%enemy] -= %deltaScore;

			Event::Trigger(eventTeamScoreUpdate, %enemy, $ObjData::TeamScore[%enemy]);
		}
	}
	else
	{
		%id = Objective::Set(%o, $Objective::Flag, $Objective::Carry, %client);
	}

	Event::Trigger(eventObjectiveFlagTaken, %client, %id, %team, Objective::GetType(%id));
}

// An F&R flag was dropped.
function Objective::FandR::onDropped(%o, %p)
{
	$Objective::MissionType = "Find and Retrieve";

	%client = Objective::GetClientFromMsg(%p);
	%team = Client::GetTeam(%client);

	%id = Objective::Set(%o, $Objective::Flag, $Objective::Dropped, %client);

	Event::Trigger(eventObjectiveFlagDropped, %client, %id, %team, Objective::GetType(%id));
}

// An F&R flag was conveyed to a teams base.
function Objective::FandR::onConveyed(%o, %p)
{
	$Objective::MissionType = "Find and Retrieve";

	%client = Objective::GetClientFromMsg(%p);
	%team = Client::GetTeam(%client);

	%id = Objective::Set(%o, $Objective::Flag, %team, %client);

	// Request a score update from the server.
	if ($Objective::BWAdmin)
	{
		Objective::TeamScoreUpdate();
	}
	else
	{
		%deltaScore = Objective::GetScore(%id);
		$ObjData::TeamScore[%team] += %deltaScore;

		Event::Trigger(eventTeamScoreUpdate, %team, $ObjData::TeamScore[%team]);
	}

	Event::Trigger(eventObjectiveFlagHolds, %client, %id, %team, Objective::GetType(%id));
}

// An F&R flag was returned to it's initial position.
function Objective::FandR::onReturnToInitial(%o, %p)
{
	$Objective::MissionType = "Find and Retrieve";

	if (%p == "")
	{
		%client = "";
	}
	else
	{
		%client = Objective::GetClientFromMsg(%p);
	}

	%id = Objective::Set(%o, $Objective::Flag, $Objective::Neutral, %client);

	// Request a score update from the server.
	if ($Objective::BWAdmin)
	{
		Objective::TeamScoreUpdate();
	}
	else
	{
		%laststatus = $ObjData::[%id, laststatus];
		if (%laststatus != $Objective::Dropped
		&&  %laststatus != $Objective::Carry
		&&  %laststatus != "")
		{
			%deltaScore = Objective::GetScore(%id);
			$ObjData::TeamScore[%laststatus] -= %deltaScore;

			Event::Trigger(eventTeamScoreUpdate, %laststatus, $ObjData::TeamScore[%laststatus]);
		}
	}

	Event::Trigger(eventObjectiveFlagReturned, %client, %id, "", Objective::GetType(%id));
}

// A player didn't get the flag to a base in time so it was returned.
function Objective::FandR::onPlayerReturn(%p)
{
	$Objective::MissionType = "Find and Retrieve";

	$ObjData::Client = GetClientFromMsg(%p);
}

// An F&R flag was returned to a team's base.
function Objective::FandR::onReturn(%o, %t)
{
	$Objective::MissionType = "Find and Retrieve";

	%team = $ObjData::Team[%t];

	%id = Objective::Set(%o, $Objective::Flag, %team, $ObjData::Client);

	// Request a score update from the server.
	if ($Objective::BWAdmin)
	{
		Objective::TeamScoreUpdate();
	}
	else
	{
		%deltaScore = Objective::GetScore(%id);
		$ObjData::TeamScore[%team] += %deltaScore;

		Event::Trigger(eventTeamScoreUpdate, %team, $ObjData::TeamScore[%team]);
	}

	Event::Trigger(eventObjectiveFlagReturned, %client, %id, %team, Objective::GetType(%id));
}

// A player left the mission area whilst carrying a flag.
function Objective::FandR::onPlayerLeftMissionArea(%p, %t)
{
	%client = Objective::GetClientFromMsg(%p);
	%o = $ObjData::Taken[%client];	// The objective the client took.
	%team = $ObjData::Team[%t];

	%id = Objective::Set(%o, $Objective::Flag, %team, %client);

	// Request a score update from the server.
	if ($Objective::BWAdmin)
	{
		Objective::TeamScoreUpdate();
	}
	else
	{
		%deltaScore = Objective::GetScore(%id);
		$ObjData::TeamScore[%team] += %deltaScore;

		Event::Trigger(eventTeamScoreUpdate, %team, $ObjData::TeamScore[%team]);
	}

	Event::Trigger(eventObjectiveFlagReturned, %client, %id, %team, Objective::GetType(%id));
}

// Number of seconds till flag returns.
function Objective::FandR::onYouHave(%s)
{
	$Objective::MissionType = "Find and Retrieve";

	// When %s is 30 or less that means that the server is reminding you of the pending return.
	if (%s > 30)
	{
		$ObjData::FandRTimer = %s;
	}
}

// A CTF flag was taken.
function Objective::CTF::onTaken(%t, %p)
{
	$Objective::MissionType = "Capture the Flag";

	%client = Objective::GetClientFromMsg(%p);
	%team = $ObjData::Team[%t];
	%o = "%"@%team@" flag";

	%id = Objective::Set(%o, $Objective::Flag, $Objective::Carry, %client);

	if ($Presto::version == 0.93)
	{
		// Update TeamTrak.
		%name = Client::GetName(%client);
		Team::MoveFlag(%team, %name);
		Event::Trigger(eventFlagTaken, %team, %client);
	}

	Event::Trigger(eventObjectiveFlagTaken, %client, %id, %team, Objective::GetType(%id));
}

// A CTF flag was returned.
function Objective::CTF::onPlayerReturned(%t, %p)
{
	$Objective::MissionType = "Capture the Flag";

	%client = Objective::GetClientFromMsg(%p);
	%team = $ObjData::Team[%t];
	%o = "%"@%team@" flag";

	%id = Objective::Set(%o, $Objective::Flag, $Objective::Neutral, %client);

	if ($Presto::version == 0.93)
	{
		// Update TeamTrak.
		Team::MoveFlag(%team, $Trak::locationHome);
		Event::Trigger(eventFlagReturned, %team, %client);
	}

	Event::Trigger(eventObjectiveFlagReturned, %client, %id, %team, Objective::GetType(%id));
}

// A CTF flag was captured.
function Objective::CTF::onCaptured(%t, %p)
{
	$Objective::MissionType = "Capture the Flag";

	%client = Objective::GetClientFromMsg(%p);
	%team = $ObjData::Team[%t];
	%o = "%"@%team@" flag";

	%id = Objective::Set(%o, $Objective::Flag, $Objective::Neutral, %client);

	%enemy = Client::GetTeam(%client);
	%enemyFlag = "%"@%enemy@" flag";
	%enemyFlagID = $ObjData::[%enemyFlag];

	// We know the flag of the team that captured the enemy flag msut of been at home.
	if ($ObjData::[%enemyFlagID, status] != $Objective::Neutral)
	{
		Objective::Set(%enemyFlag, $Objective::Flag, $Objective::Neutral, "");
	}

	// Request a score update from the server.
	if ($Objective::BWAdmin)
	{
		Objective::TeamScoreUpdate();
	}
	else
	{
		%deltaScore = Objective::GetScore(%id);
		$ObjData::TeamScore[%enemy] += %deltaScore;

		Event::Trigger(eventTeamScoreUpdate, %enemy, $ObjData::TeamScore[%enemy]);
	}

	if ($Presto::version == 0.93)
	{
		// Update TeamTrak.
		Team::MoveFlag(%team, $Trak::locationHome);
		Team::MoveFlag(%enemy, $Trak::locationHome);
		Event::Trigger(eventFlagCaptured, %team, %client);
	}

	Event::Trigger(eventObjectiveFlagCaptured, %client, %id, %team, Objective::GetType(%id));
}

// A CTF flag was dropped.
function Objective::CTF::onDropped(%t, %p)
{
	$Objective::MissionType = "Capture the Flag";

	%client = Objective::GetClientFromMsg(%p);
	%team = $ObjData::Team[%t];
	%o = "%"@%team@" flag";

	%id = Objective::Set(%o, $Objective::Flag, $Objective::Dropped, %client);

	if ($Presto::version == 0.93)
	{
		// Update TeamTrak.
		Team::MoveFlag(%team, $Trak::locationField);
		Event::Trigger(eventFlagDropped, %team, %client);
	}

	Event::Trigger(eventObjectiveFlagDropped, %client, %id, %team, Objective::GetType(%id));
}

// A player left the mission area with a CTF flag.
function Objective::CTF::onPlayerLeftMissionArea(%p)
{
	$ObjData::Client = Objective::GetClientFromMsg(%p);
}

// A CTF flag was returned.
function Objective::CTF::onReturned(%t)
{
	$Objective::MissionType = "Capture the Flag";

	if ($ObjData::Client != "")
	{
		%client = $ObjData::Client;
		$ObjData::Client = "";
	}
	else
	{
		// Server is returning flag after a timeout.
		%client = 0;
	}

	%team = Objective::GetTeamFromMsg(%t);
	%o = "%"@%team@" flag";

	// Only return flag if it's not already been returned.
	%id = $ObjData::[%o];
	if ($ObjData::[%id, status] != $Objective::Neutral)
	{
		%id = Objective::Set(%o, $Objective::Flag, $Objective::Neutral, %client);

		if ($Presto::version == 0.93)
		{
			// Update TeamTrak.
			Team::MoveFlag(%team, $Trak::locationHome);
			Event::Trigger(eventFlagReturned, %team, %client);
		}

		Event::Trigger(eventObjectiveFlagReturned, %client, %id, %team, Objective::GetType(%id));
	}
}

// A team lost some points from losing a vehicle.
function Objective::TAC::onTeamLost(%t, %n)
{
	if (%t == "your team" || %t == "Your team") %t = "your";
	if (%n == "") %n = 2;

	%team = Objective::GetTeamFromMsg(%t);
	$ObjData::TeamScore[%team] -= %n;

	Event::Trigger(eventTeamScoreUpdate, %team, $ObjData::TeamScore[%team]);
}

// A team scored some points from destroying a vehicle.
function Objective::TAC::onTeamScored(%t, %n)
{
	%team = Objective::GetTeamFromMsg(%t);
	$ObjData::TeamScore[%team] += %n;

	Event::Trigger(eventTeamScoreUpdate, %team, $ObjData::TeamScore[%team]);
}

// A team scored some points from destroying a vehicle.
function Objective::TAC::onPlayerDestroyed(%p, %n)
{
	%client = Objective::GetClientFromMsg(%p);
	%team = Client::GetTeam(%client);
	$ObjData::TeamScore[%team] += %n;

	Event::Trigger(eventTeamScoreUpdate, %team, $ObjData::TeamScore[%team]);
}

// A team scored some points from destroying a vehicle.
function Objective::TAC::onTeamDestroyed(%t)
{
	%team = Objective::GetTeamFromMsg(%t);
	%enemy = 1-%team;
	$ObjData::TeamScore[%enemy] += 2;

	Event::Trigger(eventTeamScoreUpdate, %enemy, $ObjData::TeamScore[%enemy]);
}

// Someone on your team destroyed one of your team's APCs.
function Objective::TAC::onTeamDestroyedYour()
{
	%friendly = Team::Friendly();
	$ObjData::TeamScore[%friendly] -= 2;

	Event::Trigger(eventTeamScoreUpdate, %friendly, $ObjData::TeamScore[%friendly]);
}

// A team destroyed one of their own APCs.
function Objective::TAC::onTeamDestroyedTheir(%t)
{
	if (%t == "Your enemy")
	{
		%enemy = Team::Enemy();
	}
	else
	{
		%enemy = $ObjData::Team[%t];
	}

	$ObjData::TeamScore[%enemy] -= 2;

	Event::Trigger(eventTeamScoreUpdate, %enemy, $ObjData::TeamScore[%enemy]);
}

// Parse messages.
function Objective::onClientMessage(%client, %msg)
{
	if (%client != 0)
	{
		return;
	}

	// Watch for a match start and make the objectives neutral in that case.
	if(Match::String(%msg, "Match starts in * seconds.") || %msg=="Match started.")
	{
		if (!$Objective::MatchStarted && $Objective::Loaded && !$Objective::BWAdmin)
		{
			$Objective::MatchStarted = true;
			Objective::MakeNeutral();
		}
		else
		{
			$Objective::MatchStarted = true;
		}
		return;
	}

	//
	// --- Capture and Hold ---
	//

	if (Match::ParamString(%msg, "%p captured %o from the %e team!")
	||  Match::ParamString(%msg, "%p claimed %o for the %f team!"))
	{
		%p = Match::Result(p);  // Player name.
		%o = Match::Result(o);  // Objective name.
		%e = Match::Result(e);  // Team name.

		// TAC bug:
		if (%o == "") %o = "Pulse Sensor";

		Objective::CandH::onCaptured(%o, %p, %e);
		return;
	}

	//
	// --- Capture the Flag ---
	//

	if ($Objective::MissionType == "Capture the Flag" || $Objective::MissionType == "")
	{
		if (Match::ParamString(%msg, "%p took the %t flag! ")) // Note space.
		{
			%p = Match::Result(p);  // Player name.
			%t = Match::Result(t);  // Team name.
			Objective::CTF::onTaken(%t, %p);
			return;
		}

		if (Match::ParamString(%msg, "%p returned the %t flag!"))
		{
			%p = Match::Result(p);  // Player name.
			%t = Match::Result(t);  // Team name.
			Objective::CTF::onPlayerReturned(%t, %p);
			return;
		}

		if (Match::ParamString(%msg, "%p captured the %t flag!"))
		{
			%p = Match::Result(p);  // Player name.
			%t = Match::Result(t);  // Team name.
			Objective::CTF::onCaptured(%t, %p);
			return;
		}

		if (Match::ParamString(%msg, "%p dropped the %t flag!"))
		{
			%p = Match::Result(p);  // Player name.
			%t = Match::Result(t);  // Team name.
			Objective::CTF::onDropped(%t, %p);
			return;
		}
	}

	if ($Objective::MissionType == "Defend and Destroy" || $Objective::MissionType == "")
	{
		if (Match::ParamString(%msg, "%p destroyed an objective!")
		||  Match::ParamString(%msg, "%p destroyed a friendly objective%z"))
		{
			%p = Match::Result(p);  // Player name.
			Objective::DandD::onPlayerDestroyed(%p);
			return;
		}

		if (Match::ParamString(%msg, "%t objective %o destroyed."))
		{
			%o = Match::Result(o);  // Objective name.
			%t = Match::Result(t);  // Team name.
			Objective::DandD::onDestroyed(%o, %t);
			return;
		}
	}

	//
	// --- Find and Retrieve ---
	//

	if ($Objective::MissionType == "Find and Retrieve" || $Objective::MissionType == "")
	{
		if (Match::ParamString(%msg, "%p took %o."))
		{
			%o = Match::Result(o);  // Objective name.
			%p = Match::Result(p);  // Player name.
			Objective::FandR::onTaken(%o, %p);
			return;
		}

		if (Match::ParamString(%msg, "%p dropped %o!"))
		{
			%o = Match::Result(o);  // Objective name.
			%p = Match::Result(p);  // Player name.
			Objective::FandR::onDropped(%o, %p);
			return;
		}

		if (Match::ParamString(%msg, "%p conveyed %o to base."))
		{
			%o = Match::Result(o);  // Objective name.
			%p = Match::Result(p);  // Player name.
			Objective::FandR::onConveyed(%o, %p);
			return;
		}

		if (Match::ParamString(%msg,
			"%o was not put in a flag stand in time!  It was returned to its initial position.")
		||  Match::ParamString(%msg,
			"%p didn't get %o to a flag stand in time!  It was returned to its initial position.")
		||  Match::ParamString(%msg,
			"%p didn't put %o in a flag stand in time!  It was returned to its initial position.")
		||  Match::ParamString(%msg,
			"%p left the mission area while carrying %o!  It was returned to its initial position."))
		{
			%o = Match::Result(o);  // Objective name.
			%p = Match::Result(p);  // Player name.
			Objective::FandR::onReturnToInitial(%o, %p);
			return;
		}

		if (Match::ParamString(%msg, "%p didn't get %o to a flag stand in time!")
		||  Match::ParamString(%msg, "%p didn't put %o in a flag stand in time!"))
		{
			%p = Match::Result(p);  // Player name.
			Objective::FandR::onPlayerReturn(%p);
			return;
		}

		if (Match::ParamString(%msg, "%o was returned to the %t base."))
		{
			%o = Match::Result(o);  // Objective name.
			%t = Match::Result(t);  // Team name.
			Objective::FandR::onReturn(%o, %t);
			return;
		}

		if (Match::ParamString(%msg, "You have %s sec to put the flag in a stand."))
		{
			%s = Match::Result(s);  // Number of seconds.
			Objective::FandR::onYouHave(%s);
			return;
		}
	}

	//
	// --- Shared messages ---
	//

	// These messages are used in both CTF missions and F&R missions.
	// Therefore, we can only use these messages if we know the mission type.
	// This isn't too much of a problem, we will learn the mission type from the other messages.
	if ($Objective::MissionType == "Capture the Flag")
	{
		if (Match::ParamString(%msg, "%p left the mission area while carrying the %t flag!"))
		{
			%p = Match::Result(p);  // Player name.
			Objective::CTF::onPlayerLeftMissionArea(%p);
			return;
		}

		if (Match::ParamString(%msg, "%t flag was returned to base."))
		{
			%t = Match::Result(t);  // Team name.
			Objective::CTF::onReturned(%t);
			return;
		}
	}
	else
	if ($Objective::MissionType == "Find and Retrieve")
	{
		if (Match::ParamString(%msg, "%p left the mission area while carrying the %t flag!"))
		{
			%p = Match::Result(p);  // Player name.
			%t = Match::Result(t);  // Team name.
			Objective::FandR::onPlayerLeftMissionArea(%p, %t);
			return;
		}
	}

	//
	// --- Team Aerial Combat mod ---
	//

	if ($Objective::TAC)
	{
		if (Match::ParamString(%msg, "%t lost %n points because %p crashed a %v.")
		||  Match::ParamString(%msg, "%t team lost %n points from crashing a %v.")
		||  Match::ParamString(%msg, "%p lost %t 2 team points for destroying own team's %v."))
		{
			%t = Match::Result(t);  // Team name.
			%n = Match::Result(n);  // Points value.
			Objective::TAC::onTeamLost(%t, %n);
			return;
		}

		if (Match::ParamString(%msg, "%t team scored %n points because %p destroyed an enemy %v.")
		||  Match::ParamString(%msg, "%t team scored %n points because your enemy lost a %v."))
		{
			%t = Match::Result(t);  // Team name.
			%n = Match::Result(n);  // Points value.
			Objective::TAC::onTeamScored(%t, %n);
			return;
		}

		if (Match::ParamString(%msg, "One of your team's %v was destroyed by %p whos team gained %n points.")
		||  Match::ParamString(%msg, "%p's team scored %n points because %p destroyed an enemy %v."))
		{
			%p = Match::Result(p);  // Player name.
			%n = Match::Result(n);  // Points value.
			Objective::TAC::onPlayerDestroyed(%p, %n);
			return;
		}

		if (Match::ParamString(%msg, "One of %t team's %v was destroyed."))
		{
			%t = Match::Result(t);  // Team name.
			Objective::TAC::onTeamDestroyed(%t);
			return;
		}

		if (Match::ParamString(%msg, "Your teammate destroyed your own %v."))
		{
			Objective::TAC::onTeamDestroyedYour();
			return;
		}

		if (Match::ParamString(%msg, "%t destroyed their own %v."))
		{
			%t = Match::Result(t);  // Team name.
			Objective::TAC::onTeamDestroyedTheir(%t);
			return;
		}
	}

	return;
}

// Print an objectives details.
function Objective::Print(%id)
{
	echo("Objective: ", %id);
	echo("Name: ", $ObjData::[%id, name]);
	echo("Type: ", $ObjData::[%id, type]);
	echo("Client: ", $ObjData::[%id, client]);
	echo("Time: ", $ObjData::[%id, time]);
	echo("Status: ", Objective::GetStatus(%id));
	echo();
}

// Print the details of all the objectives.
function Objective::PrintAll()
{
	for (%id = 0; %id < $ObjData::Num; %id++)
	{
		Objective::Print(%id);
	}
}

// Store the team names.
function Objective::onTeamAdd(%team, %name)
{
	// Adjust team number.
	%team -= 1;

	if (%team == -1)
	{
		%name = "Observers";
	}
	else
	{
		$ObjData::TeamNum++;
		if ($Presto::version == 0.93)
		{
			Team::SetName(%team, %name);
		}
	}

	$ObjData::TeamName[%team] = %name;
	$ObjData::Team[%name] = %team;
}

//
// --- BWAdmin ---
//

// Register with the mod to receive info.
function Objective::Register()
{
	$ObjData::NumTowers = 0;
	remoteEval(2048, bwadmin::reg);
}

// Get a team score update from the server.
function Objective::TeamScoreUpdate()
{
	remoteEval(2048, bwadmin::teamScoreList);
}

// Convert the BWAdmin name to the format ObjectiveTrak uses.
function Objective::ConvertName(%name, %type)
{
	if (Objective::ConvertType(%type) == $Objective::DandD)
	{
		%name = "%"@$ObjData::Team@" "@%name;
	}
	else
	{
		for (%team = 0; %team < $ObjData::TeamNum; %team++)
		{
			%name = String::Replace(%name, $ObjData::TeamName[%team], "%"@%team);
		}
	}

	return %name;
}

// Convert the BWAdmin status to an ObjectiveTrak state.
function Objective::ConvertStatus(%status, %type, %id)
{
	// Check to see if the status is a team name.
	for (%team = 0; %team < $ObjData::TeamNum; %team++)
	{
		if (%status == $ObjData::TeamName[%team])
		{
			if (Objective::ConvertType(%type) == $Objective::DandD)
			{
				%status = $Objective::Neutral;
				$Objective::MissionType = "Defend and Destroy";
			}
			else
			{
				%status = %team;
			}

			$ObjData::Team = %team;
			return %status;
		}
	}

	// If not check to see if it's a player name.
	if (%status == home || %status == unnamed)
	{
		%status = $Objective::Neutral;
	}
	else
	if (%status == dropped)
	{
		%status = $Objective::Dropped;
	}
	else
	{
		$ObjData::[%id, client] = getClientbyName(%status);
		%status = $Objective::Carry;
	}

	return %status;
}

// Test for the mission type.
function Objective::CheckType(%type, %name)
{
	// If the type is a flag, work out if it's a CTF or an F&R flag.
	if (%type == flag)
	{
		for (%team = 0; %team < $ObjData::TeamNum; %team++)
		{
			if (%name == $ObjData::TeamName[%team]@" flag")
			{
				$Objective::MissionType = "Capture the Flag";
			}
		}

		if ($Objective::MissionType == "")
		{
			$Objective::MissionType = "Find and Retrieve";
		}
	}
	else
	// Count the number of towers, if there are only towers then it's a C&H mission.
	if (%type == "TowerSwitch")
	{
		$ObjData::NumTowers++;
	}
}

// Recieve objective info.
function remotebwadmin::setObjList(%server, %id, %total, %name, %type, %status)
{
	if (%server == 2048)
	{
		echo("Objective: ", %id, ", Name: ",%name,", Type: ",%type," Status: ",%status);

		if (%type == "Flag")
		{
			%type = "flag";
		}

		Objective::CheckType(%type, %name);

		%status = Objective::ConvertStatus(%status, %type, %id);
		%name = Objective::ConvertName(%name, %type);
		$ObjData::[%id, name] = %name;
		$ObjData::[%id, type] = %type;
		$ObjData::[%id, status] = %status;
		$ObjData::[%name] = %id;
		$ObjData::[%id, score] = "1 0";

		$ObjData::TypeNum[%type]++;
		$ObjData::TypeNum[Objective::ConvertType(%type)]++;

		// Last objective in the list.
		if (%id == %total-1)
		{
			// If we couldn't figure the mission type but there were towers then it must be C&H.
			if ($Objective::MissionType == "" && $ObjData::NumTowers > 0)
			{
				$Objective::MissionType = "Capture and Hold";
				for (%i = 0; %i < %total; %i++)
				{
					$ObjData::[%i, score] = "0 12";
				}
			}

			// If we still don't know the mission type just use $ServerMissionType.
			if ($Objective::MissionType == "")
			{
				$Objective::MissionType = $ServerMissionType;
			}

			$ObjData::Num = %total;
			Objective::MakeAbbreviations();
			$Objective::Loaded = true;
			Event::Trigger(eventObjectiveLoaded, %total);

			// If we're playing a C&H map then update the score periodically.
			if ($Objective::MissionType == "Capture and Hold")
			{
				Schedule::Add("Objective::_TeamScoreUpdate();", $Objective::UpdateWait);
			}
		}
	}
}

// Get team score info.
function remotebwadmin::setTeamScoreList(%server, %team, %numTeams, %name, %players, %score, %winscore)
{
	if (%server == 2048)
	{
		echo("Team: ", %team,
			 ", Name: ", %name,
			 ", Players: ", %players,
			 ", TeamScore: ", %score,
			 ", Winscore: ", %winscore);

		// Check for disabled score limit.
		if (%winscore == false)
		{
			$ObjData::WinScore = 999;
		}
		else
		// Check for no score limit.
		if (%winscore == "")
		{
			$ObjData::WinScore = 0;
		}
		else
		{
			$ObjData::WinScore = %winscore;
		}

		$ObjData::TeamScore[%team] = %score;

		Event::Trigger(eventTeamScoreUpdate, %team, %score);
	}
}

//
// --- Abbreviations ---
//

// Setup abreviations.
function Objective::AbbreviationReset()
{
	$ObjAbrv::Num = 0;
}
function Objective::Abbreviation(%name, %len, %abrv)
{
	%i = $ObjAbrv::Num;
	$ObjAbrv::[%i] = %name;
	$ObjAbrv::[%i, len] = %len;
	$ObjAbrv::[%i, abrv] = %abrv;
	$ObjAbrv::Num++;
}

// Set a team name abbreviation.
function Objective::TeamAbbreviation(%name, %level1, %level2, %level3)
{
	$ObjAbrv::[%name, level1] = %level1;
	$ObjAbrv::[%name, level2] = %level2;
	$ObjAbrv::[%name, level3] = %level3;
}

// Abbreviate objective names.
function Objective::Abbreviate(%msg)
{
	%abrv = true;
	while(%abrv)
	{
		// If this is still false at end of loop we didn't find any more occurances so we can exit.
		%abrv = false;
		for(%i = 0; %i < $ObjAbrv::Num; %i++)
		{
			if((%pos = String::findSubStr(%msg, $ObjAbrv::[%i])) != -1)
			{
				%msg = String::getSubStr(%msg, 0, %pos) @
					   $ObjAbrv::[%i, abrv] @
					   String::getSubStr(%msg, %pos + $ObjAbrv::[%i, len], 256);
				%abrv = true;
			}
		}
	}

	return %msg;
}

// Make up the abbreviations once so we don't have to do this expensive operation all the time.
function Objective::MakeAbbreviations()
{
	for(%id = 0; %id < $ObjData::Num; %id++)
	{
		$ObjData::[%id, abrv] = Objective::Abbreviate(Objective::GetNameStripped(%id));
	}
}

Include(File::FindFirst("*ObjectiveAbbreviations.cs"));

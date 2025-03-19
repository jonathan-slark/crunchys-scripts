// -- Waypoint Manager -- v1.2 -----------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
//
// *** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
// *** Requires Zear's NewOpts v0.966 or later *** http://www.planetstarsiege.com/zear/
// *** Requires my ObjectiveTrak script v3.2 or later ***
//
//
// Changes in v1.2:
//  + Option for a default waypoint to be set instead of clearing the waypoint.
//  + Option to hide Tribes CompassHUD when there is no waypoint set.
//  + Added escort last player to speak.  Like the standard escort but updates the HUD and the waypoint is
//    added to the list of waypoints.
//  + Added some sounds to the waypoints.
//
// Changes in v1.1:
//  + Option to properly clear the waypoint, which avoids the flickery compass.  Caveat is that it forces
//    a team message, this is filtered out for anyone who has the script.
//  + Filters out MrPoops clear message "!AP Off".
//  + All-in-one Waypoint Menu, which saves having to use seperate keys.
//
// Changes in v1.0.1:
//  = Fixed a bug where the HUD wouldn't work for some ppl.
//

//
// --- Header ---
//

// Dirs
$Waypoint::dir = "";
$Waypoint::BMPdir = "";
$Waypoint::GUIdir = "";

// HUD bmps
$WaypointHUD::FriendlyBase    = $Waypoint::BMPdir@"Waypoint_FriendlyBase";
$WaypointHUD::EnemyBase       = $Waypoint::BMPdir@"Waypoint_EnemyBase";
$WaypointHUD::FriendlyCarrier = $Waypoint::BMPdir@"Waypoint_FriendlyCarrier";
$WaypointHUD::EnemyCarrier    = $Waypoint::BMPdir@"Waypoint_EnemyCarrier";
$WaypointHUD::Objective       = $Waypoint::BMPdir@"Waypoint_Switch";
$WaypointHUD::Player          = $Waypoint::BMPdir@"Waypoint_Player";
$WaypointHUD::NoWaypoint      = $Waypoint::BMPdir@"Waypoint_None";

// Includes
Include("Presto\\Event.cs");
Include("Presto\\TeamTrak.cs");
Include("Crunchy\\ObjectiveTrak.cs");

// Events
Event::Attach(eventConnected, "Waypoint::Reset();");
Event::Attach(eventChangeMission, "Waypoint::Reset();");
Event::Attach(eventClientChangeTeam, Waypoint::onChangeTeam);
Event::Attach(eventExtendedClientMessage, Waypoint::onClientMessage);
Event::Attach(eventObjectiveLoaded, Waypoint::onObjectiveLoaded);

// Messages.
$Waypoint::MsgEscortCarrier = "Escort the flag carrier %p!~wescfr";
$Waypoint::MsgKillCarrier   = "Kill the flag carrier %p!~wattway";
$Waypoint::MsgObjective     = "Waypoint set to %o.";
$Waypoint::MsgFlagDropped   = "%p dropped %t flag!";
$Waypoint::MsgFlagCaptured  = "%p captured %t flag!";
$Waypoint::MsgFlagReturned  = "%p returned %t flag!";
$Waypoint::MsgFlagReturned0 = "%t flag was returned to base.";
$Waypoint::MsgEscort        = "Escort %p.~wescfr";
$Waypoint::MsgAttack        = "Attack %p.~wattway";
$Waypoint::MsgNoWaypoint    = "No waypoint set.";
$Waypoint::MsgEscorting     = "I'm escorting you %p!";

// Length of time to protect from flood for.
$Waypoint::EscortingFlood = 30;
// Message tags.
$Waypoint::ClearTag = "~wp_clear";
$Waypoint::PoopTag = "!AP Off";

//EditActionMap("playMap.sae");
//bindCommand(keyboard0, make, "n", TO, "Waypoint::FlagCarrier(Team::Friendly());");
//bindCommand(keyboard0, make, "m", TO, "Waypoint::FlagCarrier(Team::Enemy(), true);");
//bindCommand(keyboard0, make, "j", TO, "Waypoint::FlagBase(Team::Enemy());");
//bindCommand(keyboard0, make, "k", TO, "Waypoint::FlagBase(Team::Friendly());");
//bindCommand(keyboard0, make, "enter", TO, "Waypoint::Remove();");
//bindCommand(keyboard0, make, "backspace", TO, "Waypoint::Cycle();");
//bindCommand(keyboard0, make, "o", TO, "Menu::Display(menuObjective);");
//bindCommand(keyboard0, make, "l", TO, "Menu::Display(menuWaypoint);");
//bindCommand(keyboard0, make, control, "e", TO, "Waypoint::Escort();");

function Waypoint::SetDefaults()
{
	$CrunchyPref::Waypoint::ClearMessages = "false";
	$CrunchyPref::Waypoint::UseDefault = "true";
	$CrunchyPref::Waypoint::Default = "enemyBase";
	$CrunchyPref::Waypoint::Automate = "true";
	$CrunchyPref::Waypoint::HUD = "true";
	$CrunchyPref::Waypoint::HUDPos = "100% 70% 40 24";
	$CrunchyPref::Waypoint::Escorting = true;
	$CrunchyPref::Waypoint::AlternateClear = true;
	$CrunchyPref::Waypoint::HideCompass = "false";
}
if (isFile("config\\CrunchyPrefs.cs"))
{
	Include("CrunchyPrefs.cs");
}
if ($CrunchyPref::Waypoint::UseDefault == "")
{
	Waypoint::SetDefaults();
}

//
// --- Setting waypoints ---
//

function Waypoint::HideCompass()
{
	Control::SetVisible(compassHUD, false);
}

function Waypoint::ShowCompass()
{
	Control::SetVisible(compassHUD, true);
}

// Clear a waypoint.
// Implemented by waypointing self to avoid a team message.
// Alternate method clears the waypoint but forces a team say.
function Waypoint::Clear(%msg)
{
	if(%msg == "") %msg = "Waypoint cleared.";

	if ($CrunchyPref::Waypoint::UseDefault
	&& Objective::GetMissionType() == "Capture the Flag"
	&& Team::Friendly() != -1)
	{
		if ($CrunchyPref::Waypoint::Default == "enemyBase")
		{
			%id = Objective::GetFlagID(Team::Enemy());
		}
		else
		{
			%id = Objective::GetFlagID(Team::Friendly());
		}
		%x = Objective::GetLocationX(%id);
		%y = Objective::GetLocationY(%id);
		%msg = String::Replace($Waypoint::MsgObjective, "%o", Objective::GetName(%id));
		%msg = Objective::InsertTeamName(%msg);
		Waypoint::Location(%x, %y, %msg);

		if ($CrunchyPref::Waypoint::HideCompass)
		{
			Waypoint::ShowCompass();
		}
	}
	else
	if ($CrunchyPref::Waypoint::AlternateClear)
	{
		remoteEval(2048, "CStatus", 0, $Waypoint::ClearTag);

		if ($CrunchyPref::Waypoint::HideCompass)
		{
			Waypoint::HideCompass();
		}
	}
	else
	{
		%me = getManagerID();
		remoteEval(2048, "IssueTargCommand", 0, %msg, %me - 2048, %me);

		if ($CrunchyPref::Waypoint::HideCompass)
		{
			Waypoint::HideCompass();
		}
	}

	HUD::Update(WaypointHUD);

	Event::Trigger(eventWaypointClear, %msg);
}

// Waypoint a player by name.
function Waypoint::Player(%name, %msg, %icon)
{
	if(%icon == "") %icon = 1;
	if(%msg == "") %msg = "Waypoint set to "@%name@".";

	%clientId = getClientByName(%name);
	remoteEval(2048, "IssueTargCommand", %icon, %msg, %clientID - 2048, getManagerId());

	Event::Trigger(eventWaypointPlayer, %name, %msg, %icon);
}

// Waypoint a player by client id.
function Waypoint::Client(%client, %msg, %icon)
{
	if(%icon == "") %icon = 1;
	if(%msg == "") %msg = "Waypoint set to "@Client::GetName(%client)@".";

	remoteEval(2048, "IssueTargCommand", %icon, %msg, %client - 2048, getManagerId());

	Event::Trigger(eventWaypointClient, %client, %msg, %icon);
}

// Waypoint a location on the PDA map.
function Waypoint::Location(%x, %y, %msg, %icon)
{
	if(%icon == "") %icon = 5;
	if(%msg == "") %msg = "Waypoint set.";

	remoteEval(2048, "IssueCommand", %icon, %msg, %x, %y, getManagerId());

	Event::Trigger(eventWaypointLocation, %x, %y, %msg, %icon);
}

// Waypoint an objective.
function Waypoint::Objective(%id, %bmp, %msg, %icon)
{
	if (%msg == "") %msg = String::Replace($Waypoint::MsgObjective, "%o", Objective::GetName(%id));
	%msg = Objective::InsertTeamName(%msg);
	if (%bmp == "") %bmp = $WaypointHUD::Objective;

	%x = Objective::GetLocationX(%id);
	%y = Objective::GetLocationY(%id);
	Waypoint::AddHead(objective@%id, %msg, %bmp, %icon, %x, %y);
}

// Waypoint the carrier of the %team's flag.
function Waypoint::FlagCarrier(%team, %escort, %icon)
{
	%id = Objective::GetFlagID(%team);
	%client = Objective::GetClient(%id);

	// Don't waypoint self.
	if (%client == getManagerID()) return;

	%status = Objective::GetStatus(%id);
	if (%status == $Objective::FriendlyCarry)
	{
		%name = Client::GetName(%client);
		%msg = String::Replace($Waypoint::MsgEscortCarrier, "%p", %name);
		Waypoint::AddHead(flagCarrier@%team, %msg, $WaypointHUD::FriendlyCarrier, %icon, %client);

		echo(%escort);

		if (%escort
		&&  $CrunchyPref::Waypoint::Escorting
		&&  Flood::Protect("Waypoint::Escorting", $Waypoint::EscortingFlood))
		{
			%msg = String::Replace($Waypoint::MsgEscorting, "%p", %name);
			schedule("Say::Team(targetAcquired, \""@%msg@"\");", 1);
		}
	}
	else
	if (%status == $Objective::EnemyCarry)
	{
		%name = Client::GetName(%client);
		%msg = String::Replace($Waypoint::MsgKillCarrier, "%p", %name);
		Waypoint::AddHead(flagCarrier@%team, %msg, $WaypointHUD::EnemyCarrier, %icon, %client);
	}
}

// Waypoint %team's flag base.
function Waypoint::FlagBase(%team, %icon)
{
	%id = Objective::GetFlagID(%team);

	if (%team == Team::Friendly())
	{
		%bmp = $WaypointHUD::FriendlyBase;
	}
	else
	{
		%bmp = $WaypointHUD::EnemyBase;
	}

	Waypoint::Objective(%id, %bmp);
}

// Escort the last player to talk.
function Waypoint::Escort()
{
	%client = $Waypoint::LastClient;
	if (%client != "")
	{
		if (Team::Friendly() == Team::Friendly(%client))
		{
			%msg = String::Replace($Waypoint::MsgEscort, "%p", Client::GetName(%client));
		}
		else
		{
			%msg = String::Replace($Waypoint::MsgAttack, "%p", Client::GetName(%client));
		}
		Waypoint::AddHead(player@%client, %msg, $WaypointHUD::Player, "", %client);
	}
}

//
// --- Waypoint list ---
// (Doubly linked list)
//

// Reset the list of waypoints.
function Waypoint::Reset()
{
	deleteVariables("$WayData::*");
	HUD::Update(WaypointHUD);
}

// Add a waypoint to the head of the list.
// This forces this waypoint to be set as the current waypoint.
function Waypoint::AddHead(%way, %msg, %bmp, %icon, %x, %y)
{
	// Remove the waypoint if it already exists.
	if ($WayData::[%way, x] != "")
	{
		Waypoint::Remove(%way, true);
	}

	$WayData::[%way, msg] = %msg;
	$WayData::[%way, bmp] = %bmp;
	$WayData::[%way, icon] = %icon;
	$WayData::[%way, x] = %x;
	$WayData::[%way, y] = %y;

	%first = $WayData::Head;
	if (%first != "")
	{
		$WayData::[%way, next] = %first;
		$WayData::[%first, prev] = %way;
	}
	$WayData::Head = %way;
	if ($WayData::Last == "")
	{
		$WayData::Last = %way;
	}
	Waypoint::Set(%way);
}

// Add a waypoint to the tail of the list.
// This puts the waypoint at the back of the queue of waypoints.
function Waypoint::AddTail(%way, %msg, %bmp, %icon, %x, %y)
{
	// Remove the waypoint if it already exists.
	if ($WayData::[%way, x] != "")
	{
		Waypoint::Remove(%way, true);
	}

	$WayData::[%way, msg] = %msg;
	$WayData::[%way, bmp] = %bmp;
	$WayData::[%way, icon] = %icon;
	$WayData::[%way, x] = %x;
	$WayData::[%way, y] = %y;

	%last = $WayData::Last;
	if (%last != "")
	{
		$WayData::[%last, next] = %way;
		$WayData::[%way, prev] = %last;
	}
	$WayData::Last = %way;
	if ($WayData::Head == "")
	{
		$WayData::Head = %way;

		// If we have the flag don't set the waypoint.
		// Musn't interere with Tribes waypoint system.
		%loc = Team::GetFlagLocation(Team::Enemy());
		if (%loc != Client::GetName(getManagerID()) || !$pref::autoWaypoint)
		{
			Waypoint::Set(%way);
		}
	}
}

// Take the tail waypoint and stick it at the head.
function Waypoint::Cycle()
{
	%way = $WayData::Last;

	if (%way != "")
	{
		%msg = $WayData::[%way, msg];
		%bmp = $WayData::[%way, bmp];
		%icon = $WayData::[%way, icon];
		%x = $WayData::[%way, x];
		%y = $WayData::[%way, y];
		Waypoint::AddHead(%way, %msg, %bmp, %icon, %x, %y);
	}
}

// Remove a waypoint, if this is the last waypoint clear the waypoint with message %msg.
// If %way is null then the head of the list is removed.
function Waypoint::Remove(%way, %noclear, %msg)
{
	if (%msg == "") %msg = $Waypoint::MsgNoWaypoint;
	if (%way == "")
	{
		%way = $WayData::Head;
		%force = true;
	}
	else
	{
		%force = false;
	}
	%cleared = %noclear;

	// Only remove a valid waypoint.
	if ($WayData::[%way, x] == "") return;

	%prev = $WayData::[%way, prev];
	%next = $WayData::[%way, next];

	%loc = Team::GetFlagLocation(Team::Enemy());
	%myname = Client::GetName(getManagerID());

	// If there is no previous waypoint then this is the head.
	if (%prev == "")
	{
		$WayData::Head = %next;

		// If we have the flag don't set the waypoint.
		if (%loc != %myname || %force || !$pref::autoWaypoint)
		{
			// If there are no waypoints then clear the current one.
			if (%next == "")
			{
				if (!%cleared)
				{
					Waypoint::Clear(%msg);
				}
				%cleared = true;
			}
			// Else set this one.
			else
			{
				Waypoint::Set(%next);
			}
		}
	}
	else
	{
		$WayData::[%prev, next] = %next;
	}

	// Check the tail.
	if (%next == "")
	{
		$WayData::Last = %prev;

		// If we have the flag don't set the waypoint.
		if (%loc != %myname || %force || !$pref::autoWaypoint)
		{
			if (%prev == "")
			{
				if (!%cleared)
				{
					Waypoint::Clear(%msg);
				}
				%cleared = true;
			}
			else
			{
				Waypoint::Set(%prev);
			}
		}
	}
	else
	{
		$WayData::[%next, prev] = %prev;
	}

	// Wipe the waypoints data to make sure it's not used erroneously.
	$WayData::[%way, x] = "";
	$WayData::[%way, prev] = "";
	$WayData::[%way, next] = "";
}

// Set the waypoint.
function Waypoint::Set(%way)
{
	%msg = $WayData::[%way, msg];
	%icon = $WayData::[%way, icon];
	%x = $WayData::[%way, x];
	%y = $WayData::[%way, y];

	// If there is no y then x is actualy the client id.
	if (%y == "")
	{
		Waypoint::Client(%x, %msg, %icon);
	}
	else
	{
		Waypoint::Location(%x, %y, %msg, %icon);
	}

	if ($CrunchyPref::Waypoint::HideCompass)
	{
		Waypoint::ShowCompass();
	}

	HUD::Update(WaypointHUD);
}

// Print a waypoint's data.
function Waypoint::Print(%way)
{
	echo("Waypoint: ", %way);
	echo("Msg: ", $WayData::[%way, msg]);
	echo("Icon: ", $WayData::[%way, icon]);
	echo("X: ", $WayData::[%way, x]);
	echo("Y: ", $WayData::[%way, y]);
	echo();
}

// Print the list of waypoints.
function Waypoint::PrintAll()
{
	%way = $WayData::Head;
	while(%way != "")
	{
		Waypoint::Print(%way);

		%way = $WayData::[%way, next];
	}
}

// Print the list of waypoints, but starting from the tail and working backwards.
// Useful to check the structures integrety.
function Waypoint::PrintAllBack()
{
	%way = $WayData::Last;
	while(%way != "")
	{
		Waypoint::Print(%way);

		%way = $WayData::[%way, prev];
	}
}

//
// --- Events ---
//

// Allow enabling and disabling of the automated waypointing by attaching and detaching events.
function Waypoint::SetupEvents()
{
	if ($CrunchyPref::Waypoint::Automate)
	{
		Event::Attach(eventObjectiveFlagTaken, Waypoint::onFlagTaken);
		Event::Attach(eventObjectiveFlagDropped, Waypoint::onFlagDropped);
		Event::Attach(eventObjectiveFlagCaptured, Waypoint::onFlagCaptured);
		Event::Attach(eventObjectiveFlagReturned, Waypoint::onFlagReturned);
	}
	else
	{
		Event::Detach(eventObjectiveFlagTaken, Waypoint::onFlagTaken);
		Event::Detach(eventObjectiveFlagDropped, Waypoint::onFlagDropped);
		Event::Detach(eventObjectiveFlagCaptured, Waypoint::onFlagCaptured);
		Event::Detach(eventObjectiveFlagReturned, Waypoint::onFlagReturned);
	}
}
Waypoint::SetupEvents();

// Returns either "You" or the player's name.
function Waypoint::GetPlayerName(%client, %pos)
{
	if (%pos == "") %pos = 0;

	if (%client == getManagerID())
	{
		if (%pos == 0)
		{
			return "You";
		}
		else
		{
			return "you";
		}
	}
	else
	{
		Client::GetName(%client);
	}
}

// Get a team name.
function Waypoint::GetTeamName(%team, %pos)
{
	if (%pos == "") %pos = 0;

	if (%pos == 0)
	{
		return "The "@Team::GetName(%team);
	}
	else
	{
		return "the "@Team::GetName(%team);
	}
}

// When a flag is taken set a waypoint to it.
function Waypoint::onFlagTaken(%client, %id, %team, %type)
{
	if (%client == getManagerID())
	{
		if ($CrunchyPref::Waypoint::HideCompass)
		{
			Waypoint::ShowCompass();
		}
	}
	else
	if (%team == Team::Friendly())
	{
		%name = Client::GetName(%client);
		%msg = String::Replace($Waypoint::MsgKillCarrier, "%p", %name);

		%way = flagCarrier@%team;
		// Only set the waypoint if it doesn't exist already.
		if ($WayData::[%way, x] == "")
		{
			Waypoint::AddTail(%way, %msg, $WaypointHUD::EnemyCarrier, "", %client);
		}
	}
	else
	{
		%name = Client::GetName(%client);
		%msg = String::Replace($Waypoint::MsgEscortCarrier, "%p", %name);

		%way = flagCarrier@%team;
		// Only set the waypoint if it doesn't exist already.
		if ($WayData::[%way, x] == "")
		{
			Waypoint::AddTail(%way, %msg, $WaypointHUD::FriendlyCarrier, "", %client);
		}
	}
}

// Clear the taken flag's waypoint.
function Waypoint::onFlagDropped(%client, %id, %team, %type)
{
	if (%client == getManagerID())
	{
		// Revert back to Waypoint Manager.
		if ($WayData::Head != "")
		{
			Waypoint::Set($WayData::Head);
		}
		else
		{
			Waypoint::Clear($Waypoint::MsgNoWaypoint);
		}
	}
	else
	{
		if ($CrunchyPref::Waypoint::ClearMessages)
		{
			%msg = $Waypoint::MsgFlagDropped;
			%pos = String::findSubStr(%msg, "%t");
			%msg = String::Replace(%msg, "%t", Waypoint::GetTeamName(%team, %pos));
			%msg = String::Replace(%msg, "%p", Waypoint::GetPlayerName(%client));
		}
		else
		{
			%msg = "";
		}

		Waypoint::Remove(flagCarrier@%team, false, %msg);
	}
}

// Clear the taken flag's waypoint.
function Waypoint::onFlagReturned(%client, %id, %team, %type)
{
	if (%client == getManagerID())
	{
		// TCLIB
	}
	else
	{
		if ($CrunchyPref::Waypoint::ClearMessages)
		{
			if (%client == 0)
			{
				%msg = $Waypoint::MsgFlagReturned0;
			}
			else
			{
				%msg = $Waypoint::MsgFlagReturned;
				%msg = String::Replace(%msg, "%p", Waypoint::GetPlayerName(%client));
			}
			%pos = String::findSubStr(%msg, "%t");
			%msg = String::Replace(%msg, "%t", Waypoint::GetTeamName(%team, %pos));
		}
		else
		{
			%msg = "";
		}

		Waypoint::Remove(flagCarrier@%team, false, %msg);
	}
}

// Clear the taken flag's waypoint.
function Waypoint::onFlagCaptured(%client, %id, %team, %type)
{
	if (%client == getManagerID())
	{
		// Revert back to Waypoint Manager.
		if ($WayData::Head != "")
		{
			Waypoint::Set($WayData::Head);
		}
		else
		{
			Waypoint::Clear($Waypoint::MsgNoWaypoint);
		}
	}
	else
	{
		if ($CrunchyPref::Waypoint::ClearMessages)
		{
			%msg = $Waypoint::MsgFlagCaptured;
			%msg = String::Replace(%msg, "%p", Waypoint::GetPlayerName(%client));
			%pos = String::findSubStr(%msg, "%t");
			%msg = String::Replace(%msg, "%t", Waypoint::GetTeamName(%team, %pos));
		}
		else
		{
			%msg = "";
		}

		Waypoint::Remove(flagCarrier@%team, false, %msg);
	}
}

// Clear all waypoints when the player changes team.
function Waypoint::onChangeTeam(%client, %team)
{
	if (%client == getManagerID())
	{
		Waypoint::Reset();
		if ($Objective::Loaded)
		{
			Waypoint::Clear($Waypoint::MsgNoWaypoint);
		}
	}
}

// Mute other players clear messages.
// Doesn't seem to work for this client (!).
function Waypoint::onClientMessage(%client, %msg, %text)
{
	// Don't parse server messages.
	if (%client != 0)
	{
		$Waypoint::LastClient = %client;

		if (%msg == $Waypoint::ClearTag || %msg == $Waypoint::PoopTag)
		{
			echo("MUTED: Waypoint clear message");
			return mute;
		}
	}

	return true;
}

// Clear waypoint when the objectives are loaded.
// This is important if a default waypoint needs to be set.
function Waypoint::onObjectiveLoaded(%num)
{
	Waypoint::Clear($Waypoint::MsgNoWaypoint);
}

//
// --- HUD ---
//

// Update the HUD.
function WaypointHUD::Update()
{
	if ($WayData::Head == "")
	{
		%bmp = $WaypointHUD::NoWaypoint;
	}
	else
	{
		%bmp = $WayData::[$WayData::Head, bmp];
	}

	HUD::AddTextLine(WaypointHUD, "<B3,4:"@%bmp@".bmp>");

	// Only update when prompted.
	return 0;
}

// Either create or update the HUD.
if(HUD::Exists(WaypointHUD))
{
	HUD::Update(WaypointHUD);
}
else
{
	HUD::New(WaypointHUD, WaypointHUD::Update, $CrunchyPref::Waypoint::HUDPos);
}

// Toggle the HUD according to the user pref.
HUD::Display(WaypointHUD, $CrunchyPref::Waypoint::HUD);

//
// --- Objective menu ---
//

// Go through the template enabling and renaming slots for the objectives.
function Waypoint::InitMenu()
{
	for (%i = 0; %i < Objective::Number(); %i++)
	{
		%name = Objective::GetNameStripped(%i);
		%name = Objective::InsertTeamName(%name);
		Menu::SetText(menuObjective, %i, %name);
		Menu::SetEnabled(menuObjective, %i, true);
	}

	for (%i = Objective::Number(); %i < 20; %i++)
	{
		Menu::SetEnabled(menuObjective, %i, false);
	}
}

function Waypoint::InitWaypointMenu()
{
	%client = $Waypoint::LastClient;
	if (%client != "")
	{
		if (Team::Friendly() == Team::Friendly(%client))
		{
			%msg = String::Replace($Waypoint::MsgEscort, "%p", Client::GetName(%client));
		}
		else
		{
			%msg = String::Replace($Waypoint::MsgAttack, "%p", Client::GetName(%client));
		}
		%pos = String::findSubStr(%msg, ".");
		if (%pos != -1)
		{
			%msg = String::GetSubStr(%msg, 0, %pos);
		}
		Menu::SetText(menuWaypoint, 1, %msg);
	}
	else
	{
		Menu::SetText(menuWaypoint, 1, "Escort Player");
	}
}

// Objective menu template.
Menu::New(menuObjective, "Waypoint objective", "Waypoint::InitMenu();");
	Menu::AddChoice(menuObjective, "1Objective 0", "Waypoint::Objective(0);");
	Menu::AddChoice(menuObjective, "2Objective 1", "Waypoint::Objective(1);");
	Menu::AddChoice(menuObjective, "3Objective 2", "Waypoint::Objective(2);");
	Menu::AddChoice(menuObjective, "4Objective 3", "Waypoint::Objective(3);");
	Menu::AddChoice(menuObjective, "5Objective 4", "Waypoint::Objective(4);");
	Menu::AddChoice(menuObjective, "6Objective 5", "Waypoint::Objective(5);");
	Menu::AddChoice(menuObjective, "7Objective 6", "Waypoint::Objective(6);");
	Menu::AddChoice(menuObjective, "8Objective 7", "Waypoint::Objective(7);");
	Menu::AddChoice(menuObjective, "9Objective 8", "Waypoint::Objective(8);");
	Menu::AddChoice(menuObjective, "0Objective 9", "Waypoint::Objective(9);");
	Menu::AddChoice(menuObjective, "qObjective 10", "Waypoint::Objective(10);");
	Menu::AddChoice(menuObjective, "wObjective 11", "Waypoint::Objective(11);");
	Menu::AddChoice(menuObjective, "eObjective 12", "Waypoint::Objective(12);");
	Menu::AddChoice(menuObjective, "rObjective 13", "Waypoint::Objective(13);");
	Menu::AddChoice(menuObjective, "tObjective 14", "Waypoint::Objective(14);");
	Menu::AddChoice(menuObjective, "yObjective 15", "Waypoint::Objective(15);");
	Menu::AddChoice(menuObjective, "uObjective 16", "Waypoint::Objective(16);");
	Menu::AddChoice(menuObjective, "iObjective 17", "Waypoint::Objective(17);");
	Menu::AddChoice(menuObjective, "oObjective 18", "Waypoint::Objective(18);");
	Menu::AddChoice(menuObjective, "pObjective 19", "Waypoint::Objective(19);");

// All-in-one menu.
Menu::New(menuWaypoint, "Waypoint menu", "Waypoint::InitWaypointMenu();");
	Menu::AddMenu(menuWaypoint, "o", menuObjective);
	Menu::AddChoice(menuWaypoint, "eEscort Player", "Waypoint::Escort();");
	Menu::AddChoice(menuWaypoint, "nEnemy Carrier", "Waypoint::FlagCarrier(Team::Friendly());");
	Menu::AddChoice(menuWaypoint, "mFriendly Carrier", "Waypoint::FlagCarrier(Team::Enemy(), true);");
	Menu::AddChoice(menuWaypoint, "jEnemy Flag Base", "Waypoint::FlagBase(Team::Enemy());");
	Menu::AddChoice(menuWaypoint, "kFriendly Flag Base", "Waypoint::FlagBase(Team::Friendly());");
	Menu::AddChoice(menuWaypoint, "rRemove Waypoint", "Waypoint::Remove();");
	Menu::AddChoice(menuWaypoint, "cCycle Waypoints", "Waypoint::Cycle();");

//
// --- NewOpts ---
//

$Waypoint::Default[0] = "enemyBase";
$Waypoint::Default[1] = "friendlyBase";
$Waypoint::Default["enemyBase"] = 0;
$Waypoint::Default["friendlyBase"] = 1;

// Setup options page.
function Waypoint::onOpen()
{
	Control::setActive(Way::comboDefault, $CrunchyPref::Waypoint::UseDefault);
	Control::setActive(Way::checkClearMsg, !$CrunchyPref::Waypoint::AlternateClear);
	//Control::setActive(Way::checkAlternate, !$CrunchyPref::Waypoint::UseDefault);
	//Control::setActive(Way::checkClearMsg, !($CrunchyPref::Waypoint::AlternateClear || $CrunchyPref::Waypoint::UseDefault));

	FGCombo::clear(Way::comboDefault);
	FGCombo::addEntry(Way::comboDefault, "Enemy Base", 0);
	FGCombo::addEntry(Way::comboDefault, "Friendly Base", 1);
	FGCombo::setSelected(Way::comboDefault,$Waypoint::Default[$CrunchyPref::Waypoint::Default]);
}

// Update preferences and HUD if needed.
function Waypoint::onClose()
{
	$CrunchyPref::Waypoint::Default = $Waypoint::Default[FGCombo::getSelected(Way::comboDefault)];
	export("$CrunchyPref::*", "config\\CrunchyPrefs.cs", false);
	HUD::Update(WaypointHUD);
	HUD::Display(WaypointHUD, $CrunchyPref::Waypoint::HUD);
	Waypoint::SetupEvents();
	Waypoint::Reset();
	Waypoint::Clear($Waypoint::MsgNoWaypoint);

	if (!$CrunchyPref::Waypoint::HideCompass)
	{
		Waypoint::ShowCompass();
	}
}

// Options page for Zear's NewOpts
NewOpts::register("Waypoint Manager",
				  $Waypoint::GUIdir@"WaypointOptions.gui",
				  "Waypoint::onOpen();",
				  "Waypoint::onClose();",
				  TRUE);
Include($Waypoint::dir@"WaypointManagerHelp.cs");

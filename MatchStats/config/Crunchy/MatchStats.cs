// -- Match Stats -- v1.0 -----------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Email: jonslark@barrysworld.co.uk
//
// [BR]Afty requested this script for possible use in UKTL matches.  It records all kills and deaths in a
// match.  Also records who caps and who returns the flags.  The script uses Presto's KillTrak and TeamTrak
// to do the tracking.  It is intended that the admin for a match would use the script so that stats could be
// produced for a webpage.
//
// Make sure you have kill traking enabled in presto\PrestoPrefs.cs.  The script logs kills all the time, to
// reset the kills press the reset key (default alt-r), to save the kills press the save key (default alt-k).
// An admin would press alt-r at the start of the match and then alt-k at the end.  This could be automated
// but using this method the admin has control on exactly when the kill tracking is started and stopped.  The
// kills are saved to the file temp\KillsLog.cs.
//
// Interpreting the kill log
// -------------------------
//
// This example log is a snippet from one of HH's UKTL matches against McM. Yes, I did have more than 2
// kills, the full version is far too long to use an example.  The full log is 50k, which I was able to
// record off a demo.
//
//	$KillsLog::2051_name = "|HH|Crunchy";
//	$KillsLog::2051_TEAM = "0";
//		The log is recorded using the client number of the player as names can have spaces.  This line tells
//		us the name of the player with the client number 2051.  The players team is stored as the team
//		number.
//	$KillsLog::TEAM_0 = "Head Hunters";
//	$KillsLog::TEAM_1 = "McMental";
//		The name of team 0 is Head Hunters, team 1 McMental.  As the player was team 0 we can tell he was on
//		the Head Hunters team.
//	$KillsLog::2051_deaths = "2";
//	$KillsLog::2051_death_weapon_1 = "Laser Rifle";
//	$KillsLog::2051_death_weapon_2 = "Turret";
//	$KillsLog::2051_killer_1 = "2059";
//	$KillsLog::2051_killer_2 = "2051";
//		The number of deaths of this player was 2, the first death was by Laser, second by a turret.  The
//		killer in the first death was client 2059 ([McM]Afty :o).  The killer in the second is the same as
//		the player indicating there was no killer, ie a Turret.
//	$KillsLog::2051_kills = "2";
//	$KillsLog::2051_kill_weapon_1 = "Explosives";
//	$KillsLog::2051_kill_weapon_2 = "Team Kill";
//	$KillsLog::2051_victim_1 = "2061";
//	$KillsLog::2051_victim_2 = "2058";
//		The player killed two people.  First person killed was client 2061 with Explosives, second was a Team
//		Kill, the team mate killed beign client 2058 (sorry |HH|Marauder :).  Note kills here does not
//		reflect the score that you get.  Team Kills actually deducts rather than adds to your score, also
//		suicides need to be taken away from the score as well.  In adition caps count as 5 points, returns do
//		not effect your score.  Interestingly there is some unused code in scripts.vol that credited ppl that
//		returned a flag, shame it wasn't used.  Roll on Tribes 2 :).
//	$KillsLog::2051_flag_caps = "2";
//	$KillsLog::2051_flag_grabs = "3";
//	$KillsLog::2051_flag_returns = "1";
//	$KillsLog::2051_flag_carrier_kills = "1";
//		The player returned his teams flag once and killed one enemy flag carrier.  The player grabbed the
//		enemy flag three times; he converted two of these into captures.
//
// The log is difficult to read, however it is intended that a program/script would be used to convert the
// log into a nice stats table for a web page etc.
//
//
$CrunchyPref::KillsLogSaveKey = "alt k";
$CrunchyPref::KillsLogResetKey = "alt r";
//

//
// -- Header --
//

Include("Presto\\Event.cs");
Include("Presto\\KillTrak.cs");
Include("Presto\\TeamTrak.cs");

Event::Attach(eventTeamNamesUpdated, KillsLog::TeamName);
Event::Attach(eventFlagTaken, KillsLog::Taken);
Event::Attach(eventFlagDropped, KillsLog::Drop);
Event::Attach(eventKillTrak, KillsLog::Trak);
Event::Attach(eventFlagCaptured, KillsLog::Cap);
Event::Attach(eventFlagReturned, KillsLog::Return);

// Extra death messages that aren't in KillTrak.cs
KillTrak::DeathMessage("%1 mows down %3 teammate, %2", "Team Kill");
KillTrak::DeathMessage("%1 killed %3 teammate, %2 with a mine.", "Team Kill");
KillTrak::DeathMessage("%2 dies.", "Turret");  // "dies" indicates killed by something automatic, ie Turrets.

//
// -- Begin code --
//

function KillsLog::Trak(%killer, %victim, %weapon)
{
	$KillsLog::[%victim, name] = Client::GetName(%victim);
	$KillsLog::[%victim, team] = Client::GetTeam(%victim);

	%death = $KillsLog::[%victim, deaths]++;

	$KillsLog::[%victim, killer, %death] = %killer;
	$KillsLog::[%victim, death_weapon, %death] = %weapon;

	if(%killer != %victim)
	{
		if($KillsLogData::[Team::Friendly(%killer), carrier] == %victim
		&& Team::Friendly(%killer) != Team::Friendly(%victim))  // Don't credit team kill of carrier.
			$KillsLog::[%killer, flag_carrier_kills]++;

		$KillsLog::[%killer, name] = Client::GetName(%killer);
		$KillsLog::[%killer, team] = Client::GetTeam(%killer);

		%kill = $KillsLog::[%killer, kills]++;

		$KillsLog::[%killer, victim, %kill] = %victim;
		$KillsLog::[%killer, kill_weapon, %kill] = %weapon;
	}

	if($KillsLogData::ResetCarrier)
	{
		$KillsLogData::[%teamNum, carrier] = "";
		$KillsLogData::ResetCarrier = false;
	}
}

function KillsLog::Cap(%teamNum, %client)
{
	$KillsLog::[%client, name] = Client::GetName(%client);
	$KillsLog::[%client, team] = Client::GetTeam(%client);
	$KillsLog::[%client, flag_caps]++;
	$KillsLogData::[%teamNum, carrier] = "";
}

function KillsLog::Drop(%teamNum, %client)
{
	$KillsLog::[%client, name] = Client::GetName(%client);
	$KillsLog::[%client, team] = Client::GetTeam(%client);
	$KillsLogData::ResetCarrier = true;
}

function KillsLog::Return(%teamNum, %client)
{
	if(%client != 0)
	{
		$KillsLog::[%client, name] = Client::GetName(%client);
		$KillsLog::[%client, team] = Client::GetTeam(%client);
	}
	$KillsLog::[%client, flag_returns]++;
	$KillsLogData::ResetCarrier = true;
}

function KillsLog::Taken(%teamNum, %client)
{
	$KillsLog::[%client, name] = Client::GetName(%client);
	$KillsLog::[%client, team] = Client::GetTeam(%client);
	$KillsLog::[%client, flag_grabs]++;

	$KillsLogData::[%teamNum, carrier] = %client;
}

function KillsLog::TeamName(%team, %name)
{
	echo("Team: ",%team,", Name: ", %name);
	$KillsLog::[team, %team] = Team::GetName(%team);
}

function KillsLog::Save()
{
	export("$KillsLog::*", "temp\\KillsLog.cs", false);
	remoteBP(2048, "	Kills log saved to \"temp\\KillsLog.cs\".", 5);
}

function KillsLog::Reset()
{
	deleteVariables("$KillsLog::*");
	deleteVariables("$KillsLogData::*");
	remoteBP(2048, "	Kills log reset.", 5);
}

bindKey(play, $CrunchyPref::KillsLogSaveKey, "KillsLog::Save();");
bindKey(play, $CrunchyPref::KillsLogResetKey, "KillsLog::Reset();");
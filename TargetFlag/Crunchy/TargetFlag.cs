// -- Target Flag -- v1.2 -----------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
//
// *** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
// *** Requires Zear's NewOpts v0.95 or later *** http://www.planetstarsiege.com/zear/
//
// TargetFlag allows you to waypoint flag carriers.  Press one key and your waypoint will be set to the team 
// mate who has the enemy flag.  Press another key and your waypoint is set to the enemy player with your 
// flag.  You can change back and forth between the two carriers.  This could be automated but what if both 
// flags are taken, and if you have a waypoint set would you want it overidden?
//
//
// Changes in v1.2:
//	+ Added NewOpts option page for keys and new preference.
//	+ Added optional "I'm escorting you" message, with flood protection.
//
// Changes in v1.1:
//  + Added preferences for the key bindings.
//  + Added banner to main screen.
//
//
// -- Preferences --
//
// Preferences are now set using a NewOpts page!  Start Tribes then go to Options, Scripts and then select 
// TargetFlag from the pull down list.  Now you can change the options, get some help and then the options 
// are saved so you only need to set all this once.
//

Include("Presto\\TeamTrak.cs");
Include("Presto\\Say.cs");
Include("Writer\\Version.cs");

Version("Crunchy\\TeamTrak.cs", "1.2", "|HH|Crunchy", "Target enemy and friendly flag carriers.");

Presto::AddScriptBanner(TargetFlag,
	" <f2>Target Flag <jr><f0>version 1.2 <jl>\n" @
	" \n" @
	" <f0>Target a flag carrier.\n" @
	" <f1>New:<f0>	NewOpts support.\n" @
	" <f1>New:<f0>	Optional \"I'm escorting\n" @
	" <f0>			you\" message.\n" @
	" \n" @
	" <f0>Written by: <f1>|HH|Crunchy\n" @
	" <f1>crunchy@planetstarsiege.com");

function TargetFlag::SetDefaults() {
	$CrunchyPref::TargetSayTeam = true;
	}
if(isFile("config\\CrunchyPrefs.cs")) {
	// Load last prefs off disk
	Include("CrunchyPrefs.cs");  // We only want this run once, a script might want to override.

	// In the case the defaults don't contain one for TargetFlag
	if($CrunchyPref::TargetSayTeam == "")
		TargetFlag::SetDefaults();
	}
else
	TargetFlag::SetDefaults();

//
// -- Begin code --
//

function targetClientByName(%name, %msg) {
	%clientId = getClientByName(%name);
	remoteEval(2048, "IssueTargCommand", 0, %msg, %clientID - 2048, getManagerId());
	}

function TargetFlag::Taker(%friendly) {
	if(%friendly) {
		%location = Team::GetFlagLocation(Team::Enemy());
		if(%location == $Trak::locationHome
		|| %location == $Trak::locationField
		|| %location == ""
		|| %location == Client::getName(getManagerID()))
			return;

		targetClientByName(%location, "Escort the flag carrier " @ %location @ ".~wescfr");

		if($CrunchyPref::TargetSayTeam && Flood::Protect("TargetFlag::Taker"@%location, 30))
			schedule("Say::Team(targetAcquired, \"I'm escorting you "@%location@".\");", 1);
		}
	else {
		%location = Team::GetFlagLocation(Team::Friendly());
		if(%location == $Trak::locationHome
		|| %location == $Trak::locationField
		|| %location == "")
			return;

		targetClientByName(%location, "Kill the flag carrier " @ %location @ "!~wattway");
		}
	}

//
// NewOpts
//

// Setup options page
function TargetFlag::onOpen() {
	}

// Update preferences and HUD if needed.
function TargetFlag::onClose() {
	export("$CrunchyPref::*", "config\\CrunchyPrefs.cs", false);
	}

// Options page for Zear's NewOpts
NewOpts::register("TargetFlag",
				  "Crunchy\\gui\\TargetFlagoptions.gui",
				  "TargetFlag::onOpen();",
				  "TargetFlag::onClose();",
				  TRUE);
Include("Crunchy\\TargetFlagHelp.cs");


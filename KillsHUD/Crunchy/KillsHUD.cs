// -- Kills HUD -- v1.0.1 -----------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Email: jonslark@barrysworld.co.uk
// Based on code from Presto's DynHUD and utilises KillTrak.cs
// Also inspired by other HUD's, including shokHUD and stattrak.
//
// *** Requires Presto Pack 0.93 or later ***  http://www.planetstarsiege.com/presto/
// To install, put the script and pictures into a directory named "Crunchy" in your "Tribes/config" 
// directory.  Include the KillsHUD.cs in your autoexec.cs.  Now read the notes below and edit the 
// preferences to taste.
// eg:
//	exec("Presto\\Install.cs");
//	Include("Crunchy\\KillsHUD.cs");
//
// Displays kills, deaths, suicides and team kills.  Also displays number of kills you made using each 
// weapon and how many deaths you suffered at the hand of each weapon.  It even tracks deaths from turrets,
// debris, vehicles and missiles!  The definition of suicide is my interpretation, it includes the killed 
// self with a weapon, plus falling too far or being crushed.
//
// There are two modes to the HUD, normal and life-time kills displays, you can see the kills for this match 
// or kills over your career.  The life-time kills display is stored in the file config\LifeTimeKills.cs. 
// Kills for the last 10 maps are stored in the temp directory, you can go back and see how you performed in
// a particular match.
//
// The HUD uses some standard bitmaps found in the game to indicate the weapons, the weapon you last were 
// killed or killed with is highlighted.  I made some custom icons for display of kills, such as by vehicles, 
// your welcome to use these yourself, just give me some credit.  No transparency, uh, although I like the 
// way they turned out :).  Someone know how to set transparency?  Do I need to convert the bmp to pbmp?
//
// The top line shows kills/deaths.  The second line is bad deaths (hence the red skull :), ie suicides/team 
// kills.  Each line after that shows a weapon, with kills with the weapon followed by number of times you 
// have been killed by that weapon.  eg: 2/3 would mean 2 kills by you and you died 3 times using this 
// weapon.
//
// There are 3 display types:  the HUD can be on screen all the time unless toggled off; it can be off and 
// needs to be toggled on using the key below or pop's up only on a kill update.  The popup is useful if 
// screen real estate is at a premium or you don't want the HUD on screen all the time.  I did my best to 
// make the HUD as compact as possible.
//
// I wrote the HUD so that it could be extended, ie to include kills in mods.  If you are a scripter and want 
// to do this then please release a modified version!  You'd need to define the new kills messages, a weapon
// list, a list of bitmaps and kills masks.  The kills mask is necessary as certain kill combinations aren't 
// possible.  For instance you can be killed by debris but you cannot kill someone else with debris (well not 
// directly :).  The HUD can support up to 16 weapons, it uses 12 right now.  The reason it supports 16 is 
// that you can up to 8 bitmaps in a FearGuiFormattedText and so has to use two of them.  See the header 
// below.
//
// Thanks go to the many authors on similar HUD's before me.  That'll be (in order of appearance :) 
// Killerbunny, Presto, shokugE, Spade and Grymreaper.
//
//
// Changes v1.0.1:
//	= Fixed bug where Kills HUD would stay up if it had popped up as mission ended.
//  = Fixed bug where more than 10 files would be saved in the temp dir.
//  = Moved bmps to the Crunchy\bmp dir.
//
//
// -- Preferences --
//
// Size and position of the HUD.  The life kills display is larger than the normal display as the numbers
// could get quite large.  The HUD defaults to being in the bottom right hand corner, this is good as it goes 
// over where your weapon is, which is already obscuring your view.  You'll need to adjust your team 
// HUD size though.  Adjust to taste.
$CrunchyPref::KillsHUDPos[normal] = "100% 50% 64 202";
$CrunchyPref::KillsHUDPos[life] = "100% 50% 76 202";
//
// The first key toggles the display on and off.  The second toggles between normal and life-time kills 
// display but doesn't toggle the display on.
$CrunchyPref::KillsHUDTogKey = "control s";
$CrunchyPref::KillsLifeTogKey = "alt s";
//
// The HUD can start in either normal or life-time kills modes.
$CrunchyPref::KillsStartHUD = normal; // options: normal, life.
//
// The HUD can either pop up, for the length specified in seconds, on a kill or be on or off when you start 
// the game.
$CrunchyPref::KillsHUDDisplay = popup; // options: popup, true, false.
$CrunchyPref::PopupTime = 10;
//

//
// -- Header --
//

Include("Presto\\Event.cs");
Include("Presto\\HUD.cs");
if($PrestoPref::KillTrak) Include("Presto\\KillTrak.cs");

Event::Attach(eventConnected, "Kills::Reset();");
Event::Attach(eventChangeMission, "Kills::Save(); Kills::Reset();");
Event::Attach(eventExit, "Kills::Save(); Kills::SaveLife();");
Event::Attach(eventKillTrak, Kills::Trak);

// Extra death messages that aren't in KillTrak.cs
KillTrak::DeathMessage("%1 mows down %3 teammate, %2", "Team Kill");
KillTrak::DeathMessage("%1 killed %3 teammate, %2 with a mine.","Team Kill");
KillTrak::DeathMessage("%2 dies.", "Turret");  // "dies" indicates killed by something automatic, ie Turrets.

// List of all the weapons to display kills for.  Uses the first word of the weapon as refered to in the 
// death messages, ie Disc is "Disc Launcher".  Can support up to 16 weapons.
$Kills::WeaponList =
	"Blaster Plasma Chaingun Disc Explosives Laser ELF Mortar Vehicle Turret Debris Missile";

// Not all weapon combinations are possible.  'o' indicates display and track kills, 'x' means do not.
$Kills::Mask[by] 		= "oooooooooooo";
$Kills::Mask[with]	= "oooooooooxxx";

// List of bmps to use for each weapon.
$Kills::BMPdir = "Crunchy\\bmp\\";

$Kills::BMP["Blaster"]		= "lr_blaster";
$Kills::BMP["Plasma"]		= "lr_plasma";
$Kills::BMP["Chaingun"]		= "lr_chain";
$Kills::BMP["Disc"]		= "lr_disk";
$Kills::BMP["Explosives"]	= "lr_grenade";
$Kills::BMP["Laser"]		= "lr_sniper";
$Kills::BMP["ELF"]		= "lr_energyrifle";
$Kills::BMP["Mortar"]		= "lr_mortar";
$Kills::BMP["Vehicle"]		= $Kills::BMPdir@"small_vehicle";
$Kills::BMP["Turret"]		= $Kills::BMPdir@"small_turret";
$Kills::BMP["Debris"]		= $Kills::BMPdir@"small_debris";
$Kills::BMP["Missile"]		= $Kills::BMPdir@"small_missile";

function Kills::AddBanner()
{
	function DisplayOption(%option)
	{
		if(%option == popup)
			return "<f1>popup<f0>.";
		else if(%option)
			return "<f1>on<f0>.";
		else 
			return "off.";
	}

	%text =
		" <f2>Kills HUD <jr><f0>version 1.0.1 <jl>\n";
	if($CrunchyPref::KillsHUDDisplay != popup)
		%text = %text @ "\n";
	%text = %text @
		" <f1> StartHUD<f0> is <f1>"@$CrunchyPref::KillsStartHUD@"<f0>.\n" @
		" <f1> Display<f0> is <f1>"@DisplayOption($CrunchyPref::KillsHUDDisplay)@"\n";
	if($CrunchyPref::KillsHUDDisplay == popup)
		%text = %text @
		" <f1> PopupTime<f0> is <f1>"@$CrunchyPref::PopupTime@"<f0> seconds.\n";
	%text = %text @
		" <f1> Toggle<f0> is on <f1>"@$CrunchyPref::KillsHUDTogKey@"<f0>.\n" @
		" <f1> ToggleMode<f0> is on <f1>"@$CrunchyPref::KillsLifeTogKey@"<f0>.\n";
	%text = %text @ "\n" @
		" <f0>Written by <f1>|HH|Crunchy\n" @
		" <f1>jonslark@barrysworld.co.uk";

	Presto::AddScriptBanner(KillsHUD, %text);
}
Kills::AddBanner();

//
// -- Begin code --
//

$Kills::Current = $CrunchyPref::KillsStartHUD;

// See if the weapon at %index should be displayed.
function Kills::checkMask(%by,%index)
{
	return (String::getSubStr($Kills::Mask[%by],%index,1) == "o");
}

// Highlight weapon that was responsible for last kill update.
function Kills::BMPExtension(%wep,%lastwep)
{
	if((%lastwep == %wep) && $Kills::BlinkOn)
		return "_on";
	else
		return "_off";
}

// Get weapon info from the database, if not a suitible weapon returns "x".
function Kills::Print(%by,%wep,%type,%index)
{
	if(Kills::checkMask(%by,%index))
		return $Kills::[%by,%wep,%type];
	else
		return "x";
}

// Add a bmp and text to the HUD.
function Kills::AddLineWithToggle(%wep,%lastwep,%pos,%center,%type,%index)
{
	return "<jl><B"@%pos@",4:"@$Kills::BMP[%wep]@Kills::BMPExtension(%wep,%lastwep)@".bmp><f1><L4>"@
		 Kills::Print(with,%wep,%type,%index)@"<f1>"@%center@"/<R2><jr>"@Kills::Print(by,%wep,%type,%index)@"\n";
}

function Kills::HighlightKill(%kill,%type)
{
	if((%kill == $Kills::LastKill) && $Kills::BlinkOn)
		%font = "<f2>";
	else
		%font = "<f1>";

	return %font@$Kills::[%kill,%type];
}

// Update HUD on demand.
function Kills::UpdateHUD()
{
	%type = $Kills::Current;
	if(%type == normal)
	{
		%center = "<L7>";
		%pos2 = "-34";  // Hacked co-ordiantes, found no better way to do this :(
	}
	else
	{
		%center = "<L8>";
		%pos2 = "-39";  // It's trial and error. Well much more error than trial :)
	}

	Control::SetValue(Object::GetName($KillsHUD::Kills),
		"<jl><B1,3:skull_small.bmp><L4><f1>"@
		Kills::HighlightKill(kills,%type)@"<f1>"@%center@"/<R2><jr>"@
		Kills::HighlightKill(deaths,%type)@"\n"@
		"<jl><B"@%pos2@",3:"@$Kills::BMPdir@"small_redskull.bmp><L4><f1>"@
		Kills::HighlightKill(suicides,%type)@"<f1>"@%center@"/<R2><jr>"@
		Kills::HighlightKill(teamkills,%type)
	);

	// Oi, get orf moi code :)
	%currentHUD = $KillsHUD::Weapon1;
	for(%i = 0; %i < 16; %i++)
	{
		%wep = getWord($Kills::WeaponList,%i);
		if(%wep == -1)
		{
			Control::SetValue(Object::GetName(%currentHUD), %text);
			break;
		}

		if(%i==0 || %i==8)
			%pos = "1";
		else
			%pos = %pos2;
		%text = %text @ Kills::AddLineWithToggle(%wep,$Kills::LastWep,%pos,%center,%type,%i);

		if(%i == 7)
		{
			Control::SetValue(Object::GetName(%currentHUD), %text);
			%currentHUD = $KillsHUD::Weapon2;
			%text = "";
		}
	}
}

// Store each kill in database.
function Kills::Trak(%killer, %victim, %weapon)
{
	// I turn off saving kills whilst I'm testing scripts :)
	//%host = (findSubStr($Server::Address,"LOOPBACK") != -1);  // Don't save when hosting.
	%host = false;  // Save when hosting.

	%me = getManagerId();
	%weapon = getWord(%weapon, 0);
	if(%victim == %me)
	{
		$Kills::[deaths,normal]++;
		if(!$PlayingDemo && !%host) $Kills::[deaths,life]++;

		// My interpretation of suicide, edit to taste :)
		if(%weapon == "Suicide" || %weapon == "Falling" || %weapon == "Crushed")
		{
			$Kills::LastKill = suicides;
			$Kills::[suicides,normal]++;
			if(!$PlayingDemo && !%host) $Kills::[suicides,life]++;
		}
		else
		{
			$Kills::LastKill = deaths;
			$Kills::[by,%weapon,normal]++;
			if(!$PlayingDemo && !%host) $Kills::[by,%weapon,life]++;
		}
	}
	else if(%killer == %me)
	{
		if(%weapon == "Team")
		{
			$Kills::LastKill = teamkills;
			$Kills::[teamkills,normal]++;
			if(!$PlayingDemo && !%host) $Kills::[teamkills,life]++;
		}
		else
		{
			$Kills::LastKill = kills;
			$Kills::[kills,normal]++;
			if(!$PlayingDemo && !%host) $Kills::[kills,life]++;
			$Kills::[with,%weapon,normal]++;
			if(!$PlayingDemo && !%host) $Kills::[with,%weapon,life]++;
		}

	}
	else
		return;

	$Kills::LastWep = %weapon;
	Kills::UpdateHUD();

	// Schedule popup clear properly (thanks Presto).
	// Each popup won't interfer with each other, for instance if you get killed twice in a row quickly.
	if($CrunchyPref::KillsHUDDisplay == popup && !$Kills::Display)
	{
		HUD::Display(KillsHUD);
		$schedule[Kills::ClearPopup]++;
		schedule("Kills::ClearPopup("@$schedule[Kills::ClearPopup]@");",$CrunchyPref::PopupTime);
	}

	$Kills::BlinkOn = false;
	$schedule[Kills::Blink]++;
	schedule("Kills::Blink("@$schedule[Kills::Blink]@",1);", 0.5);
}

function Kills::ResetSchedule()
{
	$schedule[Kills::ClearPopup] = 0;
	$schedule[Kills::Blink] = 0;
}
Kills::ResetSchedule();

// Take away display after popup.
function Kills::ClearPopup(%id)
{
	if(%id != $schedule[Kills::ClearPopup]) return;

	HUD::Display(KillsHUD,false);
}

// Blink the icon and kill number on and off.
function Kills::Blink(%id,%i)
{
	if(%id != $schedule[Kills::Blink]) return;

	$Kills::BlinkOn = !$Kills::BlinkOn;
	Kills::UpdateHUD();
	if(%i < ($CrunchyPref::PopupTime<<1)) schedule("Kills::Blink("@%id@","@%i+1@");", 0.5);
}

// Initialise the database, either normal or life-time.
function Kills::ClearWeapons(%type)
{
	for(%i = 0; %i < 16; %i++)
	{
		%wep = getWord($Kills::WeaponList,%i);
		if(%wep == -1) break;

		if(Kills::checkMask(by,%i))
			$Kills::[by,%wep,%type] = 0;
		if(Kills::checkMask(with,%i))
			$Kills::[with,%wep,%type] = 0;
	}
}

// Reset kills on each mission change.
function Kills::Reset()
{
	$Kills::[kills,normal] = 0;
	$Kills::[deaths,normal] = 0;
	$Kills::[suicides,normal] = 0;
	$Kills::[teamkills,normal] = 0;

	Kills::ClearWeapons(normal);

	// Could've been in middle of a popup.
	Kills::ResetSchedule();
	$Kills::BlinkOn = false;
	Kills::SetDisplay();

	Kills::UpdateHUD();
}

// Read in life-time kills or initialise database if no previus record.
if(isFile("config\\LifeTimeKills.cs"))
	exec("LifeTimeKills.cs");
else
{
	$Kills::[kills,life] = 0;
	$Kills::[deaths,life] = 0;
	$Kills::[suicides,life] = 0;
	$Kills::[teamkills,life] = 0;

	Kills::ClearWeapons(life);
}

// Save life-time kills to a file.
function Kills::SaveLife()
{
	export("$Kills::*_life", "config\\LifeTimeKills.cs", false);
}

// Each mission the kills are saved, find the next number that is available.
function Kills::FindNextFile()
{
	%file = 0;
	while(isFile("temp\\Kills"@%file@".cs"))
		%file++;

	if(%file > 9) return 0;

	return %file;
}
$Kills::NextFile = Kills::FindNextFile();

// Save kills after each mission.
function Kills::Save()
{
	export("$Kills::*_normal", "temp\\Kills"@$Kills::NextFile@".cs", false);
	$Kills::NextFile++;
	if($Kills::NextFile > 9) $Kills::NextFile = 0;
}

// Toggle HUD on and off, need to know when user has toggled on the HUD so popup won't clear the HUD.
function Kills::ToggleHUD()
{
	$Kills::Display = !$Kills::Display;
	HUD::Display(KillsHUD,$Kills::Display);
}

// Toggle between normal and life-time kills display.
function Kills::Toggle()
{
	if($Kills::Current == normal)
		$Kills::Current = life;
	else
		$Kills::Current = normal;
	HUD::Move(KillsHUD,$CrunchyPref::KillsHUDPos[$Kills::Current]);

	Kills::UpdateHUD();
}

// Make sure correct display at start of each level (in case we were in middle of a popup).
function Kills::SetDisplay()
{
	if($CrunchyPref::KillsHUDDisplay==popup)
		$Kills::Display = false;
	else
		$Kills::Display = $CrunchyPref::KillsHUDDisplay;
	HUD::Display(KillsHUD,$Kills::Display);
}

if(HUD::Exists(KillsHUD))
{
	HUD::Move(KillsHUD,$CrunchyPref::KillsHUDPos[$CrunchyPref::KillsStartHUD]);
	Kills::UpdateHUD();
	$Kills::Display = HUD::GetDisplayed(KillsHUD);
}
else
{
	HUD::NewFrame(KillsHUD, "", $CrunchyPref::KillsHUDPos[$CrunchyPref::KillsStartHUD]);
	$KillsHUD::Kills = HUD::AddObject(KillsHUD, FearGuiFormattedText, 4,2, HUD::Width(KillsHUD)-8, 30);
	$KillsHUD::Weapon1 = HUD::AddObject(KillsHUD, FearGuiFormattedText, 4,34, HUD::Width(KillsHUD)-8, 106);
	$KillsHUD::Weapon2 = HUD::AddObject(KillsHUD, FearGuiFormattedText, 4,142, HUD::Width(KillsHUD)-8, 64);

	Kills::SetDisplay();
}


// Bind the keys.
bindKey(play, $CrunchyPref::KillsHUDTogKey, "Kills::ToggleHUD();");
bindKey(play, $CrunchyPref::KillsLifeTogKey, "Kills::Toggle();");

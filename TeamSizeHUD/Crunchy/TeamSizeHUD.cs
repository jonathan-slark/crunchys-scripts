// -- TeamSize HUD -- v1.2 ----------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Email: jonslark@barrysworld.co.uk
// Inspired by team size indicator on the HyperHUD by DarkNinja.
//
// *** Requires Presto Pack 0.93 or later ***  http://www.planetstarsiege.com/presto/
// *** Requires Zear's NewOpts v0.9 or later *** http://www.cetisp.com/~thammock/scripts/
// Put the script in a directory name "Crunchy" in your tribes/config dir, along with the pictures, and 
// include it in your autoexec.cs.
// eg:
//	exec("Presto\\Install.cs");
//	Include("NewOpts\\Install.cs");
//	Include("Crunchy\\TeamSizeHUD.cs");
//
// Display's the sizes of the two teams, if there is a sufficient difference between the two team sizes then 
// the green "ok" icon will change to a red "bad" icon.  See the Preferences below.  Designed to be part of 
// the suite of HUDs I have written.
//
//
// Changes in v1.2:
//	+ Added options page for NewOpts.
//
// Changes in v1.1:
//	+ Changed layout of the HUD slightly.  Thanks to |HH|Apollo for the cool modification :)
//  + If your an observer shows sizes of the two teams and not the observers.
//
//
// -- Preferences --
//
// Set your options in the normal Tribes option menu.  Goto options then scripts.  From the pull down menu 
// select Item HUD.  From here you can set your options and preferences.
//

//
// -- Header --
//

if (!$Presto::installed || $Presto::version < 0.93) // Check presto pack is installed.
	echo("TeamSizeHUD: requires Presto Pack 0.93 or later.");
else {

function TeamSizeHUD::SetDefaults()
{
	$CrunchyPref::TeamSizeHUDPos = "0% 28% 40 56";
	$CrunchyPref::TeamSizeRatio = 0.75;
}
if(isFile("config\\CrunchyPrefs.cs"))
// Load last prefs off disk
	Include("CrunchyPrefs.cs");  // We only want this run once, a script might want to override.
else
	TeamSizeHUD::SetDefaults();
if($CrunchyPref::TeamSizeHUDPos == "")
	TeamSizeHUD::SetDefaults();

Include("Presto\\HUD.cs");
Include("Presto\\Event.cs");
Include("Presto\\TeamTrak.cs");
Include("Presto\\List.cs");

Event::Attach(eventConnected, "HUD::Update(TeamSizeHUD);");
Event::Attach(eventChangeMission, "HUD::Update(TeamSizeHUD);");

//
// -- Begin code --
//

if(HUD::Exists(TeamSizeHUD))
{
	HUD::Move(TeamSizeHUD, $CrunchyPref::TeamSizeHUDPos);
	HUD::Update(TeamSizeHUD);
}
else
{
	HUD::New(TeamSizeHUD, TeamSize::UpdateHUD, $CrunchyPref::TeamSizeHUDPos);
	HUD::Display(TeamSizeHUD);
}

function TeamSize::UpdateHUD()
{
	if(Client::GetTeam(getManagerID()) == -1)
	{
		%myteam = List::Count(Team::GetList(0));
		%nmeteam = List::Count(Team::GetList(1));
	}
	else
	{
		%myteam = List::Count(Team::GetList(Team::Friendly()));
		%nmeteam = List::Count(Team::GetList(Team::Enemy()));
	}

	if(%nmeteam == 0)
		%ratio = 0;
	else
		%ratio = %myteam/%nmeteam;
	if(%ratio >= (1/$CrunchyPref::TeamSizeRatio) || %ratio <= $CrunchyPref::TeamSizeRatio)
		%icon = "<B5,2:M_Player_red.bmp>";
	else
		%icon = "<B5,4:M_Player_green.bmp>";

	HUD::AddTextLine(TeamSizeHUD,"<f1>"@%myteam@%icon@%nmeteam);

	return 2;
}

//
// NewOpts
//

function TeamSizeHUD::onOpen()
{
	Control::setText("TeamSizeHUD::editXPos", getWord($CrunchyPref::TeamSizeHUDPos,0));
	Control::setText("TeamSizeHUD::editYPos", getWord($CrunchyPref::TeamSizeHUDPos,1));
	Control::setText("TeamSizeHUD::editWidth", getWord($CrunchyPref::TeamSizeHUDPos,2));
	Control::setText("TeamSizeHUD::editHeight", getWord($CrunchyPref::TeamSizeHUDPos,3));
}

function TeamSizeHUD::onClose()
{
	$CrunchyPref::TeamSizeHUDPos = Control::getText("TeamSizeHUD::editXPos")@" "@
								   Control::getText("TeamSizeHUD::editYPos")@" "@
								   Control::getText("TeamSizeHUD::editWidth")@" "@
								   Control::getText("TeamSizeHUD::editHeight");

	if(HUD::Exists(TeamSizeHUD))
		HUD::Move(TeamSizeHUD, $CrunchyPref::TeamSizeHUDPos);

	export("$CrunchyPref::*", "config\\CrunchyPrefs.cs", false);
}

NewOpts::register("TeamSize HUD",
				  "Crunchy\\gui\\TeamSizeHUDoptions.gui",
				  "TeamSizeHUD::onOpen();",
				  "TeamSizeHUD::onClose();",
				  TRUE);
Include("Crunchy\\TeamSizeHUDHelp.cs");

} // PrestoPack installed check

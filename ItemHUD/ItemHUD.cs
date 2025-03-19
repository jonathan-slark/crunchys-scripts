// -- Item HUD -- v1.3 ---------------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
// Based on code from Presto's DynHUD.
//
// *** Requires the Presto Pack v0.93 or later *** http://www.planetstarsiege.com/presto/
// *** Requires Zear's NewOpts v0.966 or later *** http://www.planetstarsiege.com/zear/
//
// Displays mines, grenades, beacons and repair pack count.  Uses icons so the HUD is compact and easy to 
// read.  The icons are highlighted when that item is available, dull if not.  Designed to be used with my 
// other HUDs as a replacement for DynHUD.
//
//
// Changes in v1.3:
//	+ Everything is now in a autoload volume file.
//
// Changes in v1.2:
//	+ HUDMover compatible.
//
// Changes in v1.1:
//	+ Added options page for NewOpts.
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

$ItemHUD::dir = "";
$ItemHUD::BMPdir = "";
$ItemHUD::GUIdir = "";

if ($Presto::version >= 0.93) {

function ItemHUD::SetDefaults()
{
	$CrunchyPref::ItemHUDPos = "0% 26% 40 56";
}
if (isFile("config\\CrunchyPrefs.cs"))
{
	// Load last prefs off disk
	Include("CrunchyPrefs.cs");  // We only want this run once, a script might want to override.
}
else
{
	ItemHUD::SetDefaults();
}
if ($CrunchyPref::ItemHUDPos == "")
{
	ItemHUD::SetDefaults();
}

Include("Presto\\Event.cs");
Include("Presto\\HUD.cs");

Include("Writer\\inventory.cs");
if (!$Enabled["writer\\inventory.cs"])
{
	Include($ItemHUD::dir@"inventory.cs");
}

Event::Attach(eventConnected, "HUD::Update(ItemHUD);");
Event::Attach(eventChangeMission, "HUD::Update(ItemHUD);");

//
// -- Begin code --
//

// If the HUD exists update it, otherwise create it.
if (HUD::Exists(ItemHUD))
{
	HUD::Move(ItemHUD, $CrunchyPref::ItemHUDPos);
	HUD::Update(ItemHUD);
}
else
{
	HUD::New(ItemHUD, Item::UpdateHUD, $CrunchyPref::ItemHUDPos);
	HUD::Display(ItemHUD);
}

function Item::UpdateHUD()
{
	function iconFlag(%flag)
	{
		if(%flag)
		{
			return "on";
		}
		else
		{
			return "DF";
		}
	}

	%mines = getItemCount($Inv::Name[$Inv::Mine]);
	%grens = getItemCount($Inv::Name[$Inv::Grenade]);
	%beacons = getItemCount($Inv::Name[$Inv::Beacon]);
	%repair = getItemCount($Inv::Name[$Inv::Repair_Kit]);
	HUD::AddTextLine(ItemHUD,
		"<B2,4:"@$ItemHUD::BMPdir@"small_mine_"@iconFlag(%mines)@".bmp><L4><f1>"@%mines);
	HUD::AddTextLine(ItemHUD,
		"<B-18,4:"@$ItemHUD::BMPdir@"small_grens_"@iconFlag(%grens)@".bmp><L4><f1>"@%grens);
	HUD::AddTextLine(ItemHUD,
		"<B-18,4:"@$ItemHUD::BMPdir@"small_beacon_"@iconFlag(%beacons)@".bmp><L4><f1>"@%beacons);
	HUD::AddTextLine(ItemHUD,
		"<B-18,4:"@$ItemHUD::BMPdir@"small_repair_"@iconFlag(%repair)@".bmp><L4><f1>"@%repair);

	return 1;
}

//
// NewOpts
//

function ItemHUD::onOpen()
{
}

function ItemHUD::onClose()
{
}

NewOpts::register("Item HUD",
				  $ItemHUD::GUIdir@"ItemHUDoptions.gui",
				  "ItemHUD::onOpen();",
				  "ItemHUD::onClose();",
				  TRUE);
Include($ItemHUD::dir@"ItemHUDHelp.cs");


} // Presto Pack check
else echo("TeamSizeHUD: requires Presto Pack 0.93 or later.");

// -- DropHUD -- v1.3 --------------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
// Original concept by |HH|Rico.
// Icons designed by |HH|Crunchy and |HH|Rico.  Do NOT use the icons without permission.
//
// The HUD displays an icon that represents the currently selected group.  A group contains items that will be 
// dropped when you press the drop group key.  You can move between the groups with the next and prev group 
// keys.  The HUD resets to the default group when you exit an invetory station.  The default groups are:
//
// Drop All
//   Drop all weapons
//   Drop all ammo
//   Drop pack / deployable
//   Repair kit
//   Mines
//   Grenades
//   Beacons
//
// Drop Rig
//   Drop everything apart from:
//     Blaster
//     Chaingun and ammo
//     Disc Launcher and ammo
//     Repair Kit
//
// Heavy Ammo
//   Grenade ammo
//   Mortar ammo
//
// Heavy Ammo+
//   Grenade ammo
//   Mortar ammo
//   Repair kit
//   Mines
//   Grenades
//   Beacons
//
// Light Rig
//   Energy pack
//   Some disc ammo
//   Repair kit
//   Mines
//   Grenades
//   Beacons
//
// Single item groups:
//   Repair Kit
//   Mines
//   Grenades
//   Beacons
//   Bullets
//   Grenade Ammo
//   Mortar Ammo
//   Plasma Ammo
//   Deployables
//   Ammo Pack
//   Energy Pack
//   Sensor Jammer Pack
//   Repair Pack
//   Shield Pack
//
// Note: If you know what you are doing you could edit the groups at the bottom of this script.  The top group 
// is the default.
//
//
// Changes in v1.3:
//  + The default groups is set whether you are using Writer's scripts or not.  Also fixed to work with 
//    Writer's latest pack.
//
// Changes in v1.2:
//	+ When you drop a group the next group is only selected if the current group can not be dropped again.
//	+ Moved group definitions to seperate file and added some help on how to define your own groups.
//	+ All the files apart from the group definitions are in a AutoLoad .vol file.
//  + The default group can be selected on the options pages.
//	+ All groups can be turned on or off from the options page.
//	+ Added packs as single item groups.
//	+ Added weapons and packs to the Drop All group.
//	+ Added Drop Rig group which drops everything apart from the default equipment.  This allows you to drop a 
//	  lot of stuff but leaving yourself with something to fight with.
//	+ Added Light Rig group that drops Energy Pack, Repair Kit, Mines, Grens and some Disc ammo.  Useful if a 
//	  team mate has the enemy flag but hasn't been able to get kitted up.
//	+ Added ability to drop some and not necessarily all of a particular item type.
//	+ The drop team message now says the groups label, as apposed to every item dropped, there is often too 
//	  many individual items in a group to be able to say them all.
//
// Changes in v1.1:
//	+ Made Drop HUD same width as Item HUD so the HUDs stack neatly.
//	+ Optionally tells your team what you dropped.
//	+ You can only select a group for which you can drop at least one item.  eg: If you drop the repair kit 
//	  you will not be able to select the repair kit group again.
//	+ |HH|Rico redesigned some of the icons, they look really nice :).
//	+ Changed default group to DropAll, you don't start off with heavy ammo.
//

//
// -- Header --
//

$DropHUD::dir = "";
$DropHUD::GUIdir = "";
$DropHUD::BMPdir = "";

Include("Presto\\HUD.cs");
Include("Presto\\Say.cs");

Include("Writer\\inventory.cs");
if (!$Enabled["writer\\inventory.cs"])
{
	Include($DropHUD::dir@"inventory.cs");
}

Include("Writer\\station_events.cs");
if (!$Enabled["writer\\station_events.cs"])
{
	Include("Presto\\Inventory.cs");
}

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
	Include($DropHUD::dir@"Schedule.cs");
}

if (isFile("config\\Crunchy\\Say.cs"))
{
	Include("Crunchy\\Say.cs");
}
else
{
	Say::New(sayGeneric, "", "");
}

Event::Attach(eventExitStation, "DropHUD::onExitInv();");

function DropHUD::SetDefaults()
{
	$CrunchyPref::DropHUDPos = "100% 50% 40 24";
	$CrunchyPref::DropHUD::Announce = true;
	$CrunchyPref::DropHUD::DefaultGroup = 4;
}
if (isFile("config\\CrunchyPrefs.cs"))
{
	Include("CrunchyPrefs.cs");
	if ($CrunchyPref::DropHUD::DefaultGroup == ""
	||  getword($CrunchyPref::DropHUDPos,2) == "32")
	{
		DropHUD::SetDefaults();
	}
}
else 
{
	DropHUD::SetDefaults();
}

$DropHUD::CurrentGroup = $CrunchyPref::DropHUD::DefaultGroup;

//
// -- Begin code --
//

$DropHUD::GroupNum = 0;

// Start a new group.
function DropHUD::NewGroup(%group, %label, %bmp, %display)
{
	if(%display == "") %display = true;
	%i = $DropHUD::GroupNum++;
	$DropHUD::Group::[$DropHUD::GroupNum] = %group;
	$DropHUD::Group::[%group, num] = 0;
	$DropHUD::Group::[%group, label] = %label;
	$DropHUD::Group::[%group, bmp] = %bmp;
	$DropHUD::Group::[%group, display] = %display;
}

// Add an item to a group.
function DropHUD::AddItem(%group, %item, %count)
{
	%n = $DropHUD::Group::[%group, num]++;
	$DropHUD::Group::[%group, %n, item] = %item;
	$DropHUD::Group::[%group, %n, count] = %count;
}

// Add the text label for a group.
function DropHUD::AddText(%group, %text)
{
	$DropHUD::Group::[%group, text] = %text;
}

// Method for this comes from Writer's drop unecessary ammo code :)
function DropHUD::DropAllAmmo()
{
	%dropped = false;
    for(%itemtype = 100; %itemtype > 0; %itemtype--)
    {
        if((%ammo = $Inv::Ammo[%itemtype]) != "" )
        {
			if(getItemCount(%ammo))
			{
				%dropped = true;
				for(%i = 0; %i < 20; %i++)
				{
					drop(%ammo);
				}
			}
		}
	}
	return %dropped;
}

// Drop an (optionally some) items.
function DropHUD::Drop(%item, %count)
{
	%item = $Inv::Name[%item];
	if(%count == "") %count = 20;

	%dropped = false;
	if(getItemCount(%item))
	{
		%dropped = true;
		for(%i = 0; %i < %count; %i++)
		{
			drop(%item);
		}
	}
	return %dropped;
}

// Either drop all ammo items or drop an item.
function DropHUD::DropItem(%item, %count)
{
	if(%item == DropAllAmmo)
	{
		%dropped = DropHUD::DropAllAmmo();
	}
	else
	{
		%dropped = DropHUD::Drop(%item, %count);
	}

	return %dropped;
}

// Check to see if the current group is valid.
function DropHUD::CheckCurrent()
{
	if(!DropHUD::VerifyGroup($DropHUD::CurrentGroup))
	{
		DropHUD::Cycle(1);
	}
}

// Drop a group of items.
function DropHUD::DropGroup(%i)
{
	%group = $DropHUD::Group::[%i];
	%dropped = false;
	for (%n = 1; %n <= $DropHUD::Group::[%group, num]; %n++)
	{
		%item = $DropHUD::Group::[%group, %n, item];
		%count = $DropHUD::Group::[%group, %n, count];
		if (%count != "")
		{
			%dropped = eval("DropHUD::DropItem("@%item@","@%count@");") || %dropped;
		}
		else
		{
			%dropped = eval("DropHUD::DropItem("@%item@");") || %dropped;
		}
	}

	if ($CrunchyPref::DropHUD::Announce
	&&  Flood::Protect("DropHUD::"@%i, 20)
	&&  %dropped)
	{
		%text = $DropHUD::Group::[%group, label];
		Say::Team(sayGeneric, "Dropped: "@%text);
	}
	Schedule::Add("DropHUD::CheckCurrent();",1);
}

// Verify that a group is valid by checking you have some of the items.
function DropHUD::VerifyGroup(%i)
{
	%group = $DropHUD::Group::[%i];
	if(!$DropHUD::Group::[%group, display]) return false;
	%have = false;
	for(%n = 1; %n <= $DropHUD::Group::[%group, num]; %n++)
	{
		%item = $DropHUD::Group::[%group, %n, item];
		if(%item == DropAllAmmo) return true;
		%have = %have || eval("getItemCount($Inv::Name["@%item@"]);");
	}
	return %have;
}

// Set the default group when you exit an inv.
function DropHUD::onExitInv()
{
	$DropHUD::CurrentGroup = $CrunchyPref::DropHUD::DefaultGroup;
	HUD::Update(DropHUD);
}

// Cycle to the another group.
function DropHUD::Cycle(%delta)
{
	%start = $DropHUD::CurrentGroup;
	$DropHUD::CurrentGroup += %delta;
	if($DropHUD::CurrentGroup > $DropHUD::GroupNum) $DropHUD::CurrentGroup = 1;
	if($DropHUD::CurrentGroup < 1) $DropHUD::CurrentGroup = $DropHUD::GroupNum;
	while(!DropHUD::VerifyGroup($DropHUD::CurrentGroup))
	{
		$DropHUD::CurrentGroup += %delta;
		if($DropHUD::CurrentGroup > $DropHUD::GroupNum) $DropHUD::CurrentGroup = 1;
		if($DropHUD::CurrentGroup < 1) $DropHUD::CurrentGroup = $DropHUD::GroupNum;

		if($DropHUD::CurrentGroup == %start) break; // Stop infinite loop if no group is dropable
	}

	HUD::Update(DropHUD);
}

// Update the HUD.
function DropHUD::Update()
{
	%group = $DropHUD::Group::[$DropHUD::CurrentGroup];
	%bmp = "<B3,4:"@$DropHUD::BMPdir@$DropHUD::Group::[%group, bmp]@".bmp>";
	HUD::AddTextLine(DropHUD, %bmp);

	return 0;
}

// Either create or update the HUD.
if(HUD::Exists(DropHUD))
{
	HUD::Update(DropHUD);
}
else
{
	HUD::New(DropHUD, DropHUD::Update, $CrunchyPref::DropHUDPos);
	HUD::Display(DropHUD);
}

//
// NewOpts
//

// Handle the drop down list.
function DropHUD::onCombo()
{
	%i = FGCombo::getSelected(Drop::comboGroup);
	%group = $DropHUD::Group::[%i];

	if(%i == $CrunchyPref::DropHUD::DefaultGroup) Control::setValue(Drop::checkDefault, true);
	else Control::setValue(Drop::checkDefault, false);

	if($DropHUD::Group::[%group, display]) Control::setValue(Drop::checkDisplay, true);
	else Control::setValue(Drop::checkDisplay, false);

	%bmp = "<B0,0:"@$DropHUD::Group::[%group, bmp]@".bmp>";
	Control::SetValue(Drop::bmpGroup, %bmp);
	%text = $DropHUD::Group::[%group, text];
	Control::SetValue(Drop::textDesc, %text);
}

// Set default group.
function DropHUD::onDefault(%flag)
{
	if(!%flag)
	{
		Control::setValue(Drop::checkDefault, true);
		return;
	}

	%i = FGCombo::getSelected(Drop::comboGroup);
	%group = $DropHUD::Group::[%i];

	$DropHUD::CurrentGroup = $CrunchyPref::DropHUD::DefaultGroup = %i;
	$DropHUD::Group::[%group, display] = true;
	Control::setValue(Drop::checkDisplay, true);
}

// Enable a group.
function DropHUD::onDisplay(%flag)
{
	%i = FGCombo::getSelected(Drop::comboGroup);
	%group = $DropHUD::Group::[%i];

	if(%i == $CrunchyPref::DropHUD::DefaultGroup) Control::setValue(Drop::checkDisplay, true);
	else $DropHUD::Group::[%group, display] = %flag;

	DropHUD::CheckCurrent();
}

// Setup options page.
function DropHUD::onOpen()
{
	FGCombo::clear(Drop::comboGroup);
	for(%i = 1; %i <= $DropHUD::GroupNum; %i++)
	{
		%group = $DropHUD::Group::[%i];
		%label = $DropHUD::Group::[%group, label];
		FGCombo::addEntry(Drop::comboGroup, %label, %i);
	}
	FGCombo::setSelected(Drop::comboGroup, $CrunchyPref::DropHUD::DefaultGroup);
	Control::setValue(Drop::checkDefault, true);
	Control::setValue(Drop::checkDisplay, true);

	%group = $DropHUD::Group::[$CrunchyPref::DropHUD::DefaultGroup];
	%bmp = "<B0,0:"@$DropHUD::BMPdir@$DropHUD::Group::[%group, bmp]@".bmp>";
	Control::SetValue(Drop::bmpGroup, %bmp);
	%text = $DropHUD::Group::[%group, text];
	Control::SetValue(Drop::textDesc, %text);
}

// Update preferences and HUD if needed.
function DropHUD::onClose()
{
	export("$CrunchyPref::*", "config\\CrunchyPrefs.cs", false);
	export("$DropHUD::Group::*_display", "config\\DropHUDPrefs.cs", false);
	HUD::Update(DropHUD);
}

// Options page for Zear's NewOpts
NewOpts::register("Drop HUD",
				  $DropHUD::GUIdir@"DropHUDoptions.gui",
				  "DropHUD::onOpen();",
				  "DropHUD::onClose();",
				  TRUE);
Include($DropHUD::dir@"DropHUDHelp.cs");

//
// Group definitions
//

exec("DropHUDgroups.cs");
if (isFile("config\\DropHUDPrefs.cs"))
{
	Include("DropHUDPrefs.cs");
}

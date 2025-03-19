// ---------------------------------------------------------------------------
// inventory.cs -- Version 2.7 -- January 10, 2000
// by Lorne Laliberte (writer@videon.wave.ca)
//
// http://www.planetstarsiege.com/lorne/
// ---------------------------------------------------------------------------

include("writer\\version.cs");
version("writer\\inventory.cs", "2.7", "Lorne Laliberte", "- Jan 10, 2000 - dynamic inventory table");

include("writer\\event.cs");
include("writer\\needs.cs");

$Enabled["writer\\inventory.cs"] = true;


// Convert an item name to its equivalent $Inv:: variable name
//
function Inv::makeName(%itemname)
{
    if(%itemname == "")
        return "";

    %i = 0;
    %newstring = "$Inv::";

    while( (%char = String::getSubStr(%itemname, %i, 1)) != "" )
    {
        if(%char == " ")
            %char = "_";
        else if( "k" @ %char == "k-" )
            %char = "_";
        else if( "k" @ %char == "k+" )
            %char = "_";
        else if(%char == "=")
            %char = "_";
        else if(%char == "*")
            %char = "_";
        else if(%char == "/")
            %char = "_";
        else if(%char == ",")
            %char = ""; // might have to make this "_" but only Pantheon uses "," so far

        // I'll add more if it becomes necessary :)

        %newstring = %newstring @ %char;
        %i++;
    }

    return %newstring;
}


// Test whether an item exists
//
function Inv::exists(%itemname)
{
    return (getItemType(%itemname) != -1);
}


// Force a specific variable name onto an inventory item
//
function Inv::force(%itemvarname, %itemname)
{
    eval("$Inv::" @ %itemvarname @ "=getItemType(\""@ %itemname @ "\");");
}


function Inv::Init(%itemname, %ammoname)
{
    %itemvarname = Inv::makename(%itemname);

    // Assign the type # for %itemname to $Inv::<%itemvarname>
    %itemtype = eval(%itemvarname @ "=getItemType(\""@ %itemname @ "\");");

    if(%itemtype != -1) // item exists on this server
    {
        // Assign the item name to our $Inv::Name array
        $Inv::Name[%itemtype] = %itemname;

        if(%ammoname != "") // %ammoname arg provided
        {
            %ammovarname = Inv::makename(%ammoname);

            // Associate this ammo name with the item (weapon)
            $Inv::Ammo[%itemtype] = %ammoname;
            $Inv::Ammo[%itemname] = %ammoname;

            // Assign the type # for %ammoname to $Inv::<%ammovarname>
            %ammotype = eval(%ammovarname @ "=getItemType(\""@ %ammoname @ "\");");

            // Assign the ammo name to our $Inv::Name array
            $Inv::Name[%ammotype] = %ammoname;
        }
        return true;
    }
    else // item has been replaced or renamed on this server
    {
        return false;
    }
}

function Inv::InitWeapon(%itemname, %ammoname)
{
    if((%type = getItemType(%itemname)) != -1)
    {
        $Inv::WeaponList[$Inv::WeaponCount++] = %type;

        if(%ammoname != "")
            $Inv::AmmoList[$Inv::AmmoCount++] = getItemType(%ammoname);

        return Inv::Init(%itemname, %ammoname);
    }
    return false;
}

function Inv::InitAmmo(%itemname)
{
    if((%type = getItemType(%itemname)) != -1)
    {
        $Inv::AmmoList[$Inv::AmmoCount++] = %type;
        return Inv::Init(%itemname);
    }
    return false;
}

function Inv::InitArmor(%itemname)
{
    if((%type = getItemType(%itemname)) != -1)
    {
        $Inv::ArmorList[$Inv::ArmorCount++] = %type;
        return Inv::Init(%itemname);
    }
    return false;
}

function Inv::InitPack(%itemname)
{
    if((%type = getItemType(%itemname)) != -1)
    {
        $Inv::PackList[$Inv::PackCount++] = %type;
        return Inv::Init(%itemname);
    }
    return false;
}

function Inv::InitMisc(%itemname)
{
    if((%type = getItemType(%itemname)) != -1)
    {
        $Inv::MiscList[$Inv::MiscCount++] = %type;
        return Inv::Init(%itemname);
    }
    return false;
}

function Inv::InitVehicle(%itemname)
{
    if((%type = getItemType(%itemname)) != -1)
    {
        $Inv::VehicleList[$Inv::VehicleCount++] = %type;
        return Inv::Init(%itemname);
    }
    return false;
}


// Set up the inventory table variables
//
function Inv::Initialize()
{
    echo("Clearing Inventory Table");

    deleteVariables("$Inv::*");

    echo("Initializing Inventory Table");

    // Special:

    Inv::InitWeapon("Weapon");
    Inv::InitAmmo("Ammo");
    Inv::InitPack("Backpack");

    // BASE armor:

    Inv::InitArmor("Light Armor");
    Inv::InitArmor("Medium Armor");
    Inv::InitArmor("Heavy Armor");

    // BASE vehicles:

    Inv::InitVehicle("Scout"); // on some modified servers this will point to the Scout armor? =/
    Inv::InitVehicle("LPC");
    Inv::InitVehicle("HPC");

    // BASE weapons and ammo:

    Inv::InitWeapon("Blaster");
    Inv::InitWeapon("Chaingun", "Bullet");
    Inv::InitWeapon("Disc Launcher", "Disc");
    Inv::InitWeapon("ELF gun");
    if(Inv::exists("Grenade Ammo"))
    {
        Inv::InitWeapon("Grenade Launcher", "Grenade Ammo");
    }
    else
    {
        Inv::force("Grenade_Ammo", "GrenadeAmmo");
        Inv::InitWeapon("Grenade Launcher", "GrenadeAmmo");
    }
    Inv::InitWeapon("Laser Rifle");
    if(Inv::exists("Mortar Ammo"))
    {
        Inv::InitWeapon("Mortar", "Mortar Ammo");
    }
    else
    {
        Inv::force("Mortar_Ammo", "MortarAmmo");
        Inv::InitWeapon("Mortar", "MortarAmmo");
    }
    Inv::InitWeapon("Plasma Gun", "Plasma Bolt");
    Inv::InitWeapon("Repair Gun");
    Inv::InitWeapon("Targeting Laser");

    // BASE packs:

    Inv::InitPack("Inventory Station");
    Inv::InitPack("Ammo Station");
    Inv::InitPack("Energy Pack");
    Inv::InitPack("Repair Pack");
    Inv::InitPack("Shield Pack");
    Inv::InitPack("Ammo Pack");
    Inv::InitPack("Sensor Jammer Pack");
    Inv::InitPack("Motion Sensor");
    Inv::InitPack("Pulse Sensor");
    Inv::InitPack("Sensor Jammer");
    Inv::InitPack("Camera");
    Inv::InitPack("Turret");

    // BASE misc equipment:

    Inv::InitMisc("Repair Kit");
    Inv::InitMisc("Mine");
    Inv::InitMisc("Grenade");
    Inv::InitMisc("Beacon");


    // ---------------------------------------------------------------------------
    // Now for items from the various mods

    // MOD armor:

    Inv::InitArmor("Alien");
    Inv::InitArmor("Arbitor");
    Inv::InitArmor("Assassin");
    Inv::InitArmor("Assault");
    Inv::InitArmor("Burster");
    Inv::InitArmor("Chemeleon"); // misspelled in the mod?
    Inv::InitArmor("Cyborg");
    Inv::InitArmor("Dragoon");
    Inv::InitArmor("DreadNaught");
    Inv::InitArmor("Engineer");
    Inv::InitArmor("Goliath");
    Inv::InitArmor("Hoplite");
    Inv::InitArmor("Juggernaught");
    Inv::InitArmor("Mercenary");
    Inv::InitArmor("Myrmidon");
    Inv::InitArmor("Peltast");
    Inv::InitArmor("Scout"); // on base servers this will point to the Scout vehicle =/
    Inv::InitArmor("Sniper");
    Inv::InitArmor("Spy");


    // Warhammer 40k armors according to Michael A. McCormick
    Inv::InitArmor("Assault Marine");
    Inv::InitArmor("Devastator");
    Inv::InitArmor("Dark Reaper");
    Inv::InitArmor("Terminator");
    Inv::InitArmor("Wraithlord");
    Inv::InitArmor("Eversor");
    Inv::InitArmor("Tech Suit");
    Inv::InitArmor("Psycher");
    Inv::InitArmor("Tactical Marine");
    Inv::InitArmor("Guardian");
    Inv::InitArmor("Swooping Hawk");

    // MOD vehicles:

    Inv::InitVehicle("Firestorm Bomber");
    Inv::InitVehicle("Stealth LPC");
    Inv::InitVehicle("Wraith");
    Inv::InitVehicle("Interceptor");
    Inv::InitVehicle("BomberLPC");
    Inv::InitVehicle("StealthHPC");

    // Warhammer 40k vehicles according to Michael A. McCormick
    Inv::InitVehicle("Vyper Var.3");
    Inv::InitVehicle("Falcon");
    Inv::InitVehicle("Land Speeder Var.2");
    Inv::InitVehicle("Vyper Var.1");
    Inv::InitVehicle("Land Speeder Var.1");
    Inv::InitVehicle("Jet Bike");
    Inv::InitVehicle("Land Speeder Var.3");
    Inv::InitVehicle("Tempest");
    Inv::InitVehicle("Vyper Var.2");

    // MOD weapons and ammo:

    Inv::InitWeapon("BoomStick", "Boom Shell");
    Inv::InitWeapon("Dart Rifle", "Poison Dart");
    if(Inv::exists("EMPGrenadeAmmo"))
    {
        Inv::Force("EMP_Grenade_Launcher_Ammo", "EMPGrenadeAmmo");
        Inv::InitWeapon("EMP Grenade Launcher", "EMPGrenadeAmmo");
    }
    else if(Inv::exists("EMP Grenades"))
    {
        Inv::Force("EMP_Grenade_Launcher_Ammo", "EMP Grenades");
        Inv::InitWeapon("EMP Grenade Launcher", "EMP Grenades");
    }
    Inv::InitWeapon("Engineer Repair-Gun");
    Inv::InitWeapon("FGC-9000", "FGC-9000 Nukes");
    Inv::InitWeapon("Flame Thrower");
    Inv::InitWeapon("Flare Gun", "Flares");
    Inv::InitWeapon("Fusion Blaster");
    Inv::InitWeapon("Gauss Gun");
    Inv::InitWeapon("Grav Gun");
    Inv::InitWeapon("Hyper Blaster");
    Inv::InitWeapon("Ion Rifle");
    Inv::InitWeapon("IX-2000 Sniper Rifle", "Rifle Ammo");
    Inv::InitWeapon("Jailers Gun");
    Inv::InitWeapon("Laser Rifle, Rapidfire"); // becomes $Inv::Laser_Rifle_Rapidfire
    Inv::InitWeapon("MAG Gun");
    Inv::InitWeapon("Magnum", "Magnum Bullets");
    Inv::InitWeapon("Mine Launcher", "Mine Launcher Ammo");
    Inv::InitWeapon("Omega Cannon");
    Inv::InitWeapon("Particle Accelerator");
    Inv::InitWeapon("Phalanxx Cannon", "Phalanxx Ammo");
    Inv::InitWeapon("Pyro-Torch", "Pyro Charge");
    Inv::InitWeapon("Railgun", "Railgun Bolt");
    if(Inv::exists("RocketAmmo"))
    {
        Inv::Force("Rocket_Ammo", "RocketAmmo");
        Inv::InitWeapon("Rocket Launcher", "RocketAmmo");
    }
    else if(Inv::exists("Rockets"))
    {
        Inv::Force("Rocket_Ammo", "Rockets");
        Inv::InitWeapon("Rocket Launcher", "Rockets");
    }
    Inv::InitWeapon("Shockwave Cannon");
    if(Inv::exists("Shotgun Shells"))
    {
        Inv::Force("Shotgun_Shell", "Shotgun Shells");
        Inv::InitWeapon("Shotgun", "Shotgun Shells");
    }
    else if(Inv::exists("Shotgun Shell"))
    {
        Inv::InitWeapon("Shotgun", "Shotgun Shell");
    }
    Inv::InitWeapon("Sniper Rifle", "Sniper Bullet");
    Inv::InitWeapon("Stinger", "Rockets");
    Inv::InitWeapon("Tactical Nuke", "Nuclear Warhead");
    Inv::InitWeapon("Thunderbolt");
    Inv::InitWeapon("Volter");
    Inv::InitWeapon("Vulcan", "Vulcan Bullet");

    // Warhammer 40k weapons according to Michael A. McCormick
    // Talk about an overdone mod!  Ridiculous.
    Inv::InitWeapon("AutoCannon", "Bullet");
    Inv::InitWeapon("Assault Cannon", "Bullet");
    Inv::InitWeapon("Plasma Pistol");
    Inv::InitWeapon("Wraith Cannon");
    Inv::InitWeapon("Bolter", "Bolter Rounds");
    Inv::InitWeapon("Bright Lance");
    Inv::InitWeapon("Virus Launcher", "Neurotoxin");
    Inv::InitWeapon("Combi-Gun", "Combi Shells");
    Inv::InitWeapon("Cyclone Launcher", "Cyclone Missile");
    Inv::InitWeapon("Needle Rifle", "Poison Dart");
    Inv::InitWeapon("Shock Bolt");
    Inv::InitWeapon("Las Blaster");
    Inv::InitWeapon("Demolisher Cannon");
    Inv::InitWeapon("Disrupt");
    Inv::InitWeapon("Drainer");
    Inv::InitWeapon("Scrambler/Haywire", "Haywire Grenade");
    Inv::InitWeapon("Eldar Mis. Launcher", "Plasma Missiles");
    Inv::InitWeapon("Eversor Autopistol", "Bolt Pistol Rounds");
    Inv::InitWeapon("Eversor Needler", "Poison Rounds");
    Inv::InitWeapon("Psychic Disruption");
    Inv::InitWeapon("Alien Weapon");
    Inv::InitWeapon("Fireball");
    Inv::InitWeapon("Firepike");
    Inv::InitWeapon("Tech Repair Pistol");
    Inv::InitWeapon("Flamer");
    Inv::InitWeapon("Fusion Gun");
    Inv::InitWeapon("Regenerate");
    Inv::InitWeapon("Heavy Flamer");
    Inv::InitWeapon("Heavy Bolter", "Heavy Bolter Bolt");
    Inv::InitWeapon("Las Gun");
    Inv::InitWeapon("Web Gun", "Web Fluid");
    Inv::InitWeapon("Mass Heal");
    Inv::InitWeapon("Las Pistol");
    Inv::InitWeapon("Long Rifle", "Long Rifle Ammo");
    Inv::InitWeapon("Reaper Launcher", "Reaper Shell");
    Inv::InitWeapon("Meltagun");
    Inv::InitWeapon("Multi-Melta", "Melta Charge");
    Inv::InitWeapon("Particle Rifle");
    Inv::InitWeapon("Particle Accelerator");
    Inv::InitWeapon("Plasma Cannon", "Plasma Cannon Charges");
    Inv::InitWeapon("Plasma Autocannon", "AutoPlasma Charges");
    Inv::InitWeapon("Plasma Pistol");
    Inv::InitWeapon("Focus Disc");
    Inv::InitWeapon("Beam");
    Inv::InitWeapon("Erupt");
    Inv::InitWeapon("Destructor");
    Inv::InitWeapon("Power Sword");
    Inv::InitWeapon("Drain");
    Inv::InitWeapon("Mind Blast");
    Inv::InitWeapon("Missile Launcher", "Missile");
    Inv::InitWeapon("Arch");
    Inv::InitWeapon("Scatter Laser");
    Inv::InitWeapon("Scorch");
    Inv::InitWeapon("Shoota", "Shoota Bullits");
    Inv::InitWeapon("Shotgun", "Shotgun Shells");
    Inv::InitWeapon("Shuriken Cannon", "Heavy Shuriken");
    Inv::InitWeapon("Shuriken Catapult", "Shuriken");
    Inv::InitWeapon("Storm Bolter", "Bolter Shells");
    Inv::InitWeapon("Storm Burst");
    Inv::InitWeapon("Implode");
    Inv::InitWeapon("Target Marking Laser");
    Inv::InitWeapon("Grav Beam");
    Inv::InitWeapon("Vibro Cannon", "Vibro Shell");
    Inv::InitWeapon("Fusion Cannon", "Fusion Bomb");

    // MOD packs:

    Inv::InitPack("3X4 Force Field");
    Inv::InitPack("3X4 Field Door");
    Inv::InitPack("4X8 Force Field");
    Inv::InitPack("4X8 Field Door");
    Inv::InitPack("4X14 Force Field");
    Inv::InitPack("4X14 Field Door");
    Inv::InitPack("4X17 Force Field");
    Inv::InitPack("4X17 Field Door");
    Inv::InitPack("5X3 Force Field");
    Inv::InitPack("5X3 Field Door");
    Inv::InitPack("Accelerator Device");
    Inv::InitPack("Accelerator Pad");
    Inv::InitPack("Air Large Platform");
    Inv::InitPack("Air Base");
    Inv::InitPack("Air Ammo Pad");
    Inv::InitPack("Auto-Rocket Cannon");
    Inv::InitPack("Avenger");
    Inv::InitPack("Base Alarm");
    Inv::InitPack("Blast Wall");
    Inv::InitPack("Chaingun Turret");
    Inv::InitPack("Cloaking Device");
    Inv::InitPack("Command Laptop");
    Inv::InitPack("Command Station");
    Inv::InitPack("Containment Pack");
    Inv::InitPack("Cybernetic Laser");
    Inv::InitPack("Deployable Platform");
    Inv::InitPack("Deployable ELF Turret");
    Inv::InitPack("Dissection Turret");
    Inv::InitPack("ELF Turret");
    Inv::InitPack("Emp Turret");
    Inv::InitPack("FGC-9000 Pack");
    Inv::InitPack("Flag Decoy Pack");
    Inv::InitPack("Flak Cannon");
    Inv::InitPack("Flak Turret");
    Inv::InitPack("Flight Pack");
    Inv::InitPack("Force Field");
    Inv::InitPack("Force Field, Door");
    Inv::InitPack("Force Field, Large");
    Inv::InitPack("Force Field, Small");
    Inv::InitPack("Guard Dog");
    Inv::InitPack("Healing Plant");
    Inv::InitPack("Heat Sink");
    Inv::InitPack("Hologram");
    Inv::InitPack("Ion Turret");
    Inv::InitPack("Interceptor Pack");
    Inv::InitPack("Jail Cell");
    Inv::InitPack("Jail Capture Pad");
    Inv::InitPack("Kamikaze Pack");
    Inv::InitPack("Laser Turret");
    Inv::InitPack("Large Force Field");
    Inv::InitPack("Launch Pad");
    Inv::InitPack("Lightning Pack");
    Inv::InitPack("LR Motion Sensor");
    Inv::InitPack("Mechanical Tree");
    Inv::InitPack("Mini-ELF Turret");
    Inv::InitPack("Mini-Plasma Turret");
    Inv::InitPack("Missile Turret");
    Inv::InitPack("Mortar Turret");
    Inv::InitPack("Nuke Pack");
    Inv::InitPack("ObeliskofDeath");
    Inv::InitPack("ObeliskofLight");
    Inv::InitPack("ObeliskofPowerSource");
    Inv::InitPack("Outpost");
    Inv::InitPack("Panel One");
    Inv::InitPack("Panel Two");
    Inv::InitPack("Panel Three");
    Inv::InitPack("Panel Four");
    Inv::InitPack("PhaseLok");
    Inv::InitPack("Plasma Turret");
    Inv::InitPack("Regeneration Pack");
    Inv::InitPack("Rail Turret");
    Inv::InitPack("Shield Generator");
    Inv::InitPack("Shock Turret");
    Inv::InitPack("Repairgun");
    Inv::InitPack("Rocket Battery");
    Inv::InitPack("Rocket Booster"); // should this be InitMisc instead?
    Inv::InitPack("Rocket Launcher Pack");  Inv::InitAmmo("Auto Rockets"); // used by Rocket Launcher Pack
    Inv::InitPack("Rocket Turret");
    Inv::InitPack("Seeker");
    Inv::InitPack("Seeking Missile Turret");
    Inv::InitPack("Sentry");
    Inv::InitPack("Small Force Field");
    Inv::InitPack("Sniper Pad");
    Inv::InitPack("Solar Panel");
    Inv::InitPack("Springboard");
    Inv::InitPack("Springboard Pad");
    Inv::InitPack("StealthHPC Pack");
    Inv::InitPack("StealthShield Pack");
    Inv::InitPack("Suicide DetPack");
    Inv::InitPack("Teleport Pad");
    Inv::InitPack("Teleport Pad Decoy");
    Inv::InitPack("Vengeance Missile Pack");
    Inv::InitPack("Vulcan Turret");
    Inv::InitPack("Watchdog");

    // Warhammer 40k packs according to Michael A. McCormick
    Inv::InitPack("Conceal");
    Inv::InitPack("Wraith Device");
    Inv::InitPack("Command Uplink");
    Inv::InitPack("Belt Feeder");
    Inv::InitPack("Napalm Pack");
    Inv::InitPack("Tracker Missiles");
    Inv::InitPack("Coolant Pack");
    Inv::InitPack("Adv. Energy Pack");
    Inv::InitPack("Electron Pack");
    Inv::InitPack("Concentrate");
    Inv::InitPack("Eversor Pack");
    Inv::InitPack("Hologram Pack");
    Inv::InitPack("Optic Laser");
    Inv::InitPack("Vortex Missile");
    Inv::InitPack("Death Rain");
    Inv::InitPack("Regeneration Pack");
    //Inv::InitPack("Repair Gun"); // I find this one hard to believe
    Inv::InitPack("Rune Shield");
    Inv::InitPack("Adv. Shield Pack");
    Inv::InitPack("Phase Pack(stealthshield");
    Inv::InitPack("Adv. Jammer Pack");
    Inv::InitPack("Smart Bomb");
    Inv::InitPack("Virus Bomb");
    Inv::InitPack("Acid Bomb");

    // MOD misc equipment:

    Event::Trigger(eventAddToInventoryTable);

    Event::Trigger(eventInventoryTableReady);
}



function inventory::init()
{
    if($Enabled["writer\\inventory.cs"])
    {
        //needs("writer\\inventory.cs", "writer\\event.cs 2.5");

        Event::Attach(eventConnected, "Inv::Initialize();");
    }
    else
    {
        inventory::exit();
    }
}
Event::Attach(writerInit, "inventory::init();");


function inventory::exit()
{
    Event::Detach(eventConnected, "Inv::Initialize();");
}
Event::Attach(writerExit, "inventory::exit();");


inventory::init();

// -- Deploy Announce -- v1.2 --------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Email: jonslark@barrysworld.co.uk
// This idea was brought to you by the |HH|Rico idea factory
//
// *** Requires the Presto Pack v0.93 or later *** http://www.planetstarsiege.com/presto/
//
// This script will announce when you are deploying items, such as turrets.  It will update the TeamHud
// from the PrestoPak accordingly.  When you leave a station with an item, such as a turret, it will say
// "I am deploying Turrets", when you place the turret it will say "Remote Turret deployed".  Note that
// if you keep going back for more turrets it won't keep repeating over and over (uses flood protection), 
// although it will announce every turret actually placed.
//
//
// Changes in v1.2:
//	+ Everything is now in a autoload volume file.
//
// Changes in v1.1:
//  + Changed script to use Writer's inventory script (included in PrestoPack 0.93).
//  + Uses new exit station event.
//  + When a station is deployed that job is considered finsished.
//  + Added banner to main screen.
//  + Added option to use sounds.
//  + Increased interval between announcing jobs (flood protect).
//
//
// -- Preferences --
//
// When your job is announced it will not repeat it within this interval in seconds:
$CrunchyPref::DeployTime = 60;
//

//
// -- Header --
//

$Auto::dir = "";

if ($Presto::version >= 0.93)
{

Include("Writer\\inventory.cs");
if (!$Enabled["writer\\inventory.cs"])
{
	Include($Auto::dir@"inventory.cs");
}

Include("Presto\\Event.cs");
Include("Presto\\Match.cs");
Include("Presto\\Say.cs");
Include("Presto\\Chores.cs");
if(isFile("config\\Crunchy\\Say.cs"))
{
	Include("Crunchy\\Say.cs");	// Use my Say.cs if available
}
else
{
	Say::New(sayGeneric, "", "");		// Sound to announce deploying (none by default)
	Say::New(sayGeneric3, "", "");	// Sound when item is deployed (none by default)
}

Event::Attach(eventClientMessage, Auto::DeployAnnounce);
Event::Attach(eventExitStation, Auto::BoughtAnnounce);

Presto::AddScriptBanner(DeployAnnounce,
	" <f2>Deploy Announce <jr><f0>version 1.2 <jl>\n" @
	" \n" @
	" <f0>Announces that you are\n" @
	" deploying items and updates\n" @
	" TeamHud. It then tells your\n" @
	" team when you deploy them.\n" @
	" \n" @
	" <f0>Written by: <f1>|HH|Crunchy\n" @
	" <f1>jonslark@barrysworld.co.uk");

//
// -- Begin code --
//

function Auto::DeployAnnounce(%client, %msg)
{
	if (%client == 0)
	{
		// Announce deployed items
		if (Match::ParamString(%msg, "%i deployed"))
		{
			%item = Match::Result(i);  // Get item that was deployed

			// Announce job done if station, simple deployed message otherwise.
			if (%item == "Inventory Station")
			{
				Job::Do("Inventory Station deployed", "fin-depl-dinv", sayGeneric3);
			}
			else
			if (%item == "Ammo Station")
			{
				Job::Do("Ammo Station deployed", "fin-depl-dammo", sayGeneric3);
			}
			else
			{
				Say::Team(sayGeneric3, %msg); // Say message to team
			}
		}
	}
}

function Auto::BoughtAnnounce(%client, %msg)
{
	%pack = getMountedItem(1); // Get players current back pack

	// Announce job depending on what is being carried
	if (%pack == $Inv::Turret)
	{
		// Don't repeat job too much
		if (Flood::Protect(deploy_tur, $CrunchyPref::DeployTime))
		{
			Job::Do("I am deploying Turrets", "iam-depl-dtur", sayGeneric); // Announce job
		}
	}
	else
	if (%pack == $Inv::Inventory_Station)
	{
		Job::Do("I am deploying an Inventory Station","iam-depl-dinv", sayGeneric);
	}
	else
	if (%pack == $Inv::Ammo_Station)
	{
		Job::Do("I am deploying an Ammo Station","iam-depl-dammo", sayGeneric);
	}
	else
	if (%pack == $Inv::Motion_Sensor
	||  %pack == $Inv::Pulse_Sensor)
	{
		if (Flood::Protect(deploy_sen, $CrunchyPref::DeployTime))
		{
			Job::Do("I am deploying Sensors","iam-depl-dsen", sayGeneric);
		}
	}
	else
	if (%pack == $Inv::Sensor_Jammer)
	{
		if (Flood::Protect(deploy_jam, $CrunchyPref::DeployTime))
		{
			Job::Do("I am deploying Sensor Jammers","iam-depl-djam", sayGeneric);
		}
	}
	else
	if (%pack == $Inv::Camera)
	{
		if (Flood::Protect(deploy_cam, $CrunchyPref::DeployTime))
		{
			Job::Do("I am deploying Cameras","iam-depl-dcam", sayGeneric);
		}
	}
}

} // PrestoPack installed check
else echo("DeployAnnounce: requires Presto Pack 0.93 or later.");

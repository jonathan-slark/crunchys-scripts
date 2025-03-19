Objective HUD v2.7
==================
27th June 2000

Written by |HH|Crunchy (Jonathan Slark)
Web page: http://www.planetstarsiege.com/crunchy/
Email: crunchy@planetstarsiege.com

*** Requires the Presto Pack 0.93 or later *** http://www.planetstarsiege.com/presto/
*** Requires Zear's NewOpts 0.966 or later *** http://www.planetstarsiege.com/zear/
*** Requires Writer's Support Pack v4.0.4 BETA or later *** http://www.planetstarsiege.com/lorne/
*** Requires my ObjectiveTrak script v3.3 or later ***


Uninstalling Objective HUD v2.4
-------------------------------

Remove the following line from your autoexec.cs:
Include("Crunchy\\ObjectiveHUD.cs");

Remove any fixes you may have for Objective HUD.

(Optional)
To clean up your config dir, remove the following files:
Tribes\config\Crunchy\ObjectiveHUD.cs
Tribes\config\Crunchy\ObjectiveHUDHelp.cs
Tribes\config\Crunchy\Readme_ObjectiveHUD.txt
Tribes\config\Crunchy\bmp\small_flag_dropped.bmp
Tribes\config\Crunchy\bmp\small_flag_enemy.bmp
Tribes\config\Crunchy\bmp\small_flag_enemyCarry.bmp
Tribes\config\Crunchy\bmp\small_flag_friendly.bmp
Tribes\config\Crunchy\bmp\small_flag_friendlyCarry.bmp
Tribes\config\Crunchy\bmp\small_flag_neutral.bmp
Tribes\config\Crunchy\bmp\small_flag_unknown.bmp
Tribes\config\Crunchy\bmp\small_objective_destroyed.bmp
Tribes\config\Crunchy\bmp\small_objective_safe.bmp
Tribes\config\Crunchy\bmp\small_objective_unknown.bmp
Tribes\config\Crunchy\bmp\small_tower_enemy.bmp
Tribes\config\Crunchy\bmp\small_tower_friendly.bmp
Tribes\config\Crunchy\bmp\small_tower_neutral.bmp
Tribes\config\Crunchy\bmp\small_tower_unknown.bmp
Tribes\config\Crunchy\gui\ObjectiveHUDoptions.gui


Installing Objective HUD v2.7
-----------------------------

1. Unzip the files into the Tribes\config\ dir.

2. Now start tribes, go to the Options menu, then the Scripts menu, now select
   Objective HUD from the pull down menu.  From here you can set various options
   and get some help.

3. Don't forget to install ObjectiveTrak v3.3.


Note for scripters
------------------

The HUD uses ObjectiveTrak.cs, check it out!

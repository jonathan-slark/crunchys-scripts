ObjectiveTrak v3.3
==================
27th June 2000

Written by |HH|Crunchy (Jonathan Slark)
Web page: http://www.planetstarsiege.com/crunchy/
Email: crunchy@planetstarsiege.com

*** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
*** Requires Writer's Support Pack v4.0.4 BETA or later *** http://www.planetstarsiege.com/lorne/
*** Supports Poker's BWAdmin mod ***  http:www.pokerspockets.freeserve.co.uk/


Removing the MissionInfo pack
-----------------------------

The MissionInfo pack is no more, it is being replaced by ObjectiveExtract.exe.

Remove the folder Tribes\config\missions.  If you do not do this ObjectiveTrak will
not work!

Also remove any mission info files in your Tribes\config directory, from now on
files are only saved to the Tribes\base\missions directory so this will not happen
again.  Mission info files are called <mission name>.cs.  eg: Broadside.cs, Anthill.cs
etc.


Removing previous versions of ObjectiveTrak
-------------------------------------------

Remove the following files:
config\Crunchy\Events.cs
config\Crunchy\ObjectiveTrak.cs


Installation
------------

1. Unzip the files into the Tribes\config\ dir.

2. Move ObjectiveExtract.exe to your Tribes\base\missions dir.

2. [Optional] Run ObjectiveExtract.exe by double clicking it.  It builds the mission info files
   the script uses.  Note you could skip this step but the HUD will have to learn about
   the objectives as it goes along.

3. [Optional] Unzip the TAC.zip to the Tribes\ directory.  It contains mission info files for
   the TAC mod.  These mission info files must be in Tribes\TAC\missions.


Disclaimer
----------

I can't be held responsible if ObjectiveExtract.exe does or does not do harm to your
computer.  Do not use ObjectiveExtract.exe in life threatening situations or in medical
equipment.  Do not use ObjectiveExtract.exe in Air Traffic Control software. Do not
immerse ObjectiveExtract.exe in water.  Use a lint free cloth to clean ObjectiveExtract.exe.
Do not open the door whilst ObjectiveExtract.exe is running.  Never allow small children
near ObjectiveExtract.exe.  Seek advice if you read this far.


Scripting notes
---------------

See the document ObjectiveTrak_notes.txt for more details.

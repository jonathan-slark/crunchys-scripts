TeamSize HUD
============
3rd June 1999

Written by |HH|Crunchy (Jonathan Slark)
Email: jonslark@barrysworld.co.uk

*** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
*** Requires Zear's NewOpts v0.9 or later *** http://www.cetisp.com/~thammock/scripts/

Installation
------------

Unzip the script into the tribes\config directory.  The file should now be 
in a directory called tribes\config\Crunchy. There should also be a .gui file
in a directory called tribes\config\Crunchy\gui.

Open up the file tribes\config\autoexec.cs into notepad.  After the lines:
    exec("Presto\\Install.cs");
    Include("NewOpts\\Install.cs");
add this line, it gets my script to work:
    Include("Crunchy\\TeamSizeHUD.cs");

If you look at the file tribes\config\Crunchy\TeamSizeHUD.cs in notepad it
has further notes.

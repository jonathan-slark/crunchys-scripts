Kills HUD v1.0.1
================
2nd June 1999

Written by |HH|Crunchy (Jonathan Slark)
Email: jonslark@barrysworld.co.uk

*** Requires the Presto Pack ***  http://www.planetstarsiege.com/presto/

Installation
------------

The zip file has all the files you need in the right directories.  Unzip
them all into the tribes\config directory.  The files should now be in a
directory called tribes\config\KillsHUD.

Open up the file tribes\config\autoexec.cs into notepad.  After the line:
    exec("Presto\\Install.cs");
add this line, it gets my HUD to work:
    Include("Crunchy\\KillsHUD.cs");

If you look at the file tribes\config\Crunchy\KillsHUD.cs in notepad it
has further notes.

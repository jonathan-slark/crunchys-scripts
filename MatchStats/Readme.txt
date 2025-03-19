Match Stats v1.1
================
19th May 1999

Match Stats is a joint project involving |HH|Crunchy and Rincon.

MatchStats.cs written by:
  |HH|Crunchy (Jonathan Slark)
  Email: jonslark@barrysworld.co.uk
makehtml.exe written by:
  Greg "Rincon" Moreland
  Email: rincon@wcc.net

*** Requires the Presto Pack ***  http://www.planetstarsiege.com/presto/

Installation
------------

The zip file has all the files you need in the right directories.  Unzip
them all into the tribes directory.  MatchStats.cs should be in your
Tribes\config\Crunchy\ directory.  makehtml.exe and makehtml.txt will be in
your Tribes\ directory.

Open up the file Tribes\config\autoexec.cs into notepad.  After the line:
    exec("Presto\\Install.cs");
add this line, it gets MatchStats to work:
    Include("Crunchy\\MatchStats.cs");

Look at the files Tribes\config\Crunchy\MatchStats.cs and Tribes\makehtml.txt
in notepad for further information and instructions.

There are three logs included so you can try the makehtml.exe program out.


Note for scripters
------------------

A more generic loging system a la MatchStats.cs and KillsHUD.cs is on it's way.
This way the one script will handle all the logging and other scripts can tap
into it.

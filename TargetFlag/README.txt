TargetFlag v1.2
===============
2nd September 1999

Written by |HH|Crunchy (Jonathan Slark)
Web page: http://www.planetstarsiege.com/crunchy/
Email: crunchy@planetstarsiege.com

*** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
*** Requires Zear's NewOpts v0.95 or later *** http://www.planetstarsiege.com/zear/


Installation
------------

The zip file has all the files you need in the right directories.  Unzip
them all into the tribes\config directory.  The files should now be in a
directory called tribes\config\Crunchy, with a sub-directorie: gui.

Open up the file tribes\config\autoexec.cs into notepad.  After the lines:
    exec("Presto\\Install.cs");
	Include("NewOpts\\Install.cs");
add this line, it gets my script to work:
    Include("Crunchy\\TargetFlag.cs");

Now start tribes, go to the Options menu, then the Scripts menu, now select
TargetFlag from the pull down menu.  From here you can set various options
and get help.

If you look at the file tribes\config\Crunchy\TargetFlag.cs in notepad it
has further notes.


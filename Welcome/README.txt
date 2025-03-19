Welcome
=======
2nd June 1999

Written by |HH|Crunchy (Jonathan Slark)
Email: jonslark@barrysworld.co.uk

*** Requires the Presto Pack v0.92 or later ***  http://www.planetstarsiege.com/presto/

Installation
------------

Unzip the script into the tribes\config directory.  The file should now be 
in a directory called tribes\config\Crunchy.

Open up the file tribes\config\autoexec.cs into notepad.  After the line:
    exec("Presto\\Install.cs");
add this line, it gets my script to work:
    Include("Crunchy\\Welcome.cs");

If you look at the file tribes\config\Crunchy\Welcome.cs in notepad it
has further notes.

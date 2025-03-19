// -- RegisterPref -- v1.0 -------------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
//

// Register a preference.
function RegisterPref(%var, %default)
{
	eval("if ("@%var@" == \"\")"@
	     "{"@
	     	%var@" = %default;"@
	     	"echo(%var, \" = \", %default);"@
         "}");
}

%pagename = "Objective HUD";

%topic = "HUD startup";
%text = "Choose from one of the three options:\n"@
		"<f2>on<f1> The HUD will be on when you start playing.\n"@
		"<f2>off<f1> The HUD will be off when you start playing.\n"@
		"<f2>with objectives<f1> In this mode the HUD will only come on when you play a mission "@
		"that has objectives.  Handy if you don't display CTF flags.  <f0>Note<f1> that the HUD might not "@
		"know about the objectives on some missions and so won't come on in this mode.  However as soon as "@
		"an objective is found out about it will come on.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Display CTF flags";
%text = "Check this if you want the HUD to display the status of CTF flags as well as other objectives.  "@
		"If you have a flag HUD you may not want this one to display flags as well.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Flag carry timer";
%text = "Check this if you want to see how long someone has been carrying a CTF flag.  This is useful when your trying to work out if there is a flag stand off, if there is then the timers for both flags will have a large value, ie over 30 seconds.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Show team names";
%text = "Check this if you want the HUD to display team names instead of \"Your flag\" and \"Enemy flag\" for CTF flags.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Abbreviate team names";
%text = "Check this if you want the team names to be abbreviated.  This can help to make sure the names fit in the HUD.  This is only valid for CTF missions and when \"Show team names\" is checked.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Abbreviate objective names";
%text = "The names of objectives can be quite long, if you want the HUD to make abbreviations check this.  "@
		"Making the abbreviations means the HUD doesn't have to be as big.  If you play on a mission where "@
		"the HUD doesn't know what abbreviations to make then the names will run off the edge of the "@
		"HUD.\n\n"@
		"If you play on missions not supported and the names run off the edge then mail me with details "@
		"about the mission and I will add support.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Use specific D&D icons";
%text = "Check this if you want to have specific icons for D&D objectives.  There are specific icons to differentiate turrets, stations, generators and sensors.  If you uncheck this only one general icon will be used, much like the objective screen.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Time till CTF flag returns";
%text = "This is the value that the dropped flag timer uses.  The normal CTF flag takes 45 seconds + 3 seconds fade to return, ie 48 seconds.  This value may be different for mods etc, change this if you play a lot on a server which doesn't use the standard return time.";
NewOpts::registerHelp(%pagename, %topic, %text);

//%topic = "Sort objectives";
//%text = "Choose from one of the three options:\n"@
//		"<f2>alphanumerically<f1> Sort the objectives alphanumerically.  <f0>Note<f1> for CTF flags, \"Your //flag\" will always be before any \"Enemy flag\".\n"@
//		"<f2>unsorted<f1> Don't sort, leave in order of discovery.\n"@
//		"<f2>if new objectives<f1> Sort only when the script finds out about new objectives.  If the objectives were known about, ie you have the MissionInfo Pack in which the objectives are stored in a pre-defined order.  For instance for the C&H mission TempleOfDoom the objectives are stored in floor order.  \"The basement\" is at the bottom and the \"Top floor\" is at the top. However if there are new objectives the list is sorted, rather than being shown in the muddled order of discovery.";
//NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "ObjectiveHUD Toggle";
%text = "Bind this key so you can toggle the HUD on and off.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Reset Objectives";
%text = "Bind this key so you can set the objectives to a neutral state.  Use it when you join mid-game and know the objectives are neutral.  For instance you join a CTF game in progress and both flags are in their bases.  It's impossible for the script to find this out for itself.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Defaults";
%text = "Select this to set all the options to default values.  <f0>Note<f1> it won't set all your keys to "@
		"default, they will not change.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Postion";
%text = "The position of the HUD is now set with HUDMover.  See the NewOpts documentation.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Author";
%text = "Objective HUD written by |HH|Crunchy.\n"@
		"<f0>http://www.planetstarsiege.com/crunchy/<f1>"@
		"<f0>crunchy@planetstarsiege.com<f1>";

NewOpts::registerHelp(%pagename, %topic, %text);

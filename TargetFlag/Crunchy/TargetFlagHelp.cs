%pagename = "TargetFlag";

%topic = "Say \"I'm escorting you\" for friendly";
%text = "Check this if you want to say to your team \"I'm escorting you\" when you target your team mate who has the enemy flag.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Target Friendly";
%text = "When you press this key your waypoint will be set to your team mate that has the enemy flag.  Will say \"I'm escorting you\" if you check the preference above.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Target Enemy";
%text = "When you press this key your waypoint will be set to the enemy player with your flag. <f0>Note:<f1> This waypoint is only set correctly when the enemy player is on your radar.  If the player isn't on radar then the waypoint gets set to a corner of the map.  Annoying, but there is nothing I can do about it.  If the player was on radar then the waypoint is set, however if they disappear off radar then the waypoint is left at where they were last seen.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Author";
%text = "TargetFlag written by |HH|Crunchy.\n"@
		"<f0>http://www.planetstarsiege.com/crunchy/<f1>"@
		"<f0>crunchy@planetstarsiege.com<f1>";

NewOpts::registerHelp(%pagename, %topic, %text);

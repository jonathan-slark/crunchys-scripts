%pagename = "Flag Response";

%topic = "Center print flag events";
%text = "Check this if you want the flag events to be printed at the bottom of the screen.  Will display player info etc.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Celebrate cap message";
%text = "Say a message when you cap.  The default list is very boring, edit the list at the bottom of the script.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Message when your flag is taken";
%text = "When your flag is taken say: \"<<<>player> has taken our flag!\" to your team.  Only one person on the team will say this to stop message spam.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Message when you take the flag";
%text = "Say to your team \"I have the enemy flag!\" when you take the enemy flag.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Use 2nd message when taking the flag";
%text = "Say to your team \"I need backup NOW!\", shortly after the above message.  This is an excellent attention grabber.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Toggle Center Print";
%text = "Use this key to enable / disable center prints during the game.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Toggle Cap Celebrate";
%text = "Use this key to enable / disable cap messages during the game.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Toggle Taken";
%text = "Use this key to enable / disable telling your team when your flag is taken.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Toggle Enemy Taken";
%text = "Use this key to enable / disable telling your team when you take the flag.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Author";
%text = "Flag Response written by |HH|Crunchy.\n"@
		"<f0>http://www.planetstarsiege.com/crunchy/<f1>"@
		"<f0>crunchy@planetstarsiege.com<f1>";

NewOpts::registerHelp(%pagename, %topic, %text);

%pagename = "Drop HUD";

%topic = "Group";
%text = "Select the group you want to change the settings for from the pull down list.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Default Group";
%text = "If you check this box the currently selected Group will become the default group.  Each time you start Tribes this group will be selected and everytime you leave an inventory station your group will change to this group.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Display Group";
%text = "You don't have to have all the defined groups in the group cycle.  You can turn them on and off using this check button.  Set the groups you need for your role and playing style, this keeps things simple and easy.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Drop Group";
%text = "Use this key to drop the group that is displayed in the HUD.  A group has pre-defined items that will be dropped.  For instance the Heavy Ammo group contains grenade and mortar ammo.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Next Group";
%text = "Use this key to select the next group in the list.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Prev Group";
%text = "Use this key to select the previous group in the list.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Toggle Drop HUD";
%text = "Use this key to toggle the HUD on and off.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Tell team what was dropped";
%text = "Check this if you want a team message sent with details of items dropped using the drop group key.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Author";
%text = "Drop HUD written by |HH|Crunchy.\n"@
		"<f0>http://www.planetstarsiege.com/crunchy/<f1>"@
		"<f0>crunchy@planetstarsiege.com<f1>";

NewOpts::registerHelp(%pagename, %topic, %text);

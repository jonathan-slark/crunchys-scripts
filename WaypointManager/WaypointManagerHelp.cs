%pagename = "Waypoint Manager";

%topic = "Waypoint Menu";
%text = "Menu that contains all the commands available for seperate keys.  You have the choice of either using the menu or using keys to do the same thing.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Automate Waypoints";
%text = "Automatically set the waypoint to a flag carrier when they take the flag.  If you have already set a waypoint then the waypoint is put on the back of the list so it doesn't overwrite your current waypoint.  If you clear your current waypoint or cycle the waypoints you can set the flag carrier waypoint.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Announce Escorting";
%text = "If this is checked when you press the key to waypoint the friendly carrier it will then say to your team that you are escorting the player.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Use default waypoint";
%text = "Instead of clearing the waypoint the default waypoint gets set.  This is pretty useful and gets around the problems associated with clearing waypoints.  <f0>Note<f1> this only works for CTF missions, if you play a non-CTF mission the waypoint is cleared.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Default";
%text = "Select the default waypoint you want from the drop down list.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Hide Compass HUD";
%text = "Hide the standard Tribes Compass HUD when there is no waypoint set.  Use this either if you want the HUD to be off as much as possible or to get round the problem of a flickery compass when it has been cleared.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "True clear";
%text = "You have the choice between using a true waypoint clear or the alternative method.  The true clear clears the waypoint properly but forces a team say.  This team say is filtered out for other players if they have the script but you still see your own blank team messages.  The alternative method actually waypoints yourself, which simulates a clear and doesn't force a team say.  The problem with the alternative method is that the compass can flicker because of lag.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Use messages on clear";
%text = "Use descriptive messages when the waypoint is cleared, giving a reason for the waypoint being cleared. eg \"|HH|Crunchy captured the Blood Eagle flag.\", instead of the usual \"No waypoint set.\"  <f0>Note<f1> this is only valid if true clears is not being used.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Use Waypoint HUD";
%text = "Use a HUD that displays an icon representing the currently set waypoint.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Toggle Waypoint HUD";
%text = "Use this key to toggle the Waypoint HUD on and off.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Objective Menu";
%text = "Menu with a list of objectives, select an objective and your waypoint is set to it.  This list is the same as shown in Objective HUD.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Waypoint Escort";
%text = "Works like the standard escort player.  Adds a waypoint to the list to the last player to speak and updates the HUD.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Remove Waypoint";
%text = "Remove the current waypoint, the next waypoint in the list is then set.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Cycle Waypoint";
%text = "Cycle to the next waypoint in the list of waypoints.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Enemy Carrier";
%text = "Press this key to set your waypoint to the enemy flag carrier.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Friendly Carrier";
%text = "Press this key to set your waypoint to the friendly flag carrier.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Enemy Base";
%text = "Press this key to set your waypoint to the enemy flag base.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Friendly Base";
%text = "Press this key to set your waypoint to your flag base.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Author";
%text = "Waypoint Manager written by |HH|Crunchy.\n"@
		"<f0>http://www.planetstarsiege.com/crunchy/<f1>"@
		"<f0>crunchy@planetstarsiege.com<f1>";
NewOpts::registerHelp(%pagename, %topic, %text);

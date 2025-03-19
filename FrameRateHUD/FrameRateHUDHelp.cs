%pagename = "FrameRate HUD";

%topic = "Good FPS";
%text = "Set this to what you consider to be a good FPS.  The icons in the HUD are shown green if the actual fps is greater or equal to this value.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Bad FPS";
%text = "Set this to what you consider to be a bad FPS.  The icons in the HUD are shown red if the actual fps is less than this value.\n\n";
%text = %text @ "<f0>Note<f1> if the fps is between the Good FPS and Bad FPS values then the icon is shown yellow.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Toggle FrameRate HUD";
%text = "Use this key to toggle the HUD on and off from within the game.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Advanced Settings";
%text = "This brings up a dialog where you can set the following options.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Update interval";
%text = "This is the time between updates of the HUD, edit to taste.  0.5 makes the HUD update smoothly but doesn't take much CPU.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Maximum FPS";
%text = "Set this to a figure somewhat higher than the fps you normally see on your system.  This figure is used for the Good FPS and Bad FPS ranges.  It is also used to disable tracking when you alt-tab.  When you alt-tab the fps goes sky high and tracking is stopped whilst the fps remains above the maxiumum fps.  This helps to make sure the stats are accurate even if you alt-tab.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Tracking delay";
%text = "This is the length of time before the tracking of fps stats kicks in.  You'll see the stats set at 0 until then.  This delay is necessary as it takes a few seconds for the fps to settle, especially if you get some disk lag when you first join a game.  Keep this value fairly high.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Survey";
%text = "You can take part in an online survey.  It is an attempt to collect data about the frame rates of players different machines and store then in a central database.  The results can then be viewed to compare the different systems.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "CPU Speed";
%text = "This is the speed of your CPU in MHz.  eg: for a PIII 500 you would put 500.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Graphics Card";
%text = "Select your graphics card from the drop down list.  If the card is not listed then select other.  If you do not know what your card is then select \"Don't know\".";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Author";
%text = "FrameRate HUD written by |HH|Crunchy.\n"@
		"<f0>http://www.planetstarsiege.com/crunchy/<f1>"@
		"<f0>crunchy@planetstarsiege.com<f1>";

NewOpts::registerHelp(%pagename, %topic, %text);

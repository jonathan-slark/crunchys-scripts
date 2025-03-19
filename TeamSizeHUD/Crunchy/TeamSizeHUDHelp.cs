%pagename = "TeamSize HUD";

%topic = "Posititon";
%text = "Read the documentation in Presto's HUD.cs.  This has a more in depth description of how to "@
		"position HUDs in general.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "X Pos";
%text = "X posistion of the HUD.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Y Pos";
%text = "Y posistion of the HUD.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Width";
%text = "Width of the HUD.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Height";
%text = "Height of the HUD.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Team Ratio";
%text = "If one team is smaller than the other by this fraction then the green player icon changes to a red one. For instance if there was 7 ppl on your team and 10 on the enemy team then the ratio is 0.7, this is less than 0.75 and so the icon would be red.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "TeamSizeHUD Toggle";
%text = "Key to toggle TeamSize HUD on and off.";
NewOpts::registerHelp(%pagename, %topic, %text);

%topic = "Author";
%text = "TeamSize HUD was written by |HH|Crunchy.\n"@
		"Email: <f0>jonslark@barrysworld.co.uk<f1>";
NewOpts::registerHelp(%pagename, %topic, %text);
// -- FrameRate HUD -- v1.2 ---------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
//
// Based on VekToRs FPSHUD.
//
// *** Requires the Presto Pack v0.93 or later ***  http://www.planetstarsiege.com/presto/
// *** Requires Zear's NewOpts v0.966 or later *** http://www.planetstarsiege.com/zear/
// *** Requires Writer's Support Pack v4.0.3 BETA or later *** http://www.planetstarsiege.com/lorne/
//
// Changes in v1.2:
//  + Added ability for user to submit fps stats to a central database.
//
// Changes in v1.1:
//  + A max fps, tracking is disabled if the fps goes out of range, ie when you alt-tab.
//	+ A couple of new options.  You can change the max fps and the delay before tracking starts when you join
//    a game.
//

//
// --- Header ---
//

$FrameRateHUD::dir = "";
$FrameRateHUD::BMPdir = "";
$FrameRateHUD::GUIdir = "";
$FrameRateHUD::SurveyCGI = "http://www.tribes-central.co.uk/crunchy/fps_survey.cgi";
$FrameRateHUD::SampleMin = 60;
$FrameRateHUD::Card[0] = "Don't know";
$FrameRateHUD::Card[1] = "Other";
$FrameRateHUD::Card[2] = "NVIDIA RIVA 128";
$FrameRateHUD::Card[3] = "NVIDIA TNT";
$FrameRateHUD::Card[4] = "NVIDIA TNT2";
$FrameRateHUD::Card[5] = "NVIDIA Vanta";
$FrameRateHUD::Card[6] = "NVIDIA GeForce 256";
$FrameRateHUD::Card[7] = "NVIDIA GeForce DDR";
$FrameRateHUD::Card[8] = "NVIDIA Quadro";
$FrameRateHUD::Card[9] = "NVIDIA GeForce2 GTS";
$FrameRateHUD::Card[10] = "3DFX Voodoo 1";
$FrameRateHUD::Card[11] = "3DFX Voodoo 2";
$FrameRateHUD::Card[12] = "3DFX Banshee";
$FrameRateHUD::Card[13] = "3DFX Voodoo 3";
$FrameRateHUD::Card[14] = "3DFX Voodoo 4";
$FrameRateHUD::Card[15] = "3DFX Voodoo 5";
$FrameRateHUD::Card[16] = "Diamond Viper";
$FrameRateHUD::Card[17] = "Diamond Viper II";
$FrameRateHUD::Card[18] = "Diamond Stealth";
$FrameRateHUD::Card[19] = "Diamond Stealth II";
$FrameRateHUD::Card[20] = "Diamond Stealth III";
$FrameRateHUD::Card[21] = "Matrox Productiva G100";
$FrameRateHUD::Card[22] = "Matrox Marvel G200";
$FrameRateHUD::Card[23] = "Matrox Marvel G400";
$FrameRateHUD::Card[24] = "Matrox Millenium G200";
$FrameRateHUD::Card[25] = "Matrox Millenium G400";
$FrameRateHUD::Card[26] = "ATI Xpert";
$FrameRateHUD::Card[27] = "ATI Rage Fury";
$FrameRateHUD::Card[28] = "ATI All in Wonder";
$FrameRateHUD::Cards = 29;

Include("presto\\HUD.cs");
include("support.acs.cs");
include("writer\\schedule.cs");

function FrameRateHUD::SetDefaults()
{
	$CrunchyPref::FrameRateHUD::HUDPos = "0% 60% 40 56";
	$CrunchyPref::FrameRateHUD::Time = 0.5;
	$CrunchyPref::FrameRateHUD::playGuiDelay = 10;
	$CrunchyPref::FrameRateHUD::GoodFPS = 60;
	$CrunchyPref::FrameRateHUD::BadFPS = 30;
	$CrunchyPref::FrameRateHUD::FPSMax = 150;
}
if (isFile("config\\CrunchyPrefs.cs"))
{
	Include("CrunchyPrefs.cs");
}
if ($CrunchyPref::FrameRateHUD::playGuiDelay == "")
{
	FrameRateHUD::SetDefaults();
}

$FrameRateHUD::FpsBMP = $FrameRateHUD::BMPdir@"FrameRateHUD_FPS";
$FrameRateHUD::AvgBMP = $FrameRateHUD::BMPdir@"FrameRateHUD_AVG";
$FrameRateHUD::HiBMP = $FrameRateHUD::BMPdir@"FrameRateHUD_HI";
$FrameRateHUD::LoBMP = $FrameRateHUD::BMPdir@"FrameRateHUD_LO";

Event::Attach(eventConnected, "FrameRateHUD::Reset();");
Event::Attach(eventChangeMission, "FrameRateHUD::Reset();");
Event::Attach(eventGuiOpen, FrameRateHUD:onGuiOpen);
Event::Attach(eventGuiClose, FrameRateHUD:onGuiClose);
Event::Attach(eventMissionInfo, FrameRateHUD::onMissionInfo);

//
// --- The HUD ---
//

// Reset the HUD.
function FrameRateHUD::Reset()
{
	Schedule::Cancel("$FrameRateHUD::Active = true;");
	$FrameRateHUD::Active = false;

	$FrameRateHUD::Total = 0;
	$FrameRateHUD::Hi = 0;
	$FrameRateHUD::Lo = 0;
	$FrameRateHUD::Num = 0;

	HUD::Update(FrameRateHUD);
}

// Activate the tracking when the playGui is opened.
function FrameRateHUD:onGuiOpen(%gui)
{
	if (%gui == playGui)
	{
		echo("playGui opened");

		if (!$FrameRateHUD::Active)
		{
			// Delay until fps has settled.
			Schedule::Add("$FrameRateHUD::Active = true;", $CrunchyPref::FrameRateHUD::playGuiDelay);
		}
	}
}

// Turn off the tracking when the playGui is closed.
// This stops tracking of falsely high fps when in the options menu or changing mission etc.
function FrameRateHUD:onGuiClose(%gui)
{
	if (%gui == playGui)
	{
		echo("playGui closed");

		if ($FrameRateHUD::Active)
		{
			Schedule::Cancel("$FrameRateHUD::Active = true;");
			$FrameRateHUD::Active = false;
		}
	}
}

// Get the BMP depending on what the fps is.
function FrameRateHUD::GetBMP(%bmp, %fps)
{
	if (%fps >= $CrunchyPref::FrameRateHUD::GoodFPS || %fps == 0)
	{
		return %bmp @ "_green";
	}
	else
	if (%fps > $CrunchyPref::FrameRateHUD::BadFPS)
	{
		return %bmp @ "_yellow";
	}
	else
	{
		return %bmp @ "_red";
	}
}

// Update the HUD.
function FrameRateHUD::Update()
{
	%fps = floor($ConsoleWorld::FrameRate);

	// Only track average when playGui has opened and fps has settled.
	if ($FrameRateHUD::Active)
	{
		// Make sure the fps is within acceptable bounds.
		// fps goes wild when you alt-tab.
		if (%fps > 0 && %fps <= $CrunchyPref::FrameRateHUD::FPSMax)
		{
			if (%fps > $FrameRateHUD::Hi)
			{
				$FrameRateHUD::Hi = %fps;
			}
			if (%fps < $FrameRateHUD::Lo || $FrameRateHUD::Lo == 0)
			{
				$FrameRateHUD::Lo = %fps;
			}

			// Update average.
			$FrameRateHUD::Total += $ConsoleWorld::FrameRate;
			$FrameRateHUD::Num++;
		}
	}

	// Check for division by zero.
	if ($FrameRateHUD::Num > 0)
	{
		%avg = floor($FrameRateHUD::Total / $FrameRateHUD::Num);
	}
	else
	{
		%avg = 0;
	}

	%avgBMP = FrameRateHUD::GetBMP($FrameRateHUD::AvgBMP, %avg);
	%hiBMP = FrameRateHUD::GetBMP($FrameRateHUD::HiBMP, $FrameRateHUD::Hi);
	%loBMP = FrameRateHUD::GetBMP($FrameRateHUD::LoBMP, $FrameRateHUD::Lo);
	%fpsBMP = FrameRateHUD::GetBMP($FrameRateHUD::FpsBMP, %fps);

	Hud::AddTextLine(FrameRateHUD, "<B0,7:"@%fpsBMP@".bmp><f1><l3>"@%fps);
	Hud::AddTextLine(FrameRateHUD, "<B-15,6:"@%avgBMP@".bmp><f1><l3>"@%avg);
	Hud::AddTextLine(FrameRateHUD, "<B-15,6:"@%hiBMP@".bmp><f1><l3>"@$FrameRateHUD::Hi);
	Hud::AddTextLine(FrameRateHUD, "<B-15,6:"@%loBMP@".bmp><f1><l3>"@$FrameRateHUD::Lo);

	return $CrunchyPref::FrameRateHUD::Time;
}

// Either create or update the HUD.
if(HUD::Exists(FrameRateHUD))
{
	HUD::Update(FrameRateHUD);
}
else
{
	HUD::New(FrameRateHUD, FrameRateHUD::Update, $CrunchyPref::FrameRateHUD::HUDPos);
	HUD::Display(FrameRateHUD);
}

//
// NewOpts
//

// Good FPS slider.
function FrameRateHUD::onSlideGood()
{
	%value = Control::getValue(Frame::SlideGood);
	%actual = floor(%value * $CrunchyPref::FrameRateHUD::FPSMax);
	$CrunchyPref::FrameRateHUD::GoodFPS = %actual;

	// Update the label to reflect the sliders value.
	Control::setValue(Frame::TextGood, "Good FPS ("@%actual@")");
}

// Bad FPS slider.
function FrameRateHUD::onSlideBad()
{
	%value = Control::getValue(Frame::SlideBad);
	%actual = floor(%value * $CrunchyPref::FrameRateHUD::FPSMax);
	$CrunchyPref::FrameRateHUD::BadFPS = %actual;

	// Update the label to reflect the sliders value.
	Control::setValue(Frame::TextBad, "Bad FPS ("@%actual@")");
}

function FrameRateHUD::onOpen()
{
	// Init Good FPS slider.
	%value = $CrunchyPref::FrameRateHUD::GoodFPS / $CrunchyPref::FrameRateHUD::FPSMax;
	Control::setValue(Frame::SlideGood, %value);
	Control::setValue(Frame::TextGood, "Good FPS ("@$CrunchyPref::FrameRateHUD::GoodFPS@")");

	// Init Bad FPS slider.
	%value = $CrunchyPref::FrameRateHUD::BadFPS / $CrunchyPref::FrameRateHUD::FPSMax;
	Control::setValue(Frame::SlideBad, %value);
	Control::setValue(Frame::TextBad, "Bad FPS ("@$CrunchyPref::FrameRateHUD::BadFPS@")");
}

function FrameRateHUD::onClose()
{
	export("$CrunchyPref::*", "config\\CrunchyPrefs.cs", false);
	HUD::Update(FrameRateHUD);
}

NewOpts::register("FrameRate HUD",
				  $FrameRateHUD::GUIdir@"FrameRateHUD.gui",
				  "FrameRateHUD::onOpen();",
				  "FrameRateHUD::onClose();",
				  TRUE);
Include($FrameRateHUD::dir@"FrameRateHUDHelp.cs");

//
// Advanced settings dialog
//

// Open the dialog.
function FrameRateHUD::onAdvanced()
{
	GuiPushDialog(MainWindow, $FrameRateHUD::GUIdir@"FrameRateAdvanced.gui");

	Control::setText(FrameDlg::editTime, $CrunchyPref::FrameRateHUD::Time);
	Control::setText(FrameDlg::editMax, $CrunchyPref::FrameRateHUD::FPSMax);
	Control::setText(FrameDlg::editDelay, $CrunchyPref::FrameRateHUD::playGuiDelay);
}

// Validate the parameters.
function FrameRateHUD::ValidateAdvanced()
{
	%time = Control::getText(FrameDlg::editTime);
	%max = Control::getText(FrameDlg::editMax);
	%delay = Control::getText(FrameDlg::editDelay);

	%correctTime = NewOpts::validateNumericText(%time, true, false);

	if (%max > 0 && %max < 1000
	&&  %time == %correctTime && %time > 0
	&&  %delay > 0)
	{
		Control::setActive(FrameDlg::buttonDone, TRUE);
	}
	else
	{
		Control::setActive(FrameDlg::buttonDone, FALSE);
	}
}

// Set the parameters.
function FrameRateHUD::onAdvancedDone()
{
	$CrunchyPref::FrameRateHUD::Time = Control::getText(FrameDlg::editTime);
	$CrunchyPref::FrameRateHUD::FPSMax = Control::getText(FrameDlg::editMax);
	$CrunchyPref::FrameRateHUD::playGuiDelay = Control::getText(FrameDlg::editDelay);

	if ($CrunchyPref::FrameRateHUD::GoodFPS > $CrunchyPref::FrameRateHUD::FPSMax)
	{
		$CrunchyPref::FrameRateHUD::GoodFPS = $CrunchyPref::FrameRateHUD::FPSMax;
	}
	if ($CrunchyPref::FrameRateHUD::BadFPS > $CrunchyPref::FrameRateHUD::FPSMax)
	{
		$CrunchyPref::FrameRateHUD::BadFPS = $CrunchyPref::FrameRateHUD::FPSMax;
	}

	FrameRateHUD::onOpen();
	GuiPopDialog(MainWindow, 0);
}

// Cancel the dialog.
function FrameRateHUD::onAdvancedCancel()
{
	GuiPopDialog(MainWindow, 0);
}

//
// Survey dialog
//

// Record the mission name.
function FrameRateHUD::onMissionInfo(%server, %missionName, %missionType)
{
	$FrameRateHUD::Mission = %missionName;
}

// Open the survey dialog.
function FrameRateHUD::onSurveySubmit()
{
	if ($FrameRateHUD::Num * $CrunchyPref::FrameRateHUD::Time < $FrameRateHUD::SampleMin)
	{
		ErrorDlg("To provide accurate data for the survey please play for at least "@
				 $FrameRateHUD::SampleMin@" seconds and try again.");
	}
	else
	{
		GuiPushDialog(MainWindow, $FrameRateHUD::GUIdir@"FrameRateSurvey.gui");
		FGCombo::clear(SurveyDlg::comboCard);
		for (%i = 0; %i < $FrameRateHUD::Cards; %i++)
		{
			FGCombo::addEntry(SurveyDlg::comboCard, $FrameRateHUD::Card[%i], %i);
		}
		FGCombo::setSelected(SurveyDlg::comboCard, 0);

		Control::setValue(SurveyDlg::textInfo, "Please enter the details of your machine\n"@
											   "below.  Press Done to submit your survey\n"@
											   "data.  A web page will then load to tell\n"@
											   "you if the submission was succesful.");

		%avg = floor($FrameRateHUD::Total / $FrameRateHUD::Num);
		Control::setValue(SurveyDlg::textAvg, %avg);
		Control::setValue(SurveyDlg::textMax, $FrameRateHUD::Hi);
		Control::setValue(SurveyDlg::textMin, $FrameRateHUD::Lo);
	}
}

// Validate the parameters.
function FrameRateHUD::ValidateSurvey()
{
	%cpu = Control::getText(SurveyDlg::editCPU);

	if (%cpu > 99)
	{
		Control::setActive(SurveyDlg::buttonDone, TRUE);
	}
	else
	{
		Control::setActive(SurveyDlg::buttonDone, FALSE);
	}
}

// Submit the survey.
function FrameRateHUD::onSurveyDone()
{
	%avg = floor($FrameRateHUD::Total / $FrameRateHUD::Num);
	%card = FGCombo::getSelected(SurveyDlg::comboCard);
	%cpu = Control::getText(SurveyDlg::editCPU);

	%html = $FrameRateHUD::SurveyCGI@"?"@
    	    "cpu="@%cpu@
	        "&card="@%card@
	        "&res="@$pref::videoFullScreenRes@
	        "&mission="@$FrameRateHUD::Mission@
	        "&avg="@%avg@
    	    "&max="@$FrameRateHUD::Hi@
        	"&min="@$FrameRateHUD::Lo;

	HTMLOpen(%html);

	GuiPopDialog(MainWindow, 0);

	FrameRateHUD::Reset();
}

// Cancel the survey.
function FrameRateHUD::onSurveyCancel()
{
	GuiPopDialog(MainWindow, 0);
}

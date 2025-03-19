// -- Objective Abbreviations -------------------------------------------------------------------------------
// Written by |HH|Crunchy (Jonathan Slark)
// Web page: http://www.planetstarsiege.com/crunchy/
// Email: crunchy@planetstarsiege.com
//

//
// Objective name abbreviations
// ----------------------------
//
//	Objective::AbbreviationReset();
//		Start making a list of abbreviations.
//
//	Objective::Abbreviation(%word, %wordLen, %abrv);
//		%word: Word to abbreviate.
//		%wordLen: Length of word to abbreviate.
//		%abrv: Abbreviation to use in place of accurances of the word.
//
Objective::AbbreviationReset();
Objective::Abbreviation("Generator",9,"Gen");
Objective::Abbreviation("Secondary",9,"2nd");
Objective::Abbreviation("#",1,"");
Objective::Abbreviation("Northwest", 9, "NW");
Objective::Abbreviation("Southwest", 9, "SW");
Objective::Abbreviation("Northeast", 9, "NE");
Objective::Abbreviation("Southeast", 9, "SE");
Objective::Abbreviation("North-West", 10, "NW");
Objective::Abbreviation("South-West", 10, "SW");
Objective::Abbreviation("North-East", 10, "NE");
Objective::Abbreviation("South-East", 10, "SE");
//Objective::Abbreviation("Repository", 10, "Reposit.");
Objective::Abbreviation("Central Pyramid", 15, "Pyramid");
Objective::Abbreviation("Abandoned Dropship", 18, "Dropship");
Objective::Abbreviation("Pulse Sensor", 12, "Sensor");
Objective::Abbreviation("Plasma Turret", 13, "Plasma");
Objective::Abbreviation("Command Station", 15, "Comm");
Objective::Abbreviation("Command", 7, "Comm");
Objective::Abbreviation("Hijacker Dropship", 17, "Hijacker Ship");
Objective::Abbreviation("Towing Dropship", 15, "Towing Ship");
Objective::Abbreviation("Abandoned Hydroelectric Dam", 27, "Dam");
Objective::Abbreviation("Main Radar Dish", 15, "Main Radar");
//
//
// Team name abbreviations
// -----------------------
//
//  Objective::TeamAbbreviation(%name, %level1, %level2, %level3);
//		%name: Team name to abbreviate.
//		%level1: Level one abbreviation - used in Objective HUD and TeamScore HUD.
//		%level2: Level two abbreviation - used in TeamScore HUD for when the score has three digits.
//		%level3: Level three abbreviation - not currently used.
//		The abbreviation gets more and more severe from level1 to level3.  I recommend that the
//		Level three abbreviation is just the initials of the team.
//
//
// Standard
Objective::TeamAbbreviation("Blood Eagle", "Blood Eagle", "Blood Eagle", "BE");
Objective::TeamAbbreviation("Diamond Sword", "Diamond Sword", "D. Sword", "DS");
Objective::TeamAbbreviation("Children of the Phoenix", "Phoenix", "Phoenix", "CotP");
Objective::TeamAbbreviation("Starwolf", "Starwolf", "Starwolf", "Sw");
//
// European Tribes
Objective::TeamAbbreviation("Allied Forces", "Allied Forces", "Allied Forces", "AF");
Objective::TeamAbbreviation("Blue arsed MonKeys", "B. a. MonKeys", "B. a. MonKeys", "BaM");
Objective::TeamAbbreviation("Chaos Legion", "Chaos Legion", "Chaos Legion", "CL");
Objective::TeamAbbreviation("Crushed Frogs", "Crushed Frogs", "Crushed Frogs", "cF");
Objective::TeamAbbreviation("Demented Dodos", "D. Dodos", "D. Dodos", "DD");
Objective::TeamAbbreviation("Dragon Templars", "Dragon T.", "Dragon T.", "DT");
Objective::TeamAbbreviation("Evil Minds Of Mass Detruction", "EmOmD", "EmOmD", "EmOmD");
Objective::TeamAbbreviation("Head Hunters", "Head Hunters", "Head Hunters", "HH");
Objective::TeamAbbreviation("Insomniacs", "Insomniacs", "Insomniacs", "IS");
Objective::TeamAbbreviation("Knights of Nee!", "Knights of Nee", "Knights of N.", "KoN");
Objective::TeamAbbreviation("Knights of Thunder", "Knights of T.", "Knights of T.", "KT");
Objective::TeamAbbreviation("No Mercy", "No Mercy", "No Mercy", "NM");
Objective::TeamAbbreviation("The Silent Knights", "The Silent K.", "The Silent K.", "TSK");
Objective::TeamAbbreviation("Tribe Called Best", "T. Called Best", "T. Called Best", "TCB");
Objective::TeamAbbreviation("Tribes Eternal Knights", "T. E. Knights", "T. E. Knights", "TEK");
Objective::TeamAbbreviation("Warriors Tribes clan", "W. Tribes clan", "W. Tribes clan", "WTc");
//

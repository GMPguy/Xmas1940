using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using Random=UnityEngine.Random;

public class MenuScript : MonoBehaviour {

	//References
	public GameScript GS;
	public ItemClasses IC;
	public GameObject Windows;
	public GameObject AnchorUp;
	public GameObject AnchorDown;
	// Loading
	public Transform LoadingWindow;
	public Text LoadingText;
	public Text LoadingTipText;
	// Loading
	// Prologue
	public Transform PrologueWindow;
	public Text PrologueText;
	public Image PrologueBlackImage;
	// Prologue
	// Start
	public Transform StartWindow;
	public Text VersionName;
	// Start
	// Main
	public Transform MainWindow;
	public ButtonScript[] MainButtons;
	public Text[] MainButtonTextes;
	int SelectedFile = 0;
	// Main
	// New game
	public Transform NewGameWindow;
	public Text NGtitle;
	public InputField NGinput;
	public GameObject[] NGbuttons;
	int NGchosenMode = 0;
	int NGchosenDiff = 0;
	// New game
	// Scores
	public Transform ScoresWindow;
	public ButtonScript[] ScoreButtons;
	public Text[] ScoreButtonTextes;
	public Text sTitle;
	public Text sScores;
	string prevScore, scoreMain = "";
	int sMode, sDiff = 0; 
	int sSorting = -1;
	// Scores
	// Campaign Main
	public Transform CMainWidnow;
	public GameObject CMPlayButton;
	public GameObject CMCustomizeButton;
	public GameObject CMQuitButton;
    public GameObject CMMessageButton;
	public Text CMInfo;
    public Image CMPImage;
    public Text CMPText;
	// Campaign Main
	// Campaign Customization
	public Transform CCustomWindow;
	public GameObject CustomizationBack;
	public Text CCInfo;
    public Image CCPImage;
    public Text CCPText;
	public Text CurrentCustomizationOptionText;
	public GameObject CustomizationButtons;
	public ButtonScript[] pnButtons;
	public int WhichPage = 1;
	public GameObject CustomizationInfo;
	public string currentCustomizationOption = "";
	public Text OptionInfoName;
	public Text OptionInfoDescription;

	public GameObject PlaneinCustomization;
	public ItemClasses.PlaneModel PCPlaneModel;
	public float PCPlaneModelDTN = 0f;
	public ItemClasses.Engine PCEngineModel;
	public float PCEngineModelDTN = 0f;
	public ItemClasses.Paint PCPaint;
	public float PCPaintDTN = 0f;
	public ItemClasses.Special PCAddition;
	public float PCAditionDTN = 0f;

	public string OriginString = "";
	public ItemClasses.Item[] OriginArray;

	public AudioSource[] CustomizationSounds; // Buy, equip
    // Campaign Customization
    // Campaign Message
	public Transform CMessWindow;
    public Text MessageText;
    public GameObject CloseMessage;
    // Campaign Message
	// Game Over
	public Transform GameOverWindow;
	public Image GameOverImage;
	public Text GameOverText;
	public Text GameOverText2;
	public Text GameOverText3;
	public GameObject DeadPlanes;
	public GameObject DeadCamera;
	// Game Over
	public Camera MainCamera;
	public GameObject MainMenuPlane;
	public GameObject StartLands;
	public float StartLandsPosition = 0f;
	string SLand1 = "EmptyLand";
	int SRLand1 = 1;
	string SLand2 = "EmptyLand";
	int SRLand2 = 1;
	string SLand3 = "EmptyLand";
	int SRLand3 = 1;
	public AudioSource Click;
	public GameObject Musics;
	//References

	// Variables
	public string[] CurrentWindow = {"Prologue", ""}; // current, previous
	float WindowLoaded = 0f;
	float Loading = 3f;
	int LoadingTip = 1;
	// variables

	// Misc
	Vector3 SetPos;
	float CampaignCamRotation = 0f;
    float CapmaignFreeRotation = 0f;

	string WindowToLoad = "";
	float TimeUntilLoad = 0f;

	public float EraseOptionInfo = 0f;

	string CurrentMusic = "";
	// Misc

	// Use this for initialization
	void Start () {

		Cursor.lockState = CursorLockMode.None;
		
		SetPos = new Vector3(0f, -3.5f, 0f);

		GS = GameObject.Find("GameScript").GetComponent<GameScript>();
		IC = GS.GetComponent<ItemClasses>();

		LoadingTip = Random.Range (1, 7);
		Loading = Random.Range (1f, 3f);

		// Set sky color
		/*RenderSettings.skybox.SetColor("_Tint", new Color32(0, 0, 75, 255));
		RenderSettings.skybox.SetColor("_SkyTint", new Color32(0, 0, 75, 255));
		RenderSettings.skybox.SetColor("_GroundColor", new Color32(0, 25, 55, 255));
		RenderSettings.ambientLight = new Color32 (0, 25, 55, 255);
		RenderSettings.fogColor = new Color32 (0, 25, 55, 255);
		RenderSettings.fogEndDistance = 1000f;
		GameObject.Find ("MainLight").GetComponent<Light> ().color = new Color32 (0, 55, 75, 255);
		GameObject.Find ("MainLight").transform.eulerAngles = new Vector3 (10f, 0f, 0f);*/
		GS.SetLighting((int)Random.Range(0f, 3.9f));
		// Set sky color


		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		
	}
	
	void WhileLoading(bool show = false){
		if(show){
			Loading -= Time.deltaTime;
			LoadingWindow.transform.position = AnchorUp.transform.position;
			LoadingText.text = GS.SetText("Loading", "Wczytywanie");
			switch(LoadingTip){
	            case 1:
    	            LoadingTipText.text = GS.SetText("While dogfightig, try shooting in advance if you're not directly behind/in front of your opponent!", "Gdy strzelasz do ruchomych objektów, spróbuj strzelać z wyprzedzeniem, jeśli nie jesteś prosto za/przed celem");
					break;
				case 2:
            	    LoadingTipText.text = GS.SetText("Delivering presents restores some of your health, fuel, and ammo", "Dostarczanie prezentów odnawia ci trochę zdrowia, paliwa, oraz ammunicji");
	            	break;
				case 3:
    	            LoadingTipText.text = GS.SetText("Try not to dogfight with too many planes at once!", "Nie próbuj walczyć z zbyt dużą ilością samolotów!");
					break;
				case 4:
            	    LoadingTipText.text = GS.SetText("While shooting or delivering present, aim from your gun perspective (RMB on default), for better accuracy", "Podczas strzelania lub dostarczania prezentów, celuj z perspektywy swojej broni (Przytrzymując PPM), dla lepszej celności");
	            	break;
				case 5:
    	            LoadingTipText.text = GS.SetText("Colliding with anything usually damages the plane, or destroys it immediately. Be careful when you're flying straight at any object!", "Kolizje z czymkolwiek przeważnie uszkadzają lub niszczą twój samolot. Bądź ostrożny gdy lecisz prosto na coś!");
					break;
				case 6:
            	    LoadingTipText.text = GS.SetText("Having speed greater than 175% damages the plane. If you have speed lower than 25%, you'll be unable to rotate your aircraft", "Jeśli twoja prędkość wyniesie więcej niż 175%, zaczniesz tracić zdrowie. Jeśli zaś będzie wynosić mniej niż 25%, nie będziesz mógł obracać swojej maszyny");
					break;
			}
		} else if(GS.WhichMenuWindowToLoad != ""){
			CurrentWindow[0] = GS.WhichMenuWindowToLoad;
			GS.WhichMenuWindowToLoad = "";
		} else {
			LoadingWindow.transform.position = AnchorDown.transform.position;
			LoadingTip = (int)Random.Range(0f, 6.9f);
		}
	}

	void Cutscene(string WhichOne = ""){

		if(WhichOne != ""){
			PrologueWindow.position = AnchorUp.transform.position;
			string[] CutsceneVars = new string[]{"Music", "Window", "Text"};
			float[] CutsceneBlack = new float[]{20f, 1f, 0f};
			switch(WhichOne){
				case "Prologue": CutsceneVars = new string[]{
					"Prologue",
					"Start",
					GS.SetText(
			            "Let me tell you a Christmas story:\n\n It is year 1940. It has been about a year, since the German invasion of Poland, and the following beginning of the Second World War. The German forces have been continuing to fight and invade other countries ever since, and it doesn't look like it's going to end anytime soon. Although Germany's occupying almost the entirety of europe, the fight is still present everywhere: on the land, on the sea, and in the sky. It is now December 25th, and Christmas broke out. It is a time, when Santa Claus is flying in his sleigh, and is delivering presents to the kind children. But since there is a grand conflict going on, he cannot be flying in his sleigh, because that's too dangerous! For that reason, he has replaced his sleigh and his reindeers, with a modern, heavily armed, military-class airplane. Now, he'll deliver the presents without getting stopped, and if any nazi scum will try to ruin the christmas spirit, that scum shall bite the dust! Or maybe Santa Claus will be the one who's gonna be taken down? Find it out, in...",
        			    "Pozwól że opowiem ci świąteczną opowieść:\n\n Jest rok 1940. Minął rok od Niemieckiej inwazji na Polskę, oraz rozpoczęcia drugiej wojny światowej. Siły niemieckie dalej kontynuują inwazję na nie swoje ziemie, i wydawać by się mogło, że taki stan rzeczy prędko się nie zmieni. Pomimo tego, iż Niemcy okupują już prawie całą europę, walka dalej jest kontynuowana: na ziemi, na wodzie, i w powietrzu. Nadszedł jednak 25'ego grudnia, a wraz z nim, Święta Bożego Narodzenia. Jest to czas, w którym Święty Mikołaj lata na swych saniach, i rozdaje prezenty grzecznym dziecią. Jednak, ze względu na wielki konflikt zbrojeniowy, nie może latać on w swych saniach, gdyż to jest niebiezpieczne! Z tego powodu, zastąpił on swoje sanie i renifery, świetnie uzbrojonym samolotem klasy wojskowej. Teraz, może w spokoju porozdawać dziecią ich prezenty, a jeśli jakiś szkop zechce zrujnować świąteczny nastrój, będzie on gryzł piach! A może to jednak Święty Mikołaj będzie tym, którego zestrzelą? Zaraz się tego dowiemy, w...")}; 
					PrologueText.alignment = TextAnchor.UpperCenter;
					break;
				case "Credits": CutsceneVars = new string[]{
					"",
					"Main",
					GS.SetText (
				"Credits\n\n\nGame created by:\nGMPguy\n\nTools used:\nUnity\nBlender\nPaint 3D\nAudacity\n\nMusic used:\n\"Almost New\" \"Take a Chance\" \"The Reveal\" \"The Descent\" \"Dream Culture\" \"Five Armies\" \"Chase\"\n Kevin MacLeod (incompetech.com) Licensed under Creative Commons: By Attribution 3.0 License http://creativecommons.org/licenses/by/3.0/\n\n\nCreated for Game Off 2018", 
				"Lista Płac\n\n\nGra zrobiona przez:\nGMPguy\n\nUżyte narzędzia:\nUnity\nBlender\nPaint 3D\n\nUżyta muzyka:\n\"Almost New\" \"Take a Chance\" \"The Reveal\" \"The Descent\" \"Dream Culture\" \"Five Armies\" \"Chase\"\n Kevin MacLeod (incompetech.com) Licensed under Creative Commons: By Attribution 3.0 License http://creativecommons.org/licenses/by/3.0/\n\n\nGra stworzona na Game Off 2018")}; 
					CutsceneBlack = new float[]{10f, 0f, 1f};
					PrologueText.alignment = TextAnchor.UpperLeft;
					break;
			}

			if (CutsceneVars[0] != "" && CurrentMusic != CutsceneVars[0]) {
    	        PlayMusic(CutsceneVars[0]);
        	}

	        PrologueBlackImage.color = new Color(0f,0f,0f, Mathf.Lerp(CutsceneBlack[1], CutsceneBlack[2], WindowLoaded/CutsceneBlack[0] * 2f));
    	    PrologueText.transform.position = Vector3.Lerp(AnchorDown.transform.position, AnchorUp.transform.position, WindowLoaded/CutsceneBlack[0]);
        	PrologueText.text = CutsceneVars[2];
	        if (Input.anyKeyDown) {
				if(WindowLoaded < CutsceneBlack[0]) WindowLoaded = CutsceneBlack[0];
        	    else CurrentWindow[0] = CutsceneVars[1];
        	}
		} else {
			PrologueWindow.position = AnchorDown.transform.position;
		}
	}

	void Bootup(bool show = false){

		if(show){
			StartWindow.position = AnchorUp.transform.position;
			StartWindow.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, WindowLoaded);
    	    MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, GameObject.Find("StartCamera").transform.position, 0.1f * (Time.deltaTime * 100f));
	        MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, GameObject.Find("StartCamera").transform.rotation, 0.1f * (Time.deltaTime * 100f));
    	    if ((Input.anyKeyDown && WindowLoaded > 1f) || WindowLoaded > 5f) {
        	    CurrentWindow[0] = "Main";
	        }
		} else {
			StartWindow.localScale = Vector3.Lerp(Vector3.one, Vector3.one*2f, WindowLoaded);
			StartWindow.position = AnchorDown.transform.position;
		}

	}

	void Main(string WhichMain = ""){

		if(WhichMain != ""){
			MainWindow.position = AnchorUp.transform.position;

			if(WindowLoaded < 2f){
				for(int ba = 0; ba < 6; ba++){
					MainButtonTextes[ba].color = new Color(1f,1f,1f, (WindowLoaded*6f) - (ba/6f));
				}
			}

			string[] mainInfos = new string[]{"000000", "","","","","",""};
			switch(WhichMain){
				case "Main": 
					if(GS.Build == "Web") mainInfos = new string[]{"111101", "Play", "Scores", "Options", "Credits", "", "Quit"};
					else mainInfos = new string[]{"011110", "", "Play", "Scores", "Options", "Credits", ""};
					break;
				case "Play":
					mainInfos = new string[]{"111101", "SF", "SF", "SF", "SF", "", "Main"};
					break;
				case "Quit":
					mainInfos = new string[]{"000110", "", "QuitUnsure", "", "QuitYes", "QuitNo", ""};
					break;
				case "File":
					mainInfos = new string[]{"001110", "SelectedFile", "", "SFload", "SFerase", "SFback", ""};
					break;
			}

			if(WhichMain == "Options"){
				if(GS.OptionsChoice == "-1"){
					CurrentWindow[0] = "Main";
					GS.OptionsChoice = "";
				}
				GS.OptionsButtons(MainButtons, MainButtons[5]);
				MainButtons[4].IsActive = false; MainButtonTextes[4].text = "";
			} else {
				string fileNameA = "SF" + SelectedFile + "-" + GS.Version + "-" + GS.Build;
				for(int mb = 0; mb < 6; mb++){
					if(mainInfos[0][mb] == '0') MainButtons[mb].IsActive = false;
					else if(mainInfos[0][mb] == '1') MainButtons[mb].IsActive = true;

					int Click = 0; if(MainButtons[mb].IsSelected && Input.GetMouseButtonDown(0)) Click = 1;
					switch(mainInfos[mb+1]){
						case "Play": MainButtonTextes[mb].text = GS.SetText("Play", "Graj");
							if(Click == 1) CurrentWindow[0] = mainInfos[mb+1]; break;
						case "Scores": MainButtonTextes[mb].text = GS.SetText("Scores", "Wyniki");
							if(Click == 1) CurrentWindow[0] = mainInfos[mb+1]; break;
						case "Options": MainButtonTextes[mb].text = GS.SetText("Options", "Opcje");
							if(Click == 1) CurrentWindow[0] = mainInfos[mb+1]; break;
						case "Credits": MainButtonTextes[mb].text = GS.SetText("Credits", "Lista płac");
							if(Click == 1) CurrentWindow[0] = mainInfos[mb+1]; break;
						case "Quit": MainButtonTextes[mb].text = GS.SetText("Quit", "Wyjdź");
							if(Click == 1) CurrentWindow[0] = mainInfos[mb+1]; break;
						case "Main": MainButtonTextes[mb].text = GS.SetText("Go back", "Wróć");
							if(Click == 1) CurrentWindow[0] = mainInfos[mb+1]; break;

						case "SF":
							string fileName = "SF" + mb + "-" + GS.Version + "-" + GS.Build;
							string[] Modes = new string[]{GS.SetText("Campaign", "Kampania"), GS.SetText("Endless", "Tryb bez końca"), GS.SetText("Skirmishes", "Tryb potyczek")};
							if(PlayerPrefs.HasKey(fileName + "SavedGame")){
								MainButtonTextes[mb].text = PlayerPrefs.GetString(fileName + "Name") + " - " + Modes[PlayerPrefs.GetInt(fileName + "Mode")] + " - " + PlayerPrefs.GetInt(fileName + "Level") + GS.SetText(" Level", " Poziom");
								if(Click == 1) {
									SelectedFile = mb;
									CurrentWindow[0] = "File";
								}
							} else {
								MainButtonTextes[mb].text = GS.SetText("~ EMPTY FILE ~", "~ PUSTY ZAPIS ~");
								if(Click == 1) {
									SelectedFile = mb;
									CurrentWindow[0] = "NewGame";
								}
							}
							break;

						case "QuitUnsure": 
							MainButtonTextes[mb].text = GS.SetText("Are you sure, you want to quit?", "Napewno chcesz wyjść?"); break;
						case "QuitYes": MainButtonTextes[mb].text = GS.SetText("Yes", "Tak");
							if(Click == 1) Application.Quit(); break;
						case "QuitNo": MainButtonTextes[mb].text = GS.SetText("No", "Nie");
							if(Click == 1) CurrentWindow[0] = "Main"; break;
						
						case "SelectedFile": 
							MainButtonTextes[mb].text = GS.SetText("Selected save file: ", "Wybrano zapis: ") + PlayerPrefs.GetString(fileNameA + "Name"); break;
						case "SFload": MainButtonTextes[mb].text = GS.SetText("Load", "Wczytaj");
							if(Click == 1) {
								GS.SetGameOptions("Load", "SF" + SelectedFile);
								CurrentWindow[0] = "CampaignMain";
							} break;
						case "SFerase": MainButtonTextes[mb].text = GS.SetText("Overwrite with a new game", "Nadpisz nową grą");
							if(Click == 1) CurrentWindow[0] = "NewGame"; break;
						case "SFback": MainButtonTextes[mb].text = GS.SetText("Go back", "Wróć");
							if(Click == 1) CurrentWindow[0] = "Play"; break;

						default:
							MainButtonTextes[mb].text = "";
							break;
					}
				}
			}

		} else {
			MainWindow.position = AnchorDown.transform.position;
		}

	}

	void NewGame(bool show = false){
		if(show){
			NewGameWindow.position = Vector3.Lerp(AnchorDown.transform.position, AnchorUp.transform.position, WindowLoaded*3f);
			string[] ButtonTypes = new string[]{"110011", "Mode", "Diff", "", "", "Begin", "Cancel"};

			for(int bt = 0; bt < 6; bt++){
				ButtonScript btButton = NGbuttons[bt].GetComponent<ButtonScript>();
				Text btText = NGbuttons[bt].GetComponent<Text>();

				if(ButtonTypes[0][bt] == '0') btButton.IsActive = false;
				else btButton.IsActive = true;

				int Click = 0; if(btButton.IsSelected && Input.GetMouseButtonDown(0)) Click = 1; else if(btButton.IsSelected && Input.GetMouseButtonDown(1)) Click = -1;
				switch(ButtonTypes[bt+1]){
					case "Mode":
						string[] ModeNames = {GS.SetText("Campaign", "Kampania"), GS.SetText("Endless", "Tryb bez końca"), GS.SetText("Skirmishes", "Tryb potyczek")};
						btText.text = GS.SetText("Game mode: ", "Tryb gry: ") + ModeNames[NGchosenMode];
						if(Click != 0) NGchosenMode = (3+NGchosenMode+Click) % 3;
						break;
					case "Diff":
						string[] DiffNames = {GS.SetText("Easy", "Łatwy"), GS.SetText("Normal", "Normalny"), GS.SetText("Hard", "Trudny"), GS.SetText("HARDCORE", "HARDKOROWY")};
						btText.text = GS.SetText("Difficulty level: ", "Poziom trudności: ") + DiffNames[NGchosenDiff];
						if(Click != 0) NGchosenDiff = (4+NGchosenDiff+Click) % 4;
						break;
					case "Begin":
						if(PlayerPrefs.HasKey("SF" + SelectedFile + "-" + GS.Version + "-" + GS.Build + "SavedGame")) btText.text = GS.SetText("Overwrite and begin", "Nadpisz i rozpocznij");
						else btText.text = GS.SetText("Begin", "Rozpocznij");
						if(Click != 0) {
							int[] SetCampLives = new int[]{999999, 999999, 10, 3};
							int[] SetLives = new int[]{5, 3, 1, 0};
							GS.saveIndex = SelectedFile;
							GS.SetGameOptions("Empty", "SF" + SelectedFile + "-" + GS.Version + "-" + GS.Build);
							if(NGinput.text == "") GS.Name = GS.SetText("Player - ", "Gracz - ") + Random.Range(1000, 9999).ToString();
							else GS.Name = NGinput.text;
							GS.GameMode = NGchosenMode;
							GS.Difficulty = NGchosenDiff;
							if(NGchosenMode == 0) GS.Parachutes = SetCampLives[NGchosenDiff];
							else GS.Parachutes = SetLives[NGchosenDiff];
							TimeUntilLoad = 5f;
							WindowToLoad = "MainGame";
						}
						break;
					case "Cancel":
						btText.text = GS.SetText("Cancel", "Anuluj");
						if(Click != 0) CurrentWindow[0] = "Play";
						break;
					default: btText.text = ""; break;
				}
			}
		} else if (CurrentWindow[2] == "NewGame") {
			NewGameWindow.position = Vector3.Lerp(AnchorUp.transform.position, AnchorDown.transform.position, WindowLoaded*3f);
		} else {
			NewGameWindow.position = AnchorDown.transform.position;
		}
	}

	void Scores(bool show = false){
		if(show){
			ScoresWindow.position = Vector3.Lerp(AnchorDown.transform.position, AnchorUp.transform.position, WindowLoaded*3f);

			// Buttons
			string[] buttonVars = {"111101", "Sort", "Mode", "Diff", "Erase", "", "Cancel"};
			if(scoreMain == "Erase") buttonVars = new string[]{"100011", "EraseUnsure", "", "", "", "EraseYes", "EraseNo"};
			for(int sb = 0; sb < 6; sb++){
				if(buttonVars[0][sb] == '1') ScoreButtons[sb].IsActive = true;
				else ScoreButtons[sb].IsActive = false;

				int Click = 0; if (ScoreButtons[sb].IsSelected && Input.GetMouseButtonDown(0)) Click = 1; else if (ScoreButtons[sb].IsSelected && Input.GetMouseButtonDown(1)) Click = -1;

				switch(buttonVars[sb+1]){
					case "Sort":
						string[] sortText = {GS.SetText("Best", "Od najlepszych"), GS.SetText("Worst", "Od najgorszych"), GS.SetText("Newest", "Od najnowszych"), GS.SetText("Oldest", "Od najstarszych")};
						sSorting = (3 + sSorting + Click) % 3;
						ScoreButtonTextes[sb].text = GS.SetText("Sort: ", "Sortuj: ") + sortText[sSorting];
						break;
					case "Mode":
						string[] modeText = {GS.SetText("Any", "Wszystkie"), GS.SetText("Campaign", "Kampania"), GS.SetText("Endless", "Tryb bez końca"), GS.SetText("Skirmishes", "Tryb potyczek")};
						sMode = (4 + sMode + Click) % 4;
						ScoreButtonTextes[sb].text = GS.SetText("Modes: ", "Tryb: ") + modeText[sMode];
						break;
					case "Diff":
						string[] diffText = {GS.SetText("Any", "Wszystkie"), GS.SetText("Easy", "Łatwy"), GS.SetText("Normal", "Normalny"), GS.SetText("Hard", "Trudny"), GS.SetText("Hardcore", "Hardkorowy")};
						sDiff = (5 + sDiff + Click) % 5;
						ScoreButtonTextes[sb].text = GS.SetText("Difficulty: ", "Poziom trudności: ") + diffText[sDiff];
						break;
					case "Erase":
						ScoreButtonTextes[sb].text = GS.SetText("Erase all entries", "Usuń wszystkie wpisy");
						if(Click != 0) scoreMain = "Erase";
						break;
					case "Cancel":
						ScoreButtonTextes[sb].text = GS.SetText("Go back", "Wróć");
						if(Click != 0) CurrentWindow[0] = "Main";
						break;

					case "EraseUnsure":
						ScoreButtonTextes[sb].text = GS.SetText("You sure?", "Jesteś pewien?");
						break;
					case "EraseYes":
						ScoreButtonTextes[sb].text = GS.SetText("Yes", "Tak");
						if(Click != 0) {
							GS.SaveScore("Wipe");
							scoreMain = "";
						}
						break;
					case "EraseNo":
						ScoreButtonTextes[sb].text = GS.SetText("No", "Nie");
						if(Click != 0) scoreMain = "";
						break;
					default: ScoreButtonTextes[sb].text = ""; break;
				}
			}

			// Scores display
			if(prevScore != GS.displayedHS + sSorting + sMode + sDiff){
				GS.SaveScore("Sort", new[]{sSorting});
				GS.SaveScore("Display", new[]{sMode, sDiff});
				prevScore = GS.displayedHS + sSorting + sMode + sDiff;
				sScores.text = "";
				if(WindowLoaded > 1f) WindowLoaded = 1f;
			} else {
				if(GS.displayedHS != "") sScores.text = GS.displayedHS[..(int)Mathf.Clamp((WindowLoaded-1f) * 100f, 0, GS.displayedHS.Length-1)];
				else sScores.text = GS.SetText("No entries found...", "Nie znaleziono żadnych wpisów...");
			}

		} else if (CurrentWindow[2] == "Scores") {
			ScoresWindow.position = Vector3.Lerp(AnchorUp.transform.position, AnchorDown.transform.position, WindowLoaded*3f);
		} else {
			ScoresWindow.position = AnchorDown.transform.position;
			prevScore = "";
		}
	}

	void Projector(bool enable = false, string WhichOne = ""){

	}

	void Campaign(string Specific = ""){

		switch(Specific){

			case "CampaignMain":
				CMainWidnow.position = AnchorUp.transform.position;
				CCustomWindow.position = CMessWindow.position = AnchorDown.transform.position;

				CMPlayButton.GetComponent<Text>().text = GS.SetText("Play (Level " + GS.Level + ")", "Graj (Poziom " + GS.Level + ")");
	            CMCustomizeButton.GetComponent<Text>().text = GS.SetText("Customize", "Edytuj Samolot");
    	        CMQuitButton.GetComponent<Text>().text = GS.SetText("Save'n'Quit", "Zapisz i Wyjdź");
        	    CMInfo.text = GS.SetText("Score: " + GS.CurrentScore + "\nMooney: " + GS.Mooney, "Wynik: " + GS.CurrentScore + "\nPiniądze: " + GS.Mooney);
            	if (GS.Parachutes > 0){
                	CMPText.text = GS.Parachutes.ToString();
	                CMPText.color = new Color32(255, 255, 255, 255);
    	            CMPImage.color = new Color32(255, 255, 255, 255);
        	    } else {
            	    CMPText.text = GS.Parachutes.ToString();
                	CMPText.color = new Color32(255, 0, 0, 255);
	                CMPImage.color = new Color32(255, 0, 0, 255);
    	        }

        	    if (MessageText.text != "") CMMessageButton.SetActive(true);
            	else CMMessageButton.SetActive(false);

				if(Input.GetMouseButtonDown(0)){
		            if (CMPlayButton.GetComponent<ButtonScript>().IsSelected == true) {
    		            WindowToLoad = "MainGame";
        		        TimeUntilLoad = Random.Range(1f, 3f);
            		} else if (CMCustomizeButton.GetComponent<ButtonScript>().IsSelected == true) {
                		CurrentWindow[0] = "CampaignCustomization";
		            } else if (CMMessageButton.GetComponent<ButtonScript>().IsSelected == true) {
    		            CurrentWindow[0] = "CampaignMessage";
        		    } else if (CMQuitButton.GetComponent<ButtonScript>().IsSelected == true) {
            		    CurrentWindow[0] = "Main";
                		GS.SetGameOptions("Save", "SF" + GS.saveIndex);
            		}
				}
				break;

			case "CampaignMessage":
				CMessWindow.position = AnchorUp.transform.position;
				CCustomWindow.position = CMainWidnow.position = AnchorDown.transform.position;
				CloseMessage.GetComponent<Text>().text = GS.SetText("Close", "Zamknij");

	            if (MessageText.text == "") CurrentWindow[0] = "CampaignMain";

            	if (CloseMessage.GetComponent<ButtonScript>().IsSelected == true && Input.GetMouseButtonDown(0)){
	                GS.HasDied = false;
    	            CurrentWindow[0] = "CampaignMain";
        	    }
				break;

			case "CampaignCustomization":
				CCustomWindow.position = AnchorUp.transform.position;
				CMessWindow.position = CMessWindow.position = AnchorDown.transform.position;
				CustomizationBack.GetComponent<Text> ().text = GS.SetText ("Back", "Wróć");
				CCInfo.text = GS.SetText ("Score: " + GS.CurrentScore + "\nMooney: " + GS.Mooney, "Wynik: " + GS.CurrentScore + "\nPiniądze: " + GS.Mooney);
    	        if (GS.Parachutes > 0) {
        	        CCPText.text = GS.Parachutes.ToString();
            	    CCPText.color = new Color32(255, 255, 255, 255);
                	CCPImage.color = new Color32(255, 255, 255, 255);
	            } else {
    	            CCPText.text = GS.Parachutes.ToString();
        	        CCPText.color = new Color32(255, 0, 0, 255);
            	    CCPImage.color = new Color32(255, 0, 0, 255);
            	}

	            CampaignCustomization ();

				if (CustomizationBack.GetComponent<ButtonScript> ().IsSelected == true && Input.GetMouseButtonDown (0)) {
					if (currentCustomizationOption == "") {
						CurrentWindow[0] = "CampaignMain";
					} else {
						currentCustomizationOption = "";
					}
				}

				if (EraseOptionInfo > 0f) {
					EraseOptionInfo -= 1f;
				} else {
					OptionInfoName.text = "";
					OptionInfoDescription.text = "";
				}
				break;

			default:
				if(CurrentWindow[1] == "CampaignMain" || CurrentWindow[1] == "CampaignCustomization" || CurrentWindow[1] == "CampaignMessage"){
					CMainWidnow.position = Vector3.Lerp(CMainWidnow.position, AnchorDown.transform.position, WindowLoaded/4f);
					CMessWindow.position = Vector3.Lerp(CMessWindow.position, AnchorDown.transform.position, WindowLoaded/4f);
					CCustomWindow.position = Vector3.Lerp(CCustomWindow.position, AnchorDown.transform.position, WindowLoaded/4f);
				} else {
					CMainWidnow.position = CMessWindow.position = CCustomWindow.position = AnchorDown.transform.position;
				}
				break;

		}

	}

	void GameOver(string overType = ""){
		if(overType == "GameOver"){
			GameOverWindow.position = AnchorUp.transform.position;

			if(CurrentMusic != "GameOver"){
				PlayMusic ("GameOver");
			}

			GameOverText.text = GS.SetText ("Game Over", "Koniec Gry");
			GameOverText2.text = GS.SetText ("Score: " + GS.PreviousScore + "\nPress ESC", "Wynik: " + GS.PreviousScore + "\nNaciśnij ESC");
			GameOverText.transform.position = Vector3.MoveTowards (GameOverText.transform.position, AnchorUp.transform.position, ((float)Screen.height / 1000f) * (Time.deltaTime * 100f));
			GameOverImage.color = new Color32 (0, 0, 0, (byte)(128f + ((Vector3.Distance (GameOverText.transform.position, AnchorUp.transform.position) / (Screen.height * 0.5f)) * 128f)));

			MainCamera.transform.position = DeadCamera.transform.position;
			MainCamera.transform.rotation = DeadCamera.transform.rotation;

			foreach(Transform ChoosenPlane in DeadPlanes.transform){
				if (ChoosenPlane.name == GS.PreviousPlane) {
					ChoosenPlane.gameObject.SetActive (true);
				} else {
					ChoosenPlane.gameObject.SetActive (false);
				}
			}

			if(GameOverText.transform.position == AnchorUp.transform.position && GameOverText2.color.a < 255){
				GameOverText2.color += new Color32 (0, 0, 0, 5);
			}

			if(Input.GetKeyDown(KeyCode.Escape)){
				CurrentWindow[0] = "Start";
			}

		} else {
			GameOverWindow.position = AnchorDown.transform.position;
		}
	}

	void Update () {

        SettingMessage();

		if(CurrentWindow[1] != CurrentWindow[0]){
			CurrentWindow[2] = CurrentWindow[1];
			CurrentWindow[1] = CurrentWindow[0];
			WindowLoaded = 0f;
		} else {
			WindowLoaded += Time.unscaledDeltaTime;
		}

		if(Loading > 0f){
			WhileLoading(true);
			Cutscene();
			Bootup();
			Main();
			NewGame();
			Scores();
			Projector();
			GameOver();
			Campaign();
		} else {
			switch(CurrentWindow[0]){
				case "Projector":
					WhileLoading();
					Cutscene();
					Bootup();
					Main();
					NewGame();
					Scores();
					Projector(true, CurrentWindow[0]);
					GameOver();
					Campaign();
					break;
				case "Scores":
					WhileLoading();
					Cutscene();
					Bootup();
					Main();
					NewGame();
					Scores(true);
					Projector();
					GameOver();
					Campaign();
					break;
				case "NewGame":
					WhileLoading();
					Cutscene();
					Bootup();
					Main();
					NewGame(true);
					Scores();
					Projector();
					GameOver();
					Campaign();
					break;
				case "Main": case "Play": case "Options": case "File": case "Quit":
					WhileLoading();
					Cutscene();
					Bootup();
					Main(CurrentWindow[0]);
					NewGame();
					Scores();
					Projector();
					GameOver();
					Campaign();
					break;
				case "Prologue": case "Credits":
					WhileLoading();
					Cutscene(CurrentWindow[0]);
					Bootup();
					Main();
					NewGame();
					Scores();
					Projector();
					GameOver();
					Campaign();
					break;
				case "Start":
					WhileLoading();
					Cutscene();
					Bootup(true);
					Main();
					NewGame();
					Scores();
					Projector();
					GameOver();
					Campaign();
					break;
				case "GameOver": case "TheEnd":
					WhileLoading();
					Cutscene();
					Bootup();
					Main();
					NewGame();
					Scores();
					Projector();
					GameOver(CurrentWindow[0]);
					Campaign();
					break;
				case "CampaignMain": case "CampaignMessage": case "CampaignCustomization":
					WhileLoading();
					Cutscene();
					Bootup();
					Main();
					NewGame();
					Scores();
					Projector();
					GameOver();
					Campaign(CurrentWindow[0]);
					break;
				default:
					WhileLoading();
					Cutscene();
					Bootup();
					Main();
					NewGame();
					Scores();
					Projector();
					GameOver();
					Campaign();
					break;
			}
		}

		// Main Menu Plane
		if(CurrentWindow[0] == "Start" || CurrentWindow[0] == "Main" || CurrentWindow[0] == "Options" || CurrentWindow[0] == "NewGame" || CurrentWindow[0] == "Play" || CurrentWindow[0] == "NewGameInfo" || CurrentWindow[0] == "Credits"){
			
			if(CurrentMusic != "Main"){
				PlayMusic ("Main");
			}

			MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, GameObject.Find ("StartCamera").transform.position, 0.1f * (Time.deltaTime * 100f));
			MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, GameObject.Find ("StartCamera").transform.rotation, 0.1f * (Time.deltaTime * 100f));

			if (StartLandsPosition < 1000f) {
				foreach(Transform Land in StartLands.transform){
					if(Land.name == "Land1"){
						foreach(Transform LiL1 in Land){
							if(LiL1.name == SLand1){
								LiL1.gameObject.SetActive(true);
								LiL1.eulerAngles = new Vector3 (-90f, 0f, (float)SRLand1 * 90f);
							} else {
								LiL1.gameObject.SetActive(false);
							}
						}
					} else if(Land.name == "Land2"){
						foreach(Transform LiL2 in Land){
							if(LiL2.name == SLand2){
								LiL2.gameObject.SetActive(true);
								LiL2.eulerAngles = new Vector3 (-90f, 0f, (float)SRLand2 * 90f);
							} else {
								LiL2.gameObject.SetActive(false);
							}
						}
					} else if(Land.name == "Land3"){
						foreach(Transform LiL3 in Land){
							if(LiL3.name == SLand3){
								LiL3.gameObject.SetActive(true);
								LiL3.eulerAngles = new Vector3 (-90f, 0f, (float)SRLand3 * 90f);
							} else {
								LiL3.gameObject.SetActive(false);
							}
						}
					}
				}
				StartLandsPosition += 10f * (Time.deltaTime * 100f);
				StartLands.transform.position = new Vector3 (0f, 2000f, 0f + StartLandsPosition);
			} else {
				StartLandsPosition = 0f;

				SLand3 = SLand2;
				SRLand3 = SRLand2;
				SLand2 = SLand1;
				SRLand2 = SRLand1;
				SRLand1 = Random.Range (1, 4);
				int SSLand1 = Random.Range (0, 3);
				if(SSLand1 == 0){
					SLand1 = "EmptyLand";
				} else if(SSLand1 == 1){
					SLand1 = "Forest";
				} else if(SSLand1 == 2){
					SLand1 = "Village";
				}

			}

		}
		if (Vector3.Distance(MainMenuPlane.transform.position, MainMenuPlane.transform.parent.position + SetPos) < 0.2f) {
			SetPos = new Vector3(
				0f,
				Mathf.Clamp(SetPos.y + Random.Range(-0.5f, 0.5f), -3f, -4f),
				Mathf.Clamp(SetPos.z + Random.Range(-0.5f, 0.5f), -4f, 4f)
			);
		}
		MainMenuPlane.transform.GetChild (0).Rotate (-20f * (Time.fixedDeltaTime * 100f), 0f, 0f);
		MainMenuPlane.transform.LookAt (MainMenuPlane.transform.parent.position, Vector3.left);
		MainMenuPlane.transform.position = Vector3.MoveTowards (MainMenuPlane.transform.position, MainMenuPlane.transform.parent.position + SetPos, 0.0025f * (Time.deltaTime * 100f));
        // Main Menu Plane

        // Campaign Menu
        // Free rotation
        if (Input.GetAxis("Rudder") != 0f){
            CampaignCamRotation -= Input.GetAxis("Rudder");
            CapmaignFreeRotation = 5f;
        }
        // Free roation
        if (CampaignCamRotation < 360f) {
            if (CapmaignFreeRotation > 0f){
                CapmaignFreeRotation -= 0.01f * (Time.deltaTime * 100f);
            } else {
                CampaignCamRotation += 0.1f * (Time.deltaTime * 100f);
            }
		} else if (CampaignCamRotation > 360f){
            CampaignCamRotation = 0f;
        } else if (CampaignCamRotation < 0f){
            CampaignCamRotation = 360f;
        }
        if (CurrentWindow[0] == "CampaignMain" || CurrentWindow[0] == "CampaignCustomization" || CurrentWindow[0] == "CampaignMessage") {

			if(CurrentMusic != "Campaign" && Loading <= 0f){
				PlayMusic ("Campaign");
			}

			GameObject.Find ("CampaignCamera").transform.rotation = Quaternion.Euler (0f, CampaignCamRotation, 0f);
			MainCamera.transform.position = GameObject.Find ("CampaignCamera").transform.position + GameObject.Find ("CampaignCamera").transform.forward * 10f + Vector3.up * 5f;
			MainCamera.transform.LookAt (GameObject.Find ("CampaignCamera").transform.position + (Vector3.up * 2f));

			if (PCPlaneModelDTN > 0f) PCPlaneModelDTN -= Time.unscaledDeltaTime;
			else PCPlaneModel = IC.PlaneModels[GS.CurrentPlaneModel];
			if (PCEngineModelDTN > 0f) PCEngineModelDTN -= Time.unscaledDeltaTime;
			else PCEngineModel = IC.EngineModels[GS.CurrentEngineModel];
			if (PCPaintDTN > 0f) PCPaintDTN -= Time.unscaledDeltaTime;
			else PCPaint = IC.Paints[GS.CurrentPaint];
			if (PCAditionDTN > 0f) PCAditionDTN -= Time.unscaledDeltaTime;
			else PCAddition = IC.Additions[GS.CurrentAddition];

			foreach(Transform SelectedPlane in PlaneinCustomization.transform){
				if(SelectedPlane.name == PCPlaneModel.Names[0]){
					SelectedPlane.gameObject.SetActive(true);
					foreach(Transform Part in SelectedPlane){
						if (Part.name == PCEngineModel.Names[0] || Part.name == PCPlaneModel.Names[0] || (Part.name == "Turret" && PCAddition.Names[0] == "Turret")) {
							Part.gameObject.SetActive (true);
                            if (Part.name == PCPlaneModel.Names[0] || Part.name == PCEngineModel.Names[0]) {
                                foreach (Material PlaneMat in Part.GetComponent<MeshRenderer>().materials) {
                                    if (PlaneMat.name == "PlaneColor1 (Instance)") {
										PlaneMat.color = PCPaint.Paints[0];
                                    } else if (PlaneMat.name == "PlaneColor2 (Instance)") {
										PlaneMat.color = PCPaint.Paints[1];
                                    }
                                }
                            } else if (Part.name == "Turret") {
                                foreach (Material PlaneMat in Part.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().materials){
                                    if (PlaneMat.name == "PlaneColor1 (Instance)") {
										PlaneMat.color = PCPaint.Paints[0];
                                    } else if (PlaneMat.name == "PlaneColor2 (Instance)") {
										PlaneMat.color = PCPaint.Paints[1];
                                    }
                                }
                            }
						} else {
							Part.gameObject.SetActive (false);
						}
					}
				} else {
					SelectedPlane.gameObject.SetActive(false);
				}
			}

		}
		// Campaign Menu

		// Loading
		if(WindowToLoad != ""){
			if (TimeUntilLoad > 0f) {
				Loading = 1f;
				TimeUntilLoad -= Time.deltaTime;
			} else {
				GS.LoadLevel(WindowToLoad, "");
			}
		}
		// Loading
		
	}

	void PlayMusic(string MusicName){
		foreach(Transform Music in Musics.transform){
			if (Music.name == MusicName) {
				Music.GetComponent<AudioSource> ().Play ();
				CurrentMusic = MusicName;
			} else {
				Music.GetComponent<AudioSource> ().Stop ();
			}
		}
	}

    void SettingMessage(){

        if (GS.HasDied == true) {
            if (GS.Parachutes > 0 && GS.Parachutes < 1000) {
                MessageText.text = GS.SetText(
                "You have crashed. All of the score and mooney you have earned in this round, have been lost, and you also have to repeat this round. Fortunetaly, you still have " + GS.Parachutes + " parachutes left!",
                "Rozbiłeś się. Twój wynik oraz piniądze zdobyte w tej rundzie, zostały utracone, i musisz powtórzyć tą rundę. Na szczęscie, jeszcze masz " + GS.Parachutes + " spadochron/y/ów!");
            } else if (GS.Parachutes < 1000) {
                MessageText.text = GS.SetText(
                "You have crashed. All of the score and mooney you have earned in this round, have been lost, and you also have to repeat this round. You also don't have any parachutes left!\n\nIF YOU CRASH AGAIN, IT'S GAME OVER",
                "Rozbiłeś się. Twój wynik oraz piniądze zdobyte w tej rundzie, zostały utracone, i musisz powtórzyć tą rundę. Warto wspomnieć, że nie masz już ani jednego spadochronu!\n\nJEŚLI ZNOWU SIĘ ROZBIJESZ, TO KONIEC GRY");
            }
        } else if (GS.Level == 2) {
            MessageText.text = GS.SetText(
                "So, you've beaten the first level. Easy, wasn't it? Well, it's gonna get harder from here. In the second level, there will be enemy planes patroling the map. If you get too close to one of them, they will start shooting at you, so you better prepare yourself for a little bit of dogfighting! Oh, and BTW, this is the menu, where you're sent after every level. You can customize your plane, read new messages, and save your progress in here!",
                "Widocznie udało ci się zaliczyć pierwszy poziom. Łatwizna, no nie? No cóż, z tąd już będzie tylko pod górkę. W drugim poziomie, pojawią się wrogie samoloty. Jeżeli podlecisz zbyt blisko jednego z nich, zaczną do ciebie strzelać, tak więc przygotuj się na walkę! Aha, i jeszcze jedno, to jest menu do którego będziesz wysyłany po zaliczeniu każdego z poziomów. Tutaj, możesz edytować swój samolot, czytać nowe wiadomości, oraz zapisywać swój postęp!");
        } else if (GS.Level == 3) {
            MessageText.text = GS.SetText(
                "If you're reading this, then that means you've survived! Congratulations! Just keep in mind, that you don't have to eliminate all of the foes, but each foe gives you some mooney and score!",
                "Jeśli to czytasz, to znaczy że przeżyłeś! Gratulacje! Miej tylko na uwadze, że nie musisz eliminować wszystkich przeciwników, aczkolwiek każdy przeciwnik daje ci trochę piniędzy i punktów!");
        } else if (GS.Level == 4) {
            MessageText.text = GS.SetText(
                "More houses! Now, you'll have to deliver presents to two houses, instead of one. Every four levels, an additional house will be spawned.",
                "Zaraz będzie więcej domów! Teraz, zamiast dostarczyć prezent do jednego domu, będziesz musiał dostarczyć dwa prezenty do dwóch domów. Co cztery poziomy, dodatkowy dom będzie się pojawiał.");
        } else if (GS.Level == 5) {
            MessageText.text = GS.SetText(
                "The enemy is not only growing, but is also evolving! Now, aside from bigger amount of foes, there are new types of enemies to deal with. These new types are: Messerschmitt K4, and AA Gun. Messerschmitt K4, is pretty much the same as your regular Messerschmitt, except it has twice as much health, and has M2 Browning guns, instead of Vickers. AA Gun on the other hand, is immobile, but is constantly aiming and shooting at you, so be careful!",
                "Wróg nie tylko się powiększa, ale także ewouluje! Teraz, oprócz większej ilości nieprzyjaciół, są nowe rodzaje nieprzyjaciół. Tymi nowymi rodzajami są: Messerschmitt K4, oraz Broń Przeciwlotnicza. Messerschmitt K4, jest prawie taki sam jak zwykły Messerschmitt, z taką różnicą: że ma dwukrotnie więcej zdrowia, i ma broń M2 Browning, zamiast Vickers. Broń Przeciwlotnicza, jest nieruchoma, aczkolwiek cały czas w ciebie celuje i strzela, tak więc bądź ostrożny!");
        } else if (GS.Level == 10) {
            MessageText.text = GS.SetText(
                "Those german scientis have came up with new ways to eliminate you! They have created: Messerschmitt 110, and Balloons! Messerschmitt 110 is yet again pretty much the same as the previous models, the difference is that it has Cannons instead of Vickers, and that it has a backgunner (if you get behind this plane, it's backgunner will start shooting straight at you). Balloons are, well, AA Guns, except they are suspended in air. These two, are surely dangerous, so watch out!",
                "Ci niemieccy naukowcy wynaleźli nowe sposoby na twoją eliminację! Stworzyli: Messerschmitt 110, oraz balony! Messerschmit 110, jest taki sam jak inne messerschmitty, różnica jest taka, że ma Cannon'y zamiast Vickers, i że ma tylniego strzelca (jeśli znajdziesz się za tym samolotem, jego strzelec tylni zacznie strzelać prosto na ciebie). Czym są balony? To poprostu bronie przeciwlotnicze, tylko że wiszące w powietrzu. Te dwa wynalazki, są z pewnością niebezpieczne, tak więc uważaj na siebie!");
        } else if (GS.Level == 10) {
            MessageText.text = GS.SetText(
                "Having troubles with dogfighting? No? Very good, because they've created a new type of plane: Messerschmitt Me 262! This bad boy is as tuff as a Messerschmitt K4, is twice as fast as regular Messerschmitt, and has a powerful Jet Gun! With these fellas, the dogfighting is sure to become more interesting!",
                "Masz problemy z wrogimi samolotami? Nie? To bardzo dobrze, bo na scenę wchodzi nowy rodzaj samolotów: Messerschmitt Me 262! Ten zły chłopiec jest tak silny jak Messerschmitt K4, jest podwójnie szybszy od zwykłego Messerschmitt'a, i ma potężną broń Jet Gun! Z tymi delikwentami, walka stanie się bardzo interesująca!");
        } else {
            MessageText.text = "";
        }

    }

	string getOrigin(string Category, int What = -1, string setCategory = ""){
		if(What == -1) { // Receive to origin
			switch(Category){
				case "Plane Model": return GS.OwnedPlaneModels;
				case "Engine Model": return GS.OwnedEngineModels;
				case "Gun Type": return GS.OwnedGundTypes;
				case "Present Cannon Type": return GS.OwnedPresentCannonTypes;
				case "Special Type": return GS.OwnedSpecialTypes;
				case "Addition": return GS.OwnedAdditions;
				case "Paint": return GS.OwnedPaints;
				default:
					Debug.LogError("No return value for " + Category + " - " + What);
					return "";
			}
		} else if (What == -2) { // Set to origin
			switch(Category){
				case "Plane Model": GS.OwnedPlaneModels = setCategory; break;
				case "Engine Model": GS.OwnedEngineModels = setCategory; break;
				case "Gun Type": GS.OwnedGundTypes = setCategory; break;
				case "Present Cannon Type": GS.OwnedPresentCannonTypes = setCategory; break;
				case "Special Type": GS.OwnedSpecialTypes = setCategory; break;
				case "Addition": GS.OwnedAdditions = setCategory; break;
				case "Paint": GS.OwnedPaints = setCategory; break;
				default: Debug.LogError("No return value for " + Category + " - " + What); break;
			}
			return "";
		} else {
			switch(Category){
				case "Plane Model": 
					if(setCategory == "Equip") GS.CurrentPlaneModel = What; 
					else if (setCategory == "Receive") return GS.CurrentPlaneModel.ToString(); break;
				case "Engine Model": 
					if(setCategory == "Equip") GS.CurrentEngineModel = What;
					else if (setCategory == "Receive") return GS.CurrentEngineModel.ToString(); break;
				case "Gun Type": 
					if(setCategory == "Equip") GS.CurrentGunType = What;
					else if (setCategory == "Receive") return GS.CurrentGunType.ToString(); break;
				case "Present Cannon Type": 
					if(setCategory == "Equip") GS.CurrentPresentCannonType = What;
					else if (setCategory == "Receive") return GS.CurrentPresentCannonType.ToString(); break;
				case "Special Type": 
					if(setCategory == "Equip") GS.CurrentSpecialType = What;
					else if (setCategory == "Receive") return GS.CurrentSpecialType.ToString(); break;
				case "Addition": 
					if(setCategory == "Equip") GS.CurrentAddition = What;
					else if (setCategory == "Receive") return GS.CurrentAddition.ToString(); break;
				case "Paint": 
					if(setCategory == "Equip") GS.CurrentPaint = What;
					else if (setCategory == "Receive") return GS.CurrentPaint.ToString(); break;
				default: Debug.LogError("No return value for " + Category + " - " + What); break;
			}
			return "";
		}
	}

	string Replace(string sauce, int Index){
		sauce = sauce.Remove(Index, 1); sauce = sauce.Insert(Index, "1"); return sauce;
	}

	void CampaignCustomization(){

		if (currentCustomizationOption == "") {
			WhichPage = 0;
			CurrentCustomizationOptionText.text = GS.SetText ("", "");
			OptionInfoName.text = "";
			OptionInfoDescription.text = "";
			pnButtons[0].transform.parent.localScale = Vector3.zero;
			foreach (Transform Button in CustomizationButtons.transform) {
				string[] ChooseApropiate = new string[]{"", ""};
				Button.GetComponent<ButtonScript>().IsActive = true;
				if (Button.gameObject.name == "1") {
					ChooseApropiate[0] = GS.SetText ("Plane Model:\n" + IC.PlaneModels[GS.CurrentPlaneModel].Names[0], "Model Samolotu:\n" + IC.PlaneModels[GS.CurrentPlaneModel].Names[1]);
					ChooseApropiate[1] =  "Plane Model";
				} else if (Button.gameObject.name == "2") {
					ChooseApropiate[0] = GS.SetText ("Engine Model:\n" + IC.EngineModels[GS.CurrentEngineModel].Names[0], "Model Silnika:\n" + IC.EngineModels[GS.CurrentEngineModel].Names[1]);
					ChooseApropiate[1] =  "Engine Model";
				} else if (Button.gameObject.name == "3") {
					ChooseApropiate[0] = GS.SetText ("Gun Type:\n" + IC.Guns[GS.CurrentGunType].Names[0], "Typ Broni:\n" + IC.Guns[GS.CurrentGunType].Names[1]);
					ChooseApropiate[1] =  "Gun Type";
				} else if (Button.gameObject.name == "4") {
					ChooseApropiate[0] = GS.SetText ("Present Cannon Type:\n" + IC.Presents[GS.CurrentPresentCannonType].Names[0], "Typ Wyrzutni Prezentów:\n" + IC.Presents[GS.CurrentPresentCannonType].Names[1]);
					ChooseApropiate[1] =  "Present Cannon Type";
				} else if (Button.gameObject.name == "5") {
					ChooseApropiate[0] = GS.SetText ("Special Type:\n" + IC.Specials[GS.CurrentSpecialType].Names[0], "Typ Ekwipunku Specjalnego:\n" + IC.Specials[GS.CurrentSpecialType].Names[1]);
					ChooseApropiate[1] =  "Special Type";
				} else if (Button.gameObject.name == "6") {
					ChooseApropiate[0] = GS.SetText ("Addition:\n" + IC.Additions[GS.CurrentAddition].Names[0], "Dodatek:\n" + IC.Additions[GS.CurrentAddition].Names[1]);
					ChooseApropiate[1] =  "Addition";
				} else if (Button.gameObject.name == "7") {
					ChooseApropiate[0] = GS.SetText ("Paint:\n" + IC.Paints[GS.CurrentPaint].Names[0], "Farba:\n" + IC.Paints[GS.CurrentPaint].Names[1]);
					ChooseApropiate[1] = "Paint";
				}

				Button.gameObject.GetComponent<Text> ().text = ChooseApropiate[0];
				if (Button.gameObject.GetComponent<ButtonScript> ().IsSelected && Input.GetMouseButtonDown (0)) {
					currentCustomizationOption = ChooseApropiate[1];
					Button.gameObject.GetComponent<ButtonScript> ().IsSelected = false;
					Click.Play ();
					OriginString = getOrigin(currentCustomizationOption);
					switch(currentCustomizationOption){
						case "Plane Model": OriginArray = IC.PlaneModels; break;
						case "Engine Model": OriginArray = IC.EngineModels; break;
						case "Gun Type": OriginArray = IC.Guns; break;
						case "Present Cannon Type": OriginArray = IC.Presents; break;
						case "Special Type": OriginArray = IC.Specials; break;
						case "Addition": OriginArray = IC.Additions; break;
						case "Paint": OriginArray = IC.Paints; break;
					}
				}
			}
		} else {		

			pnButtons[0].transform.parent.localScale = Vector3.one;
			for(int sb = 0; sb < 7; sb++){
				int Index = sb + WhichPage*7;
				ButtonScript currButt = CustomizationButtons.transform.GetChild(sb).GetComponent<ButtonScript>();
				Text currText = currButt.GetComponent<Text>();

				if(Index < OriginArray.Length){

					currButt.IsActive = true;
					char OwnStatus = OriginString[Index];

					switch(OwnStatus){
						case '0': currText.text = GS.SetText(OriginArray[Index].Names[0], OriginArray[Index].Names[1]) + "\n" + OriginArray[Index].Price; break;
						case '1':
							if (getOrigin(currentCustomizationOption, Index, "Receive") == Index.ToString()) currText.text = GS.SetText(OriginArray[Index].Names[0], OriginArray[Index].Names[1]) + GS.SetText("\nEQUIPED", "\nZAMONTOWANE");
							else currText.text = GS.SetText(OriginArray[Index].Names[0], OriginArray[Index].Names[1]) + GS.SetText("\nOwned", "\nPosiadasz"); break;
					}

					if(currButt.IsSelected){
						EraseOptionInfo = 1f;
						OptionInfoName.text = GS.SetText(OriginArray[Index].Names[0], OriginArray[Index].Names[1]);
						OptionInfoDescription.text = GS.SetText(OriginArray[Index].Desc[0], OriginArray[Index].Desc[1]);

						if(Input.GetMouseButtonDown(0)){
							if(OwnStatus == '0' && GS.Mooney >= OriginArray[Index].Price){
								OriginString = Replace(OriginString, Index);
								getOrigin(currentCustomizationOption, -2, OriginString);
								GS.Mooney -= OriginArray[Index].Price;
								getOrigin(currentCustomizationOption, Index, "Equip");
								CustomizationSounds[0].Play();
							} else if (OwnStatus == '1'){
								getOrigin(currentCustomizationOption, Index, "Equip");
								CustomizationSounds[1].Play();
							}
						}

						switch(currentCustomizationOption){
							case "Plane Model": PCPlaneModelDTN = 0.3f; PCPlaneModel = IC.PlaneModels[Index]; break;
							case "Engine Model": PCEngineModelDTN = 0.3f; PCEngineModel = IC.EngineModels[Index]; break;
							case "Addition": PCAditionDTN = 0.3f; PCAddition = IC.Additions[Index]; break;
							case "Paint": PCPaintDTN = 0.3f; PCPaint = IC.Paints[Index]; break;
						}
					}

					if(WhichPage > 0) {
						pnButtons[0].GetComponent<Text>().text = "<---";
						pnButtons[0].IsActive = true;
						if(pnButtons[0].IsSelected && Input.GetMouseButtonDown(0)) WhichPage -= 1;
					} else {
						pnButtons[0].GetComponent<Text>().text = "";
						pnButtons[0].IsActive = false;
					}

					if(sb==6 && Index < OriginArray.Length-1) {
						pnButtons[1].GetComponent<Text>().text = "--->";
						pnButtons[1].IsActive = true;
						if(pnButtons[1].IsSelected && Input.GetMouseButtonDown(0)) WhichPage += 1;
					} else {
						pnButtons[1].GetComponent<Text>().text = "";
						pnButtons[1].IsActive = false;
					}

				} else {

					currButt.IsActive = false;
					currText.text = "";

				}

			}

		}

	}

}

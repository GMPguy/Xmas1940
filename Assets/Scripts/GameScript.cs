using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;

public class GameScript : MonoBehaviour {

	// References
	public PostProcessingProfile PPP1;
	// References

	// Options
	public int currentResolution = -1;
	public Vector2[] LoadedResolutions;
	public string Language = "English";
	public float[] Volumes = new float[] { 1f, 1f, 1f, -1}; // master, music, sounds, prev sounds
	public int ControlScheme = 0; // Mouse and Keyboard, Pointing
	public bool InvertedMouse = false;
    public Color[] PrevLight = new Color[2];
	int SwitchDesperate = 0;
	public string OptionsChoice = "";
	int selectedKB = 0;
	int KBphase = 0;
	KeyBind[] KeyBinds;
	[System.Serializable] struct KeyBind{
		public string[] Names;
		public KeyCode[] theBind;
		public KeyBind(string[] nNames, KeyCode[] NewBind){
			Names = nNames; theBind = NewBind;
		}
	}
	KeyCode[] potentialKB;
	// Options

	// Game
	public int saveIndex = -1;
	public string Name;
	public int GameMode = 0; // 0 Campaign, 1 Endless, 2 Skirmish
	public Stat[] Statistics;
	public int Level = 1;
	public int Difficulty = 0;
	public int CurrentScore = 0;
    public int TempScore = 0;
	public int Mooney = 0;
    public int TempMooney = 0;
    public int Parachutes = 0;

	public int CurrentPlaneModel = 0;
	public string OwnedPlaneModels = "10000";
	public int CurrentEngineModel = 0;
	public string OwnedEngineModels = "10000";
	public int CurrentGunType = 0;
	public string OwnedGundTypes = "10000";
	public int CurrentPresentCannonType = 0;
	public string OwnedPresentCannonTypes = "100";
	public int CurrentSpecialType = 0;
	public string OwnedSpecialTypes = "10000";
	public int CurrentAddition = 0;
	public string OwnedAdditions = "10000";
	public int CurrentPaint = 0;
	public string OwnedPaints = "10000";

	public List<HighScore> HighScores;
	[System.Serializable] public struct HighScore{
		public string hsName;
		public int hsMode, hsDiff, hsScore, hsDate;
		public HighScore(string newName, int newScore, int newMode, int newDiff, int newDate){
			hsName = newName; hsScore = newScore; hsDiff = newDiff; hsMode = newMode; hsDate = newDate;
		}
	}
	int hsDates = 0;
	public string displayedHS;
	// Game

	// Misc
	public string WhichMenuWindowToLoad = "";
	bool EreasePrefs = false;
	public int PreviousScore = 0;
	public string PreviousPlane = "";
    public bool HasDied = false;
	// Misc

	// Main Variables
	public string Version = "1.0";
	public string Build = "Screen";
	// Main Variables

	// Use this for initialization
	void Start () {

		if(this.gameObject.name == "GameScriptSTART"){
			if (GameObject.Find ("GameScript") == null) {
				this.gameObject.name = "GameScript";
				DontDestroyOnLoad (this.gameObject);
				// Load stuff
				if(PlayerPrefs.GetInt("HasSaved") == 1){
					currentResolution = PlayerPrefs.GetInt("Resolution");
					Language = PlayerPrefs.GetString ("Language");
					Volumes[0] = PlayerPrefs.GetFloat ("MasterVolume");
					Volumes[1] = PlayerPrefs.GetFloat ("AudioVolume");
					Volumes[2] = PlayerPrefs.GetFloat ("MusicVolume");
					SaveScore("Load");
					ControlScheme = PlayerPrefs.GetInt("Controls");

					if (PlayerPrefs.GetString ("Inverted") == "True") InvertedMouse = true;
					else InvertedMouse = false;

					if (PlayerPrefs.GetString ("Fullscreen") == "True") Screen.fullScreen = true;
					else Screen.fullScreen = false;
					
				}
				// Load stuff

				List<Vector2> loadResolutions = new();
				int defaulte = 0;
				int prefRR = -1;
				foreach(Resolution res in Screen.resolutions)
				if(res.refreshRate == prefRR || prefRR == -1) {
					loadResolutions.Add(new Vector2(res.width, res.height));
					defaulte++;
					if(res.width == 800 && currentResolution == -1) currentResolution = defaulte;
					if(prefRR == -1) prefRR = res.refreshRate;
				}
				LoadedResolutions = loadResolutions.ToArray();

				statModify("SetUp", 0);

				KeyBinds = new KeyBind[]{
					new (new []{"Throttle", "Przepustnica"}, new []{KeyCode.LeftShift, KeyCode.LeftControl}),
					new (new []{"Pitch", "Pochylanie"}, new []{KeyCode.W, KeyCode.S}),
					new (new []{"Roll", "Przechylanie"}, new []{KeyCode.D, KeyCode.A}),
					new (new []{"Yaw", "Odchylanie"}, new []{KeyCode.E, KeyCode.Q}),
					new (new []{"Fire guns", "Strzelanie"}, new []{KeyCode.Mouse0}),
					new (new []{"Aiming", "Celowanie"}, new []{KeyCode.Mouse1}),
					new (new []{"Fire a present", "Wystrzelenie prezentu"}, new []{KeyCode.Space}),
					new (new []{"Use a special", "Przedmiot specjalny"}, new []{KeyCode.R}),
					new (new []{"Free view", "Wolny obrót"}, new []{KeyCode.V})
				};

				List<KeyCode> newPKB = new ();
				foreach(KeyCode add in System.Enum.GetValues(typeof(KeyCode))){
					newPKB.Add (add);
				}
				potentialKB = newPKB.ToArray();
				recInput("Load");

			} else {
				Destroy (this.gameObject);
			}
		}
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(GameObject.Find("RoundScript") != null){
			if(GameObject.Find("RoundScript").GetComponent<RoundScript>().enabled == false){
				GameObject.Find ("RoundScript").GetComponent<RoundScript> ().enabled = true;
			}
		}

		// Erase prefs
		if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.L)){
			if(EreasePrefs == false){
				EreasePrefs = true;
				PlayerPrefs.DeleteAll ();
				print("Data erased successfully");
			}
		}
		// Erase prefs

		// Set Graphics
		if(GameObject.Find("MainCamera") != null){
			
			if((QualitySettings.GetQualityLevel() == 0 || QualitySettings.GetQualityLevel() == 1) && GameObject.Find("MainCamera").GetComponent<Camera>().allowMSAA == true){
				GameObject.Find ("MainCamera").GetComponent<Camera> ().allowMSAA = false;
			} else if((QualitySettings.GetQualityLevel() == 2 || QualitySettings.GetQualityLevel() == 3) && GameObject.Find("MainCamera").GetComponent<Camera>().allowMSAA == false){
				GameObject.Find ("MainCamera").GetComponent<Camera> ().allowMSAA = true;
			}

			if (QualitySettings.GetQualityLevel () == 0 && SwitchDesperate != 0) {
				SwitchDesperate = 0;
				GameObject.Find ("MainCamera").GetComponent<Camera> ().clearFlags = CameraClearFlags.Color;
				foreach(GameObject HidMarker in GameObject.FindGameObjectsWithTag("Foe")) HidMarker.GetComponent<EnemyVesselScript>().Marker(-1);
				if(GameObject.Find("MainLight")){
					Light S = GameObject.Find("MainLight").GetComponent<Light>();
					GameObject.Find ("MainCamera").GetComponent<Camera> ().backgroundColor = S.color;
					RenderSettings.ambientLight = S.color;
					S.intensity = 0f;
				}
			} else if (QualitySettings.GetQualityLevel () != 0 && SwitchDesperate != 1) {
				SwitchDesperate = 1;
				GameObject.Find ("MainCamera").GetComponent<Camera> ().clearFlags = CameraClearFlags.Skybox;
				if(GameObject.Find("MainLight")){
					Light S = GameObject.Find("MainLight").GetComponent<Light>();
					GameObject.Find ("MainCamera").GetComponent<Camera> ().backgroundColor = PrevLight[0];
					RenderSettings.ambientLight = PrevLight[1];
					S.intensity = 1f;
				}
			}

			if (QualitySettings.GetQualityLevel () == 3) {
				GameObject.Find ("MainCamera").GetComponent<PostProcessingBehaviour> ().enabled = true;
			} else {
				GameObject.Find ("MainCamera").GetComponent<PostProcessingBehaviour> ().enabled = false;
			}
				
		}
		// Set Graphics

		// Set Audios
		if(Volumes[3] != Volumes[0] + Volumes[1] + Volumes[2]) {
			Volumes[3] = Volumes[0] + Volumes[1] + Volumes[2];
			foreach(SoundControll A in FindObjectsOfType<SoundControll>()) A.Set();
		}
		// Set Audios
		
	}

	public string SetText(string English, string Polish){
		if(Language == "Polski"){
			return Polish;
		} else {
			return English;
		}
	}

	public void LoadLevel(string LevelName, string LevelBonus){
		if(LevelName == "MainMenu") WhichMenuWindowToLoad = LevelBonus;
		SceneManager.LoadScene (LevelName);
		SwitchDesperate = 1;
	}

	public void SetGameOptions(string Which, string File){
		File += "-" + Version + "-" + Build;
		if(Which == "Empty"){
			PlayerPrefs.SetInt (File + "SavedGame", saveIndex);
			Name = "TestSave";
			GameMode = 1;
			Level = 1;
			Difficulty = 1;
			CurrentScore = 0;
            TempScore = 0;
			Mooney = 0;
            TempMooney = 0;
            Parachutes = 3;
			CurrentPlaneModel = 0;
			OwnedPlaneModels = "100000000000000000000000000000";
			CurrentEngineModel = 0;
			OwnedEngineModels = "100000000000000000000000000000";
			CurrentGunType = 0;
			OwnedGundTypes = "100000000000000000000000000000";
			CurrentPresentCannonType = 0;
			OwnedPresentCannonTypes = "100000000000000000000000000000";
			CurrentSpecialType = 0;
			OwnedSpecialTypes = "100000000000000000000000000000";
			CurrentAddition = 0;
			OwnedAdditions = "100000000000000000000000000000";
			CurrentPaint = 0;
			OwnedPaints = "100000000000000000000000000000";
			statModify("SetUp", 0);
		} else if(Which == "Save"){
			PlayerPrefs.SetInt (File + "SavedGame", saveIndex);
			PlayerPrefs.SetString (File + "Name", Name);
			PlayerPrefs.SetInt (File + "Mode", GameMode);
			PlayerPrefs.SetInt (File + "Level", Level);
			PlayerPrefs.SetInt (File + "DifficultyLevel", Difficulty);
			PlayerPrefs.SetInt (File + "CurrentScore", CurrentScore);
			PlayerPrefs.SetInt (File + "Mooney", Mooney);
            PlayerPrefs.SetInt (File + "Parachutes", Parachutes);
			PlayerPrefs.SetInt (File + "CurrentPlaneModel", CurrentPlaneModel);
			PlayerPrefs.SetString (File + "OwnedPlaneModels", OwnedPlaneModels);
			PlayerPrefs.SetInt (File + "CurrentEngineModel", CurrentEngineModel);
			PlayerPrefs.SetString (File + "OwnedEngineModels", OwnedEngineModels);
			PlayerPrefs.SetInt (File + "CurrentGunType", CurrentGunType);
			PlayerPrefs.SetString (File + "OwnedGunTypes", OwnedGundTypes);
			PlayerPrefs.SetInt (File + "CurrentPresentCannonType", CurrentPresentCannonType);
			PlayerPrefs.SetString (File + "OwnedPresentCannonTypes", OwnedPresentCannonTypes);
			PlayerPrefs.SetInt (File + "CurrentSpecialType", CurrentSpecialType);
			PlayerPrefs.SetString (File + "OwnedSpecialTypes", OwnedSpecialTypes);
			PlayerPrefs.SetInt (File + "CurrentAddition", CurrentAddition);
			PlayerPrefs.SetString (File + "OwnedAdditions", OwnedAdditions);
			PlayerPrefs.SetInt (File + "CurrentPaint", CurrentPaint);
			PlayerPrefs.SetString (File + "OwnedPaints", OwnedPaints);
			PlayerPrefs.SetString (File + "Statistics", statModify("Save", 0, File));
			SaveScore("Save");
		} else if(Which == "Load"){
			saveIndex = PlayerPrefs.GetInt (File + "SavedGame");
			Name = PlayerPrefs.GetString (File + "Name");
			GameMode = PlayerPrefs.GetInt (File + "Mode");
			Level = PlayerPrefs.GetInt (File + "Level");
			Difficulty = PlayerPrefs.GetInt (File + "DifficultyLevel");
			CurrentScore = PlayerPrefs.GetInt (File + "CurrentScore");
            TempScore = 0;
			Mooney = PlayerPrefs.GetInt (File + "Mooney");
            TempMooney = 0;
            Parachutes = PlayerPrefs.GetInt(File + "Parachutes");
			CurrentPlaneModel = PlayerPrefs.GetInt (File + "CurrentPlaneModel");
			OwnedPlaneModels = PlayerPrefs.GetString (File + "OwnedPlaneModels");
			CurrentEngineModel = PlayerPrefs.GetInt (File + "CurrentEngineModel");
			OwnedEngineModels = PlayerPrefs.GetString (File + "OwnedEngineModels");
			CurrentGunType = PlayerPrefs.GetInt (File + "CurrentGunType");
			OwnedGundTypes = PlayerPrefs.GetString (File + "OwnedGunTypes");
			CurrentPresentCannonType = PlayerPrefs.GetInt (File + "CurrentPresentCannonType");
			OwnedPresentCannonTypes = PlayerPrefs.GetString (File + "OwnedPresentCannonTypes");
			CurrentSpecialType = PlayerPrefs.GetInt (File + "CurrentSpecialType");
			OwnedSpecialTypes = PlayerPrefs.GetString (File + "OwnedSpecialTypes");
			CurrentAddition = PlayerPrefs.GetInt (File + "CurrentAddition");
			OwnedAdditions = PlayerPrefs.GetString (File + "OwnedAdditions");
			CurrentPaint = PlayerPrefs.GetInt (File + "CurrentPaint");
			OwnedPaints = PlayerPrefs.GetString (File + "OwnedPaints");
			statModify("Load", 0, File + "Statistics");
			SaveScore("Load");
		} else if (Which == "Erase"){
			PlayerPrefs.DeleteKey (File + "SavedGame");
			PlayerPrefs.DeleteKey (File + "Name");
			PlayerPrefs.DeleteKey (File + "Mode");
			PlayerPrefs.DeleteKey (File + "Level");
			PlayerPrefs.DeleteKey (File + "DifficultyLevel");
			PlayerPrefs.DeleteKey (File + "CurrentScore");
			PlayerPrefs.DeleteKey (File + "Mooney");
            PlayerPrefs.DeleteKey (File + "Parachutes");
			PlayerPrefs.DeleteKey (File + "CurrentPlaneModel");
			PlayerPrefs.DeleteKey (File + "OwnedPlaneModels");
			PlayerPrefs.DeleteKey (File + "CurrentEngineModel");
			PlayerPrefs.DeleteKey (File + "OwnedEngineModels");
			PlayerPrefs.DeleteKey (File + "CurrentGunType");
			PlayerPrefs.DeleteKey (File + "OwnedGunTypes");
			PlayerPrefs.DeleteKey (File + "CurrentPresentCannonType");
			PlayerPrefs.DeleteKey (File + "OwnedPresentCannonTypes");
			PlayerPrefs.DeleteKey (File + "CurrentSpecialType");
			PlayerPrefs.DeleteKey (File + "OwnedSpecialTypes");
			PlayerPrefs.DeleteKey (File + "CurrentAddition");
			PlayerPrefs.DeleteKey (File + "OwnedAdditions");
			PlayerPrefs.DeleteKey (File + "CurrentPaint");
			PlayerPrefs.DeleteKey (File + "OwnedPaints");
			PlayerPrefs.DeleteKey (File + "Statistics");
			statModify("SetUp", 0);
			//SaveScore("Wipe");
		}
	}

	public void GainScore(int GainScore){
		GameObject.Find("RoundScript").GetComponent<RoundScript>().TempScore += (int)(GainScore * Mathf.Clamp(Difficulty, 0.5f, Mathf.Infinity));
	}

	public void SaveScore(string newName, int[] args = default){
		switch(newName){
			case "Save":
				string Entry = "";
				foreach(HighScore addScore in HighScores){
					Entry += addScore.hsName + "©" + addScore.hsScore + "©" + addScore.hsMode + "©" + addScore.hsDiff + "©"+ addScore.hsDate + "©";
				}
				PlayerPrefs.SetString("HighScores", Entry);
				break;
			case "Load":
				string scoreOutput = PlayerPrefs.GetString("HighScores");
				int scoreParse = 0;
				string tempName = "";
				int[] tempVars = {0,0,0,0};

				string temp = "";
				for(int pa = 0; pa < scoreOutput.Length; pa++){
					if(scoreOutput[pa] == '©'){
						switch(scoreParse){
							case 0: tempName = temp; scoreParse++; break;
							case 4:
								tempVars[scoreParse-1] = int.Parse(temp);
								HighScores.Add(new(tempName, tempVars[0], tempVars[1], tempVars[2], tempVars[3]));
								scoreParse = 0;
								break;
							default: tempVars[scoreParse-1] = int.Parse(temp); scoreParse++; break;
						}
						temp = "";
					} else {
						temp += scoreOutput[pa];
					}
				}
				break;
			case "Sort":
				bool Bubble = true;
				int How = args[0]; // 0 Decreasing, 1 Incresing, 2 Newest, 3 Oldest
				for(int bs = 0; bs < HighScores.ToArray().Length-1; bs++){
					if(bs == HighScores.ToArray().Length-1 && !Bubble){
						Bubble = true; bs = 0;
					} else if ( (How == 0 && HighScores[bs].hsScore > HighScores[bs+1].hsScore) || (How == 1 && HighScores[bs].hsScore < HighScores[bs+1].hsScore) || (How == 2 && HighScores[bs].hsDate > HighScores[bs+1].hsDate) || (How == 2 && HighScores[bs].hsDate < HighScores[bs+1].hsDate)){
						Bubble = false;
						HighScore T = HighScores[bs];
						HighScores[bs] = HighScores[bs+1];
						HighScores[bs+1] = T;
					}
				}
				break;
			case "Display":
				string addtodisplay = "";
				for(int ascore = HighScores.ToArray().Length-1; ascore >= 0; ascore--)
					if((args[0] == 0 || HighScores[ascore].hsMode == args[0]-1) && (args[1] == 0 || HighScores[ascore].hsDiff == args[1]-1))
						addtodisplay += HighScores[ascore].hsName + " - " + HighScores[ascore].hsScore + "\n";
				displayedHS = addtodisplay;
				break;
			case "Wipe":
				HighScores.Clear();
				break;
			default:
				if(newName.Length >= 4 && newName[..4] == "Add_"){
					string theName = newName[4..];
					HighScores.Add(new(theName, args[0], args[1], args[2], hsDates));
					hsDates++;
				} else if (newName.Length >= 7 && newName[..6] == "Remove_"){
					string findName = newName[7..];
					for(int fk = 0; fk <= HighScores.ToArray().Length; fk++){
						if(fk == HighScores.ToArray().Length){
							Debug.LogError("No score entry of name " + newName + " found!");
						} else if (HighScores[fk].hsName == findName){
							HighScores.RemoveAt(fk);
							HighScores.TrimExcess();
							break;
						}
					}
				}

				break;
		}
	}

	public int recInput(string func, int sub = 0){
		switch(func){
			case "Save":
				string BindsToSave = "";
				foreach(KeyBind saveKB in KeyBinds){
					if(saveKB.theBind.Length == 2) BindsToSave += saveKB.theBind[0].ToString() + "-" + saveKB.theBind[1].ToString() + ";";
					else BindsToSave += saveKB.theBind[0] + ";";
				}
				print("Saved key binds " + BindsToSave);
				PlayerPrefs.SetString("SavedBinds", BindsToSave);
				break;
			case "Load":
				if(PlayerPrefs.HasKey("SavedBinds")){
					string BindsLoaded = PlayerPrefs.GetString("SavedBinds");
					print("Loaded key binds " + BindsLoaded);
					string[] blTemp = {"", ""};
					int Flip = 0;
					int Index = 0;
					for(int bl = 0; bl < BindsLoaded.Length; bl++){
						if(BindsLoaded[bl] == ';'){
							Flip=0;
							for(int fk = 0; fk < potentialKB.Length; fk++){
								if(potentialKB[fk].ToString() == blTemp[0]) KeyBinds[Index].theBind[0] = potentialKB[fk];
								else if(potentialKB[fk].ToString() == blTemp[1]) KeyBinds[Index].theBind[1] = potentialKB[fk];
							}
							Index++;
						} else if(BindsLoaded[bl] == '-'){
							Flip=1;
						} else {
							blTemp[Flip] += BindsLoaded[bl];
						}
					}
				}
				break;
			default:
				for(int fkb = 0; fkb <= KeyBinds.Length; fkb++){
					if(fkb == KeyBinds.Length) Debug.LogError("No keybind of name " + func + " found!");
					else if (KeyBinds[fkb].Names[0] == func){
						if((sub == 0 && Input.GetKeyDown(KeyBinds[fkb].theBind[0])) || (sub == 1 && Input.GetKey(KeyBinds[fkb].theBind[0]))) return 1;
						else if (KeyBinds[fkb].theBind.Length > 1 && ((sub == 0 && Input.GetKeyDown(KeyBinds[fkb].theBind[1])) || (sub == 1 && Input.GetKey(KeyBinds[fkb].theBind[1]))) ) return -1;
						else return 0;
					}
				}
				break;
		}
		return 0;
	}

	void OnApplicationQuit(){

		if(EreasePrefs == false){
			PlayerPrefs.SetInt ("HasSaved", 1);
			PlayerPrefs.SetInt ("Resolution", currentResolution);
			PlayerPrefs.SetString ("Language", Language);
			PlayerPrefs.SetInt ("Controls", ControlScheme);
			PlayerPrefs.SetString ("Inverted", InvertedMouse.ToString());
			PlayerPrefs.SetString ("Fullscreen", Screen.fullScreen.ToString());
			PlayerPrefs.SetFloat ("MasterVolume", Volumes[0]);
			PlayerPrefs.SetFloat ("AudioVolume", Volumes[1]);
			PlayerPrefs.SetFloat ("MusicVolume", Volumes[2]);
			SaveScore("Save");//PlayerPrefs.SetInt ("HighScore", HighScore);
			recInput("Save");
		}

	}

	public void OptionsButtons(ButtonScript[] OptionButtons, ButtonScript OptionsBack){

		OptionsBack.GetComponent<Text> ().text = SetText ("Back", "Wróć");
		OptionsBack.IsActive = true;

			string[] optiones = new string[]{};
			switch(OptionsChoice){
				case "Graph": 
					if (Build == "Web") optiones = new string[]{"11000", "Quality", "Fullscreen", "", "", ""};
					else optiones = new string[]{"11100", "Quality", "Fullscreen", "Resolution", "", ""}; 
					break;
				case "Sound": optiones = new string[]{"11100", "Master", "Music", "SFX", "", ""}; break;
				case "Game": optiones = new string[]{"11110", "Controls", "Inverted", "KeyBinds", "Language", ""}; break;
				case "KeyBinds": optiones = new string[]{"10001", "currKBname", "currKBbind1", "currKBbind2", "", "currKBchange"}; break;
				case "KeyBinds-Change": optiones = new string[]{"11000", "currKBname", "currKBanyKey", "", "", ""}; break;
				default: optiones = new string[]{"11100", "Graph", "Sound", "Game", "", ""}; break;
			}

			for(int setOpt = 0; setOpt < 5; setOpt++){
				ButtonScript currButt = OptionButtons[setOpt];
				Text currText = OptionButtons[setOpt].GetComponent<Text>();
				int Click = 0;

				if(optiones[0][setOpt] == '1') currButt.IsActive = true;
				else currButt.IsActive = false;

				if(currButt.IsSelected)
					if(Input.GetMouseButtonDown(0)) Click = 1; else if(Input.GetMouseButtonDown(1)) Click = -1;
				
				switch(optiones[setOpt+1]){

					case "Graph":
						currText.text = SetText("Graphics", "Grafika");
						if(Click==1) OptionsChoice = optiones[setOpt+1];
						break;
					case "Sound":
						currText.text = SetText("Audio", "Udźwiękowienie");
						if(Click==1) OptionsChoice = optiones[setOpt+1];
						break;
					case "Game":
						currText.text = SetText("Game", "Gra");
						if(Click==1) OptionsChoice = optiones[setOpt+1];
						break;

					case "Quality":
						string[] quotas = new string[]{"Desperate", "Low", "Medium", "High", "Desperacka", "Niska", "Średnia", "Wysoka"};
						currText.text = SetText("Quality: " + quotas[QualitySettings.GetQualityLevel()], "Jakość: " + quotas[QualitySettings.GetQualityLevel()+4]);
						if(Click==-1 && QualitySettings.GetQualityLevel() == 0) QualitySettings.SetQualityLevel(3);
						else if (Click==1 && QualitySettings.GetQualityLevel() == 3) QualitySettings.SetQualityLevel(0);
						else if(Click==1) QualitySettings.IncreaseLevel();
						else if(Click==-1) QualitySettings.DecreaseLevel();
						break;
					case "Fullscreen":
						if(Screen.fullScreen == true) currText.text = SetText("Fullscreen mode: enabled", "Pełen ekran: włączony");
						else currText.text = SetText("Fullscreen mode: disabled", "Pełen ekran: wyłączony");
						if(Click!=0) Screen.fullScreen = !Screen.fullScreen;
						break;
					case "Resolution":
						currText.text = SetText("Resolution: ", "Rozdzielczość: ") + LoadedResolutions[currentResolution].x + "x" + LoadedResolutions[currentResolution].y;
						if(Click!=0) currentResolution = (LoadedResolutions.Length + currentResolution + Click) % LoadedResolutions.Length;
						break;
					
					case "Master":
						currText.text = SetText("Master volume: ", "Ogólna głośność: ") + Mathf.Round(Volumes[0]*100f) + "%";
						if(Click!=0) Volumes[0] = (1f + Volumes[0] + Click/20f) % 1f;
						break;
					case "Music":
						currText.text = SetText("Music volume: ", "Głośność muzyki: ") + Mathf.Round(Volumes[1]*100f) + "%";
						if(Click!=0) Volumes[1] = (1f + Volumes[1] + Click/20f) % 1f;
						break;
					case "SFX":
						currText.text = SetText("Sound effects: ", "Efekty dźwiękowe: ") + Mathf.Round(Volumes[2]*100f) + "%";
						if(Click!=0) Volumes[2] = (1f + Volumes[2] + Click/20f) % 1f;
						break;
					
					case "Controls":
						string[] conts = new string[]{"Mouse and Keyboard", "Pointing", "Klawiatura i mysz", "Wskazywanie"};
						currText.text = SetText("Controls: " + conts[ControlScheme], "Sterowanie: " + conts[ControlScheme+2]);
						if(Click!=0) ControlScheme = (2 + ControlScheme + Click) % 2;
						break;
					case "Inverted":
						if(InvertedMouse == true) currText.text = SetText("Inverted Y axis: enabled", "Odwrócona oś Y: włączony");
						else currText.text = SetText("Inverted Y axis: disabled", "Odwrócona oś Y: wyłączony");
						if(Click!=0) InvertedMouse = !InvertedMouse;
						break;
					case "Language":
						currText.text = SetText("Language: English", "Język: Polski");
						if(Click!=0) 
							if(Language == "English") Language = "Polski";
							else Language = "English";
						break;
					case "KeyBinds":
						currText.text = SetText("Key binds", "Przypisanie klawiszy");
						if(Click!=0) OptionsChoice = optiones[setOpt+1];
						break;
					
					case "currKBname":
						currText.text = SetText("< Selected action - " + KeyBinds[selectedKB].Names[0], "< Wybrana akcja - " + KeyBinds[selectedKB].Names[1]) + " >";
						if(Click!=0) selectedKB = (KeyBinds.Length + selectedKB + Click) % KeyBinds.Length;
						break;
					case "currKBbind1":
						if (KeyBinds[selectedKB].theBind.Length == 1) currText.text = SetText("Current key: ", "Przypisany klawisz: ") + KeyBinds[selectedKB].theBind[0].ToString();
						else currText.text = SetText("Positive value key: ", "Klawisz dodatnej wartości: ") + KeyBinds[selectedKB].theBind[1].ToString();
						break;
					case "currKBbind2":
						if (KeyBinds[selectedKB].theBind.Length == 1) currText.text = "";
						else currText.text = SetText("Negative value key: ", "Klawisz ujemnej wartości: ") + KeyBinds[selectedKB].theBind[0].ToString();
						break;
					case "currKBchange":
						currText.text = SetText("Change it", "Zmnień go");
						if(Click!=0) { OptionsChoice = "KeyBinds-Change"; KBphase = 0; }
						break;
					case "currKBanyKey":
						if (KeyBinds[selectedKB].theBind.Length == 1) currText.text = SetText("Press any key to rebind", "Naciśnij dowolny klawisz do przypisania");
						else if (KBphase == 0) currText.text = SetText("Press any key to rebind the positive action", "Naciśnij dowolny klawisz do przypisania do akcji pozytywnej");
						else if (KBphase == 1) currText.text = SetText("Press any key to rebind the negative action", "Naciśnij dowolny klawisz do przypisania do akcji ujemnej");

						if(!OptionsBack.IsSelected && Input.anyKeyDown) {
							foreach(KeyCode checkKC in System.Enum.GetValues(typeof(KeyCode))) if (Input.GetKeyDown(checkKC)) {
								KeyBinds[selectedKB].theBind[KBphase] = checkKC;
								KBphase+=1;
								if (KBphase >= KeyBinds[selectedKB].theBind.Length) OptionsChoice = "KeyBinds";
							}
						}
						break;

					default: currText.text = ""; break;
				}
			}

			if (OptionsBack.IsSelected == true && Input.GetMouseButtonDown (0)) {
				if(OptionsChoice == "") OptionsChoice = "-1";
				else if(OptionsChoice == "KeyBinds") OptionsChoice = "Game";
				else if(OptionsChoice == "KeyBinds-Change") OptionsChoice = "KeyBinds";
				else OptionsChoice = "";
			}

	}

	public void SetLighting(int ChooseSky){
		string[] SkyColors = new string[]{"Blue", "DarkBlue", "Violet", "Orange"};

		Color32 Sky = new(0,0,0,0);
		float SkySize = 0.5f;
		Color32 Ambient = new(0,0,0,0);
		Color32 Fog = new(0,0,0,0);
		switch(SkyColors[ChooseSky]){
			case "Blue":
				Sky = new Color32(0, 64, 125, 255);
				Ambient = new Color32 (25, 64, 128, 255);
				Fog = new Color32 (72, 108, 179, 255);
				GameObject.Find ("MainLight").GetComponent<Light> ().color = new Color32 (0, 55, 75, 255);
				GameObject.Find ("MainLight").transform.eulerAngles = new Vector3 (45f, 0f, 0f);
				break;
			case "DarkBlue":
				Sky = new Color32(0, 125, 255, 255);
				SkySize = 0.2f;
				Ambient = new Color32 (0, 0, 55, 255);
				Fog = new Color32 (13, 32, 99, 255);
				GameObject.Find ("MainLight").GetComponent<Light> ().color = new Color32 (0, 125, 255, 255);
				GameObject.Find ("MainLight").transform.eulerAngles = new Vector3 (10f, 0f, 0f);
				break;
			case "Violet":
				Sky = new Color32(100, 0, 75, 255);
				Ambient = new Color32 (37, 35, 58, 255);
				Fog = new Color32 (77, 70, 116, 255);
				GameObject.Find ("MainLight").GetComponent<Light> ().color = new Color32 (75, 65, 116, 255);
				GameObject.Find ("MainLight").transform.eulerAngles = new Vector3 (10f, 0f, 0f);
				break;
			case "Orange":
				Sky = new Color32(255, 100, 0, 255);
				SkySize = 0.5f;
				Ambient = new Color32 (100, 50, 50, 255);
				Fog = new Color32 (173, 127, 119, 255);
				GameObject.Find ("MainLight").GetComponent<Light> ().color = new Color32 (125, 75, 55, 255);
				GameObject.Find ("MainLight").transform.eulerAngles = new Vector3 (30f, 180f, 0f);
				break;
		}

		RenderSettings.ambientLight = Ambient;
		RenderSettings.skybox.SetColor("_SkyTint", Sky);
		RenderSettings.skybox.SetFloat("_AtmosphereThickness", SkySize);
		RenderSettings.skybox.SetColor("_GroundColor", Fog);
		RenderSettings.fogColor = GameObject.Find ("MainCamera").GetComponent<Camera> ().backgroundColor = Fog;

		PrevLight[0] = Fog;
		PrevLight[1] = Ambient;
	}

	[System.Serializable] public class Stat{

		GameScript GS;
		public string[] Names;
		public int Score = 0;
		public int tempScore = 0;
		int WriteP = 0; // 0 always write, 1 write only in main statistics, 2 don't show

		public Stat(string[] setNames, GameScript setGS, int setWriteP){
			Names = setNames;
			GS = setGS;
			WriteP = setWriteP;
		}

		public void Add(int Change){ tempScore += Change; }
		public void AddSurpas(int Change){ Score += Change; }
		public void Summary(int Gotit){ // 0 Add to score, 1 ignore, 2 calculate the specials

			switch(Names[0]){
				case "Presents accuracy":
					float[] indP = new float[]{(float)GS.ReceiveStat("Presents delivered", false), (float)GS.ReceiveStat("Presents missed", false), (float)GS.ReceiveStat("Presents delivered", true), (float)GS.ReceiveStat("Presents missed", true)};
					if (indP[0]+indP[1] > 0) Score = (int)Mathf.Lerp(0f, 100f, indP[0] / (indP[0]+indP[1]));
					else Score = 0;
					if (indP[2]+indP[3] > 0) tempScore = (int)Mathf.Lerp(0f, 100f, indP[2] / (indP[2]+indP[3]));
					else tempScore = 0;
					break;
				case "Shooting accuracy":
					float[] indS = new float[]{(float)GS.ReceiveStat("Shots hit", false), (float)GS.ReceiveStat("Shots fired", false), (float)GS.ReceiveStat("Shots hit", true), (float)GS.ReceiveStat("Shots fired", true)};
					if (indS[1] != 0) Score = (int)Mathf.Lerp(0f, 100f, indS[0] / indS[1]);
					else Score = -1;
					if (indS[3] != 0) tempScore = (int)Mathf.Lerp(0f, 100f, indS[2] / indS[3]);
					else tempScore = -1;
					break;
				case "Morality ratio":
					float[] indM = new float[]{(float)GS.ReceiveStat("Enemies downed", false), (float)GS.ReceiveStat("Enemies spared", false), (float)GS.ReceiveStat("Enemies encountered", false), (float)GS.ReceiveStat("Enemies downed", true), (float)GS.ReceiveStat("Enemies spared", true), (float)GS.ReceiveStat("Enemies encountered", true)};
					if(indM[2] > 0 && indM[0] > indM[1]) Score = (int)Mathf.Lerp(0f, -100f, indM[0]/indM[2]);
					else if(indM[2] > 0 && indM[0] < indM[1]) Score = (int)Mathf.Lerp(0f, 100f, indM[1]/indM[2]);
					else Score = 0;
					if(indM[5] > 0 && indM[3] > indM[4]) tempScore = (int)Mathf.Lerp(0f, -100f, indM[3]/indM[5]);
					else if(indM[5] > 0 && indM[3] < indM[4]) tempScore = (int)Mathf.Lerp(0f, 100f, indM[4]/indM[5]);
					else tempScore = 0;
					break;
				default:
					if(Gotit == 0) { Score += tempScore; tempScore = 0; } else if (Gotit == 1) { tempScore = 0; }
					break;
			}
			
		}
		public string ReceiveName(bool temp = false, string newLine = ""){

			switch(Names[0]){
				case "Presents accuracy": case "Shooting accuracy":
					if(Score != -1 && !temp && (WriteP == 0 || WriteP == 1)) return GS.SetText(Names[0], Names[1]) + ": " + Score + "%" + newLine;
					else if(tempScore != -1 && temp && WriteP == 0) return GS.SetText(Names[0], Names[1]) + ": " + tempScore + "%" + newLine;
					break;
				case "Morality ratio":
					if(!temp) {
						if(Score > 0) return GS.SetText(Names[0], Names[1]) + ": " + GS.SetText("pacifist ", "pacyfista ") + Score + "%" + newLine;
						else if (Score < 0) return GS.SetText(Names[0], Names[1]) + ": " + GS.SetText("aggressor ", "agresor ") + -Score + "%" + newLine;
					} else if(temp){
						if(tempScore > 0) return GS.SetText(Names[0], Names[1]) + ": " + GS.SetText("pacifist ", "pacyfista ") + tempScore + "%" + newLine;
						else if (tempScore < 0) return GS.SetText(Names[0], Names[1]) + ": " + GS.SetText("aggressor ", "agresor ") + -tempScore + "%" + newLine;
					}
					break;
				default:
					if(Score != 0 && !temp && (WriteP == 0 || WriteP == 1)) return GS.SetText(Names[0], Names[1]) + ": " + Score + newLine;
					else if(tempScore != 0 && temp && WriteP == 0) return GS.SetText(Names[0], Names[1]) + ": " + tempScore + newLine;
					break;
			}

			return "";
			
		}

	}

	public string statModify(string Name, int Value, string filename = ""){

		switch(Name){

			case "SetUp": // set up
				List<Stat> newStat = new()
                {
                    new(new string[] { "Presents delivered", "Dostarczone prezenty" }, this, 0),
					new(new string[] { "Presents missed", "Spudłowane prezenty" }, this, 0),
					new(new string[] { "Presents accuracy", "Celność z prezentami" }, this, 0), // this one specific
					new(new string[] { "Shots fired", "Oddane strzały" }, this, 0),
					new(new string[] { "Shots hit", "Trafione strzały"}, this, 0),
					new(new string[] { "Shooting accuracy", "Celność strzelania" }, this, 0), // this one specific
					new(new string[] { "Enemies downed", "Zdjęci przeciwnicy" }, this, 0),
					new(new string[] { "Enemies spared", "Oszczędzeni przeciwnicy" }, this, 0),
					new(new string[] { "Enemies encountered", "Oszczędzeni przeciwnicy" }, this, 0),
					new(new string[] { "Morality ratio", "Stosunek moralności" }, this, 0), // this one specific
                };
				string[] EVDarray = new string[]{"Messerschmitt", "Messerschmitt K4", "Messerschmitt Me 262", "Messerschmitt 110", SetText("AA Gun", "broni przeciwlotniczych"), SetText("Balloon", " balonów")};
				for(int EVD = 0; EVD < EVDarray.Length; EVD++){
					newStat.Add(new( new string[]{EVDarray[EVD] + " take downs", "Ilość zdjętych " + EVDarray[EVD]}, this, 0));
				}
				Statistics = newStat.ToArray();
				return "";

			case "Summarize": // summarize
				foreach(Stat ReadStats in Statistics) ReadStats.Summary(Value);
				return "";

			case "ReturnAll": case "ReturnTemp":
				string raList = "";
				foreach(Stat WriteStats in Statistics) 
					if(Name == "ReturnTemp") raList += WriteStats.ReceiveName(true, "\n");
					else raList += WriteStats.ReceiveName(false, "\n");
				return raList;

			case "Save":
				string theFile = "";
				for(int ss = 0; ss < Statistics.Length; ss++){
					theFile += Statistics[ss].Score + ";";
				}
				return theFile;

			case "Load":
				string[] decode = new string[] {PlayerPrefs.GetString(filename), ""};
				int checkCurr = 0;
				for(int dec = 0; dec < decode[0].Length; dec++){
					if(decode[0][dec] == ';'){
						Statistics[checkCurr].Score = int.Parse(decode[1]);
						decode[1] = "";
						checkCurr ++;
					} else {
						decode[1] += decode[0][dec];
					}
				}
				return "";

			default: // change
				for(int fn = 0; fn <= Statistics.Length; fn++){
					if(fn == Statistics.Length) Debug.LogError("No stat of name " + Name + " found!");
					else if (Statistics[fn].Names[0] == Name) {Statistics[fn].Add(Value); break;}
				}
				return "";
		}

	}

	public int ReceiveStat(string Name, bool Temp){
		for(int fn = 0; fn <= Statistics.Length; fn++){
			if(fn >= Statistics.Length) {Debug.LogError("No stat of name " + Name + " found!"); return 0;}
			else if (Statistics[fn].Names[0] == Name && Temp) return Statistics[fn].tempScore; // 1 return temp score
			else if (Statistics[fn].Names[0] == Name && !Temp) return Statistics[fn].Score; // 0 return score
		}
		return 0;
	}

}

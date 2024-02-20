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
	public int HighScore = 0;
	public float[] Volumes = new float[] { 1f, 1f, 1f, -1}; // master, music, sounds, prev sounds
	public int ControlScheme = 0; // Mouse and Keyboard, Pointing
	public bool InvertedMouse = false;
    public Color[] PrevLight = new Color[2];
	int SwitchDesperate = 0;
	// Options

	// Game
	public Stat[] Statistics;
	public int Level = 1;
	public int DifficultyLevel = 1;
	public int CurrentScore = 0;
    public int TempScore = 0;
	public int Mooney = 0;
    public int TempMooney = 0;
    public int Parachutes = 0;

	public string CurrentPlaneModel = "BP Mark.I";
	public string OwnedPlaneModels = "10000";
	public string CurrentEngineModel = "Basic Propeller";
	public string OwnedEngineModels = "10000";
	public string CurrentGunType = "Vickers";
	public string OwnedGundTypes = "10000";
	public string CurrentPresentCannonType = "Slingshot";
	public string OwnedPresentCannonTypes = "100";
	public string CurrentSpecialType = "None";
	public string OwnedSpecialTypes = "10000";
	public string CurrentAddition = "None";
	public string OwnedAdditions = "10000";
	public string CurrentPaint = "Basic Paint";
	public string OwnedPaints = "10000";
	// Game

	// Misc
	public string WhichMenuWindowToLoad = "";
	bool EreasePrefs = false;
	public int PreviousScore = 0;
	public string PreviousPlane = "";
    public bool HasDied = false;
	public string OptionsChoice = "";
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
					HighScore = PlayerPrefs.GetInt ("HighScore");
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
		if(Which == "Empty"){
			PlayerPrefs.SetInt (File + "SavedGame", 0);
			Level = 1;
			DifficultyLevel = 1;
			CurrentScore = 0;
            TempScore = 0;
			Mooney = 0;
            TempMooney = 0;
            Parachutes = 3;
			CurrentPlaneModel = "BP Mark.I";
			OwnedPlaneModels = "100000000000000000000000000000";
			CurrentEngineModel = "Basic Propeller";
			OwnedEngineModels = "100000000000000000000000000000";
			CurrentGunType = "Vickers";
			OwnedGundTypes = "100000000000000000000000000000";
			CurrentPresentCannonType = "Slingshot";
			OwnedPresentCannonTypes = "100000000000000000000000000000";
			CurrentSpecialType = "None";
			OwnedSpecialTypes = "100000000000000000000000000000";
			CurrentAddition = "None";
			OwnedAdditions = "100000000000000000000000000000";
			CurrentPaint = "Basic Paint";
			OwnedPaints = "100000000000000000000000000000";
		} else if(Which == "Save"){
			PlayerPrefs.SetInt (File + "SavedGame", 1);
			PlayerPrefs.SetInt (File + "Level", Level);
			PlayerPrefs.SetInt (File + "DifficultyLevel", DifficultyLevel);
			PlayerPrefs.SetInt (File + "CurrentScore", CurrentScore);
			PlayerPrefs.SetInt (File + "Mooney", Mooney);
            PlayerPrefs.SetInt (File + "Parachutes", Parachutes);
			PlayerPrefs.SetString (File + "CurrentPlaneModel", CurrentPlaneModel);
			PlayerPrefs.SetString (File + "OwnedPlaneModels", OwnedPlaneModels);
			PlayerPrefs.SetString (File + "CurrentEngineModel", CurrentEngineModel);
			PlayerPrefs.SetString (File + "OwnedEngineModels", OwnedEngineModels);
			PlayerPrefs.SetString (File + "CurrentGunType", CurrentGunType);
			PlayerPrefs.SetString (File + "OwnedGunTypes", OwnedGundTypes);
			PlayerPrefs.SetString (File + "CurrentPresentCannonType", CurrentPresentCannonType);
			PlayerPrefs.SetString (File + "OwnedPresentCannonTypes", OwnedPresentCannonTypes);
			PlayerPrefs.SetString (File + "CurrentSpecialType", CurrentSpecialType);
			PlayerPrefs.SetString (File + "OwnedSpecialTypes", OwnedSpecialTypes);
			PlayerPrefs.SetString (File + "CurrentAddition", CurrentAddition);
			PlayerPrefs.SetString (File + "OwnedAdditions", OwnedAdditions);
			PlayerPrefs.SetString (File + "CurrentPaint", CurrentPaint);
			PlayerPrefs.SetString (File + "OwnedPaints", OwnedPaints);
		} else if(Which == "Load"){
			PlayerPrefs.SetInt (File + "SavedGame", 1);
			Level = PlayerPrefs.GetInt (File + "Level");
			DifficultyLevel = PlayerPrefs.GetInt (File + "DifficultyLevel");
			CurrentScore = PlayerPrefs.GetInt (File + "CurrentScore");
            TempScore = 0;
			Mooney = PlayerPrefs.GetInt (File + "Mooney");
            TempMooney = 0;
            Parachutes = PlayerPrefs.GetInt(File + "Parachutes");
			CurrentPlaneModel = PlayerPrefs.GetString (File + "CurrentPlaneModel");
			OwnedPlaneModels = PlayerPrefs.GetString (File + "OwnedPlaneModels");
			CurrentEngineModel = PlayerPrefs.GetString (File + "CurrentEngineModel");
			OwnedEngineModels = PlayerPrefs.GetString (File + "OwnedEngineModels");
			CurrentGunType = PlayerPrefs.GetString (File + "CurrentGunType");
			OwnedGundTypes = PlayerPrefs.GetString (File + "OwnedGunTypes");
			CurrentPresentCannonType = PlayerPrefs.GetString (File + "CurrentPresentCannonType");
			OwnedPresentCannonTypes = PlayerPrefs.GetString (File + "OwnedPresentCannonTypes");
			CurrentSpecialType = PlayerPrefs.GetString (File + "CurrentSpecialType");
			OwnedSpecialTypes = PlayerPrefs.GetString (File + "OwnedSpecialTypes");
			CurrentAddition = PlayerPrefs.GetString (File + "CurrentAddition");
			OwnedAdditions = PlayerPrefs.GetString (File + "OwnedAdditions");
			CurrentPaint = PlayerPrefs.GetString (File + "CurrentPaint");
			OwnedPaints = PlayerPrefs.GetString (File + "OwnedPaints");
		} else if (Which == "Erase"){
			PlayerPrefs.DeleteKey (File + "SavedGame");
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
		}
	}

	public void GainScore(int GainScore){
		GameObject.Find("RoundScript").GetComponent<RoundScript>().TempScore += GainScore * DifficultyLevel;
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
			PlayerPrefs.SetInt ("HighScore", HighScore);
		}

	}

	public void OptionsButtons(ButtonScript[] OptionButtons, ButtonScript OptionsBack){

		OptionsBack.GetComponent<Text> ().text = SetText ("Back", "Wróć");
		OptionsBack.IsActive = true;

			string[] optiones = new string[]{};
			switch(OptionsChoice){
				case "Graph": 
					if (Build == "Web") optiones = new string[]{"1100", "Quality", "Fullscreen", "", ""};
					else optiones = new string[]{"1110", "Quality", "Fullscreen", "Resolution", ""}; 
					break;
				case "Sound": optiones = new string[]{"1110", "Master", "Music", "SFX", ""}; break;
				case "Game": optiones = new string[]{"1110", "Controls", "Inverted", "Language", ""}; break;
				default: optiones = new string[]{"1110", "Graph", "Sound", "Game", ""}; break;
			}

			for(int setOpt = 0; setOpt < 4; setOpt++){
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
						if(Click!=0) Volumes[0] = (1f + Volumes[0] + Click/10f) % 1.1f;
						break;
					case "Music":
						currText.text = SetText("Music volume: ", "Głośność muzyki: ") + Mathf.Round(Volumes[1]*100f) + "%";
						if(Click!=0) Volumes[1] = (1f + Volumes[1] + Click/10f) % 1.1f;
						break;
					case "SFX":
						currText.text = SetText("Sound effects: ", "Efekty dźwiękowe: ") + Mathf.Round(Volumes[2]*100f) + "%";
						if(Click!=0) Volumes[2] = (1f + Volumes[2] + Click/10f) % 1.1f;
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

					default: currText.text = ""; break;
				}
			}

			if (OptionsBack.IsSelected == true && Input.GetMouseButtonDown (0)) {
				if(OptionsChoice == "") OptionsChoice = "-1";
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

	public string statModify(string Name, int Value){

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

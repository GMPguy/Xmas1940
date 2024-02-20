using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour {

	// References
	public GameScript GS;
    // While paused
    public bool isPaused = false;
    public float ShowBars = 0f;
    public Transform PausedWindow;
    public Image[] Bars;
    public ButtonScript[] pButtons;
    string pOptions;
    // While Playing
    public Camera MainCamera;
    public PlayerScript player;
	public GameObject PlayerHud;
	public Image HealthBar;
	public Image FuelBar;
	public Text Throttle;
	public Text Speed;
	public Text Altitude;
	public Image GunCooldown;
	public Image PresentCooldown;
	public Image SpecialCooldown;
	public Text AmmoText;
	public Image Crosshair;
	public Image PresentsHud;
	public Text CurrentLevelText;
	public Image SteeringCircle;
	public Text MooneyScore;
    public Image PIcon;
    public Text PText;
    // While intro
    public Transform IntroWindow;
    string[] IntroTextes;
    float IntroTime = 0f;
    // While results
    public Transform ResultsWindow;
    public Text ResultsText;
	// Flash
	public Image FlashImg;
	float[] FlashDisp = {0f, 1f};
    // Flash
    // Radar/Map
    public GameObject Radar;
    public GameObject Map;
    public GameObject AirplaneMark;
    public GameObject AAGunMark;
    public GameObject BalloonMark;
    public GameObject HomeMark;
    public float RadarDistance = 1000f;
    public float Refresh = 1f;
    // Radar/Map
	// While Playing
	public RoundScript RS;
	public Text CanvasMessage;
    float[] MessageVars = {0f, 1f, 2f}; // Time, max Time, Scale
    public AudioClip[] MessageAudios;
	public GameObject Musics;
	// References

	// Misc
	GameObject CurrentMusic;
	// Misc

	// Use this for initialization
	void Start () {

        GS = GameObject.Find("GameScript").GetComponent<GameScript>();
        RS = GameObject.Find("RoundScript").GetComponent<RoundScript>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        MainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        Flash(Color.black, new float[]{2f, 1f});

        // Set intro textes
        IntroTextes = new string[] { GS.SetText("Level ", "Poziom ") + GS.Level, GS.SetText("Deliver the presents", "Dostarcz prezenty")};
        switch(GS.Level){
            case 2: IntroTextes[1] = GS.SetText("Watch out for the enemy airplanes!", "Uważaj na wrogie samoloty!"); break;
            case 5: IntroTextes[1] = GS.SetText("Watch out for the AA Guns, and the new Messerschmitt K4 planes!", "Uważaj na Bronie Przeciwlotnicze, i na nowe Messerschmitt K4"); break;
            case 10: IntroTextes[1] = GS.SetText("Watch out for the Balloons, and the new Messerschmitt 110 planes!", "Uważaj na Balony, i na nowe Messerschmitt 110!"); break;
            case 20: IntroTextes[1] = GS.SetText("Watch out for the new Messerschmitt Me 262! These are Jets!", "Uważaj na nowego Messerschmitt Me 262! To są odrzutowce!"); break;
            default: IntroTextes[1] = GS.SetText("Deliver the presents!", "Dostarcz prezenty!"); break;
        }

    }

    public void Flash(Color SetColor, float[] SetTime, int Act = 0){
        if(Act == 1){
            FlashDisp[0] = Mathf.MoveTowards(FlashDisp[0], 0f, Time.deltaTime);
            Color SA = FlashImg.color;
            SA.a = Mathf.Lerp(0f, 1f, FlashDisp[0] / FlashDisp[1]);
            FlashImg.color = SA;
        } else {
            FlashImg.color = SetColor;
            FlashDisp = new float[]{SetTime[0], SetTime[1]};
        }
    }

    public void Message(string setText = "", Color setColor = default, float[] setVars = default, string SoundClip = ""){
        if(setText != ""){
            CanvasMessage.text = setText;
            CanvasMessage.color = setColor;
            MessageVars[0] = MessageVars[1] = setVars[0];
            MessageVars[2] = setVars[1];

            if(SoundClip != "")
                for(int pA = 0; pA <= MessageAudios.Length; pA++){
                    if(pA == MessageAudios.Length){
                        Debug.LogError("No message audio of name " + SoundClip + " found!");
                    } else if (MessageAudios[pA].name == SoundClip){
                        CanvasMessage.GetComponent<AudioSource>().clip = MessageAudios[pA];
                        CanvasMessage.GetComponent<AudioSource>().Play();
                        break;
                    }
                }

        } else {
            MessageVars[0] = Mathf.MoveTowards(MessageVars[0], 0, Time.deltaTime);
            CanvasMessage.transform.localScale = Vector3.one * Mathf.Lerp(1f, MessageVars[2], ((MessageVars[0] / MessageVars[1])-0.5f) * 2f) / 2f;
            Color SC = CanvasMessage.color;
            SC.a = Mathf.Lerp(0f, 1f, MessageVars[0] * 4f);
            CanvasMessage.color = SC;
        }
    }
	
	void Update(){

        Music();
        Flash(default, default, 1);
        Message();
        
        if(RS.State == "SuccessDP"){
            Paused();
            Alive();
            Intro();
            Results(RS.State);
            Time.timeScale = 1f;
        } else if(isPaused){
            Paused(true);
            Alive();
            Intro();
            Results();
            Time.timeScale = 0f;
        } else if(player != null && player.Intro <= 0f){
            Paused();
            Alive(true);
            Intro();
            Results();
            Time.timeScale = 1f;
        } else if(player != null && player.Intro > 0f){
            Paused();
            Alive();
            Intro(true);
            Results();
            Time.timeScale = 1f;
        } else {
            Paused();
            Alive();
            Intro();
            Results();
            Time.timeScale = 1f;
        }

        if(ShowBars > 0f){
            Bars[0].color = Bars[1].color = new Color(0f,0f,0f,ShowBars);
            ShowBars -= Time.unscaledDeltaTime;
        } else if (Bars[0].color.a > 0f){
            Bars[0].color = Bars[1].color = new Color(0f,0f,0f,0f);
        }

        if(Input.GetKeyDown(KeyCode.Escape) && player)
            isPaused = !isPaused;

    }

    void Results(string view = ""){

        if(view != ""){
            ResultsWindow.localScale = Vector3.Lerp(ResultsWindow.localScale, Vector3.one, Time.unscaledDeltaTime*10f);
            if(ResultsWindow.localScale.x > 0.9f){
                ResultsText.color = new Color(1f,1f,1f,Mathf.MoveTowards(ResultsText.color.a, 1f, Time.unscaledDeltaTime));
            } else {
                ResultsText.text = GS.statModify("ReturnTemp", 0);
                if(view == "SuccessDP") ResultsText.text += "\n" + 
                GS.SetText("Accuracy bonus: ", "Bonus za celność: ") + (int)RS.accBonus +
                GS.SetText("\nHealth bonus: ", "\nBonus za zdrowie: ") + (int)RS.hpBonus +
                GS.SetText("\nMorality bonus: ", "\nBonus za moralność: ") + (int)RS.morBonus +
                GS.SetText("\n\nScore gained: ", "\n\nUzyskany wynik: ") + RS.TempScore +
                GS.SetText("\nPayout: ", "\nWypłata: ") + RS.TempMooney;
            }

            if(Input.anyKeyDown){
                RS.State = "Left1";
            }
        } else {
            ResultsWindow.localScale = Vector3.zero;
            ResultsText.color = new Color(1f,1f,1f,0f);
        }

    }

    void Paused(bool view = false){

        if(view){

            if(player) player.GunCooldown = Mathf.Clamp(player.GunCooldown, 1f, Mathf.Infinity);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            PausedWindow.localScale = Vector3.one;
            ShowBars = Mathf.MoveTowards(ShowBars, 1.1f, Time.unscaledDeltaTime*2f);

            string[] buttonStuff;
            switch(pOptions){
                case "Options": buttonStuff = new string[]{"222202", "", "", "", "", "", ""}; break;
                case "Quit": buttonStuff = new string[]{"000110", "", "unsure", "", "quityes", "quitno", ""}; break;
                default: buttonStuff = new string[]{"011010", "", "res", "opt", "", "quit", ""}; break;
            }

            if(pOptions == "Options"){
                GS.OptionsButtons(pButtons, pButtons[5]);
                if(GS.OptionsChoice == "-1"){
                    GS.OptionsChoice = "";
                    pOptions = "";
                }
            }

            for(int cb = 0; cb < 6; cb++){
                char al = buttonStuff[0][cb];
                ButtonScript getB = pButtons[cb];
                Text textB = getB.GetComponent<Text>();
                int click = 0; if(getB.IsSelected && Input.GetMouseButtonDown(0)) click = 1;
                if(al == '1'){
                    getB.IsActive = true;
                    switch(buttonStuff[cb+1]){

                        case "res": 
                            textB.text = GS.SetText("Resume", "Wznów grę"); 
                            if(click == 1) isPaused = false;
                            break;
                        case "opt": 
                            textB.text = GS.SetText("Options", "Opcje"); 
                            if(click == 1) pOptions = "Options";
                            break;
                        case "quit": 
                            textB.text = GS.SetText("Quit", "Wyjdź"); 
                            if(click == 1) pOptions = "Quit";
                            break;

                        case "quityes": 
                            textB.text = GS.SetText("Yes", "Tak"); 
                            if(click == 1) {
                                isPaused = false;
                                RS.State = "Quit1";
                            }
                            break;
                        case "quitno": 
                            textB.text = GS.SetText("No", "Nie"); 
                            if(click == 1) pOptions = "";
                            break;

                    }
                } else if (al == '0'){
                    getB.IsActive = false;
                    switch(buttonStuff[cb+1]){
                        case "": textB.text = GS.SetText("", ""); break;
                        case "unsure": textB.text = GS.SetText("You wanna go back? You'll need to redo the level", "Chcesz wrócić? Będziesz musiał powtórzyć ten poziom"); break;
                    }
                }
            }

        } else {

            PausedWindow.localScale = Vector3.zero;

        }

    }

    void Intro(bool view = false){

        if(view){
            ShowBars = 1.5f;

            if(IntroTime > 0f){
                IntroTime -= Time.deltaTime;
            } else {
                for(int it = 0; it <= 1; it++) if (IntroTextes[it].Length > 0) {
                    IntroTime = 0.02f;
                    IntroWindow.GetChild(it).GetComponent<Text>().text += IntroTextes[it].Substring(0, 1);
                    IntroTextes[it] = IntroTextes[it].Remove(0, 1);
                    if(IntroTextes[it].Length <= 0f) IntroTime = 0.5f;
                    break;
                } else if (player.Intro < 0.5f){
                    IntroWindow.GetChild(it).GetComponent<Text>().color = new Color(1f, 1f, 1f, player.Intro*2f);
                }
            }
        } else if (IntroWindow.GetChild(0).GetComponent<Text>().color.a > 0f) {
            IntroWindow.GetChild(0).GetComponent<Text>().color = IntroWindow.GetChild(1).GetComponent<Text>().color = new Color(1f, 1f, 1f, Mathf.MoveTowards(IntroWindow.GetChild(0).GetComponent<Text>().color.a, 0f, Time.deltaTime*2f) );
        }

    }

	void Alive (bool view = false) {

		// While Playing
		if(view && player){

            if(!PlayerHud.activeInHierarchy) PlayerHud.SetActive (true);

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetButtonDown("Map")) {
                if (RadarDistance == 100f) {
                    RadarDistance = 500f;
                } else if (RadarDistance == 500f) {
                    RadarDistance = 1000f;
                } else if (RadarDistance == 1000f) {
                    RadarDistance = 2000f;
                } else if (RadarDistance == 2000f) {
                    RadarDistance = 5000f;
                } else if (RadarDistance == 5000f) {
                    RadarDistance = 100f;
                }
            }

			HealthBar.fillAmount = player.Health / player.MaxHealth;
			HealthBar.transform.GetChild (0).GetComponent<Text> ().text = Mathf.Round(player.Health).ToString () + " / " + player.MaxHealth;
			FuelBar.fillAmount = player.Fuel / player.MaxFuel;
			FuelBar.transform.GetChild (0).GetComponent<Text> ().text = Mathf.Round(player.Fuel).ToString () + " / " + player.MaxFuel;
			Throttle.text = (Mathf.Round(player.Throttle * 100f)) + "%";
			Speed.text = GS.SetText("Speed:\n" + Mathf.Round(player.Speed).ToString() + " KM/H", "Prędkość:\n" + Mathf.Round(player.Speed).ToString() + " KM/H");
			Altitude.text = GS.SetText("Altitude:\n" + Mathf.Round(player.transform.position.y).ToString() + " MASL", "Wysokość:\n" + Mathf.Round(player.transform.position.y).ToString() + " NPM");
			MooneyScore.text = GS.SetText("Mooney: " + (GS.Mooney + RS.TempMooney) + "\nScore: " + (GS.CurrentScore + RS.TempScore), "Piniądze: " + (GS.Mooney + RS.TempMooney) + "\nWynik: " + (GS.CurrentScore + RS.TempScore));
            if (Input.GetButton("Map") && !Input.GetKey(KeyCode.LeftShift)) {
                RadarMap("Map");
            } else {
                RadarMap("Radar");
            }

			if(player.GunCooldown <= 0f && (player.Heat >= 0)) {
				GunCooldown.fillAmount = 1f;
				GunCooldown.color = Color.Lerp( new Color (1f, 1f, 1f, 1f) , new Color(1f, 0f, 0f, 1f) , (player.Heat-(player.MaxHeat*0.75f)) / (player.MaxHeat*0.75f) );
			} else {
				GunCooldown.fillAmount = 1f - (player.GunCooldown / player.MaxGunCooldown);
				GunCooldown.color = Color.Lerp( new Color (0.5f, 0.5f, 0.5f, 1f) , new Color(0.5f, 0f, 0f, 1f) , (player.Heat-(player.MaxHeat*0.75f)) / (player.MaxHeat*0.75f) );
			}

			if(player.PresentCooldown <= 0f) {
				PresentCooldown.fillAmount = 1f;
				PresentCooldown.color = new Color32 (255, 255, 255, 255);
			} else {
				PresentCooldown.fillAmount = 1f - (player.PresentCooldown / player.MaxPresentCooldown);
				PresentCooldown.color = new Color32 (155, 155, 155, 255);
			}

			if(player.SpecialCooldown <= 0f){
				SpecialCooldown.fillAmount = 1f;
				SpecialCooldown.color = new Color32 (255, 255, 255, 255);
			} else {
				SpecialCooldown.fillAmount = 1f - (player.SpecialCooldown / player.MaxSpecialCooldown);
				SpecialCooldown.color = new Color32 (155, 155, 155, 255);
			}

			AmmoText.text = GS.SetText("Heat: ", "Przegrzanie: ");
            AmmoText.color = Color.Lerp( new Color (1f, 1f, 1f, 1f) , new Color(1f, 0f, 0f, 1f) , (player.Heat-(player.MaxHeat*0.75f)) / (player.MaxHeat*0.75f) );
            for(float gh = 0f; gh < 1f; gh+=0.1f){
                if(player.Heat >= 0f && gh > player.Heat / player.MaxHeat) AmmoText.text += ".";
                else if(player.Heat <= -10f && gh > (player.Heat+10f)*-1f / player.MaxHeat) AmmoText.text += ".";
                else AmmoText.text += "I";
            }

			if((player.Stalling > 0f) || (player.Speed >= (player.MaxSpeed * 1.75f))){
				Speed.color = new Color32 (255, 0, 0, 255);
			} else {
				Speed.color = new Color32 (255, 255, 255, 255);
			}

			if (RS.GetComponent<RoundScript> ().State == "Deliver Presents") {
				PresentsHud.gameObject.SetActive(true);
				PresentsHud.transform.GetChild(0).GetComponent<Text>().text = GS.SetText( "Houses left:\n" + GameObject.FindGameObjectsWithTag ("HomeUnchecked").Length, "Pozostało:\n" + GameObject.FindGameObjectsWithTag ("HomeUnchecked").Length);
			} else {
				PresentsHud.gameObject.SetActive (false);
			}

			CurrentLevelText.text = GS.SetText ("Level: " + GS.Level, "Poziom: " + GS.Level);

            if (GS.Parachutes > 0) {
                PIcon.color = new Color32(255, 255, 255, 125);
                PText.text = GS.Parachutes.ToString();
                PText.color = new Color32(255, 255, 255, 125);
            } else {
                PIcon.color = new Color32(255, 0, 0, 125);
                PText.text = GS.Parachutes.ToString();
                PText.color = new Color32(255, 0, 0, 125);
            }

            // Cursors and steering wheel
            if(player.ControlType == "Point"){

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                SteeringCircle.rectTransform.anchorMin = SteeringCircle.rectTransform.anchorMax = Vector2.one/2f;
                SteeringCircle.rectTransform.anchoredPosition += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * -player.Inverted)*10f;
    			SteeringCircle.rectTransform.anchoredPosition = Vector2.Lerp(SteeringCircle.rectTransform.anchoredPosition, Vector2.zero, Time.unscaledDeltaTime * 5f);

                SteeringCircle.GetComponent<Image>().color = new Color(1f,1f,1f, Vector3.Angle(player.transform.forward, player.PointThere.forward) / 10f);

            } else {

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;

                SteeringCircle.rectTransform.anchorMin = SteeringCircle.rectTransform.anchorMax = new Vector2 (Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
    			SteeringCircle.rectTransform.anchoredPosition = Vector2.zero;

                SteeringCircle.GetComponent<Image>().color = Color.white;

            }

            if(player.WhichCamera == "Turret"){
                Crosshair.transform.position = MainCamera.WorldToScreenPoint(MainCamera.transform.position + MainCamera.transform.forward);
                Crosshair.transform.localScale = Vector3.one;
            } else {
                Crosshair.transform.position = MainCamera.WorldToScreenPoint(player.transform.position + player.transform.forward * Mathf.Clamp(player.GunDistane, 0f, 900f));
                if(player.WhichCamera == "Aim") Crosshair.transform.localScale = Vector3.one;
                else Crosshair.transform.localScale = Vector3.one/2f;
            }

		} else {

            PlayerHud.SetActive (false);

        }
		// While Playing
		
	}

    void RadarMap(string WhichOne){

        if (Refresh > 0f) {
            Refresh -= 1f;
        } else {
            Refresh = Mathf.Clamp(Mathf.Floor(RS.GetComponent<RoundScript>().Level / 10f), 1f, 100f);
        }

        if (WhichOne == "Radar") {
            Radar.transform.localScale = Vector3.Lerp(Radar.transform.localScale, new Vector3(1f, 1f, 1f), 0.25f * (Time.unscaledDeltaTime * 100f));
            Map.transform.localScale = Vector3.Lerp(Map.transform.localScale, new Vector3(0f, 0f, 0f), 0.25f * (Time.unscaledDeltaTime * 100f));

            if (Refresh == 0f) {
                foreach (Transform Obj in Radar.transform) {
                    if (Obj.name == "You") {
                        Obj.transform.localEulerAngles = new Vector3(0f, 0f, player.transform.eulerAngles.y * -1f);
                    } else if (Obj.name == "Radar") {
                        Obj.transform.localEulerAngles = new Vector3(0f, 0f, GameObject.Find("MainCamera").transform.eulerAngles.y * -1f);
                    } else if (Obj.name == "RadarDistanceText") {
                        Obj.GetComponent<Text>().text = RadarDistance + "m";
                    } else {
                        Destroy(Obj.gameObject);
                    }
                }
                List<GameObject> TheMarkings = new List<GameObject>();
                foreach (GameObject Enemy in GameObject.FindGameObjectsWithTag("Foe")) {
                    if (Enemy.GetComponent<EnemyVesselScript>().Health > 0f) {
                        TheMarkings.Add(Enemy);
                    }
                }
                foreach (GameObject Home in GameObject.FindGameObjectsWithTag("HomeUnchecked")) {
                    TheMarkings.Add(Home);
                }
                if (GameObject.Find("Portal") != null) {
                    TheMarkings.Add(GameObject.Find("Portal"));
                }
                foreach (GameObject Marker in TheMarkings) {
                    if (Vector3.Distance(new Vector3(Marker.transform.position.x, 0f, Marker.transform.position.z), new Vector3(player.transform.position.x, 0f, player.transform.position.z)) < RadarDistance) {
                        GameObject PickMarker = null;
                        Color32 PickColor = new Color32(0, 0, 0, 0);
                        float MarkerRotation = 0f;
                        if (Marker.GetComponent<EnemyVesselScript>() != null) {
                            if (Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Messerschmitt" || Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Messerschmitt K4" || Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Messerschmitt 110" || Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Messerschmitt Me 262") {
                                PickMarker = AirplaneMark;
                                PickColor = new Color32(255, 0, 0, 255);
                                MarkerRotation = Marker.transform.eulerAngles.y * -1f;
                            }
                            if (Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "AA Gun") {
                                PickMarker = AAGunMark;
                                PickColor = new Color32(255, 0, 0, 255);
                            }
                            if (Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Balloon") {
                                PickMarker = BalloonMark;
                                PickColor = new Color32(255, 0, 0, 255);
                            }
                        } else if (Marker.GetComponent<HomeScript>() != null) {
                            PickMarker = HomeMark;
                            PickColor = new Color32(0, 255, 0, 255);
                        } else if (Marker.name == "Portal") {
                            PickMarker = HomeMark;
                            PickColor = new Color32(0, 255, 255, 255);
                        }
                        Vector3 DesiredPosition = Marker.transform.position - player.transform.position;
                        GameObject Mark = Instantiate(PickMarker) as GameObject;
                        Mark.transform.SetParent(Radar.transform);
                        Mark.transform.position = Radar.transform.position;
                        Mark.transform.localScale *= this.transform.localScale.y;
                        Mark.transform.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.5f + (DesiredPosition.x / (RadarDistance * 2f)), 0.5f + (DesiredPosition.z / (RadarDistance * 2f)));
                        Mark.transform.GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(0.5f + (DesiredPosition.x / (RadarDistance * 2f)), 0.5f + (DesiredPosition.z / (RadarDistance * 2f)));
                        Mark.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
                        Mark.transform.GetChild(0).localEulerAngles = new Vector3(0f, 0f, MarkerRotation);
                        if (Vector3.Distance(new Vector3(Marker.transform.position.x, 0f, Marker.transform.position.z), new Vector3(player.transform.position.x, 0f, player.transform.position.z)) < (RadarDistance * 0.75f)) {
                            Mark.transform.GetChild(0).GetComponent<Image>().color = new Color(PickColor.r, PickColor.g, PickColor.b, 1f);
                        } else {
                            Mark.transform.GetChild(0).GetComponent<Image>().color = new Color(PickColor.r, PickColor.g, PickColor.b, 1f - (((Vector3.Distance(new Vector3(Marker.transform.position.x, 0f, Marker.transform.position.z), new Vector3(player.transform.position.x, 0f, player.transform.position.z)) / RadarDistance) - 0.75f) * 4f));
                        }
                    }
                }
            }
        } else if (WhichOne == "Map") {
            Radar.transform.localScale = Vector3.Lerp(Radar.transform.localScale, new Vector3(0f, 0f, 0f), 0.25f * (Time.unscaledDeltaTime * 100f));
            Map.transform.localScale = Vector3.Lerp(Map.transform.localScale, new Vector3(1f, 1f, 1f), 0.25f * (Time.unscaledDeltaTime * 100f));

            if (Refresh == 0f) {
                foreach (Transform Obj in Map.transform) {
                    Destroy(Obj.gameObject);
                }
                List<GameObject> TheMarkings = new List<GameObject>();
                foreach (GameObject Enemy in GameObject.FindGameObjectsWithTag("Foe")) {
                    TheMarkings.Add(Enemy);
                }
                foreach (GameObject Home in GameObject.FindGameObjectsWithTag("HomeUnchecked")) {
                    TheMarkings.Add(Home);
                }
                if (GameObject.Find("Portal") != null) {
                    TheMarkings.Add(GameObject.Find("Portal"));
                }
                if (player != null) {
                    TheMarkings.Add(player.gameObject);
                }
                foreach (GameObject Marker in TheMarkings) {
                    if (Marker.transform.position.x > -RS.MapSize/2f && Marker.transform.position.x < RS.MapSize/2f && Marker.transform.position.z > -RS.MapSize/2f && Marker.transform.position.z < RS.MapSize/2f) {
                        GameObject PickMarker = null;
                        Color32 PickColor = new Color32(0, 0, 0, 0);
                        float MarkerRotation = 0f;
                        if (Marker.GetComponent<EnemyVesselScript>() != null) {
                            if (Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Messerschmitt" || Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Messerschmitt K4" || Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Messerschmitt 110" || Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Messerschmitt Me 262") {
                                PickMarker = AirplaneMark;
                                PickColor = new Color32(255, 0, 0, 255);
                                MarkerRotation = Marker.transform.eulerAngles.y * -1f;
                            }
                            if (Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "AA Gun") {
                                PickMarker = AAGunMark;
                                PickColor = new Color32(255, 0, 0, 255);
                            }
                            if (Marker.GetComponent<EnemyVesselScript>().TypeofVessel == "Balloon") {
                                PickMarker = BalloonMark;
                                PickColor = new Color32(255, 0, 0, 255);
                            }
                        } else if (Marker.GetComponent<HomeScript>() != null) {
                            PickMarker = HomeMark;
                            PickColor = new Color32(0, 255, 0, 255);
                        } else if (Marker.name == "Portal") {
                            PickMarker = HomeMark;
                            PickColor = new Color32(0, 255, 255, 255);
                        } else if (Marker.GetComponent<PlayerScript>() != null) {
                            PickMarker = AirplaneMark;
                            PickColor = new Color32(255, 255, 255, 255);
                            MarkerRotation = Marker.transform.eulerAngles.y * -1f;
                        }
                        Vector3 DesiredPosition = Marker.transform.position;
                        GameObject Mark = Instantiate(PickMarker) as GameObject;
                        Mark.transform.SetParent(Map.transform);
                        Mark.transform.position = Map.transform.position;
                        Mark.transform.localScale *= this.transform.localScale.y;
                        Mark.transform.GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0.5f + (DesiredPosition.x / (RS.MapSize/4f)), 0.5f + (DesiredPosition.z / (RS.MapSize/4f)));
                        Mark.transform.GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(0.5f + (DesiredPosition.x / (RS.MapSize/4f)), 0.5f + (DesiredPosition.z / (RS.MapSize/4f)));
                        Mark.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
                        Mark.transform.GetChild(0).localEulerAngles = new Vector3(0f, 0f, MarkerRotation);
                        Mark.transform.GetChild(0).GetComponent<Image>().color = PickColor;
                    }
                }
            }

        }

    }

    void Music(){
		
		bool ChangeMusic = false;
		if(CurrentMusic == null){
			ChangeMusic = true;
		} else if(CurrentMusic.GetComponent<AudioSource>().isPlaying == false){
			ChangeMusic = true;
		}

		if(ChangeMusic == true){
			List<GameObject> MusicCandidates = new List<GameObject>();
			foreach(Transform Music in Musics.transform){
				if(Music.gameObject != CurrentMusic){
					MusicCandidates.Add(Music.gameObject);
				}
			}
			CurrentMusic = MusicCandidates.ToArray()[Random.Range(0, MusicCandidates.Count)];
			CurrentMusic.GetComponent<AudioSource> ().Play ();
		}

		if(GameObject.Find("MainPlane") == null){
			foreach(Transform Music in Musics.transform){
				Music.gameObject.GetComponent<AudioSource> ().Stop ();
			}
		}

	}

}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour {

	// References
	public GameScript GS;
	public ItemClasses IC;
	public RoundScript RS;
	public GameObject PlaneModels;
	public GameObject CurrentPlane;
	public GameObject Camera;
	public GameObject AimPart;
    public GameObject Turret;
	public GameObject Projectile;
	public List<GameObject> Guns;
	public ParticleSystem Smoke;
	public ParticleSystem Sparkles;
    public ParticleSystem Wind;
    public ParticleSystem NearGroundSnow;
	public GameObject Alarms;
	public GameObject PresentCannon;
	public CanvasScript mainCanvas;
	public GameObject Effect;
	public GameObject Special;
    public GameObject FreeLook;
	// References

	// Plane options
	public ItemClasses.PlaneModel PlaneModel;
	public ItemClasses.Engine EngineModel;
	public ItemClasses.Gun GunType;
	public ItemClasses.Present PresentType;
	public ItemClasses.Special SpecialType;
	public ItemClasses.Special Addition;
	public ItemClasses.Paint Paint;
	// Plane options

	// Stats
	public float MaxHealth = 100f;
	public float Health = 100f;

	public float MaxFuel = 180f;
	public float Fuel = 180f;

	public float Heat = 0f;
	public float MaxHeat = 1f;
	public float HeatDispare = 1f;
	public float MaxGunCooldown = 1f;
	public float GunCooldown = 0f;
	float AmountOfGuns = 0f;

	public float MaxPresentCooldown = 10f;
	public float PresentCooldown = 0f;

	public float MaxSpecialCooldown = 10f;
	public float SpecialCooldown = 0f;
	public int SpecialCharges = 0;

	public float MaxSpeed = 100f;
	public float Speed = 0f;
	public float FowardSpeed = 0f;
	public float RotationSpeed = 1f;

	public int AirplaneClass;
	// Stats

	// Mechanics
	public Vector3 ElevatorFlapsRudder;
	public Vector2 SteerPosition;
	public float Throttle = 1f;
	public float Inverted = -1f;
	public float Stalling = 0f;
	public string ControlType = "Point";
	public Transform PointThere;
	Vector2 PTturn;
    public string WhichCamera = "Normal";
	float tAnglez, tCF = 0;
	Vector3 FlyingTowards;
	Vector3 LiftDir;
	public Vector3 PrevPos;
	// Mechanics

	// Screen Shake
	public float ShakePower = 0f;
	public float ShakeDecay = 1f;
	Vector3 ShakeScreen;
	// Screen Shake

	// Bombs
	Vector3[] BombPositions = {Vector3.zero, Vector3.zero};
	bool[] BombShows = {false,false};
	public Transform[] BombGuis;
	// Bombs

	// Misc
	public Color32 PresentColor1;
	public Color32 PresentColor2;
	public float AlarmCooldown = 0f;
	public float GunDistane = 0f;
	public float PresentCannonDistane = 0f;
	public float Desertion = 5f;
	float DesertionBoom = 0f;
	public Color32 PlaneColor1;
	public Color32 PlaneColor2;
	bool BeyondMap = false;
	public float Flares = 0f;
	public float Intro = 5f;
	int IntroZoom = 0;
	float earProtection = 0;
	// Misc

	void Start () {

		GS = GameObject.Find("GameScript").GetComponent<GameScript>();
		IC = GS.GetComponent<ItemClasses>();
		RS = GameObject.Find("RoundScript").GetComponent<RoundScript>();

		PlaneModel = IC.PlaneModels[GS.CurrentPlaneModel];
		EngineModel = IC.EngineModels[GS.CurrentEngineModel];
		GunType = IC.Guns[GS.CurrentGunType];
		PresentType = IC.Presents[GS.CurrentPresentCannonType];
		SpecialType = IC.Specials[GS.CurrentSpecialType];
		Addition = IC.Additions[GS.CurrentAddition];
		Paint = IC.Paints[GS.CurrentPaint];

		mainCanvas = GameObject.Find ("MainCanvas").GetComponent<CanvasScript>();
		PlaneSettings (1);
		SetStats ();

		PointThere.SetParent(null);
		IntroZoom = 0;//(int)Random.Range(0f, 2.9f);

		LeadBomb(0, 0f);
		LeadBomb(1, 0f);
		
	}

	void FixedUpdate () {

		if (GS.InvertedMouse == true) Inverted = 1f; else Inverted = -1f;
		if (GS.ControlScheme == 1) ControlType = "Point"; else ControlType = "";

		PlaneSettings (0);
		if(Intro > 0f){
			Intro -= 0.02f;
		} else {
			Mechanics ();
			Controlling ();
		}

		// Values
		Health = Mathf.Clamp(Health, 0f, MaxHealth);
		Fuel = Mathf.Clamp(Fuel, 0f, MaxFuel);
		if(Heat > 0f){
			if(GunCooldown <= -0.9f && HeatDispare < 9000f) {
				Heat = Mathf.Clamp (Heat - 0.02f, 0f, MaxHeat*2f);
			}
			if(Heat > MaxHeat) {
				Heat = -MaxHeat - 10f;
				mainCanvas.Message(GS.SetText("Gun overheated!", "Broń się przegrzała!"), Color.red, new float[]{3f,1.5f}, "Overheating");
			}
		} else if (Heat <= -10f) {
			Heat = Mathf.Clamp (Heat + 0.02f*(MaxHeat/HeatDispare), -Mathf.Infinity, -10f);
			if(Heat >= -10f) Heat = 0f;
		}
        float LowFuelMultiplier;
        if (Fuel > (MaxFuel / 5f)) {
            if (Addition.Names[0] == "Boost") LowFuelMultiplier = 1.5f;
            else LowFuelMultiplier = 1f;
		} else {
            if (Addition.Names[0] == "Boost") LowFuelMultiplier = Fuel / MaxFuel / 5f * 1.5f;
            else LowFuelMultiplier = Fuel / (MaxFuel / 5f);
		}

		if (Health > (MaxHealth / 3f)) Throttle = Mathf.Clamp (Throttle, 0.004f,LowFuelMultiplier);
		else if ((Health / MaxHealth) <= (Fuel / MaxFuel)) Throttle = Mathf.Clamp (Throttle, 0.004f, (0.75f + (Health / MaxHealth * 0.25f)) * LowFuelMultiplier);

        if (Addition.Names[0] == "Boost") {
            if (Throttle > 1f) {
                ShakePower = (Throttle - 1f) * 0.5f;
                ShakeDecay = 0.1f;
                if (GS.recInput("Throttle", 1) == 0) Throttle -= 0.01f;
            }
        }

		if(earProtection > 0f) earProtection -= 0.02f;
		if(GunCooldown > -1f)GunCooldown -= 0.02f;
		if(Flares > 0f) Flares -= 0.01f;
		if(PresentCooldown > 0f){
			if (AirplaneClass == 1) {
				PresentCooldown -= 0.03f;
			} else {
				PresentCooldown -= 0.01f;
			}
		}
		if(SpecialCooldown > 0f) SpecialCooldown -= 0.01f;
		if(AlarmCooldown > 0f) AlarmCooldown -= 0.01f;
		// Values

		// Camera
		if(Intro <= 0f){
			float divASC = (Camera.GetComponent<Camera> ().fieldOfView-30f) / 30f;
			if (ControlType == "Point") {

				if (GS.recInput("Free view", 1) != 0) {
	            	WhichCamera = "Free";
				} else if (GS.recInput("Aiming", 1) != 0) {
	    	        if (Addition.Names[0] == "Turret"){
    	    	        WhichCamera = "Turret";
					} else {
						WhichCamera = "Aim";
					}
				} else {
					WhichCamera = "Normal";
				}

				Camera.transform.position = this.transform.position + this.transform.up * Mathf.Lerp(4f, 2f, divASC) + PointThere.forward*-15f + ShakeScreen;
				Camera.transform.LookAt(PointThere.position + PointThere.forward*1000f);
				UnityEngine.Cursor.lockState = CursorLockMode.Locked;

			} else {

				if (GS.recInput("Free view", 1) != 0 || Quaternion.Angle(FreeLook.transform.localRotation, Quaternion.identity) > 1f) {
	    	        WhichCamera = "Free";

    	    	    // Rotate Camera
	            	Camera.transform.position = FreeLook.transform.position + this.transform.up*2f + (FreeLook.transform.forward * -15f);
	            	Camera.transform.LookAt (FreeLook.transform.position + FreeLook.transform.forward * 1000f, this.transform.up);
    	    	    if (GS.recInput("Free view", 1) != 0) FreeLook.transform.localRotation = Quaternion.Lerp(FreeLook.transform.localRotation, Quaternion.Euler(-90f * SteerPosition.y, 180f * SteerPosition.x, 0f), 0.1f);
        		    else FreeLook.transform.localRotation = Quaternion.Lerp(FreeLook.transform.localRotation, Quaternion.identity, 0.5f);
					// Rotate Camera
				} else if(GS.recInput("Aiming", 1) != 0 && Addition.Names[0] == "Turret"){
        		    WhichCamera = "Turret";
        	        Camera.transform.position = Turret.transform.GetChild(0).position + (ShakeScreen / 10f);
    	        	Camera.transform.rotation = Turret.transform.GetChild(0).rotation;
		            Turret.transform.GetChild(0).localRotation = Quaternion.Euler(Mathf.Clamp((-60f * SteerPosition.y), -90f, 30f), 200f * SteerPosition.x, 0f);
	            	Turret.transform.GetChild(3).rotation = Quaternion.Slerp(Turret.transform.GetChild(3).rotation, Turret.transform.GetChild(0).rotation, 0.25f);

	        	    Turret.transform.GetChild(3).gameObject.SetActive(true);
    		        Turret.transform.GetChild(1).gameObject.SetActive(false);
    	            Turret.transform.GetChild(2).gameObject.SetActive(true);
				} else {
					if(GS.recInput("Aiming", 1) != 0) WhichCamera = "Aim";
    		        else WhichCamera = "Normal";
	        	    Vector3 AddSteerCamera = this.transform.right * ((ElevatorFlapsRudder.z - ElevatorFlapsRudder.y) * -2f) + this.transform.up * (ElevatorFlapsRudder.x * -2f);
					Camera.transform.position = AddSteerCamera*divASC + (this.transform.position + ShakeScreen) + (this.transform.forward * -5f) + (this.transform.forward * (Speed / MaxSpeed) * -10f) + (this.transform.up * Mathf.Lerp(4f, 2f, divASC) );
					//Camera.transform.LookAt (Camera.transform.position + (this.transform.forward * 1f), this.transform.up * 1f);
					mainCanvas.GetComponent<CanvasScript> ().Crosshair.transform.localScale = Vector3.one*0.1f;
					Camera.transform.LookAt (FreeLook.transform.position + FreeLook.transform.forward * 1000f, this.transform.up);
				}

			}
		} else {

			switch(IntroZoom){
				case 0:
					Camera.transform.LookAt(this.transform.position + this.transform.up*2f + this.transform.forward*10f);
					Camera.transform.position = this.transform.position + this.transform.up*2f + this.transform.forward*-Mathf.Lerp(15f, -500f, Intro*Intro/25f) + this.transform.right*Mathf.Sin(Intro*Intro/25f)*250f;
					Camera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(60f, 10f, Intro/5f);
					break;
				case 1:
					Camera.transform.LookAt(this.transform.position + this.transform.up*2f + this.transform.forward*1000f);
					Camera.transform.position = this.transform.position + this.transform.up*Mathf.Lerp(2f, 10f, Intro/5f) + this.transform.forward*Mathf.Lerp(-15f, 5f, Intro/5f);
					Camera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(60f, 10f, Intro/5f);
					break;
			}

		}

        float DesiredPOV = 60f;
        if (WhichCamera == "Aim" || WhichCamera == "Turret") {
			if (Addition.Names[0] == "Zoom") DesiredPOV = 10f;
			else if (!(ControlType != "Point" && WhichCamera == "Turret")) DesiredPOV = 30f;
		} else {
			if(WhichCamera == "Free") DesiredPOV = 80f;
            if (Addition.Names[0] == "Turret" && WhichCamera != "Turret") {
                Turret.transform.GetChild(3).gameObject.SetActive(false);
                Turret.transform.GetChild(1).gameObject.SetActive(true);
                Turret.transform.GetChild(2).gameObject.SetActive(false);
            }
		}
		Camera.GetComponent<Camera> ().fieldOfView = Mathf.Lerp(Camera.GetComponent<Camera> ().fieldOfView , DesiredPOV, 0.1f);
		// Camera

		// Visual Effects
		if (Health <= (MaxHealth / 3f)) {
			ParticleSystem myParticleSystem;
			ParticleSystem.EmissionModule emissionModule;
			myParticleSystem = Smoke.GetComponent<ParticleSystem> ();
			emissionModule = myParticleSystem.emission;
			emissionModule.rateOverTime = 200f - ((Health / (MaxHealth / 3f)) * 200f);
			Smoke.GetComponent<AudioSource> ().volume = (1f - (Health / (MaxHealth / 3f) * 1f)) * GS.Volumes[0]*GS.Volumes[2];
			Smoke.GetComponent<AudioSource> ().pitch = Random.Range (0.8f, 1.2f);
		} else {
			ParticleSystem myParticleSystem;
			ParticleSystem.EmissionModule emissionModule;
			myParticleSystem = Smoke.GetComponent<ParticleSystem> ();
			emissionModule = myParticleSystem.emission;
			emissionModule.rateOverTime = 0f;
			Smoke.GetComponent<AudioSource> ().volume = 0f;
		}

		if(Health > (MaxHealth / 5f)){
			Sparkles.GetComponent<ParticleSystem> ().Stop ();
		} else {
			Sparkles.GetComponent<ParticleSystem> ().Play ();
		}

		if((Health <= (MaxHealth / 5f)) || (Fuel <= (MaxFuel / 5f)) || BeyondMap == true){
			if(AlarmCooldown <= 0f){
				AlarmCooldown = 0.25f;
				Alarms.GetComponent<AudioSource> ().Play ();
				Alarms.GetComponent<Light> ().intensity = 5f;
			}
		} else if((Health < (MaxHealth / 3f)) || (Fuel < (MaxFuel / 3f))){
			if(AlarmCooldown <= 0f){
				AlarmCooldown = 2f;
				Alarms.GetComponent<AudioSource> ().Play ();
				Alarms.GetComponent<Light> ().intensity = 5f;
			}
		}
		if(Alarms.GetComponent<Light> ().intensity > 0f){
			Alarms.GetComponent<Light> ().intensity -= 0.1f;
		}

		if (PresentCooldown > 0f) {
			PresentCannon.transform.localScale = Vector3.zero;
			PresentColorGenerator ();
		} else {
			PresentCannon.transform.localScale = Vector3.Lerp (PresentCannon.transform.localScale, new Vector3(1f, 1f, 1f), 0.5f);;
			foreach(Material Mat in PresentCannon.transform.GetChild (0).GetComponent<MeshRenderer> ().materials){
				if(Mat.name == "Material1 (Instance)")Mat.color = PresentColor1;
				else if(Mat.name == "Material2 (Instance)")Mat.color = PresentColor2;
			}
		}

        float WindAlpha = Speed / MaxSpeed * GameObject.Find("RoundScript").GetComponent<RoundScript>().SnowIntensity;
		ParticleSystem.MainModule WC = Wind.main;
        WC.startColor = new Color(1f, 1f, 1f, WindAlpha * 0.001f);
		Wind.transform.eulerAngles = PointThere.eulerAngles;

		ParticleSystem.MainModule NGS = NearGroundSnow.main;
        if (this.transform.position.y > 100f) NGS.startColor = new Color(1f, 1f, 1f, 0f);
        else NGS.startColor = new Color(1f, 1f, 1f, (1f - (this.transform.position.y / 100f)) * 0.05f);
        Vector3 PlaneP = this.transform.position + (this.transform.forward * (Speed / 10f));
        NearGroundSnow.transform.position = new Vector3(PlaneP.x, 1f, PlaneP.z);
        NearGroundSnow.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
		// Visual Effects

		// Screen Shake
		ShakePower = Mathf.MoveTowards(ShakePower, 0f, ShakeDecay);
		ShakeScreen = new Vector3 (Random.Range(-ShakePower, ShakePower), Random.Range(-ShakePower, ShakePower), Random.Range(-ShakePower, ShakePower));
		// Screen Shake

		// Dead
		if(Health <= 0f){
			Health = 999999f;
			GameObject Corps = RS.DeadPlane(new[]{this.transform, CurrentPlane.transform}, Speed, ElevatorFlapsRudder);
			GS.statModify("Summarize", 1);
			GS.Parachutes -= 1;
            if (GS.Parachutes < 0){
				GS.CurrentScore += RS.TempScore;
                GS.PreviousScore = GS.CurrentScore;
                GS.PreviousPlane = PlaneModel.Names[0];
				Corps.GetComponent<PlaneDead>().IsGameOver = true;
                GS.SaveScore("Add_" + GS.Name, new[]{GS.CurrentScore, GS.GameMode, GS.Difficulty});
            } else {
                GS.HasDied = true;
            }
			Destroy(this.gameObject);

		}
		// Dead

		// Beyond Map
		if (this.transform.position.x < RS.MapSize/-2f || this.transform.position.x > RS.MapSize/2f || this.transform.position.z < RS.MapSize/-2f || this.transform.position.z > RS.MapSize/2f) {
			BeyondMap = true;
			if (Desertion > 0f) {
				mainCanvas.Message(GS.SetText("You're leaving the map! Go back, now!\n " + Mathf.Round (Desertion) + " seconds", "Opuszczasz mapę! Wracaj natychmiast!\n " + Mathf.Round (Desertion) + " sekundy"), Color.red, new float[]{0.25f, 1f});
				Desertion -= 0.01f;
			} else {
				mainCanvas.Message(GS.SetText("You'll be taken down for desertion", "Zostaniesz zdjęty za dezercję"), Color.red, new float[]{0.25f, 1f});
				if (DesertionBoom > 0f) {
					DesertionBoom -= 0.01f;
				} else {
					DesertionBoom = Random.Range (0.05f, 0.5f);
					GameObject Boom = Instantiate (Special) as GameObject;
					Boom.GetComponent<SpecialScript>().TypeofSpecial = "Explosion";
					Boom.GetComponent<SpecialScript> ().ExplosionPower = 100f;
					Boom.transform.position = this.transform.position + new Vector3 (Random.Range(-6f ,6f), Random.Range(-6f ,6f), Random.Range(-6f ,6f));
				}
			}
		} else {
			BeyondMap = false;
			if (Desertion < 5f) {
				Desertion = 5f;
			}
		}
		// Beyond Map

		// Fuel
		if(Throttle > 0f && EngineModel.Names[0] != "Magic Reindeer Dust")
			Fuel -= Throttle / 100f * Mathf.Clamp(GS.Difficulty, 1f, 2f);
		// Fuel

	}

	void PlaneSettings (int DoOnce) {


		foreach(Transform SelectedPlane in PlaneModels.transform){
			if (SelectedPlane.gameObject.name == PlaneModel.Names[0]) {
				CurrentPlane = SelectedPlane.gameObject;
				foreach(Material Mat in SelectedPlane.GetChild(0).GetComponent<MeshRenderer>().materials){
					if(Mat.name == "PlaneColor1 (Instance)"){
						Mat.color = PlaneColor1;
					} else if(Mat.name == "PlaneColor2 (Instance)"){
						Mat.color = PlaneColor2;
					}
				}
			} else {
				SelectedPlane.gameObject.SetActive (false);
			}
		}

		if(CurrentPlane != null){
			foreach (Transform SelectedPart in CurrentPlane.transform) {
				switch(SelectedPart.name){
	                case "AimPart":
                    	AimPart = SelectedPart.gameObject;
						break;
            	    case "Turret":
        	            if (Addition.Names[0] == "Turret") {
    	                    SelectedPart.gameObject.SetActive(true);
	                        Turret = SelectedPart.gameObject;
                        	foreach (Material Mat in Turret.transform.GetChild(1).GetComponent<MeshRenderer>().materials) {
                    	        if (Mat.name == "PlaneColor1 (Instance)")
                	                Mat.color = PlaneColor1;
            	                else if (Mat.name == "PlaneColor2 (Instance)")
        	                        Mat.color = PlaneColor2;
    	                    }
	                        foreach (Material Mat in Turret.transform.GetChild(2).GetComponent<MeshRenderer>().materials) {
                            	if (Mat.name == "PlaneColor1 (Instance)")
                        	        Mat.color = PlaneColor1;
                    	        else if (Mat.name == "PlaneColor2 (Instance)")
                	                Mat.color = PlaneColor2;
            	            }
        	            } else {
    	                    SelectedPart.gameObject.SetActive(false);
	                    }
						break;
                	case "PresentCannon":
						PresentCannon = SelectedPart.gameObject;
						break;
					case "Basic Propeller":
						if (EngineModel.Names[0] == "Basic Propeller") {
							SelectedPart.gameObject.SetActive (true);
							SelectedPart.transform.Rotate (new Vector3 (Speed / -10f, 0f, 0f));
							SelectedPart.gameObject.GetComponent<AudioSource> ().pitch = (Speed / MaxSpeed / 2f) * Throttle;
							SelectedPart.gameObject.GetComponent<AudioSource> ().volume = GS.Volumes[0] * GS.Volumes[2];
						} else {
							SelectedPart.gameObject.SetActive (false);
							SelectedPart.gameObject.GetComponent<AudioSource> ().Stop ();
						}
						break;
					case "Double Propeller":
						if (EngineModel.Names[0] == "Double Propeller") {
							SelectedPart.gameObject.SetActive (true);
							foreach(Material Mat in SelectedPart.GetComponent<MeshRenderer>().materials){
								if(Mat.name == "PlaneColor1 (Instance)")
									Mat.color = PlaneColor1;
								else if(Mat.name == "PlaneColor2 (Instance)")
									Mat.color = PlaneColor2;
							}
							SelectedPart.transform.GetChild(0).Rotate (new Vector3 (Speed / -10f, 0f, 0f));
        	                SelectedPart.gameObject.GetComponent<AudioSource>().pitch = (Speed / MaxSpeed / 2f) * Throttle;
							SelectedPart.gameObject.GetComponent<AudioSource> ().volume = GS.Volumes[0] * GS.Volumes[2];
    	                } else {
							SelectedPart.gameObject.SetActive (false);
							SelectedPart.gameObject.GetComponent<AudioSource> ().Stop ();
						}
						break;
					case "Jet Engine":
						if (EngineModel.Names[0] == "Jet Engine") {
							SelectedPart.gameObject.SetActive (true);
							foreach(Material Mat in SelectedPart.GetComponent<MeshRenderer>().materials){
								if(Mat.name == "PlaneColor1 (Instance)")
									Mat.color = PlaneColor1;
								else if(Mat.name == "PlaneColor2 (Instance)")
									Mat.color = PlaneColor2;
							}
							ParticleSystem.MainModule JE = SelectedPart.transform.GetChild(0).GetComponent<ParticleSystem> ().main;
							JE.startColor = new Color(1f, 1f, 1f, (Mathf.Clamp((Speed / MaxSpeed) * (0.5f * Throttle), 0f, 255f)));
							SelectedPart.gameObject.GetComponent<AudioSource> ().pitch = Speed / MaxSpeed * Throttle;
							SelectedPart.gameObject.GetComponent<AudioSource> ().volume = GS.Volumes[0] * GS.Volumes[2];
						} else {
							SelectedPart.gameObject.SetActive (false);
							SelectedPart.gameObject.GetComponent<AudioSource> ().Stop ();
						}
						break;
					case "Double Jet Engine":
						if (EngineModel.Names[0] == "Double Jet Engine") {
							SelectedPart.gameObject.SetActive (true);
							foreach(Material Mat in SelectedPart.GetComponent<MeshRenderer>().materials){
								if(Mat.name == "PlaneColor1 (Instance)"){
									Mat.color = PlaneColor1;
								} else if(Mat.name == "PlaneColor2 (Instance)"){
									Mat.color = PlaneColor2;
								}
							}
							ParticleSystem.MainModule DJE = SelectedPart.transform.GetChild(0).GetComponent<ParticleSystem> ().main;
							DJE.startColor = new Color(1f, 1f, 1f, (Mathf.Clamp((Speed / MaxSpeed) * (0.5f * Throttle), 0f, 255f)));
        	                SelectedPart.gameObject.GetComponent<AudioSource> ().pitch = Speed / MaxSpeed * Throttle;
							SelectedPart.gameObject.GetComponent<AudioSource> ().volume = GS.Volumes[0] * GS.Volumes[2];
						} else {
							SelectedPart.gameObject.SetActive (false);
							SelectedPart.gameObject.GetComponent<AudioSource> ().Stop ();
						}
						break;
					case "Magic Reindeer Dust":
						if (EngineModel.Names[0] == "Magic Reindeer Dust") {
							SelectedPart.gameObject.SetActive (true);
							float SetPitch = Mathf.Clamp ((Speed / MaxSpeed) * Throttle, 0.75f, 2f);
							SelectedPart.gameObject.GetComponent<AudioSource> ().pitch = SetPitch;
							SelectedPart.gameObject.GetComponent<AudioSource> ().volume = GS.Volumes[0] * GS.Volumes[2];
						} else {
							SelectedPart.gameObject.SetActive (false);
							SelectedPart.gameObject.GetComponent<AudioSource> ().Stop ();
						}
						break;
					case "Gun":
						if(DoOnce == 1){
							Guns.Add (SelectedPart.gameObject);
							AmountOfGuns += 1;
						}
						break;
				}
			}
		}

	}

	void Mechanics () {

		// Set speed
		// Set X Angle
		float AngleX = 1f; // When x is greater, it means, the plane is diving down
		if( this.transform.eulerAngles.x >= 0f &&  this.transform.eulerAngles.x <= 90f) AngleX = 0.5f + ((this.transform.eulerAngles.x / 90f) / 2f);
		else if( this.transform.eulerAngles.x >= 270f &&  this.transform.eulerAngles.x <= 360f) AngleX = ((270f - this.transform.eulerAngles.x) / -90f) / 2f;
        // Set X Angle

        float DesiredSpeed;
        if (AngleX > 0.5f) DesiredSpeed = Mathf.Lerp(MaxSpeed * Throttle, MaxSpeed*3f, (AngleX - 0.5f) * 2f);
        else DesiredSpeed = Mathf.Lerp(-MaxSpeed, MaxSpeed * Throttle, AngleX * 2f);
        if (FowardSpeed < DesiredSpeed || AngleX < 0.5f) FowardSpeed = Mathf.MoveTowards (FowardSpeed, DesiredSpeed, MaxSpeed / 500f);
		else FowardSpeed = Mathf.MoveTowards (FowardSpeed, DesiredSpeed, MaxSpeed / 1000f);
		Speed = Mathf.Lerp(FowardSpeed, 0f, Vector3.Angle(this.transform.forward, FlyingTowards) / 180f);
		// Set speed

		// Diving too fast
		if(Speed / MaxSpeed >= 1.75f){
			mainCanvas.Message(GS.SetText("You're going too fast!", "Prędkość zbyt wysoka!"), Color.red, new float[]{0.25f, 1f});
            Health -= Random.Range (MaxHealth / 1000f, MaxHealth / 500f);
			ShakePower = (1.75f - (Speed / MaxSpeed)) * 2f;
			ShakeDecay = 0.1f;
		}
		// Diving too fast

		// Move forward
		PrevPos = this.transform.position;
		FlyingTowards = Vector3.Lerp(FlyingTowards, this.transform.forward, Mathf.Clamp(Speed / MaxSpeed, 0f, 1f) * 0.03f);
		this.transform.position += FlyingTowards  * (Speed/90f);
		// Move forward

		// Chandelle
		float AngleZ = 0f;
		float CF = 0f;
        float AngleDecreaser;
        if (AngleX >= 0f && AngleX < 0.75f) AngleDecreaser = 1f;
		else AngleDecreaser = 1f - ((AngleX - 0.75f) / 0.25f);

		if( this.transform.eulerAngles.z >= 0f && this.transform.eulerAngles.z <= 90f){
			AngleZ = (this.transform.eulerAngles.z / 90f) * -1f;
			CF = 0f;
		} else if( this.transform.eulerAngles.z >= 270f && this.transform.eulerAngles.z <= 360f){
			AngleZ = 1f - ((270f - this.transform.eulerAngles.z) / -90f);
			CF = 0f;
		} else if( this.transform.eulerAngles.z > 90f && this.transform.eulerAngles.z < 270f){
			AngleZ = 0f;
			CF = 1f;
		}
		AngleZ *= AngleDecreaser; tAnglez = Mathf.Lerp(tAnglez, AngleZ, 0.025f);
		CF *= AngleDecreaser; tCF = Mathf.Lerp(tCF, CF, 0.025f);
		// Chandelle
		// Flaps and rudder
		float RotationalSpeed = Speed / MaxSpeed;
		this.transform.Rotate(ElevatorFlapsRudder * (RotationSpeed * RotationalSpeed)); // Turning speed
		this.transform.Rotate(new Vector3 (0f, tAnglez / 2f * RotationSpeed, 0f));
		this.transform.eulerAngles += new Vector3 (tCF / 2f * RotationSpeed, 0f, 0f);

		Vector3 DesiredEFR;
		if(ControlType == "Point")
		{

			PointThere.transform.position = this.transform.position;
			PTturn = new Vector2(Mathf.Clamp(PTturn.x + Input.GetAxis("Mouse Y") * 10f * Inverted, -89f, 89f), PTturn.y + Input.GetAxis("Mouse X") * 10f);
			PointThere.rotation = Quaternion.Lerp(PointThere.rotation, Quaternion.Euler(PTturn), 0.1f);

			float TurnX = (this.transform.forward * 100f).y - (PointThere.forward*100f).y;
			float TurnY = Vector3.SignedAngle(this.transform.forward, PointThere.transform.forward*20f, Vector3.up);

			if(WhichCamera == "Free" || WhichCamera == "Turret")
				TurnX = TurnY = 0f;

			DesiredEFR = Vector3.Lerp(
				new Vector3(
					Mathf.Clamp(TurnX / 10f, -1f, 1f),
					Mathf.Clamp(TurnY / 10f, -1f, 1f),
					Mathf.Clamp(AngleZ * 10f, -1f, 1f)
				)
			,
				new Vector3(
					Mathf.Clamp(TurnX + Mathf.Abs(AngleZ)*-20f, -1f, 1f),
					0,//Mathf.Clamp(AngleX, -0.3f, 0.3f),
					Mathf.Clamp(Mathf.Lerp( TurnY / -2f, TurnY / 2f, Mathf.Abs(AngleZ*0.8f) ), -1f, 1f)
				)
			,
				Mathf.Clamp((Mathf.Abs(TurnY)-10f) / 30f - Mathf.Clamp(Mathf.Abs(TurnY)-160f, 0f, 10f), 0f, 1f)
			);

			if(GS.recInput("Pitch", 1) != 0 || GS.recInput("Roll", 1) != 0 || GS.recInput("Yaw", 1) != 0)
				DesiredEFR = new Vector3(
    	            GS.recInput("Pitch", 1) * Inverted,
        	    	GS.recInput("Yaw", 1) / 3f,
                	-GS.recInput("Roll", 1));

			ElevatorFlapsRudder = Vector3.Lerp(
				ElevatorFlapsRudder,
				DesiredEFR,
				0.1f
			);

		}
		else
		{

			// Steering circle
			Vector2 SetSteer = new(
				Mathf.Clamp( Mathf.Lerp(SteerPosition.x, -1f + (Input.mousePosition.x / Screen.width * 2f), Time.deltaTime * 100f) , -1f , 1f ),
				Mathf.Clamp( Mathf.Lerp(SteerPosition.y,  (1f - (Input.mousePosition.y / Screen.height * 2f)) * Inverted  , Time.deltaTime * 100f) , -1f , 1f ));
			SteerPosition = SetSteer;
			// Steering circle

			if (WhichCamera == "Free" || WhichCamera == "Turret"){
	            ElevatorFlapsRudder = Vector3.Lerp(
					ElevatorFlapsRudder,
					new Vector3(
    	            	GS.recInput("Pitch", 1) * Inverted,
        	    		GS.recInput("Yaw", 1) / 3f,
                		-GS.recInput("Roll", 1)),
					0.1f);
				ElevatorFlapsRudder = Vector3.Lerp(ElevatorFlapsRudder, Vector3.zero, Stalling /2f);
			} else {
				DesiredEFR = new Vector3(
						SteerPosition.y * -1f,
						GS.recInput("Yaw", 1) / 3f,
						SteerPosition.x * -1f);
				if(GS.recInput("Pitch", 1) != 0) DesiredEFR.x = GS.recInput("Pitch", 1) * Inverted;
				if(GS.recInput("Roll", 1) != 0) DesiredEFR.z = -GS.recInput("Roll", 1);

				ElevatorFlapsRudder = Vector3.Lerp(
					ElevatorFlapsRudder,
					DesiredEFR,
					0.1f);
				ElevatorFlapsRudder = Vector3.Lerp(ElevatorFlapsRudder, Vector3.zero, Stalling /2f);
			}

		}
		
		// Flaps and rudder

		// Lift force
		float AngleOfAttack = Mathf.Lerp(0f, 1f, AngleX);
		LiftDir = Vector3.Lerp(LiftDir, Vector3.down*0.5f + Vector3.Lerp(this.transform.up, Vector3.down, Stalling / 3f) * Mathf.Lerp(1f, AngleOfAttack, Speed/MaxSpeed), 0.1f);
		this.transform.position += LiftDir;
		// Lift force

		// Stalling
		float StallFactor = Mathf.Lerp(0.75f, 0f, AngleX);
		if(Speed <= MaxSpeed * StallFactor)
			Stalling = Mathf.Clamp(Stalling + 0.02f, 0f, 1f);

		if(Stalling > 0f){
			mainCanvas.Message(GS.SetText("You're stalling!", "Prędkość zbyt wysoka"), Color.red, new float[]{0.25f, 1f});
            Stalling -= 0.01f;
			this.transform.eulerAngles += new Vector3(Mathf.Lerp(Stalling, 0f, AngleX), 0f, 0f);
			this.transform.position += Vector3.down*Stalling/2f;
		}
		// Stalling

		// Bomb calculations
		if(PresentType.Type == 2 && PresentCooldown <= 0f) LeadBomb(0, PresentCannonDistane);
		else LeadBomb(0, 0f);
		// Bomb calculations

	}

	void LeadBomb(int WhichBomb, float EffectiveRange){
		Ray CheckPos = new (this.transform.position, (FlyingTowards * 2f * Speed/MaxSpeed) + Vector3.down);
		if(Physics.Raycast(CheckPos, out RaycastHit CheckHit, EffectiveRange) && CheckHit.distance > 10f){
			BombPositions[WhichBomb] = CheckHit.point;
			BombShows[WhichBomb] = true;
			BombGuis[WhichBomb].position = CheckHit.point + Vector3.up;
			BombGuis[WhichBomb].forward = Vector3.up;
			BombGuis[WhichBomb].localScale = Vector3.one/4f * Vector3.Distance(Camera.transform.position, BombGuis[WhichBomb].position);
		} else {
			BombShows[WhichBomb] = false;
			if(BombGuis[WhichBomb].localScale.x > 0f) BombGuis[WhichBomb].localScale = Vector3.zero;
		}
	}

	void Controlling() {

		// Throttle
		Throttle += GS.recInput("Throttle", 1) / 100f;
		// Throttle

		// Shooting
		if(GS.recInput("Fire guns", 1) != 0){
			if(GunCooldown <= 0f && Heat >= 0f){
                if (WhichCamera == "Turret") {
					Vector3 PickedPosition = Turret.transform.GetChild(0).GetChild(0).position;
					GameObject TurretBullet = Shoot("Main", PickedPosition, Turret.transform.GetChild(0).GetChild(0), PickedPosition + Camera.transform.forward);
                    GunCooldown = MaxGunCooldown;
					Heat += MaxGunCooldown;
                    if (GunType.Names[0] == "Paintball") TurretBullet.GetComponent<ProjectileScript>().PresentColor1 = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 0.5f);
                } else {
                    GameObject PickedGun = Guns.ToArray()[Random.Range(0, Guns.Count)];
					GameObject MainBullet = Shoot("Main", PickedGun.transform.position, PickedGun.transform, PickedGun.transform.position + this.transform.forward);
                    if (AirplaneClass == 2) {
                        GunCooldown = MaxGunCooldown / 2 / AmountOfGuns;
						Heat += MaxGunCooldown / 2 / AmountOfGuns;
                    } else {
                        GunCooldown = MaxGunCooldown / AmountOfGuns;
						Heat += MaxGunCooldown / AmountOfGuns;
                    }
                    if (GunType.Names[0] == "Paintball") MainBullet.GetComponent<ProjectileScript>().PresentColor1 = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 0.5f);
                }
				if(GunType.Names[0] == "Cannon" || GunType.Names[0] == "Jet Gun"){
					ShakePower = 0.5f;
					ShakeDecay = 0.05f;
				} else if(GunType.Names[0] == "Flak"){
					ShakePower = 1f;
					ShakeDecay = 0.1f;
				} else {
					ShakePower = 0.1f;
					ShakeDecay = 0.01f;
				}
				if(earProtection <= 0f) earProtection = Mathf.Clamp(GunCooldown-0.01f, 0.075f, Mathf.Infinity);
			}
		}
		if(GS.recInput("Fire a present", 1) != 0){
			if(PresentCooldown <= 0f){
				PresentCooldown = MaxPresentCooldown;

				if(PresentType.Type == 2){
					if(BombShows[0]) Shoot(PresentType.Names[0], PresentCannon.transform.position, PresentCannon.transform, BombPositions[0]);
					else mainCanvas.Message(GS.SetText("Cannot drop present like that!", "Nie można zrzucić prezentu w ten sposób!"), Color.red, new float[]{3f,1.5f}, "Overheating");
				} else {
					Shoot(PresentType.Names[0], PresentCannon.transform.position, PresentCannon.transform, PresentCannon.transform.position + this.transform.forward);
				}

				if(PresentType.Type == 0){
					ShakePower = 1f;
					ShakeDecay = 0.1f;
				}
			}
		}
		if(GS.recInput("Use a special", 1) != 0){
            if (SpecialType.Names[0] == "None"){
				mainCanvas.Message(GS.SetText("You don't have any specials!", "Nie masz żadnych specjalnych przedmiotów!"), Color.red, new float[]{3f, 1f});
			}
            if (SpecialCooldown <= 0f){
				SpecialCooldown = MaxSpecialCooldown;
                if(SpecialType.Names[0] == "Wrenches"){
					Health += MaxHealth / 2f;
					mainCanvas.Flash(Color.green, new float[]{0.5f, 1f}, 0, 1);
				} else if(SpecialType.Names[0] == "Fuel Tank"){
					Fuel = MaxFuel;
					mainCanvas.Flash(new Color(0f, 0.5f, 1f, 1f), new float[]{0.5f, 1f}, 0, 1);
				} else if(SpecialType.Names[0] == "Homing Missile"){
					Shoot("Homing Missile", PresentCannon.transform.position + (this.transform.forward * 15f), PresentCannon.transform, PresentCannon.transform.position + this.transform.forward * 16f);
				} else if(SpecialType.Names[0] == "Flares"){
					GameObject Bullet = Instantiate (Special) as GameObject;
					Bullet.transform.position = this.transform.position;
					Bullet.GetComponent<SpecialScript> ().TypeofSpecial = "Flares";
					Flares = 10f;
					mainCanvas.Flash(new Color(1f, 0.5f, 0.5f, 1f), new float[]{Flares, 1f}, 1, 1);
				} else if(SpecialType.Names[0] == "Brick"){
					Shoot("Brick", this.transform.position, this.transform, this.transform.position + this.transform.forward);
                }
			}
		}
		// Shooting

	}

	GameObject Shoot(string TypeofGun, Vector3 From, Transform GunFirePos, Vector3 There){
		GameObject Bullet = Instantiate(Projectile) as GameObject;
        Bullet.transform.position = From;
        Bullet.transform.LookAt(There);
		if(earProtection > 0f) {
			Bullet.GetComponent<ProjectileScript>().GunFirePos = null;
		} else { 
			Bullet.GetComponent<ProjectileScript>().GunFirePos = GunFirePos;
		}
        Bullet.GetComponent<ProjectileScript>().WhoShot = this.gameObject;
        Bullet.GetComponent<ProjectileScript>().PreviusSpeed = Speed;
		switch(TypeofGun){
			case "Main":
				Bullet.GetComponent<ProjectileScript>().TypeofGun = GunType.Names[0];
				GS.statModify("Shots fired", 1);
				break;
			case "Present":
				Bullet.GetComponent<ProjectileScript>().TypeofGun = "Present";
				Bullet.GetComponent<ProjectileScript> ().PresentRange = PresentCannonDistane;
				Bullet.GetComponent<ProjectileScript> ().PresentColor1 = PresentColor1;
				Bullet.GetComponent<ProjectileScript> ().PresentColor2 = PresentColor2;
				break;
			default:
				Bullet.GetComponent<ProjectileScript>().TypeofGun = TypeofGun;
				break;
		}
		return Bullet;
	}

	void PlaneData(float a, float b, float c, float d, int classa){
		MaxFuel = a; MaxHealth = b; RotationSpeed = c; MaxSpeed = d; AirplaneClass = classa;
	}

	void SetStats(){

		PlaneData(PlaneModel.Fuel, PlaneModel.Health, PlaneModel.RotationSpeed, PlaneModel.Speed, PlaneModel.Class);
		MaxSpeed *= EngineModel.SpeedMultiplier; RotationSpeed *= EngineModel.SpeedMultiplier;

		GunDistane = Random.Range(GunType.Range[0], GunType.Range[1]);
		MaxGunCooldown = GunType.Cooldown[0];
		MaxHeat = GunType.Cooldown[1];
		HeatDispare = GunType.Cooldown[2];

		PresentCannonDistane = PresentType.Range;
		MaxPresentCooldown = PresentType.Cooldown;

		MaxSpecialCooldown = SpecialType.Cooldown;
		SpecialCharges = 1;

		PlaneColor1 = Paint.Paints[0];
		PlaneColor2 = Paint.Paints[1];

        if (AirplaneClass == 2)MaxHeat *= 2;
        if (Addition.Names[0] == "Ammo Pack") MaxHeat *= 3;

		Health = MaxHealth;
		Fuel = MaxFuel;
		Heat = 0;
		Speed = FowardSpeed = MaxSpeed;


	}

	void PresentColorGenerator(){
		float PickHue = Random.Range(0f, 1f);
		PresentColor1 = Color.HSVToRGB(PickHue, 1f, 0.5f);
		PresentColor2 = Color.HSVToRGB((PickHue + 0.5f) % 1f, 1f, 0.5f);
	}

	public void Hurt(float Damage, int RedFlash, int PowerofShake){

		Health -= Damage * Mathf.Clamp(GS.Difficulty, 1f, 2f);
		if(RedFlash == 1)mainCanvas.Flash(Color.red, new float[]{0.25f, 0.25f}, 0, 1);
		else if(RedFlash == 2)mainCanvas.Flash(Color.red, new float[]{0.5f, 0.5f}, 0, 1);

		if(PowerofShake == 1){
			ShakePower = 1f;
			ShakeDecay = 0.1f;
		} else if(PowerofShake == 2){
			ShakePower = 1f;
			ShakeDecay = 0.05f;
		} else if(PowerofShake == 3){
			ShakePower = 3f;
			ShakeDecay = 0.1f;
		} else if(PowerofShake == 4){
			ShakePower = 3f;
			ShakeDecay = 0.05f;
		}

	}

    void OnTriggerEnter(Collider Col) {

        if (Col.tag == "Terrain" || Col.name == "Home") {
            if (Addition.Names[0] == "Damper") {
                Hurt(0f, 2, 4);
            } else {
                Hurt(Random.Range(25f, 50f), 2, 4);
            }
            this.transform.position += Vector3.up * 10f;
            this.transform.LookAt(this.transform.position + (Vector3.up * Random.Range(1f, 2f)) + this.transform.forward * 10f, this.transform.up);
            GameObject Hit = Instantiate(Effect) as GameObject;
            Hit.GetComponent<EffectScript>().TypeofEffect = "BullethitPlane";
            Hit.transform.position = this.transform.position;
		} else if(Col.name == "Explosion"){
			if(Col.transform.parent.GetComponent<SpecialScript>().IsLethal == true){
				Hurt (Col.transform.parent.GetComponent<SpecialScript>().ExplosionPower, 2, 3);
				GameObject Hit = Instantiate (Effect) as GameObject;
				Hit.GetComponent<EffectScript> ().TypeofEffect = "BullethitPlane";
				Hit.transform.position = this.transform.position;
			}
		} else if(Col.name == "Portal"){
			if(GameObject.Find("RoundScript").GetComponent<RoundScript>().State == "Success"){
				GameObject.Find ("RoundScript").GetComponent<RoundScript> ().State = "Left1";
			}
		} else if(Col.transform.parent != null){
			if(Col.transform.parent.name == "VesselModels"){
				if (Addition.Names[0] == "Damper") {
					Col.transform.parent.parent.GetComponent<EnemyVesselScript>().Health -= 20f;
				} else {
					Col.transform.parent.parent.GetComponent<EnemyVesselScript>().Health -= 10f;
				}
				Col.transform.parent.parent.GetComponent<EnemyVesselScript> ().HitByAPlayer = 5f;
				Hurt (Random.Range(25f, 50f), 2, 4);
				GameObject Hit = Instantiate (Effect) as GameObject;
				Hit.GetComponent<EffectScript> ().TypeofEffect = "BullethitPlane";
				Hit.transform.position = this.transform.position;
			}
		}

	}

    private void OnTriggerStay(Collider Col){
		if (Col.tag == "Cloud") {
            Flares = Mathf.Clamp(Flares + 0.04f, 0f, 3f);
			mainCanvas.Flash(RenderSettings.fogColor, new float[]{Flares/2f, Flares}, -1);
        }
    }

}

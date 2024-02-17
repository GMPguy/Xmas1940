﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EnemyVesselScript : MonoBehaviour {

	// References
	public GameObject Models;
	public GameObject Model;
	public GameObject[] FarMarkers;
	public GameObject FarMarker;
	public GameObject MarkerNear;
	float MarkerUpdate = 0f;
	public GameObject PlayerSeen;
	public GameObject PlayerActuall;
	public GameObject Bullet;
	public GameObject PlaneDead;
	public GameObject BalloonDeadEffects;
	public GameObject Special;
	GameObject Camera;
    
	GameScript GS;
	// References

	// Main Variables
	public float Power = 1f;
	public float Health, Speed, MaxSpeed, RotationSpeed = 100f;
	public bool IsDead = false;
	public string TypeofVessel = "Messerschmitt";
	public Vector3 PointofInterest;
	float GunCooldown = 0f;
	public float HitByAPlayer = 0f;
    public float Paintballed = 0f;
    // Main Variables

    // Ai Bevahiour
    float Change = 0f;
    float PullUpMultiplier = 0f;
	float DetectionDistance = 0f;
	float GunSpeed = 0f;

	public Transform PointThere;
	Vector3 FlyingTowards;
	Vector3 ElevatorFlapsRudder = Vector3.zero;
	float Stalling = 0f;

	Vector3 Origin;
    // Ai Bevahiour

    // Use this for initialization
    void Start () {

		GS = GameObject.Find("GameScript").GetComponent<GameScript>();
		Camera = GameObject.Find ("MainCamera");
		PointThere.SetParent(null);

		foreach(Transform ModelChoosen in Models.transform){
			if(ModelChoosen.name == TypeofVessel){
				Model = ModelChoosen.gameObject;
			}
		}

		Origin = this.transform.position;

		if (TypeofVessel == "Messerschmitt") {
			Health = Mathf.Lerp(10f, 100f, Power);
			MaxSpeed = Speed = Mathf.Lerp(50f, 200f, Power);
			RotationSpeed = Mathf.Lerp(0.25f, 2f, Power);
			GunSpeed = 750f;
			DetectionDistance = Mathf.Lerp(400f, 1500f, Power);
		} else if (TypeofVessel == "Messerschmitt Me 262") {
			Health = Mathf.Lerp(20f, 200f, Power);
			MaxSpeed = Speed = Mathf.Lerp(300f, 700f, Power);
			RotationSpeed = Mathf.Lerp(0.25f, 1f, Power);
			GunSpeed = 2000f;
			DetectionDistance = Mathf.Lerp(1000f, 5000f, Power);
		} else if(TypeofVessel == "Messerschmitt K4"){
			Health = Mathf.Lerp(20f, 200f, Power);
			MaxSpeed = Speed = Mathf.Lerp(50f, 200f, Power);
			RotationSpeed = Mathf.Lerp(1f, 3f, Power);
			GunSpeed = 1000f;
			DetectionDistance = Mathf.Lerp(400f, 1500f, Power);
		} else if(TypeofVessel == "Messerschmitt 110"){
			Health = Mathf.Lerp(50f, 500f, Power);
			MaxSpeed = Speed = Mathf.Lerp(50f, 200f, Power);
			RotationSpeed = Mathf.Lerp(0.25f, 1f, Power);
			GunSpeed = 500f;
			DetectionDistance = Mathf.Lerp(400f, 1500f, Power);
		} else if(TypeofVessel == "AA Gun"){
			Health  = Mathf.Lerp(20f, 200f, Power);
			GunSpeed = 1000f;
			DetectionDistance = Mathf.Lerp(250f, 2000f, Power);
		} else if(TypeofVessel == "Balloon"){
			Health = Mathf.Lerp(30f, 300f, Power);
			GunSpeed = 1000f;
			DetectionDistance = Mathf.Lerp(250f, 2000f, Power);
		}
		Speed = MaxSpeed;

        // Set Positions
		int pickedMarker = 0;
        if (TypeofVessel == "Messerschmitt" || TypeofVessel == "Messerschmitt K4" || TypeofVessel == "Messerschmitt 110" || TypeofVessel == "Messerschmitt Me 262"){
			pickedMarker = 0;
		} else if(TypeofVessel == "AA Gun"){
			pickedMarker = 1;
			Ray CheckGround = new(this.transform.position, Vector3.down);
        	if (Physics.Raycast(CheckGround, out RaycastHit CheckGroundHit, 1000f))
				this.transform.position = CheckGroundHit.point;
		} else if(TypeofVessel == "Balloon"){
			pickedMarker = 2;
		}
		// Set Positions

		Model.SetActive (true);
		FarMarker = FarMarkers[pickedMarker];
		
	}
	
	// Update is called once per frame
	void Update () {

		// Find player
		if (GameObject.Find ("MainPlane") != null) {
			if(GameObject.Find ("MainPlane").GetComponent<PlayerScript>().Flares <= 0f){
				PlayerSeen = GameObject.Find ("MainPlane");
				PlayerActuall = GameObject.Find ("MainPlane");
			} else {
				PlayerSeen = null;
				PlayerActuall = GameObject.Find ("MainPlane");
			}
		} else {
			PlayerSeen = null;
			PlayerActuall = null;
		}
		// Find player

		Marker(QualitySettings.GetQualityLevel());

		// Visual
		if(Model.name == "Messerschmitt" || TypeofVessel == "Messerschmitt K4"){
			Model.transform.GetChild (0).transform.Rotate (10f, 0f, 0f);
		}
		if(Model.name == "Messerschmitt 110"){
			Model.transform.GetChild (0).transform.Rotate (10f, 0f, 0f);
			Model.transform.GetChild (1).transform.Rotate (10f, 0f, 0f);
		}
		// Visual
		
	}

	void Marker(int Quality){

		if(MarkerUpdate > 0f){

			MarkerUpdate -= Time.deltaTime;

		} else {

			if (PlayerActuall != null && IsDead == false) {
				Transform ChostMarker = null;
				if (Vector3.Distance (this.transform.position, PlayerActuall.transform.position) > PlayerActuall.GetComponent<PlayerScript>().GunDistane) {
					FarMarker.transform.GetChild(0).GetComponent<TextMesh>().text = ((Vector3.Distance (this.transform.position, PlayerActuall.transform.position) / 1000f).ToString() + "000").Substring(0, 4) + "km";
					ChostMarker = FarMarker.transform;
					if(!FarMarker.activeSelf){
						FarMarker.SetActive (true);
						MarkerNear.SetActive (false);
					}
				} else {
					MarkerNear.transform.GetChild (0).GetComponent<TextMesh> ().text = ((Vector3.Distance(this.transform.position, PlayerActuall.transform.position) / 1000f).ToString() + "000").Substring(0, 4) + "km";
	                ChostMarker = MarkerNear.transform;
					if(FarMarker.activeSelf){
						FarMarker.SetActive (false);
						MarkerNear.SetActive (true);
					}
				}

				ChostMarker.LookAt (Camera.transform.position, Camera.transform.up * 1f);
				ChostMarker.localScale = Vector3.one * Vector3.Distance(this.transform.position, Camera.transform.position) * 0.2f;//new Vector3 (Vector3.Distance(this.transform.position, Camera.transform.position) * 0.2f, Vector3.Distance(this.transform.position, Camera.transform.position) * 0.2f, Vector3.Distance(this.transform.position, Camera.transform.position) * ScaleMultiplier);
			
				if(QualitySettings.GetQualityLevel() >= 1) {
					Color SC = ChostMarker.GetComponent<SpriteRenderer>().color;
					SC.a = Mathf.Lerp(1f, 0.1f, Quaternion.Angle (Quaternion.Euler(Camera.transform.eulerAngles.x, Camera.transform.eulerAngles.y, 0f), Quaternion.LookRotation (ChostMarker.position - Camera.transform.position)) / Mathf.Lerp(360f, 30f, (Vector3.Distance (this.transform.position, PlayerActuall.transform.position)-DetectionDistance) / DetectionDistance));
					ChostMarker.GetComponent<SpriteRenderer>().color = ChostMarker.GetChild(0).GetComponent<TextMesh>().color = SC;
				}

				if(QualitySettings.GetQualityLevel() == 2) MarkerUpdate = 0.04f;
				else if(QualitySettings.GetQualityLevel() == 1) MarkerUpdate = 0.1f;
				else if(QualitySettings.GetQualityLevel() == 0) MarkerUpdate = 0.25f;

			} else {
				if(FarMarker.activeSelf || MarkerNear.activeSelf){
					FarMarker.SetActive (false);
					MarkerNear.SetActive (false);
				}
			}

		}

	}

	void FixedUpdate(){

		if(!IsDead){

			switch(TypeofVessel){
				case "AA Gun": AAGun(); break;
				case "Balloon": Balloon(); break;
				default: Plane(); break;
			}

		}

		// Values
		if(GunCooldown > 0f) GunCooldown -= 0.01f;
		if(HitByAPlayer > 0f) HitByAPlayer -= 0.01f;
        if (Paintballed > 0f) Paintballed -= 0.01f;
		// Values

		// Dying
		if(Health <= 0f) Dead ();
		// Dying

	}

	void Mechanics (float Throttle) {

		// Set speed
		// Set X Angle
		float AngleX = 1f;
		if( this.transform.eulerAngles.x >= 0f &&  this.transform.eulerAngles.x <= 90f) AngleX = 0.5f + ((this.transform.eulerAngles.x / 90f) / 2f);
		else if( this.transform.eulerAngles.x >= 270f &&  this.transform.eulerAngles.x <= 360f) AngleX = ((270f - this.transform.eulerAngles.x) / -90f) / 2f;
        // Set X Angle

        float DesiredSpeed;
        if (AngleX > 0.5) DesiredSpeed = Mathf.Lerp(MaxSpeed * Throttle, MaxSpeed*3f, (AngleX - 0.5f) * 2f);
        else DesiredSpeed = Mathf.Lerp(-MaxSpeed, MaxSpeed * Throttle, AngleX * 2f);
        Speed = Mathf.MoveTowards (Speed, DesiredSpeed, MaxSpeed / 500f);
		// Set speed

		// Diving too fast
		if(Speed / MaxSpeed >= 1.75f) Throttle = 0f;
		// Diving too fast

		// Move forward
		FlyingTowards = Vector3.Lerp(FlyingTowards, this.transform.forward, Mathf.Clamp((Speed / MaxSpeed), 0f, 1f) * 0.03f);
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
		AngleZ *= AngleDecreaser;
		CF *= AngleDecreaser;
		// Chandelle
		// Flaps and rudder
		float RotationalSpeed = Speed / MaxSpeed;
		this.transform.Rotate(ElevatorFlapsRudder * (RotationSpeed * RotationalSpeed)); // Turning speed
		this.transform.Rotate(new Vector3 (0f, AngleZ / 2f * RotationSpeed, 0f));
		this.transform.eulerAngles += new Vector3 ((CF / 2f) * (RotationSpeed * RotationalSpeed), 0f, 0f);

		PointThere.transform.position = this.transform.position;

		float TurnX = (this.transform.forward * 100f).y - (PointThere.forward*100f).y;
		float TurnY = Vector3.SignedAngle(this.transform.forward, PointThere.transform.forward*20f, Vector3.up);

		ElevatorFlapsRudder = Vector3.Lerp(
				ElevatorFlapsRudder,
				Vector3.Lerp(
					new Vector3(
						Mathf.Clamp(TurnX / 10f, -1f, 1f),
						Mathf.Clamp(TurnY / 10f, -1f, 1f),
						Mathf.Clamp(AngleZ * 10f, -1f, 1f)
					)
				,
					new Vector3(
						Mathf.Clamp((AngleX - 0.4f) * -3f + TurnX, -1f, 1f),
						0,//Mathf.Clamp(AngleX, -0.3f, 0.3f),
						Mathf.Clamp(Mathf.Lerp( TurnY / -3f , 0f, Mathf.Abs(AngleZ*2f) ), -1f, 1f)
					)
				,
				Mathf.Clamp((Mathf.Abs(TurnY)-10f) / 30f, 0f, 1f)
				),
				0.1f
			);
		
		// Flaps and rudder

		// Lift force
		this.transform.position += Vector3.down*0.5f + Vector3.Lerp(this.transform.up, Vector3.down, Stalling / 3f) * Mathf.Lerp(0.25f, 0.75f, Speed/MaxSpeed);
		// Lift force

		// Stalling
		if(Speed <= (MaxSpeed / 3.9f))
			Stalling = Mathf.Clamp(Stalling + 0.02f, 0f, 5f);

		if(Stalling > 0f){
            Stalling -= 0.01f;
			this.transform.position += Vector3.up * -0.1f; 
			this.transform.eulerAngles += new Vector3(Mathf.Lerp(Stalling*2f, 0f, (AngleX-0.5f)*2f), 0f, 0f);
		}
		// Stalling

	}

    void Plane() {

        // Flight Mechanics
        if (Paintballed <= 0f){
			PointofInterest = new Vector3(
				PointofInterest.x,
				Mathf.Clamp(PointofInterest.y, this.transform.position.y-2000f, this.transform.position.y+10f),
				PointofInterest.z
			);
			PointThere.LookAt(PointofInterest);
			Mechanics(1f);
		}
        // Flight Mechanics

        // Think
        // ChooseOption
        string WhichOne;
        // Check for collision

        Ray CheckGround = new(this.transform.position, this.transform.forward);
        if (Physics.Raycast(CheckGround, out RaycastHit CheckGroundHit, Speed * 10f)){
            if (CheckGroundHit.collider != null && CheckGroundHit.collider.GetComponent<EnemyVesselScript>() == null && CheckGroundHit.collider.tag != "Cloud" && !(CheckGroundHit.collider.tag == "Player" && Time.timeSinceLevelLoad % 4f < 2f) ){
                PullUpMultiplier = 3f;
            }
        }
		if(this.transform.position.y < 100f) PullUpMultiplier = 3f;
        // Check for collision
        if (PullUpMultiplier > 0f) {
			WhichOne = "PullUp";
            PullUpMultiplier -= 0.1f;
		} else if (PlayerSeen != null) {
			if (Vector3.Distance (this.transform.position, PlayerSeen.transform.position) < DetectionDistance) {
				WhichOne = "Dogfight";
			} else {
				WhichOne = "Patrol";
			}
		} else {
			WhichOne = "Patrol";
		}
		// ChooseOption
		// Set gun distance
		float GunDistance = 300f;
		if(TypeofVessel == "Messerschmitt Me 262") GunDistance = 1000f;
		// Set gun distance
		if (WhichOne == "Patrol") {
			if (Change <= 0f || Vector3.Distance(this.transform.position, PointofInterest) < 100f) {
				Change = Random.Range (10f, 30f);
				PointofInterest = Origin + new Vector3 (Random.Range (-1000f, 1000f), this.transform.position.y + Random.Range (-10f, 10f), Random.Range (-1000f, 1000f));
			} else {
				Change -= 0.01f;
			}
		} else if (WhichOne == "PullUp"){
            PointofInterest = this.transform.position + this.transform.forward*10f + Vector3.up;
		} else if(WhichOne == "Dogfight"){
			PointofInterest = Lead(PlayerSeen);
			if (Vector3.Distance (this.transform.position, PlayerSeen.transform.position) < GunDistance && Quaternion.Angle (this.transform.rotation, Quaternion.LookRotation (PointofInterest - this.transform.position)) < 5f) {
				if (GunCooldown <= 0f && Paintballed <= 0f) {
					if (TypeofVessel == "Messerschmitt") {
						GunCooldown = 0.1f;
						Shoot("Vickers", PointofInterest, this.transform);
					} else if (TypeofVessel == "Messerschmitt K4") {
						GunCooldown = 0.075f;
						Shoot("M2 Browning", PointofInterest, this.transform);
					} else if (TypeofVessel == "Messerschmitt 110") {
						GunCooldown = 0.2f;
						Shoot("Cannon", PointofInterest, this.transform);
					} else if (TypeofVessel == "Messerschmitt Me 262") {
						GunCooldown = 0.05f;
						Shoot("Jet Gun", PointofInterest, this.transform);
					}
				}
			}
			if (TypeofVessel == "Messerschmitt 110") {
				if (Vector3.Distance (this.transform.position, PlayerSeen.transform.position) < 200f && Quaternion.Angle (this.transform.rotation, Quaternion.LookRotation (PointofInterest - this.transform.position)) > 90f) {
					if (GunCooldown <= 0f) {
						GunCooldown = 0.1f;
						Shoot("Vickers", Lead(PlayerSeen), this.transform);
					}
				}
			}
		}
		// Think

	}

	void Shoot(string What, Vector3 Where, Transform Slimend){
		GameObject BulletA = Instantiate (Bullet) as GameObject;
		BulletA.transform.position = Slimend.position;
		BulletA.transform.LookAt (Where);
		BulletA.GetComponent<ProjectileScript> ().TypeofGun = What;
		BulletA.GetComponent<ProjectileScript> ().WhoShot = this.gameObject;
		BulletA.GetComponent<ProjectileScript> ().GunFirePos = Slimend;
	}

	Vector3 Lead(GameObject Target){
		Vector3[] Pos = new Vector3[]{};
		float[] Speeds = new float[]{};

		if(Target.tag == "Player"){
			Pos = new Vector3[]{Target.transform.position, (Target.transform.position - Target.GetComponent<PlayerScript>().PrevPos).normalized};//Target.GetComponent<PlayerScript>().FlyingTowards};
			Speeds = new float[]{Target.GetComponent<PlayerScript>().Speed, GunSpeed};
		}

		float TimeNeeded = Vector3.Distance(this.transform.position, Pos[0]) / Speeds[1];
		return Pos[0] + Pos[1]*TimeNeeded*Speeds[0];
	}

	void AAGun(){

        if (Paintballed <= 0f) {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(new Vector3(PointofInterest.x, this.transform.position.y, PointofInterest.z) - this.transform.position), 0.1f);
        }
		Vector3 Recoil = new Vector3 (Random.Range(Vector3.Distance(this.transform.position, PointofInterest) / -100f, Vector3.Distance(this.transform.position, PointofInterest) / 100f),
			Random.Range(Vector3.Distance(this.transform.position, PointofInterest) / -100f, Vector3.Distance(this.transform.position, PointofInterest) / 100f),
			Random.Range(Vector3.Distance(this.transform.position, PointofInterest) / -100f, Vector3.Distance(this.transform.position, PointofInterest) / 100f));
		Recoil = new Vector3 (Recoil.x, Mathf.Clamp(Recoil.y, 0f, 750f), Recoil.z);
		Model.transform.GetChild (0).LookAt (PointofInterest + Recoil);
		Model.transform.GetChild (1).LookAt (PointofInterest + Recoil);
		// Fire at player
		if(PlayerSeen != null){
			if(Vector3.Distance(this.transform.position, PlayerSeen.transform.position) < 750f){
				PointofInterest = PlayerSeen.transform.position;
				if (GunCooldown <= 0f && Paintballed <= 0f) {
					GunCooldown = Mathf.Lerp(5f, 0.5f, Power);
					int PickGun = Random.Range (0, 1);
					Shoot("Flak", Lead(PlayerSeen) + Vector3.Lerp(new Vector3(Random.Range(-10f, 10f), Random.Range(0f, 5f), Random.Range(-10f, 10f)), Vector3.zero, Power), Model.transform.GetChild (PickGun));
				}
			}
		}
		// Fire at player

	}

	void Balloon(){

		if (Paintballed <= 0f) {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(new Vector3(PointofInterest.x, this.transform.position.y, PointofInterest.z) - this.transform.position), 0.1f);
        }
		Vector3 Recoil = new Vector3 (Random.Range(Vector3.Distance(this.transform.position, PointofInterest) / -100f, Vector3.Distance(this.transform.position, PointofInterest) / 100f),
			Random.Range(Vector3.Distance(this.transform.position, PointofInterest) / -100f, Vector3.Distance(this.transform.position, PointofInterest) / 100f),
			Random.Range(Vector3.Distance(this.transform.position, PointofInterest) / -100f, Vector3.Distance(this.transform.position, PointofInterest) / 100f));
		Recoil = new Vector3 (Recoil.x, Mathf.Clamp(Recoil.y, 0f, 750f), Recoil.z);
		Model.transform.GetChild (0).LookAt (PointofInterest + Recoil);
		// Fire at player
		if(PlayerSeen != null){
			if(Vector3.Distance(this.transform.position, PlayerSeen.transform.position) < 750f){
				PointofInterest = PlayerSeen.transform.position;
				if (GunCooldown <= 0f && Paintballed <= 0f) {
					GunCooldown = Mathf.Lerp(5f, 0.5f, Power);
					Shoot("Flak", Lead(PlayerSeen) + Vector3.Lerp(new Vector3(Random.Range(-10f, 10f), Random.Range(0f, 5f), Random.Range(-10f, 10f)), Vector3.zero, Power), Model.transform.GetChild (0));
				}
			}
		}
		// Fire at player

	}

	void Dead(){

		if(IsDead == false){
			IsDead = true;
			GameObject Boom = Instantiate (Special) as GameObject;
			Boom.GetComponent<SpecialScript> ().TypeofSpecial = "Explosion";
			Boom.GetComponent<SpecialScript> ().ExplosionPower = 10f;
			Boom.GetComponent<SpecialScript> ().ExplosionRadius = 4f;
			Boom.transform.position = this.transform.position;
			if(HitByAPlayer > 0f && PlayerActuall != null){
				string[] Message = {"", ""};
				int[] Scores = new int[]{0, 0};

				switch(TypeofVessel){
					case "Messerschmitt":
						Message = new string[]{"Messerschmitt Down!", "Messerschmitt Zestrzelony!"};
						Scores = new int[]{5, 250};
						break;
					case "Messerschmitt K4":
						Message = new string[]{"Messerschmitt K4 Down!", "Messerschmitt K4 Zestrzelony!"};
						Scores = new int[]{10, 500};
						break;
					case "AA Gun":
						Message = new string[]{"AA Gun Destroyed!", "Broń Przeciwlotnicza Zniszczona!"};
						Scores = new int[]{30, 1500};
						break;
					case "Balloon":
						Message = new string[]{"Balloon Down!", "Balon Zestrzelony!"};
						Scores = new int[]{20, 1000};
						break;
					case "Messerschmitt 110":
						Message = new string[]{"Messerschmitt 110 Down!", "Messerschmitt 110 Zestrzelony!"};
						Scores = new int[]{30, 1500};
						break;
					case "Messerschmitt Me 262":
						Message = new string[]{"Messerschmitt Me 262 Down!", "Messerschmitt Me 262 Zestrzelony!"};
						Scores = new int[]{20, 1000};
						break;
				}

				PlayerActuall.GetComponent<PlayerScript> ().MainCanvas.GetComponent<CanvasScript> ().SetInfoText (Message[0], Message[1], new Color32(225, 225, 225, 255), 1.5f);
				GS.GainScore (Scores[0], Scores[1], "");
			}

			GameObject ByeBye = Instantiate(PlaneDead) as GameObject;
			ByeBye.transform.position = this.transform.position;
			ByeBye.transform.rotation = this.transform.rotation;
			ByeBye.GetComponent<PlaneDead>().PreviousSpeed = Speed;
			ByeBye.GetComponent<PlaneDead>().PreviousRotation = ElevatorFlapsRudder;
			Model.transform.SetParent(ByeBye.transform);
			ByeBye.GetComponent<PlaneDead>().PlaneModel = Model;
			Destroy(this.gameObject);

		}

	}

    void OnTriggerEnter(Collider Col){

		if(TypeofVessel == "Messerschmitt" || TypeofVessel == "Messerschmitt K4"){
			if(Col.tag == "Terrain"){
				Health = 0f;
			}
		}

		if(Col.name == "Explosion"){
			Health -= Col.transform.parent.GetComponent<SpecialScript> ().ExplosionPower;
			if(Col.transform.parent.GetComponent<SpecialScript> ().CausedByPlayer){
				HitByAPlayer = 5f;
			}
		}

	}

}

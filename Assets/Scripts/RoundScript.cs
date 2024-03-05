using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class RoundScript : MonoBehaviour {

	// References
	public GameScript GS;
    ItemClasses IC;
	public PlayerScript player;
	public GameObject Enemy;
	public GameObject Home;
	public GameObject Portal;
    public GameObject Clouds;
    public ParticleSystem Snow;
    public GameObject DeadPlaneObj;
    public GameObject Special;
	// References

	// Main Variables
	public string State = "Deliver Presents";
	public int Level = 1;
    public float SnowIntensity = 1f;
    public int MapSize = 0;
    public float LevelSeed = 99999999;
	// Main Variables

    // Results variables
    public float accBonus, morBonus, hpBonus, EndGrade = 0f;
    public int TempScore = 0;
    public int TempMooney = 0;
    // Results variables

	// Use this for initialization
	void Start () {

		GS = GameObject.Find ("GameScript").GetComponent<GameScript>();
        IC = GS.GetComponent<ItemClasses>();

		// Find Player
		player = GameObject.Find("MainPlane").GetComponent<PlayerScript>();
		// Find Player

		// Set Level
        LevelSeed = (int)Random.Range(1, 1000000);
        SetUp(GS.GameMode);
        // Set Level
		
	}

    void SetUp(int How){
        switch(How){
            case 1: // Endless
                Level = GS.Level;
                GameObject.FindObjectOfType<LandScript>().spawnLand("Plains", 20, LevelSeed, (int)Mathf.Lerp(0f, 3.9f, LevelSeed%100f /100f));
                MapSize = (int)Mathf.Lerp(5000f, 15000f, (float)Level/20f);
                SetUpPawns("Delivery", Level, LevelSeed);
                player.transform.position = new Vector3(0f, 500f, MapSize/-3f);
                break;
            case 2: // Skirmish
                Level = GS.Level;
                GameObject.FindObjectOfType<LandScript>().spawnLand("Plains", 20, Random.Range(1f, 999999f), (int)Random.Range(0f, 3.9f));
                MapSize = (int)Random.Range(5000f, 15000f);
                SetUpPawns("Delivery", (int)Random.Range(5f, 20.9f), Random.Range(1f, 999999f));
                player.transform.position = new Vector3(0f, 500f, MapSize/-3f);
                break;
        }
    }

    void SetUpPawns(string Mode, int LevelState, float Seed){

        switch (Mode){
            case "Delivery":
                // Spawn Sequence
                List<Transform> placedHomes = new();
                for (int Begin = 0; Begin <= 6; Begin ++) {
                    if (Begin == 0) {
                        // Spawn homes
                        Vector3 PrevHome = new Vector3(0f, 100f, 0f);
                        int MaxHouses = 1 + (LevelState / 4);
                        for (int Spawn = MaxHouses; Spawn > 0; Spawn--){
                            GameObject HomeA = Instantiate(Home) as GameObject;
                            HomeA.GetComponent<HomeScript>().HomeIndex = Spawn;
                            float Angle = Mathf.Lerp(0f, 360f, Mathf.PerlinNoise(PrevHome.x/0.33f + Seed, PrevHome.z/0.33f + Seed));
                            HomeA.transform.position = PrevHome;
                            HomeA.transform.position = new Vector3(
                                Mathf.Clamp( HomeA.transform.position.x, MapSize/-2.5f, MapSize/2.5f ),
                                HomeA.transform.position.y,
                                Mathf.Clamp( HomeA.transform.position.z, MapSize/-2.5f, MapSize/2.5f )
                            );
                            PrevHome = HomeA.transform.position + new Vector3(Mathf.Sin(Angle) * MapSize/MaxHouses, 0f, Mathf.Cos(Angle) * MapSize/MaxHouses);
                            placedHomes.Add(HomeA.transform);
                        }
                        // Spawn homes
                    } else if (Begin == 2){
                        // Spawn planes
                        List<string> PlanesToSpawn = new();
                        for (int Spawn = (int)(LevelState / 2 * 1.5f); Spawn > 0; Spawn--)PlanesToSpawn.Add("Messerschmitt");
                        for (int Spawn = LevelState / 5; Spawn > 0; Spawn--)PlanesToSpawn.Add("Messerschmitt K4");
                        for (int Spawn = LevelState / 10; Spawn > 0; Spawn--)PlanesToSpawn.Add("Messerschmitt 110");
                        for (int Spawn = LevelState / 20; Spawn > 0; Spawn--)PlanesToSpawn.Add("Messerschmitt Me 262");
                        for(int sp = 0; sp < PlanesToSpawn.ToArray().Length; sp++){
                            string SpawnThePlanes = PlanesToSpawn[sp];
                            int PickHouse = sp % placedHomes.ToArray().Length;
                            GameObject EnemyPlane = Instantiate(Enemy) as GameObject;
                            EnemyPlane.GetComponent<EnemyVesselScript>().TypeofVessel = SpawnThePlanes;
                            EnemyPlane.GetComponent<EnemyVesselScript>().Power = LevelState/30f;
                            EnemyPlane.GetComponent<EnemyVesselScript>().GuardedTarget = placedHomes[PickHouse].gameObject;
                            EnemyPlane.transform.position = placedHomes[PickHouse].position + new Vector3(Random.Range(-100f, 100f), Random.Range(200f, 1000f), Random.Range(-100f, 100f));
                        }
                         // Spawn planes
                    } else if (Begin == 4){
                        // Spawn aa guns
                        for (int Spawn = LevelState / 5; Spawn > 0; Spawn--) {
                            GameObject EnemyPlane = Instantiate(Enemy) as GameObject;
                            EnemyPlane.GetComponent<EnemyVesselScript>().TypeofVessel = "AA Gun";
                            EnemyPlane.GetComponent<EnemyVesselScript>().Power = (float)LevelState/30f;
                            EnemyPlane.transform.position = placedHomes[Spawn % placedHomes.ToArray().Length].position + new Vector3(Random.Range(-500f, 500f), 100f, Random.Range(-500f, 500f));
                        }
                        // Spawn aa guns
                    } else if (Begin == 6){
                        // Spawn aa guns
                        for (int Spawn = LevelState / 10; Spawn > 0; Spawn--){
                            GameObject EnemyPlane = Instantiate(Enemy) as GameObject;
                            EnemyPlane.GetComponent<EnemyVesselScript>().TypeofVessel = "Balloon";
                            EnemyPlane.GetComponent<EnemyVesselScript>().Power = (float)LevelState/30f;
                            EnemyPlane.transform.position = placedHomes[Spawn % placedHomes.ToArray().Length].position + new Vector3(Random.Range(-500f, 500f), Random.Range(100f, 1000f), Random.Range(-500f, 500f));
                        }
                        // Spawn aa guns
                    }
                }
                State = "Deliver Presents";
                // Spawn Sequence
                break;
        }
        for(int ee = GameObject.FindGameObjectsWithTag("Foe").Length; ee > 0; ee--) GS.statModify("Enemies encountered", 1);

        // Clouds
        SnowIntensity = Random.Range(3f, 10f);
        for (int SpawnClouds = Random.Range(5, 15); SpawnClouds > 0; SpawnClouds --) {
            GameObject SCloud = Instantiate(Clouds.transform.GetChild(0).gameObject) as GameObject;
            var main = SCloud.GetComponent<ParticleSystem>().main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(RenderSettings.fogColor.r / 1.5f, RenderSettings.fogColor.g / 1.5f, RenderSettings.fogColor.b / 1.5f, 0.5f), new Color(RenderSettings.fogColor.r, RenderSettings.fogColor.g, RenderSettings.fogColor.b, 0.75f));
            SCloud.transform.position = new Vector3(Random.Range(MapSize/-2f, MapSize/2f), Random.Range(100f, 3000f), Random.Range(MapSize/-2f, MapSize/2f));
            SCloud.transform.eulerAngles = new Vector3(Random.Range(-10f, 10f), Random.Range(0f, 360f), Random.Range(-10f, 10f));
            SCloud.transform.localScale = new Vector3(Random.Range(1000f, 3000f), Random.Range(100f, 200f), Random.Range(1000f, 3000f));
        }
        // Clouds

    }

	void FixedUpdate () {

        ParticleSystem myParticleSystem;
        ParticleSystem.EmissionModule emissionModule;
        myParticleSystem = Snow.GetComponent<ParticleSystem>();
        emissionModule = myParticleSystem.emission;
        emissionModule.rateOverTime = (SnowIntensity * 100f);

		if (State == "Deliver Presents" && GameObject.FindGameObjectsWithTag ("HomeUnchecked").Length <= 0f && player != null) {
			State = "SuccessDP";
            foreach(GameObject Foe in GameObject.FindGameObjectsWithTag("Foe")) 
                if(!Foe.GetComponent<EnemyVesselScript>().Scarred) Foe.GetComponent<EnemyVesselScript>().Dead(true);
            GS.statModify("Summarize", 2);
            accBonus = GS.ReceiveStat("Presents accuracy", true);
            morBonus = Mathf.Abs( (GS.ReceiveStat("Morality ratio", true) + GS.ReceiveStat("Morality ratio", false)) /2f);
            hpBonus = player.Health / player.MaxHealth * 100f;
            EndGrade = (accBonus + morBonus + hpBonus) / 3f;
            TempMooney = (int)(Level * 250f * (EndGrade/100f));
            DeadPlane(new Transform[]{player.transform, player.CurrentPlane.transform}, player.Speed, player.ElevatorFlapsRudder, "Leave");
            Destroy(player.gameObject);
        } else if(State == "Quit1"){
            State = "Quit2";
			GS.LoadLevel("MainMenu", "CampaignMessage");
		} else if(State == "Left1"){
            State = "Left2";
            GS.statModify("Summarize", 0);
			GS.Level += 1;
            GS.CurrentScore += TempScore;
            GS.Mooney += TempMooney;
            TempScore = 0;
            TempMooney = 0;
			GS.LoadLevel("MainMenu", "CampaignMessage");
        }

	}

    public GameObject DeadPlane(Transform[] Model, float Speed, Vector3 Rotation, string Spec = ""){

        GameObject Corpse = Instantiate(DeadPlaneObj) as GameObject;
        Corpse.transform.position = Model[0].position;
        Corpse.transform.rotation = Model[0].rotation;

        if(Model[0].name == "MainPlane"){
            Corpse.GetComponent<PlaneDead>().IsGameOver = false;
            Corpse.GetComponent<PlaneDead>().isMine = true;
        } else {
            Corpse.GetComponent<PlaneDead>().IsGameOver = false;
            Corpse.GetComponent<PlaneDead>().isMine = false;
        }
        
        Corpse.GetComponent<PlaneDead>().PreviousSpeed = Speed;
        Corpse.GetComponent<PlaneDead>().PreviousRotation = Rotation;
		Model[1].SetParent(Corpse.transform);
		Corpse.GetComponent<PlaneDead>().PlaneModel = Model[1].gameObject;
        Corpse.GetComponent<PlaneDead>().Spec = Spec;

        if(Spec != "Leave"){
            GameObject Boom = Instantiate(Special) as GameObject;
            Boom.GetComponent<SpecialScript>().TypeofSpecial = "Explosion";
            Boom.GetComponent<SpecialScript>().ExplosionPower = 1f;
            Boom.GetComponent<SpecialScript>().ExplosionRadius = 4f;
            Boom.transform.position = Model[0].position;
        }

        return Corpse;

    }

}

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {

	public string TypeofGun;
	public int ICtype = 0; // 0 data from ItemClass.Gun - 1 data from ItemClass.Present - 2 custom data
	public GameObject WhoShot;
	public Transform GunFirePos;
	Vector3 StartPos;
	GameScript GS;
	ItemClasses IC;
	public GameObject PreviousPos;
	float Damage = 1f;
	float Speed = 1f;
	float Range = 100f;
	public float PreviusSpeed = 0f;
	public bool Hit = false;
	GameObject HomingMissileTarget;
	public float PresentRange = 0f;
    float LaserLifetime = 10f;

	public Color PresentColor1;
	public Color PresentColor2;

	// References
	public GameObject Effect;
	public GameObject Special;
	public GameObject ChostBullet;

	public GameObject Bullet, Present, HomingMissile, Laser, Brick, Paintball, Rocket;
	public ParticleSystem mSmoke;
	public GameObject TheGunFire;
	public AudioClip[] GunFires;

    // References

    // Use this for initialization
    void Start () {

		GS = GameObject.Find("GameScript").GetComponent<GameScript>();
		IC = GS.GetComponent<ItemClasses>();

		PreviousPos.transform.parent = null;
		PreviousPos.transform.position = this.transform.position - (this.transform.forward * 1f);
		PreviousPos.transform.LookAt(this.transform.position);
		StartPos = this.transform.position;

		float[] BulletVars = new float[]{0f, 0f, 0f, 1f};
		string GunFireSound = "";
		bool Smoke = true;

		ParticleSystem.MainModule Bmain = Bullet.GetComponent<ParticleSystem>().main;
		ParticleSystem.MainModule Lmain = Laser.GetComponent<ParticleSystem>().main;
		switch(TypeofGun){
		case "Vickers":
			ChostBullet = Bullet; GunFireSound = "Vickers";
			Bmain.startColor = new Color(1f, 0.5f, 0f, 0.05f);
			ICtype = 0;
			break;
		case "M2 Browning":
			ChostBullet = Bullet; GunFireSound = "M2 Browning";
			Bmain.startColor = new Color(1f, 0.75f, 0.5f, 0.1f);
			ICtype = 0;
			break;
		case "M3 Browning":
			ChostBullet = Bullet; GunFireSound = "M2 Browning";
			Bmain.startColor = new Color(1f, 0.75f, 0.5f, 0.05f);
			ICtype = 0;
			break;
		case "Cannon":
			ChostBullet = Bullet; GunFireSound = "Cannon";
			Bmain.startColor = new Color(1f, 0f, 0f, 0.05f);
			ICtype = 0;
			break;
		case "Flammable":
			ChostBullet = Bullet; GunFireSound = "Flammable";
			Bmain.startColor = new Color(1f, 0.5f, 0f, 0.1f);
			ICtype = 0;
			break;
		case "Mugger Missiles":
			ChostBullet = Bullet; GunFireSound = "Flammable";
			Bmain.startColor = new Color(0.5f, 1f, 0f, 0.1f);
			ICtype = 0;
			break;
		case "Flak":
			ChostBullet = Bullet; GunFireSound = "Flak";
			Bmain.startColor = new Color(1f, 0.75f, 0.5f, 0.05f);
			ICtype = 0;
			break;
		case "Jet Gun":
			ChostBullet = Bullet; GunFireSound = "JetGun";
			Bmain.startColor = new Color(1f, 0.75f, 0.5f, 0.05f);
			ICtype = 0;
			break;
        case "Paintball":
			ChostBullet = Paintball; GunFireSound = "Paintball"; Smoke = false;
			ICtype = 0;
			break;
        case "Rocket":
			ChostBullet = Rocket; GunFireSound = "MissileLaunch";
			ICtype = 0;
			break;
        case "Laser":
            ChostBullet = Laser; GunFireSound = "Laser"; Smoke = false;
			Lmain.startColor = new Color(1f, 0.05f, 0.05f, 1f);
			ICtype = 0;
			break;
        case "Blue Laser":
            ChostBullet = Laser; GunFireSound = "Laser"; Smoke = false;
			Lmain.startColor = new Color(0f, 0.1f, 1f, 1f);
			ICtype = 0;
			break;
        case "Slingshot": case "Present Cannon": case "Sniper Rifle":
			ChostBullet = Present; GunFireSound = "PresentCannon";
			ICtype = 1;
			break;
		case "Homing Missile":
			ChostBullet = HomingMissile; GunFireSound = "MissileLaunch";
			BulletVars = new float[]{Random.Range(50f, 100f), PreviusSpeed, 1000f, 0f};
			ICtype = 2;
			break;
        case "Brick":
			ChostBullet = Brick; GunFireSound = "Cannon";
			BulletVars = new float[]{1000f, PreviusSpeed*2f, 5000f, 0f};
			ICtype = 2;
			break;
        }

		switch(ICtype){
			case 0:
				ItemClasses.Gun rGun = IC.ReceiveGunType(TypeofGun);
				BulletVars = new float[]{Random.Range(rGun.Damage[0], rGun.Damage[1]), rGun.Speed, Random.Range(rGun.Range[0], rGun.Range[1]), rGun.Spread};
				break;
			case 1:
				ItemClasses.Present rPresent = IC.ReceivePresentType(TypeofGun);
				BulletVars = new float[]{0f, rPresent.Speed, rPresent.Range, 0f};
				break;
		}

		Damage = BulletVars[0]; Speed = BulletVars[1]; Range = BulletVars[2];
		this.transform.Rotate(new Vector3(Random.Range(-BulletVars[3], BulletVars[3]), Random.Range(-BulletVars[3], BulletVars[3]), 0f));

		ChostBullet.gameObject.SetActive (true);

		for(int fs = 0; fs <= GunFires.Length; fs++){
			if(fs == GunFires.Length) Debug.LogError("No gunfire sound of name " + GunFireSound + " found!");
			else if (GunFires[fs].name == GunFireSound){
				TheGunFire.GetComponent<AudioSource>().clip = GunFires[fs];
				TheGunFire.transform.parent = GunFirePos.transform;
				TheGunFire.transform.position = GunFirePos.position;
				TheGunFire.GetComponent<AudioSource>().Play();
				break;
			}
		}

        if (TypeofGun == "Paintball") {
			ParticleSystem.MainModule PC = Paintball.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            PC.startColor = PresentColor1;
        }

		ParticleSystem.MainModule mmSmoke = mSmoke.main;
		if(Smoke){
			mmSmoke.startColor = new ParticleSystem.MinMaxGradient(new Color(RenderSettings.fogColor.r / 1.5f, RenderSettings.fogColor.g / 1.5f, RenderSettings.fogColor.b / 1.5f, 0.5f), new Color(RenderSettings.fogColor.r, RenderSettings.fogColor.g, RenderSettings.fogColor.b, 0.1f));
			if(QualitySettings.GetQualityLevel() == 3) mmSmoke.startLifetime = 5f;
			else if(QualitySettings.GetQualityLevel() == 2) mmSmoke.startLifetime = 1f;
			else mSmoke.Stop();
		} else mSmoke.Stop();

    }
	
	// Update is called once per frame
	void FixedUpdate () {

		if(Speed < PreviusSpeed){
			Speed = PreviusSpeed;
		}

        this.transform.position += this.transform.forward * (Speed/90f);

		if(Vector3.Distance(this.transform.position, StartPos) > Range && Hit == false && TypeofGun != "Laser" && TypeofGun != "Blue Laser"){
			Hit = true;
			if(ICtype == 1) GS.statModify("Presents missed", 1);
			if(TypeofGun == "Flak" || TypeofGun == "Homing Missile"){
				GameObject Boom = Instantiate (Special) as GameObject;
				Boom.GetComponent<SpecialScript> ().TypeofSpecial = "Explosion";
				Boom.GetComponent<SpecialScript> ().ExplosionPower = 25f;
				Boom.GetComponent<SpecialScript> ().ExplosionRadius = 5f;
				Boom.transform.position = this.transform.position;
				if (WhoShot != null && WhoShot.tag == "Player")
					Boom.GetComponent<SpecialScript> ().CausedByPlayer = true;
			}
		}

        // Laser Lifetime
        if (TypeofGun == "Laser" || TypeofGun == "Blue Laser") {
            if (LaserLifetime > 0f) {
                LaserLifetime -= 1f;
            } else {
                Hit = true;
            }
        }
        // Laser Lifetime

        // Raycast Detection
        if (TypeofGun == "Laser" || TypeofGun == "Blue Laser") {
            Ray CheckRaya = new(this.transform.position, this.transform.forward * 1f);
            if (Physics.Raycast(CheckRaya, out RaycastHit CheckRayaHit, Range)) 
                if (CheckRayaHit.collider != null)
                    BulletHit(CheckRayaHit.collider, CheckRayaHit.point);
        } else {
            if (PreviousPos != null){
                if (PreviousPos.transform.position != this.transform.position){
                    PreviousPos.transform.LookAt(this.transform.position);
                    Ray CheckRay = new(PreviousPos.transform.position, PreviousPos.transform.forward * 1f);
                    if (Physics.Raycast(CheckRay, out RaycastHit CheckRayHit, Vector3.Distance(PreviousPos.transform.position, this.transform.position + this.transform.forward * 1f)))
                        if (CheckRayHit.collider != null)
                            BulletHit(CheckRayHit.collider, CheckRayHit.point);
                    PreviousPos.transform.position = this.transform.position - this.transform.forward * 1f;
                }
            }
        }
        // Raycast Detection

        // Projectiles special
        if (ICtype == 1) {
            foreach (Material Mat in Present.GetComponent<MeshRenderer>().materials) {
                if (Mat.name == "Material1 (Instance)") {
                    Mat.color = PresentColor1;
                } else if (Mat.name == "Material2 (Instance)") {
                    Mat.color = PresentColor2;
                }
            }
        } else if (TypeofGun == "Homing Missile") {

            if (HomingMissileTarget != null) {
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(new Vector3(HomingMissileTarget.transform.position.x, HomingMissileTarget.transform.position.y, HomingMissileTarget.transform.position.z) - this.transform.position), Speed / (PreviusSpeed*10f));
            } else {
                if (WhoShot != null) {
                    if (WhoShot.GetComponent<PlayerScript>() != null) {
                        float Distance = 1000f;
                        GameObject ChoosenTarget = null;
                        foreach (GameObject Enemy in GameObject.FindGameObjectsWithTag("Foe")) {
                            if (Vector3.Distance(this.transform.position, Enemy.transform.position) < Distance) {
                                Distance = Vector3.Distance(this.transform.position, Enemy.transform.position);
                                ChoosenTarget = Enemy;
                            }
                        }
                        if (ChoosenTarget != null) {
                            HomingMissileTarget = ChoosenTarget;
                        }
                    }
                }
            }
            if (Speed < PreviusSpeed * 10f) {
                Speed = Mathf.MoveTowards(Speed, PreviusSpeed*10f, 0.02f);
            } else {
                if (Hit == false) {
                    Hit = true;
                    GameObject Boom = Instantiate(Special) as GameObject;
                    Boom.GetComponent<SpecialScript>().TypeofSpecial = "Explosion";
                    Boom.GetComponent<SpecialScript>().ExplosionPower = 25f;
                    Boom.GetComponent<SpecialScript>().ExplosionRadius = 5f;
                    Boom.transform.position = this.transform.position;
                    if (WhoShot != null) {
                        if (WhoShot.tag == "Player") {
                            Boom.GetComponent<SpecialScript>().CausedByPlayer = true;
                        }
                    }
                }
            }
        } else if (TypeofGun == "Brick") {
            Brick.transform.Rotate(1f, 1f, 1f);
            this.transform.eulerAngles += new Vector3(0.1f, 0f, 0f);
        }
		// Projectiles special

		// Destroying projectile
		if(Hit == true){
			mSmoke.Stop();
			Destroy (PreviousPos);	
			switch(TypeofGun){
            case "Laser": case "Blue Laser":
                if (Laser.GetComponent<ParticleSystem>().particleCount <= 0f) {
                    Destroy(this.gameObject);
                }
				break;
            case "Brick": case "Slingshot": case "Present Cannon": case "Sniper Rifle":
                Destroy(this.gameObject);
				break;
			case "Homing Missile":
				ChostBullet.transform.GetChild (0).gameObject.SetActive (false);
				if (mSmoke.particleCount <= 0f) {
					Destroy (this.gameObject);
				}
				break;
			default:
				ChostBullet.GetComponent<ParticleSystem> ().Stop ();
				if (mSmoke.particleCount <= 0f) {
					Destroy (this.gameObject);
				}
				break;
			}
        }
		// Destroying projectile

		// Destroying gunfire
		if(TheGunFire && TheGunFire.transform.GetComponent<AudioSource>().isPlaying == false)
			Destroy(TheGunFire);
		// Destroying gunfire
		
	}

    private void OnTriggerEnter(Collider col){
        BulletHit(col, this.transform.position);
    }

    void BulletHit(Collider Col, Vector3 HitPoint){

		if (Hit == false && Col.tag != "Cloud" && Col.tag != "Explosion") {
			bool pDelivered = false;
			bool sHit = false;
			if(TypeofGun == "Flammable"){
				int chance = Random.Range (0, 5);
				if(chance == 0){
					GameObject Boom = Instantiate (Special) as GameObject;
					Boom.GetComponent<SpecialScript> ().TypeofSpecial = "Explosion";
					Boom.GetComponent<SpecialScript> ().ExplosionPower = 25f;
					Boom.GetComponent<SpecialScript> ().ExplosionRadius = 5f;
					Boom.transform.position = HitPoint;
					if (WhoShot != null){
						if(WhoShot.tag == "Player"){
							Boom.GetComponent<SpecialScript> ().CausedByPlayer = true;
						}
					}
				}
			} else if (TypeofGun == "Flak" || TypeofGun == "Homing Missile" || TypeofGun == "Rocket"){
				Hit = true;
				GameObject Boom = Instantiate (Special) as GameObject;
				Boom.GetComponent<SpecialScript> ().TypeofSpecial = "Explosion";
				Boom.GetComponent<SpecialScript> ().ExplosionPower = 25f;
				Boom.GetComponent<SpecialScript> ().ExplosionRadius = 5f;
				Boom.transform.position = HitPoint;
				if (WhoShot != null){
					if(WhoShot.tag == "Player"){
						Boom.GetComponent<SpecialScript> ().CausedByPlayer = true;
					}
				}
			}

			bool FiredByEnemy = false;
			if(WhoShot != null){
				if(WhoShot.GetComponent<EnemyVesselScript>() != null){
					FiredByEnemy = true;
				}
			}

			if(Col.name == "Home"){
				if(ICtype == 1){
					Hit = true;
					Col.transform.parent.GetComponent<HomeScript> ().PresentGot ();
					pDelivered = true;
				}
			} else if(Col.transform.parent != null){
				if (Col.transform.parent.tag == "Terrain" && TypeofGun != "Paintball") {
					GameObject Efect = Instantiate (Effect) as GameObject;
					Efect.GetComponent<EffectScript> ().TypeofEffect = "BullethitGround";
					Efect.transform.position = new Vector3 (HitPoint.x, 0f, HitPoint.z);
					Hit = true;
				} else if (Col.transform.parent.parent != null) {
					if (Col.transform.parent.parent.name == "PlaneModels" && ICtype != 1) {
						if (Col.transform.parent.parent.parent.gameObject != WhoShot) {
							GameObject Efect = Instantiate (Effect) as GameObject;
							Efect.GetComponent<EffectScript> ().TypeofEffect = "BullethitPlane";
							Efect.transform.position = Col.transform.position + new Vector3 (Random.Range (-2f, 2f), Random.Range (-2f, 2f), Random.Range (-2f, 2f));
							Col.transform.parent.parent.parent.GetComponent<PlayerScript> ().Hurt (Damage, 1, 1);
							Hit = true;
							if(Col.transform.parent.parent.gameObject.tag == "Foe") Col.transform.parent.parent.GetComponent<EnemyVesselScript>().Scarred = true;
						}
					} else if (Col.transform.parent.parent.GetComponent<EnemyVesselScript>() != null && FiredByEnemy != true && ICtype != 1) {
						if (Col.transform.parent.parent.gameObject != WhoShot) {
							sHit = true;
							GameObject Efect = Instantiate (Effect) as GameObject;
							Efect.GetComponent<EffectScript> ().TypeofEffect = "BullethitPlane";
							Efect.transform.position = Col.transform.position + new Vector3 (Random.Range (-2f, 2f), Random.Range (-2f, 2f), Random.Range (-2f, 2f));
							Col.transform.parent.parent.GetComponent<EnemyVesselScript> ().Health -= Damage;
                            if(TypeofGun == "Paintball"){
                                Col.transform.parent.parent.GetComponent<EnemyVesselScript>().Paintballed += Random.Range(0.5f, 1f);
							}
							Hit = true;
							if(WhoShot != null){
								if(WhoShot.GetComponent<PlayerScript>() != null){
									Col.transform.parent.parent.GetComponent<EnemyVesselScript> ().HitByAPlayer = 5f;
									if(TypeofGun == "Mugger Missiles"){
										WhoShot.GetComponent<PlayerScript> ().Health += (float)Random.Range (0, WhoShot.GetComponent<PlayerScript> ().MaxHealth/4f);
										WhoShot.GetComponent<PlayerScript> ().Fuel += (float)Random.Range (0, WhoShot.GetComponent<PlayerScript> ().MaxFuel/4f);
										WhoShot.GetComponent<PlayerScript> ().Heat = Mathf.Clamp( WhoShot.GetComponent<PlayerScript> ().Heat - Random.Range (0, 5) , 0f , WhoShot.GetComponent<PlayerScript> ().MaxHeat );
									}
								}
							}
						}
					}
				}
			}

            if (TypeofGun == "Paintball") {
                GameObject Efect = Instantiate(Effect) as GameObject;
                Efect.GetComponent<EffectScript>().TypeofEffect = "PaintballHit";
				ParticleSystem.MainModule EC = Efect.GetComponent<EffectScript>().PaintballHit.GetComponent<ParticleSystem>().main;
                EC.startColor = new Color(PresentColor1.r, PresentColor1.g, PresentColor1.b, 0.25f);
                Efect.transform.position = HitPoint;
            } else if (ICtype == 1){
				if(pDelivered) GS.statModify("Presents delivered", 1);
				else {
					GameObject.Find("MainCanvas").GetComponent<CanvasScript>().Message(GS.SetText("Present missed", "Prezent spudłował"), Color.red, new float[]{3f, 1f});
					GS.statModify("Presents missed", 1);
				}
			}

			if(WhoShot && WhoShot.tag == "Player" && sHit) if (sHit) GS.statModify("Shots hit", 1);

		}

	}

}

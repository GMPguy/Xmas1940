using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.Windows.Speech;

public class PlaneDead : MonoBehaviour {

	GameScript GS;
	public bool isMine = false;
	public GameObject PlaneModel;
    public GameObject Parachuter;
	public Vector3 PreviousRotation;
	public float PreviousSpeed = 0f;
	float decay = 1f;
	bool Fell = false;
	public GameObject Effect;
	public string Spec = "";
	float GoBackToMenu = 5f;
    public bool IsGameOver = false;
    int FreeTheParachuter = 50;
	public ParticleSystem Fire;
	public ParticleSystem Smoke;

	// Use this for initialization
	void Start () {

		GS = GameObject.Find("GameScript").GetComponent<GameScript>();

        if (isMine && IsGameOver == false) {
            GS.SetGameOptions("Save", "SF" + GS.saveIndex);
        } else if (isMine && IsGameOver){
			GS.SetGameOptions("Erase", "SF" + GS.saveIndex);
		} else if (!isMine){
			int[] maxDeads = new int[]{1, 4, 11, 101};
			if(GameObject.FindGameObjectsWithTag("DeadPlane").Length >= maxDeads[QualitySettings.GetQualityLevel()]) 
				Destroy(GameObject.FindGameObjectsWithTag("DeadPlane")[0]);
		}

		foreach(Transform cleanP in PlaneModel.transform){
			if(cleanP.name == "Propeller" || cleanP.name == "Basic Propeller" || cleanP.name == "Double Propeller" || cleanP.name == "Jet Engine" || cleanP.name == "Double Jet Engine" || cleanP.name == "Magic Reindeer Dust"){
				if(Spec == "Leave") cleanP.GetComponent<AudioSource>().Stop();
				else Destroy(cleanP.gameObject);
			}
		}

		if(Spec == "Leave" || QualitySettings.GetQualityLevel() <= 1){
			Fire.Stop(); Smoke.Stop();
		}

		if(Spec == "Leave"){
			PreviousSpeed = Mathf.Clamp(PreviousSpeed, 100, Mathf.Infinity);
		}

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if ((isMine && IsGameOver == false && Spec != "Leave") || (!isMine)) {
            if (FreeTheParachuter > 1 && this.transform.position.y > 1f) {
                FreeTheParachuter -= 1;
            } else if(FreeTheParachuter == 1) {
                Parachuter.transform.parent = null;
				Vector3 paraHere = this.transform.position;
				paraHere.y = Mathf.Clamp(paraHere.y, 10f, Mathf.Infinity);
                Parachuter.transform.position = paraHere;
                Parachuter.GetComponent<AudioSource>().Play();
                FreeTheParachuter = 0;
            } else {
                if (Parachuter.transform.position.y > 0f) {
                    Parachuter.transform.localScale = Vector3.Slerp(Parachuter.transform.localScale, new Vector3(5f, 5f, 5f), 0.5f);
                    Parachuter.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                    Parachuter.transform.position -= new Vector3(0f, 0.25f, 0);
                } else {
                    Parachuter.transform.localScale = Vector3.Slerp(Parachuter.transform.localScale, new Vector3(5f, 5f, 0.5f), 0.5f);
                }
            }
        }

		if(isMine){
			if(Spec == "Leave"){
				PreviousRotation = new Vector3(-1f, PreviousRotation.y, PreviousRotation.z / 10f);
				if (GoBackToMenu > 0f) {
					GoBackToMenu -= 0.01f;
					this.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (GoBackToMenu-3f)/2f);
				}
			} else {
				GameObject.Find ("MainCamera").transform.LookAt (this.transform.position, this.transform.up * 1f);
	        	if (GoBackToMenu > 0f) {
					GoBackToMenu -= 0.01f;
				} else {
					if (IsGameOver == true) {
        	    	    GameObject.Find("GameScript").GetComponent<GameScript>().LoadLevel("MainMenu", "GameOver");
		            } else {
    	        	    GameObject.Find("GameScript").GetComponent<GameScript>().LoadLevel("MainMenu", "CampaignMessage");
    		        }
				}
			}
		}

		if (Fell == false) {
			this.transform.position += this.transform.forward * (PreviousSpeed /90f);
			this.transform.Rotate (PreviousRotation * decay);
			this.transform.eulerAngles += new Vector3 (1f - decay, 0f, 0f);
			this.transform.position += Vector3.down * (5f - (decay * 5f));
			if(Spec != "Leave") decay = Mathf.MoveTowards (decay, 0f, 0.0005f);
			if(this.transform.position.y <= 1f){
				if(Spec == "Leave"){
					Fell = true;
					decay = 5f;
					this.transform.eulerAngles = new Vector3 (0f, this.transform.eulerAngles.x, 0f);
				} else {
					Fell = true;
					decay = 0f;
					for(int Ground = Random.Range(3, 5); Ground > 0; Ground --){
						GameObject Smoke = Instantiate (Effect) as GameObject;
						Smoke.GetComponent<EffectScript> ().TypeofEffect = "BullethitGround";
						Smoke.transform.position = new Vector3 (this.transform.position.x + Random.Range (-1f, 1f), 0f, this.transform.position.z + Random.Range (-1f, 1f));
					}
				}
			}
		} else {
			this.transform.position = new Vector3 (this.transform.position.x, 0f, this.transform.position.z);
			if(decay > 0f){
				this.transform.position += this.transform.forward * PreviousSpeed;
				decay -= 0.02f;
			}
		}
		
	}
}

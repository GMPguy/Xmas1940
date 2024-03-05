using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialScript : MonoBehaviour {

	// References
	public GameObject SpecialObject;
	// References

	// Main Variables
	public string TypeofSpecial = "";
	public bool CausedByPlayer = false;
	// Main Variables

	// Explosion
	public AudioClip[] ExplosionSounds;
	public float ExplosionRadius = 1f;
	public float ExplosionPower = 10f;
	public bool IsLethal = true;
	bool shook = true;
	// Explosion

	// Use this for initialization
	void Start () {

		foreach(Transform ChooseSpecialObject in this.transform){
			if (ChooseSpecialObject.name == TypeofSpecial) {
				SpecialObject = ChooseSpecialObject.gameObject;
				ChooseSpecialObject.gameObject.SetActive (true);
				if(TypeofSpecial == "Explosion"){
					if(Vector3.Distance(this.transform.position, GameObject.Find("MainCamera").transform.position) > 300f) SpecialObject.GetComponent<AudioSource>().clip = ExplosionSounds[1];
					else SpecialObject.GetComponent<AudioSource>().clip = ExplosionSounds[0];
					SpecialObject.GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
					SpecialObject.GetComponent<AudioSource>().Play();
				}
			} else {
				ChooseSpecialObject.gameObject.SetActive (false);
			}
		}
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// Explosion
		if(TypeofSpecial == "Explosion"){
			SpecialObject.transform.localScale = new Vector3 (ExplosionRadius, ExplosionRadius, ExplosionRadius);
			// Shake Camera of a player
			if(GameObject.Find("MainPlane") != null){
				if(Vector3.Distance(this.transform.position, GameObject.Find("MainPlane").transform.position) <= ExplosionRadius * 100f && shook == true){
					float shakePower = 1f - (Vector3.Distance(this.transform.position, GameObject.Find("MainPlane").transform.position) / (ExplosionRadius * 100f));
					GameObject.Find ("MainPlane").GetComponent<PlayerScript> ().ShakePower = shakePower * (ExplosionPower / 10f);
					GameObject.Find ("MainPlane").GetComponent<PlayerScript> ().ShakeDecay = 1f;
					GameObject.Find("MainCanvas").GetComponent<CanvasScript> ().Flash(new Color(1f,0.75f,0.5f, 1f), new float[]{0.5f, 5f-(shakePower*4f)}, -1);
					shook = false;
				}
			}
			// Shake Camera of a player
			// Remove
			if(SpecialObject.transform.GetChild(0).GetComponent<ParticleSystem>().particleCount <= 0f && IsLethal == true){
				IsLethal = false;
			}
			if(SpecialObject.GetComponent<AudioSource>().isPlaying == false && SpecialObject.transform.GetChild(0).GetComponent<ParticleSystem>().particleCount <= 0f && SpecialObject.transform.GetChild(1).GetComponent<ParticleSystem>().particleCount <= 0f && SpecialObject.transform.GetChild(2).GetComponent<ParticleSystem>().particleCount <= 0f){
				Destroy (this.gameObject);
			}
			// Remove
		} else if(TypeofSpecial == "Flares"){
			// Remove
			if(SpecialObject.transform.GetChild(0).GetComponent<ParticleSystem>().particleCount <= 0f && IsLethal == true){
				IsLethal = false;
			}
			if(SpecialObject.GetComponent<AudioSource>().isPlaying == false && SpecialObject.transform.GetChild(0).GetComponent<ParticleSystem>().particleCount <= 0f && SpecialObject.transform.GetChild(1).GetComponent<ParticleSystem>().particleCount <= 0f){
				Destroy (this.gameObject);
			}
			// Remove
		}
		// Explosion
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class LandScript : MonoBehaviour {

	GameScript GS;
	public GameObject plains;

	public string SkyColor = "Blue1";
	public Skybox MainSkybox;

	public void spawnLand(string LandType, int Size, float Seed, int SkyPick){
		GS = GameObject.Find("GameScript").GetComponent<GameScript>();

		// Land
		switch(LandType){
			case "Plains":
				for(int x = 0; x < Size; x++) for(int y = 0; y < Size; y++) {
					GameObject NewLand = Instantiate(plains) as GameObject;
					NewLand.transform.SetParent(this.transform);
					NewLand.transform.localPosition = new Vector3(x*1000, 0, y*1000) - new Vector3(1000f, 0f, 1000f)*(Size/2f);

					float[] ChostLand = new float[]{ 
						Mathf.PerlinNoise( (NewLand.transform.position.x / 0.3333f) + Seed, (NewLand.transform.position.z / 0.3333f) + Seed),
						Mathf.PerlinNoise( (NewLand.transform.position.z / 0.3333f) + Seed, (NewLand.transform.position.x / 0.3333f) + Seed)
					};

					NewLand.transform.GetChild((int)(ChostLand[0]*2.9f)).gameObject.SetActive(true); // Something here fuck up - fix it later
					GameObject Land = NewLand.transform.GetChild((int)(ChostLand[0]*2.9f)).gameObject;
					NewLand.transform.Rotate(Vector3.up * ((int)(ChostLand[1]*3.9f) * 90f));

					foreach(Material Mat in Land.GetComponent<MeshRenderer>().materials){
						if(Mat.name == "Building1 (Instance)" || Mat.name == "Building2 (Instance)" || Mat.name == "Building3 (Instance)" || Mat.name == "Bulding4 (Instance)" || Mat.name == "Bulding5 (Instance)"){
							Mat.color = new Color32 ((byte)Random.Range(25, 255), (byte)Random.Range(25, 255), (byte)Random.Range(25, 255), (byte)255);
						}
					}

				}
				break;
		}
		// Land

		// Lighting
		GS.SetLighting(SkyPick);
		// Lighting

	}

}

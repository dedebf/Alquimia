using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private List<Light> lightList = new List<Light>();
	GameObject[] lights;

	// Use this for initialization
	void Start () {
		lights = GameObject.FindGameObjectsWithTag ("LightMap");
		foreach (GameObject light in lights) {
			lightList.Add(light.GetComponent<Light> ());
		}
		GameEvents.OnCreateLightMap (lightList);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

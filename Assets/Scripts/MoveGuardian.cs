using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGuardian : MonoBehaviour {

	float myTime = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		myTime += Time.deltaTime;
		gravitate();
	}

	Vector3 yposVec;
	public float heightGravitate = 0.0005f;
	public void gravitate()
	{
		float ypos = Mathf.Sin(myTime) * heightGravitate;
		yposVec = Vector3.up*ypos;
		gameObject.transform.position += yposVec;
		//gameObject.transform.position = Vector3.Lerp(transform.position, yposVec + transform.position, Random.Range(0f, 5f));
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutomaticScrollContent : MonoBehaviour {
	public Button buttonUp;
	public Button buttonDown;
	public RectTransform scrollView;
	public float scrollSpeed = 0.01f;

	private Scrollbar scrollBar;
	private bool contentUp = false;
	private bool contentDown = false;

	// Use this for initialization
	void Start () {
		scrollBar = scrollView.GetComponentInChildren<Scrollbar> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (contentUp) {
			if(scrollBar.value < 1)
				scrollBar.value += scrollSpeed;
		}
		if (contentDown) {
			if(scrollBar.value > 0)
				scrollBar.value -= scrollSpeed;
		}
	}

	public void scrollUp(){
		contentUp = true;
	}

	public void scrollDown(){
		contentDown = true;
	}

	public void scrollStop(){
		contentUp = false;
		contentDown = false;
	}
}

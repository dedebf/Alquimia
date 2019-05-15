using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MushroomManager : MonoBehaviour {
	public GameObject posMush;
	public GameObject posMushZoom;
	public GameObject user;
	public GameObject pointer;
	public GameObject orbMush;
	public GameObject guardianMush;
	public Light guardiaoLight;
	public TextMesh score;
	public GameObject canvasTalk;
	public GameObject canvasStatusGame;
	public Text textButtonYes;
	public GameObject buttonNo;
	public GameObject scrollPaneAsk;
	public GameObject scrollPaneTips;

	private List<Light> lightList = new List<Light>();
	private List<float> initialLightListIntensity = new List<float> ();

	private AudioSource audioBackGround;
	private AudioSource audioGuardian;
	private VideoPlayer video;
	private Collider colliderSphere;

	private bool isPlaying = true;
	private bool isSoundUp = false;
	private double volSound = 0.03;
	private bool isFinishedStage = false;
	private bool isGuardianTalkin = false;

	private bool DEBUG = false;

	// Start is called before the first frame update
	void Awake()
	{
		GameEvents.OnChangePlace += (string place) => itsMe (place);
		GameEvents.OnCreateLightMap += (List<Light> lights) => setLights (lights);
		Application.targetFrameRate = 60;
		Debug.Log (Application.targetFrameRate.ToString());
		Debug.Log ("Teste");

	}

	void OnDestroy()
	{
		GameEvents.OnChangePlace -= (string place) => itsMe (place);
		GameEvents.OnCreateLightMap -= (List<Light> lights) => setLights (lights);
	}

	private void itsMe(string place){
		if (place == "mush") {
			goToMush ();
		} else {
			stopPlace ();
		}
	}

	private void setLights(List<Light> lights){
		Debug.Log (lights.Count);
		foreach (Light light in lights) {
			if (!light.Equals (guardiaoLight)) {
				lightList.Add (light);
				initialLightListIntensity.Add (light.intensity);
			}
		}
	}

	// Use this for initialization
	void Start () {
		audioBackGround = GetComponent<AudioSource> ();
		audioGuardian = guardianMush.GetComponent<AudioSource> ();

		video = guardianMush.GetComponent<VideoPlayer> ();
		colliderSphere = orbMush.GetComponent<Collider> ();

		score.text = ("0 / " + ((int)audioGuardian.clip.length).ToString());

		guardianMush.SetActive (false);
	}
	
	// Update is called once per frame
	float timePercent;
	float intensity;
	void FixedUpdate () {
		if (!isFinishedStage) {
			if (isPlaying) {
				score.text = ((int)audioGuardian.time).ToString() + " / " + ((int)audioGuardian.clip.length).ToString();

				if (audioGuardian.time >= (audioGuardian.clip.length - 2)) {
					isPlaying = false;
					showTalk ();
				} else {
					for (int i = 0; i < lightList.Count; i++) {
						timePercent = (audioGuardian.time * 100f) / audioGuardian.clip.length;

						intensity = (initialLightListIntensity [i] - (timePercent * initialLightListIntensity [i]) / 100);
						lightList [i].intensity = intensity;
					}
				}
			}
		}
		if (isGuardianTalkin) {
			volSound = 0.03d;
			audioBackGround.volume -= (float)volSound;
		} else {
			if (isSoundUp) {
				if (volSound <= 1) {
					volSound += 0.02;
				}
				audioBackGround.volume = (float)volSound;
			} else {
				if (volSound > 0.03) {
					volSound -= 0.03;
				} else {
					volSound = 0.03;
				}
				audioBackGround.volume = (float)volSound;
			}
		}
	}

	public void turnOnLights(){
		for (int i = 0; i < lightList.Count; i++) {
			Debug.Log (i.ToString () + " - " + initialLightListIntensity [i].ToString ());
			lightList [i].intensity = initialLightListIntensity [i];
		}
	}

	public void turnOffLights(){
		for (int i = 0; i < lightList.Count; i++) {
			lightList [i].intensity = 0f;
		}
	}

	public void showTalk(){
		Debug.Log("ShowTalk() - Mush");
		isPlaying = false;

		//hide score
		score.gameObject.SetActive(false);
		canvasStatusGame.SetActive (false);

		//show canvas
		canvasTalk.SetActive(true);

		//start interaction
		pointer.SetActive (true);
	}

	public void acceptedChallenge(){
		scrollPaneAsk.SetActive (false);
		scrollPaneTips.SetActive (true);
	}

	public void deniedChallenge(){
		buttonNo.SetActive (false);
		textButtonYes.text = "Agora eu posso ;)";
	}

	public void finishStage(){
		turnOnLights ();
		
		//change canvas
		canvasTalk.SetActive(false);
		canvasStatusGame.SetActive (true);

		isFinishedStage = true;
		GameEvents.OnFinishMush (true);
	}

	public void onEnterMush(){
		Debug.Log ("EnterMushroom");
//		VolumeUp
		isSoundUp = true;
	}

	public void onExitMush(){
		isSoundUp = false;
	}

	bool isOnMushPos = false;

	public void mushZoom(){
		if (isOnMushPos) {
			// Move the player to the play position.
			iTween.MoveTo(user,
				iTween.Hash(
					"position", posMush.transform.position,
					"time", 2,
					"easetype", "linear"
				)
			);

			//start interaction - just for garantee
			pointer.SetActive (true);

			//hide guardian
			guardianMush.SetActive (false);

			isOnMushPos = false;
		} else {
			// Move the player to the play position.
			iTween.MoveTo(user,
				iTween.Hash(
					"position", posMushZoom.transform.position,
					"time", 2,
					"easetype", "linear"
				)
			);

			//stop interaction
			if (!isFinishedStage) {
				pointer.SetActive (false);
			}

			//show guardian
			guardianMush.SetActive (true);

			isOnMushPos = true;
		}
	}

	public void onClickGuardian(){
		if (isFinishedStage) {
			canvasTalk.SetActive(!canvasTalk.activeSelf);
		}
	}

	public void onEnterGuardian(){
		if (isFinishedStage) {
			audioGuardian.Play ();
			video.Play ();
			isPlaying = true;
			isGuardianTalkin = true;
		}
	}

	public void onExitGuardian(){
		if (isFinishedStage) {
			audioGuardian.Pause ();
			video.Pause ();
			isPlaying = false;
			isGuardianTalkin = false;
		}
	}

	private void stopPlace(){
		audioGuardian.Stop ();
		video.Stop ();
		isPlaying = false;
		isGuardianTalkin = false;
		guardianMush.SetActive (false);

//		colliderSphere.gameObject.SetActive (true);
		colliderSphere.enabled = true;
	}

	private void goToMush(){
		//move user to earthPlace
		iTween.MoveTo(user,
			iTween.Hash(
				"position", orbMush.transform.position,
				"time", 2,
				"easetype", "linear"
			)
		);

//		colliderSphere.gameObject.SetActive (false);
		colliderSphere.enabled = false;
	}

	public void orbClick(){
		GameEvents.OnChangePlace ("mush");
	}
}

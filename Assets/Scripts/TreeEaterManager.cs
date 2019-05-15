using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TreeEaterManager : MonoBehaviour {

	public GameObject user;
	public GameObject pointer;
	private GvrReticlePointer reticlePointer;
	public GameObject orbTreeEater;
	public GameObject guardiaoTreeEater;
	public Light guardiaoLight;
	public GameObject guardianPos;
	public GameObject playPos;
	public GameObject finalPos;
	public TextMesh score;
	public GameObject canvasTalk;

	private List<Light> lightList = new List<Light>();
	private List<float> initialLightListIntensity = new List<float> ();

	private AudioSource audioBackGround;
	private VideoPlayer video;
	private Collider collider;

	private bool isPlaying = false;
	private bool isSoundUp = false;
	private double volSound = 0.03;
	private bool isUnblockStage = false;
	private bool isFinishedStage = false;
	private bool isGuardianTalkin = false;

	private bool DEBUG = false;

	// Start is called before the first frame update
	void Awake()
	{
		GameEvents.OnFinishGod += (bool isFinished) => unblockStage (isFinished);
		GameEvents.OnChangePlace += (string place) => itsMe (place);
		GameEvents.OnCreateLightMap += (List<Light> lights) => setLights (lights);
	}

	void OnDestroy()
	{
		GameEvents.OnFinishGod -= (bool isFinished) => unblockStage (isFinished);
		GameEvents.OnChangePlace -= (string place) => itsMe (place);
		GameEvents.OnCreateLightMap -= (List<Light> lights) => setLights (lights);
	}

	private void itsMe(string place){
		if (place == "treeEater") {
			goToTreeEater ();
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

	public void unblockStage(bool isFinished){
		orbTreeEater.SetActive (true);

		isUnblockStage = true;
	}

	// Use this for initialization
	void Start () {
//		orbTreeEater.SetActive (false);
		audioBackGround = guardiaoTreeEater.GetComponent<AudioSource> ();
		if(DEBUG){
			audioBackGround.time = audioBackGround.clip.length - 5f;
		}
		video = guardiaoTreeEater.GetComponent<VideoPlayer> ();
//		video.time = 120;
		collider = orbTreeEater.GetComponent<SphereCollider> ();

		//set score text
		score.text = ("0 / " + ((int)audioBackGround.clip.length).ToString());

		// reticlePointer.maxReticleDistance = 0f;
		reticlePointer = pointer.GetComponent<GvrReticlePointer>();
	}

	// Update is called once per frame
	float timePercent;
	float intensity;
	void Update () {
		if (!isFinishedStage) {
			if (isPlaying) {
				score.text = ((int)audioBackGround.time).ToString () + " / " + ((int)audioBackGround.clip.length).ToString ();

				for (int i = 0; i < lightList.Count; i++) {
					timePercent = (audioBackGround.time * 100f) / audioBackGround.clip.length;
					intensity = (initialLightListIntensity [i] - (timePercent * initialLightListIntensity [i]) / 100);
					lightList [i].intensity = intensity;
				}
			} 
		}
		if (isSoundUp) {
			if (volSound <= 1) {
				volSound += 0.02;
			}
			audioBackGround.volume = (float) volSound;
		} else {
			if (volSound > 0.03) {
				volSound -= 0.03;
			} else {
				volSound = 0.03;
			}
			audioBackGround.volume = (float) volSound;
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
		isPlaying = false;

		//hide score
		score.gameObject.SetActive(false);

		//show canvas
		canvasTalk.SetActive(true);

		//start interaction
		isGuardianTalkin = true;
		pointer.SetActive (true);
		reticlePointer.maxReticleDistance = 20f;
	}

	public void finishStage(){
		turnOnLights ();
		
		//change canvas
		isGuardianTalkin = false;
		canvasTalk.SetActive(false);

		isFinishedStage = true;
		isPlaying = false;
//		GameEvents.OnFinishTreeEater (true);
	}

	public void onClickGuardian(){
		if (isFinishedStage) {
			canvasTalk.SetActive(!canvasTalk.activeSelf);
		}
	}

	public void onEnterGuardian(){
		if(!isGuardianTalkin){
			if (isFinishedStage) {
				audioBackGround.Play ();
				video.Play ();
				isPlaying = true;
				isSoundUp = true;
			}else{
				if(!isPlaying){
					pointer.SetActive (false);
					reticlePointer.maxReticleDistance = 5.2f;
					GameEvents.OnStartFinalStage (audioBackGround.clip.length, finalPos.transform.position);
					audioBackGround.Play ();
					video.Play ();
					isPlaying = true;
					isSoundUp = true;
					MoveGuardianToPlayPositon (true, 0f);
					Invoke ("showTalk", audioBackGround.clip.length);
				}
			}
		}
	}

	public void onExitGuardian(){
		if (isFinishedStage) {
			audioBackGround.Pause ();
			video.Pause ();
			isPlaying = false;
			MoveGuardianToPlayPositon (false, 1);
		}
	}

	private void MoveGuardianToPlayPositon(bool isToPlayPosition, float returnTime){
		if (isToPlayPosition) {
			iTween.MoveTo (guardiaoTreeEater,
				iTween.Hash (
					"position", playPos.transform.position,
					"time", (audioBackGround.clip.length-audioBackGround.time)/2,
					"easetype", "linear"
				)
			);
			iTween.ScaleTo (guardiaoTreeEater,
				iTween.Hash (
					"scale", playPos.transform.localScale,
					"time", audioBackGround.clip.length-audioBackGround.time,
					"easetype", "linear"
				)
			);
		} else {
			iTween.MoveTo (guardiaoTreeEater,
				iTween.Hash (
					"position", guardianPos.transform.position,
					"time", returnTime,
					"easetype", "linear"
				)
			);
			iTween.ScaleTo (guardiaoTreeEater,
				iTween.Hash (
					"scale", guardianPos.transform.localScale,
					"time", returnTime,
					"easetype", "linear"
				)
			);
		}
	}

	private void stopPlace(){
		audioBackGround.Stop ();
		video.Stop ();
		isPlaying = false;
		guardiaoTreeEater.SetActive (false);
		if (isUnblockStage) {
//			collider.gameObject.SetActive (true);
			collider.enabled = true;
		}
	}

	private void goToTreeEater(){
		//move user to TreeEaterPlace
		iTween.MoveTo(user,
			iTween.Hash(
				"position", orbTreeEater.transform.position,
				"time", 2,
				"easetype", "linear"
			)
		);
		guardiaoTreeEater.SetActive (true);

//		collider.gameObject.SetActive (false);
		collider.enabled = false;
	}

	public void orbClick(){
		GameEvents.OnChangePlace ("treeEater");
	}

}

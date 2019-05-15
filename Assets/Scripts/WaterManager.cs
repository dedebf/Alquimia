using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WaterManager : MonoBehaviour {

	public GameObject user;
	public GameObject pointer;
	public GameObject orbWater;
	public GameObject guardiaoWater;
	public Light guardiaoLight;
	public GameObject guardianPos;
	public GameObject playPos;
	public GameObject finalPos;
	public TextMesh score;
	public GameObject canvasTalk;

	public WaterGroup waterGroup;

	private List<Light> lightList = new List<Light>();
	private List<float> initialLightListIntensity = new List<float> ();

	private AudioSource audioBackGround;
	private VideoPlayer video;
	private Collider collider;
	private Collider guardianCollider;

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
		GameEvents.OnFinishEarth += (bool isFinished) => unblockStage (isFinished);
		GameEvents.OnChangePlace += (string place) => itsMe (place);
		GameEvents.OnCreateLightMap += (List<Light> lights) => setLights (lights);
		GameEvents.OnStartFinalStage += (float time, Vector3 treeEaterPos) => finalStage (time, treeEaterPos);
	}

	void OnDestroy()
	{
		GameEvents.OnFinishEarth -= (bool isFinished) => unblockStage (isFinished);
		GameEvents.OnChangePlace -= (string place) => itsMe (place);
		GameEvents.OnCreateLightMap -= (List<Light> lights) => setLights (lights);
		GameEvents.OnStartFinalStage -= (float time, Vector3 treeEaterPos) => finalStage (time, treeEaterPos);
	}

	private void itsMe(string place){
		if (place == "water") {
			goTowater ();
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
		orbWater.SetActive (true);

		isUnblockStage = true;
	}

	private void finalStage(float time, Vector3 treeEaterPos){
		Debug.Log("finalStage");
		Debug.Log(time);

		guardiaoWater.SetActive (true);
		guardianCollider.enabled = false;
		score.gameObject.SetActive(false);

		guardiaoWater.transform.position = treeEaterPos;
		guardiaoWater.transform.localScale = finalPos.transform.localScale;
		guardiaoWater.transform.localRotation = finalPos.transform.localRotation;

		iTween.MoveTo (guardiaoWater,
			iTween.Hash (
				"position", finalPos.transform.position,
				"time", time*0.75f,
				"easetype", "linear"
			)
		);
	}

	// Use this for initialization
	void Start () {
//		orbWater.SetActive (false);
		audioBackGround = guardiaoWater.GetComponent<AudioSource> ();
		if(DEBUG){
			unblockStage(true);
			finishStage();
		}
		video = guardiaoWater.GetComponent<VideoPlayer> ();
		collider = orbWater.GetComponent<SphereCollider> ();
		guardianCollider = guardiaoWater.GetComponent<MeshCollider> ();

		//set score text
		score.text = ("0 / " + ((int)audioBackGround.clip.length).ToString());
	}

	// Update is called once per frame
	float timePercent;
	float intensity;
	void FixedUpdate () {
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
	}

	private void finishStage(){
		turnOnLights ();
		
		//change canvas
		isGuardianTalkin = false;
		canvasTalk.SetActive(false);

		waterGroup.startWaves ();

		isFinishedStage = true;
		isPlaying = false;
		GameEvents.OnFinishWater (true);
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
					audioBackGround.Play ();
					video.Play ();
					isPlaying = true;
					isSoundUp = true;
					MoveGuardianToPlayPositon (true, 0f);
					if (DEBUG) {
						Invoke ("showTalk", 5f);
					} else {
						Invoke ("showTalk", audioBackGround.clip.length - 1f);
					}
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
			iTween.MoveTo (guardiaoWater,
				iTween.Hash (
					"position", playPos.transform.position,
					"time", (audioBackGround.clip.length-audioBackGround.time)/2,
					"easetype", "linear"
				)
			);
			iTween.ScaleTo (guardiaoWater,
				iTween.Hash (
					"scale", playPos.transform.localScale,
					"time", audioBackGround.clip.length-audioBackGround.time,
					"easetype", "linear"
				)
			);
		} else {
			iTween.MoveTo (guardiaoWater,
				iTween.Hash (
					"position", guardianPos.transform.position,
					"time", returnTime,
					"easetype", "linear"
				)
			);
			iTween.ScaleTo (guardiaoWater,
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
		guardiaoWater.SetActive (false);
		if (isUnblockStage) {
//			collider.gameObject.SetActive (true);
			collider.enabled = true;
		}
	}

	private void goTowater(){
		//move user to waterPlace
		iTween.MoveTo(user,
			iTween.Hash(
				"position", orbWater.transform.position,
				"time", 2,
				"easetype", "linear"
			)
		);
		guardiaoWater.SetActive (true);

//		collider.gameObject.SetActive (false);
		collider.enabled = false;
	}

	public void orbClick(){
		GameEvents.OnChangePlace ("water");
	}

}

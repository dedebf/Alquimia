using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GodManager : MonoBehaviour {

	public GameObject user;
	public GameObject pointer;
	public GameObject orbGod;
	public GameObject guardiaoGod;
	public GameObject guardianPos;
	public GameObject playPos;
	public GameObject finalPos;
	public TextMesh score;
	public GameObject canvasTalk;

	public List<Light> lightList;
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
		GameEvents.OnFinishAir += (bool isFinished) => unblockStage (isFinished);
		GameEvents.OnChangePlace += (string place) => itsMe (place);
		GameEvents.OnStartFinalStage += (float time, Vector3 treeEaterPos) => finalStage (time, treeEaterPos);

		foreach (Light light in lightList) {
			initialLightListIntensity.Add (light.intensity);
			Debug.Log (light.intensity);
		}
	}

	void OnDestroy()
	{
		GameEvents.OnFinishAir -= (bool isFinished) => unblockStage (isFinished);
		GameEvents.OnChangePlace -= (string place) => itsMe (place);
		GameEvents.OnStartFinalStage -= (float time, Vector3 treeEaterPos) => finalStage (time, treeEaterPos);
	}

	private void itsMe(string place){
		if (place == "god") {
			goToGod ();
		} else {
			stopPlace ();
		}
	}

	public void unblockStage(bool isFinished){
		isUnblockStage = true;
		orbGod.SetActive (true);
//		collider.gameObject.SetActive (true);
	}

	private void finalStage(float time, Vector3 treeEaterPos){
		Debug.Log("finalStage");
		Debug.Log(time);

		guardiaoGod.SetActive (true);
		guardianCollider.enabled = false;
		score.gameObject.SetActive(false);

		guardiaoGod.transform.position = treeEaterPos;
		guardiaoGod.transform.localScale = finalPos.transform.localScale;
		guardiaoGod.transform.localRotation = finalPos.transform.localRotation;

		iTween.MoveTo (guardiaoGod,
			iTween.Hash (
				"position", finalPos.transform.position,
				"time", time*0.75f,
				"easetype", "linear"
			)
		);
	}

	// Use this for initialization
	void Start () {
		orbGod.SetActive (false);
		audioBackGround = guardiaoGod.GetComponent<AudioSource> ();
		if(DEBUG){
			unblockStage(true);
			finishStage();
		}
		video = guardiaoGod.GetComponent<VideoPlayer> ();
		video.time = 120;
		collider = orbGod.GetComponent<SphereCollider> ();
		guardianCollider = guardiaoGod.GetComponent<MeshCollider> ();

		//set score text
		score.text = ("0 / " + ((int)audioBackGround.clip.length).ToString());
	}

	// Update is called once per frame
	float timePercent;
	float intensity;
	void FixedUpdate () {
		if (!isFinishedStage) {
			if (isPlaying) {
				score.text = ((int)audioBackGround.time).ToString() + " / " + ((int)audioBackGround.clip.length).ToString();

				if (audioBackGround.time >= (audioBackGround.clip.length - 1f)) {
					isPlaying = false;
					showTalk();
				} else {
					for (int i = 0; i < lightList.Count; i++) {
						timePercent = (audioBackGround.time * 100f) / audioBackGround.clip.length;
						Debug.Log ((audioBackGround.time * 100f) / audioBackGround.clip.length);

						//					lightList [i].intensity = ((timePercent * initialLightListIntensity [i]) / 100).ToString ();
						intensity = (initialLightListIntensity [i] - (timePercent * initialLightListIntensity [i]) / 100);
						lightList [i].intensity = intensity;
						Debug.Log (i.ToString () + " - " + (initialLightListIntensity [i] - (timePercent * initialLightListIntensity [i]) / 100).ToString ());
					}
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
		isGuardianTalkin = true;

		//start interaction
		pointer.SetActive (true);
	}

	public void finishStage(){
		turnOnLights ();

		//change canvas
		canvasTalk.SetActive(false);
		isGuardianTalkin = false;

		isFinishedStage = true;
		isPlaying = false;
		GameEvents.OnFinishGod (true);
	}

	public void finishGame(){
		//showCanvas finish and reload Scene
		SceneManager.LoadScene( SceneManager.GetActiveScene().name, LoadSceneMode.Single);
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
				}
			}
		}
	}

	private void PlayVideo(){
		if (video.time < 120) {
			video.time = 120;
		}
		video.Play ();
	}

	public void onExitGuardian(){
		if (isFinishedStage) {
			audioBackGround.Pause ();
			video.Pause ();
			isPlaying = false;
			isGuardianTalkin = false;
			MoveGuardianToPlayPositon (false, 1);
		}
	}

	private void MoveGuardianToPlayPositon(bool isToPlayPosition, float returnTime){
		if (isToPlayPosition) {
			iTween.MoveTo (guardiaoGod,
				iTween.Hash (
					"position", playPos.transform.position,
					"time", (audioBackGround.clip.length-audioBackGround.time)/2,
					"easetype", "linear"
				)
			);
			iTween.ScaleTo (guardiaoGod,
				iTween.Hash (
					"scale", playPos.transform.localScale,
					"time", audioBackGround.clip.length-audioBackGround.time,
					"easetype", "linear"
				)
			);
		} else {
			iTween.MoveTo (guardiaoGod,
				iTween.Hash (
					"position", guardianPos.transform.position,
					"time", returnTime,
					"easetype", "linear"
				)
			);
			iTween.ScaleTo (guardiaoGod,
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
		guardiaoGod.SetActive (false);
		if(isUnblockStage)
			collider.gameObject.SetActive (true);
	}

	private void goToGod(){
		//move user to GodPlace
//		iTween.MoveTo(user,
//			iTween.Hash(
//				"position", orbGod.transform.position,
//				"time", 2,
//				"easetype", "linear"
//			)
//		);
		guardiaoGod.SetActive (true);

		collider.gameObject.SetActive (false);

		onEnterGuardian ();
	}

	public void orbClick(){
		GameEvents.OnChangePlace ("god");
	}

}

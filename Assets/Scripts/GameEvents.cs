using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameEvents
{

//	public delegate void SendCountryObjectList(List<CountryGarbage> list);
//	public static SendCountryObjectList OnObjectCreated;
//
//	public delegate void RandomPersonInfoEvent(string country, int age);
//
//	public static RandomPersonInfoEvent OnPersonGenerated;
//
//	public delegate void GenerateGarbageEvent(float ton);
//
//	public static GenerateGarbageEvent StartGarbageRain;

	public delegate void CreateLightMap(List<Light> lights);
	public static CreateLightMap OnCreateLightMap;

	public delegate void ChangePlace(string place);
	public static ChangePlace OnChangePlace;

	public delegate void FinishMush(bool isFinished);
	public static FinishMush OnFinishMush;

	public delegate void FinishEarth(bool isFinished);
	public static FinishEarth OnFinishEarth;

	public delegate void FinishWater(bool isFinished);
	public static FinishWater OnFinishWater;

	public delegate void FinishFire(bool isFinished);
	public static FinishFire OnFinishFire;

	public delegate void FinishAir(bool isFinished);
	public static FinishAir OnFinishAir;

	public delegate void FinishGod(bool isFinished);
	public static FinishGod OnFinishGod;

	public delegate void StartFinalStage(float time, Vector3 treeEaterPos);
	public static StartFinalStage OnStartFinalStage;

	public delegate void EarthGoTo();
	public static EarthGoTo GoToEarth;
}

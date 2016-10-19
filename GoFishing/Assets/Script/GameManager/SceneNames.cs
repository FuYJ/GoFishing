using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "SceneNamesSetting", menuName = "Custom Editor/SceneNames Data", order = 1)]
public class SceneNames : ScriptableObject {

	public string initScene;
	public SceneNameHolder[] scenes;
}

[System.Serializable]
public class SceneNameHolder {

	public string own;
	public string loading;
	public string stageName = "";
	public bool isAdditiveLoading;
	public bool isTimeLimitStage;
	public bool isFreeStage;
	public int handStarNumber = 0;
	public int footStarNumber = 0;
	public int coordinationStarNumber = 0;
	public string stageDescription = "";
}


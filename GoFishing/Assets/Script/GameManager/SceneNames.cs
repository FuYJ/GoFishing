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
	public bool isAdditiveLoading;
}


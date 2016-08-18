using UnityEngine;
using System.Collections;

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
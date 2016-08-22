using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	static public SceneLoader Instance;
	private Scenes own;

	public enum Scenes{

		Start = 0,
		SelectPlayer,
		Menu,
		SelectStage,
		Stage1,
		Stage2
	}

	void Awake(){
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);
			GameManager.Instance.SceneOwn = (int)own;
		} else {
			Destroy (gameObject);
		}
	}

	public void LoadLevel(Scenes target){
		GameManager.Instance.LoadScene((int)target);
	}
}

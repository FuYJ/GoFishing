using UnityEngine;
using System.Collections;

public class Stage1Script : StageScript {
	// Use this for initialization
	void Awake () {
		STAGE_TIME = 60;
		TERRAIN_SIZE = 200f;
	}

	void Start() {
		base.Start ();
	}

	void Update(){
		base.Update ();
	}

	public override void MoveToGameOver(){
		UnityEngine.SceneManagement.SceneManager.LoadScene("StageOverScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
	}
}

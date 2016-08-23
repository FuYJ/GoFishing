using UnityEngine;
using System.Collections;

public class Stage1Script : StageScript {
	// Use this for initialization
	void Awake () {
		STAGE_TIME = 180;
		TERRAIN_SIZE = 200f;
	}

	void Start() {
		base.Start ();
	}

	void Update(){
		base.Update ();
	}

	public override void MoveToGameOver(){
		SoundManager.Instance.StopStageBackgroundMusic ();
		SceneLoader.Instance.LoadLevel(SceneLoader.Scenes.StageOver);
	}
}

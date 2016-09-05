using UnityEngine;
using System.Collections;

public class Stage1Script : StageScript {
	// Use this for initialization
	void Awake () {
		base.Awake ();
		STAGE_TIME = 180;
		TERRAIN_SIZE = 200f;
	}

	void Start() {
		base.Start ();
	}

	void Update(){
		base.Update ();
	}
}

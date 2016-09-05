using UnityEngine;
using System.Collections;

public class Stage2Script : StageScript {

	void Awake () {
		base.Awake ();
		STAGE_TIME = 60;
		TERRAIN_SIZE = 200f;
	}

	// Use this for initialization
	void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}
}

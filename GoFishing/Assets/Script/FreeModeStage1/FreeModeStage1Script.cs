using UnityEngine;
using System.Collections;

public class FreeModeStage1Script : StageScript {

	public GameObject m_redBlock;
	public GameObject m_yellowBlock;
	public GameObject m_greenBlock;

	private float _countTime;

	// Use this for initialization
	void Awake () {
		base.Awake ();
		STAGE_TIME = (int) 1e9;
		TERRAIN_SIZE = 500f;
		_countTime = STAGE_TIME / 3;
	}

	void Start() {
		base.Start ();
	}

	void Update(){
		base.Update ();
		_countTime = CountTime (_countTime);
		if (_countTime < 0) {
			changeBlockPos ();
			_countTime = STAGE_TIME / 3;
		}
	}

	private void changeBlockPos(){
		m_redBlock.transform.position = new Vector3 (Random.Range (0, TERRAIN_SIZE), -0.01f, Random.Range (0, TERRAIN_SIZE));
		m_greenBlock.transform.position = new Vector3 (Random.Range (0, TERRAIN_SIZE), -0.01f, Random.Range (0, TERRAIN_SIZE));
		m_yellowBlock.transform.position = new Vector3 (Random.Range (0, TERRAIN_SIZE), -0.01f, Random.Range (0, TERRAIN_SIZE));
	}
}

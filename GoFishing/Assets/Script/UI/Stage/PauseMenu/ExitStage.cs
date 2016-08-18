using UnityEngine;
using System.Collections;

public class ExitStage : MonoBehaviour {

	private StageScript _stageScript;

	// Use this for initialization
	void Start () {
		GameObject stageScriptGameObject = GameObject.FindGameObjectWithTag ("StageScript");
		if (stageScriptGameObject != null) {
			_stageScript = stageScriptGameObject.GetComponent<StageScript> ();
		}
	}

	void OnMouseDown () {
		SoundManager.Instance.PlayClickSound ();
		_stageScript.MoveToGameOver ();
	}
}

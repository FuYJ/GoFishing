using UnityEngine;
using System.Collections;

public class PlayerInformation : MonoBehaviour {

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
		_stageScript.ShowPlayerInformation ();
	}

	public void OnBackButtonClick () {
		SoundManager.Instance.PlayClickSound ();
		_stageScript.SetPause (false);
	}
}

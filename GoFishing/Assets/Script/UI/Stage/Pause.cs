using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	private StageScript _stageScript;
	public AudioSource m_clickSound;

	// Use this for initialization
	void Start () {
		GameObject stageScriptGameObject = GameObject.FindGameObjectWithTag ("StageScript");
		if (stageScriptGameObject != null) {
			_stageScript = stageScriptGameObject.GetComponent<StageScript> ();
		}
	}

	void OnMouseDown () {
		m_clickSound.Play ();
		_stageScript.SetPause (true);
	}
}

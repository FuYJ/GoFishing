using UnityEngine;
using System.Collections;

public class Replay : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnMouseDown(){
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Stage1Scene", UnityEngine.SceneManagement.LoadSceneMode.Single);
	}
}

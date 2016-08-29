using UnityEngine;
using System.Collections;

public class ReplayButton : MonoBehaviour {

	void OnMouseDown(){
		SceneLoader.Instance.LoadLevel (SceneLoader.Instance.NowStage);
	}
}

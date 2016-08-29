using UnityEngine;
using System.Collections;

public class GoToSelectStageButton : MonoBehaviour {

	void OnMouseDown(){
		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.SelectStage);
	}
}

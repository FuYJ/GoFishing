using UnityEngine;
using System.Collections;

public class StageDescriptionScript : MonoBehaviour {

	private SceneLoader.Scenes _stageIndex;

	public SceneLoader.Scenes StageIndex{
		set{ 
			_stageIndex = value;
		}
	}

	public void OnEnterStageButtonClicked(){
		//Debug.Log ("Clicked!!");
		SoundManager.Instance.PlayClickSound ();
		SelectStageScript _selectStageScript;
		_selectStageScript = GameObject.Find ("SelectStageScript").GetComponent<SelectStageScript>();
		_selectStageScript.LoadStage (_stageIndex);
		Destroy (this.gameObject);
	}

	public void OnSelectOtherStageButtonClicked(){
		SoundManager.Instance.PlayClickSound ();
		Destroy (this.gameObject);
	}
}

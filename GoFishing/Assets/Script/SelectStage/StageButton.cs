using UnityEngine;
using System.Collections;

public class StageButton : MonoBehaviour {

	private SceneLoader.Scenes _stageIndex;

	public SceneLoader.Scenes StageIndex{
		get{
			return _stageIndex;
		}
		set{
			_stageIndex = value;
		}
	}

	public void OnStageButtonClick(){
		SelectStageScript _selectStageScript;
		_selectStageScript = GameObject.Find ("SelectStageScript").GetComponent<SelectStageScript>();
		_selectStageScript.LoadStage (_stageIndex);
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StageButton : MonoBehaviour {

	public GameObject m_stageDescription;
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
		SoundManager.Instance.PlayClickSound ();
		StartCoroutine(PopUpStageDescription(GameManager.Instance.m_sceneNames.scenes[(int)_stageIndex].stageDescription));
		/*SoundManager.Instance.PlayClickSound ();
		SelectStageScript _selectStageScript;
		_selectStageScript = GameObject.Find ("SelectStageScript").GetComponent<SelectStageScript>();
		_selectStageScript.LoadStage (_stageIndex);*/
	}

	private IEnumerator PopUpStageDescription(string msg){
		GameObject newDescription = Instantiate (m_stageDescription);
		Text stageDescription = newDescription.GetComponentInChildren<Text> ();
		StageDescriptionScript stageDescriptionScript = newDescription.GetComponent<StageDescriptionScript> ();
		stageDescription.text = msg;
		stageDescriptionScript.StageIndex = _stageIndex;
		yield break;
	}
}

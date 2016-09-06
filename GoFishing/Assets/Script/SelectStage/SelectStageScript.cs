using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectStageScript : MonoBehaviour {

	public GameObject m_selectStageModeBoard;
	public GameObject m_selectStageBoard;

	//Prefabs
	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;
	public GameObject m_stageButton;

	private Vector3 _stageButtonPos;

	public Texture m_starUnfilled;
	public Texture m_starFilled;

	private States _nowState = States.SelectStageMode;

	private enum States
	{
		SelectStageMode,
		SelectStage
	}

	// Use this for initialization
	void Awake () {
		if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);
	}
	
	// Update is called once per frame
	void Start () {
		_stageButtonPos = new Vector3 (-282, -56, 0);
		ShowSelectStageModeBoard ();
	}
		
	public void ShowSelectStageModeBoard (){
		_nowState = States.SelectStageMode;
		m_selectStageModeBoard.SetActive (true);
		m_selectStageBoard.SetActive (false);
	}

	public void ShowFreeModeStages(){
		_nowState = States.SelectStage;
		SoundManager.Instance.PlayClickSound ();
		m_selectStageModeBoard.SetActive (false);
		m_selectStageBoard.SetActive (true);
		int scenesNumber = GameManager.Instance.m_sceneNames.scenes.Length;
		for (int i = 0; i < scenesNumber; i++) {
			if (GameManager.Instance.m_sceneNames.scenes [i].isFreeStage) {
				CreateStageButton (GameManager.Instance.m_sceneNames.scenes [i], i);
			}
		}
	}

	public void ShowTimeLimitModeStages(){
		_nowState = States.SelectStage;
		SoundManager.Instance.PlayClickSound ();
		m_selectStageModeBoard.SetActive (false);
		m_selectStageBoard.SetActive (true);
		int scenesNumber = GameManager.Instance.m_sceneNames.scenes.Length;
		for (int i = 0; i < scenesNumber; i++) {
			if (GameManager.Instance.m_sceneNames.scenes [i].isTimeLimitStage) {
				CreateStageButton (GameManager.Instance.m_sceneNames.scenes [i], i);
			}
		}
	}

	public void CreateStageButton(SceneNameHolder scene, int index){
		GameObject o = Instantiate (m_stageButton);
		o.GetComponent<StageButton> ().StageIndex = (SceneLoader.Scenes)index;
		o.GetComponentInChildren<Text> ().text = scene.stageName;
		SetStarNumber (o, scene);
		o.transform.SetParent (m_selectStageBoard.transform);
		o.transform.localPosition = _stageButtonPos;
		o.transform.localScale = Vector3.one;
		_stageButtonPos.x += 210;
	}

	public void SetStarNumber(GameObject o, SceneNameHolder scene){
		RawImage[] stars;
		stars = o.GetComponentsInChildren<RawImage> ();
		int starNumber = scene.handStarNumber;
		for(int i = 0; i < 5; i++){
			if (starNumber > i)
				stars [i].texture = m_starFilled;
			else
				stars [i].texture = m_starUnfilled;
		}
		starNumber = scene.footStarNumber;
		for(int i = 5; i < 10; i++){
			if (starNumber + 5 > i)
				stars [i].texture = m_starFilled;
			else
				stars [i].texture = m_starUnfilled;
		}
		starNumber = scene.coordinationStarNumber;
		for(int i = 10; i < 15; i++){
			if (starNumber + 10 > i)
				stars [i].texture = m_starFilled;
			else
				stars [i].texture = m_starUnfilled;
		}
	}

	public void LoadStage(SceneLoader.Scenes stageIndex){
		SceneLoader.Instance.SetNowStage (stageIndex);
		SceneLoader.Instance.LoadLevel (stageIndex);
	}

	public void OnBackButtonClick(){
		SoundManager.Instance.PlayClickSound ();
		if (_nowState == States.SelectStageMode) {
			SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.Menu);
		} else {
			ShowSelectStageModeBoard ();
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameRecordScript : MonoBehaviour {

	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;
	public GameObject m_record;
	public GameObject m_recordsField;
	private Vector3 _recordPos;

	private List<GameRecord> _gameRecords;

	void Awake () {
		if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);
	}

	void Start () {
		//_recordPos = new Vector3 (-40, 80, 0);
		LoadPlayerRecords ();
	}

	private void LoadPlayerRecords(){
		_gameRecords = GameManager.Instance.LoadGameRecords ();
        RectTransform recordsFieldRect = m_recordsField.GetComponent<RectTransform> ();
		recordsFieldRect.offsetMin = new Vector2 (recordsFieldRect.offsetMin.x, -(225 + (_gameRecords.Count - 3) * 75));
		recordsFieldRect.offsetMax = new Vector2 (recordsFieldRect.offsetMax.x, 225 + (_gameRecords.Count - 3) * 75);
		_recordPos = new Vector3 (-40, (225 + (_gameRecords.Count - 3) * 75) - 75, 0);
		for (int i = 0; i < _gameRecords.Count; i++) {
			GameObject o = Instantiate (m_record);
			o.transform.SetParent (m_recordsField.transform);
			o.transform.localScale = Vector3.one;
			o.transform.localPosition = _recordPos;
			_recordPos.y -= 150;
			Text[] recordText = o.GetComponentsInChildren<Text> ();
			recordText [0].text = "關卡：" + GameManager.Instance.m_sceneNames.scenes [_gameRecords [i].StageIndex].stageName;
			recordText [1].text = "漁獲量：" + _gameRecords [i].Caches.ToString ();
			recordText [2].text = "航行距離：" + _gameRecords [i].Journey.ToString () + " km";
			recordText [3].text = "遊戲時長：" + _gameRecords [i].Duration.ToString ();
			recordText [4].text = "日期：" + _gameRecords [i].Date;
			recordText [5].text = "時間：" + _gameRecords [i].Time;
		}
	}

	public void OnBackButtonClick(){
		SoundManager.Instance.PlayClickSound ();
		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.Menu);
	}
}

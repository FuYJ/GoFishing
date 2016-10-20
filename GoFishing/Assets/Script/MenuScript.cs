using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;

	void Awake () {
		if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);
	}

	public void OnBackButtonClick(){
		SoundManager.Instance.PlayClickSound ();
		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.SelectPlayer);
	}

	public void OnChooseStageButtonClick () {
		SoundManager.Instance.PlayClickSound ();
		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.SelectStage);
	}

    public void OnDailyTask()
    {
        SoundManager.Instance.PlayClickSound();
        SceneLoader.Instance.LoadLevel(SceneLoader.Scenes.DailyTask);
    }

	public void OnGameRecordButtonClick () {
		SoundManager.Instance.PlayClickSound ();
		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.GameRecord);
	}

	public void OnSettingButtonClick () {
		SoundManager.Instance.PlayClickSound ();
		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.Setting);
	}
}

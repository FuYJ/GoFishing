using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingScript : MonoBehaviour {

	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;
	public Scrollbar m_BGMScrollbar;
	public Scrollbar m_soundScrollbar;

	void Awake () {
		if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);
	}

	void Start () {
		m_BGMScrollbar.value = SoundManager.Instance.BGMVolume;
		m_soundScrollbar.value = SoundManager.Instance.SoundVolume;
	}

	public void SetBGMVolume(float value){
		SoundManager.Instance.SetBGMVolume (value);
	}

	public void SetSoundVolume(float value){
		SoundManager.Instance.SetSoundVolume (value);
	}

	public void OnBackButtonClick () {
		SoundManager.Instance.PlayClickSound ();
		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.Menu);
	}
}

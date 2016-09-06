using UnityEngine;
using System.Collections;

public class StageOverScript : MonoBehaviour {

	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;
	public TextMesh m_caches;
	public TextMesh m_journey;
	public TextMesh m_time;

	void Awake () {
		if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);
	}

	// Use this for initialization
	void Start () {
		m_caches.text = "漁獲量: " + GameManager.Instance.StageRecordRegister.Caches;
		m_journey.text = "航行距離: " + GameManager.Instance.StageRecordRegister.Journey + " km";
		m_time.text = "遊戲時間: " + GameManager.Instance.StageRecordRegister.Duration + " s";
		SoundManager.Instance.PlayBackgroundMusic2 ();
	}


}

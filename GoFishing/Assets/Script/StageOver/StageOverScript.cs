using UnityEngine;
using System.Collections;

public class StageOverScript : MonoBehaviour {

	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;

	// Use this for initialization
	void Start () {
		if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);
	}
}

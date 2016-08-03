using UnityEngine;
using System.Collections;

public class Resume : MonoBehaviour {

	public GameObject m_pauseMenu;
	public Transform m_timerTransform;
	public AudioSource m_clickSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown () {
		m_clickSound.Play ();
		m_pauseMenu.SetActive (false);
		m_timerTransform.localPosition = new Vector3 (0, 1, 0);
	}
}

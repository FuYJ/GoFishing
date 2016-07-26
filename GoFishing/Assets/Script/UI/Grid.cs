using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public TextMesh m_gridState;
	public GameObject m_grid;
	public AudioSource m_clickSound;

	private bool _isGridOn;

	// Use this for initialization
	void Start () {
		_isGridOn = false;
	}

	void OnMouseDown () {
		m_clickSound.Play ();
		if (_isGridOn) {
			_isGridOn = false;
			m_grid.SetActive (false);
			m_gridState.text = "格線關閉";
		}
		else {
			_isGridOn = true;
			m_grid.SetActive (true);
			m_gridState.text = "格線開啟";
		}
		//Debug.Log ("Click grid");
	}
}

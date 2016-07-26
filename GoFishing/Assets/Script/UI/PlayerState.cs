using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour {

	Transform m_transform;
	public AudioSource m_clickSound;

	public delegate void PlayerModeChangedEventHandler();
	public event PlayerModeChangedEventHandler PlayerModeChanged;

	//public GameObject player;
	private string playerMode = "boating";

	public string PlayerMode {
		get{
			return playerMode;
		}
	}

	// Use this for initialization
	void Start () {
		m_transform = this.transform;
	}
	
	void OnMouseDown () {
		m_clickSound.Play ();
		if (playerMode == "boating")
			playerMode = "fishing";
		else
			playerMode = "boating";
		m_transform.eulerAngles += new Vector3 (0, 180, 0);
		NotifyPlayerModeChanged ();
	}

	void NotifyPlayerModeChanged(){
		if (PlayerModeChanged != null) {
			PlayerModeChanged ();
		}
	}

}

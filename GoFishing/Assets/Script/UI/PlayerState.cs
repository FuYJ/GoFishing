using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour {

	Transform m_transform;

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
		if (playerMode == "boating")
			playerMode = "fishing";
		else
			playerMode = "boating";
		//Debug.Log (playerMode);
		m_transform.eulerAngles += new Vector3 (0, 180, 0);
		NotifyObserver ();
	}

	void NotifyObserver(){
		if (PlayerModeChanged != null) {
			PlayerModeChanged ();
		}
	}
}

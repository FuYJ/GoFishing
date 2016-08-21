using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour {

	public GameObject m_bluetoothReceiver;
	public GameObject m_gameManager;

	// Use this for initialization
	void Start () {
		//_bluetoothReceiver = GameObject.Find ("BluetoothReceiver");
		DontDestroyOnLoad (m_bluetoothReceiver);
		//DontDestroyOnLoad (m_gameManager);
	}
}

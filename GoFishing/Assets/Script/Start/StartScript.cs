using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{

	private Text _buttonText;

	void Start () {
		_buttonText = GameObject.Find ("Canvas/StartButton/Text").GetComponent<Text> ();
	}

	void Update () {
		if (GameManager.Instance.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Disconnected)
			_buttonText.text = "開始連線";
		else if (GameManager.Instance.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Connected)
			_buttonText.text = "開始遊戲";
	}

	public void OnStartButtonClick(){
		if (GameManager.Instance.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Disconnected) {
			#if UNITY_EDITOR
			GameManager.Instance.SendMessageForEachListener ("OnXBikeConnectionStatusChange", "1");
			#elif UNITY_ANDROID
			XBikeEventReceiver.Connect();
			#endif
		} else if (GameManager.Instance.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Connected) {
			GameManager.Instance.StartSport ();
			SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.SelectPlayer);
		}
	}
}

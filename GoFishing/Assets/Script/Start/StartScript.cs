using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{

	private Button _startbutton;
	private Text _startButtonText;

	void Start () {
		_startbutton = GameObject.Find ("Canvas/StartButton").GetComponent<Button>();
		_startButtonText = GameObject.Find ("Canvas/StartButton/Text").GetComponent<Text> ();
	}

	void Update () {
		if (GameManager.Instance.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Disconnected) {
			_startbutton.interactable = true;
			_startButtonText.text = "開始連線";
		} else if (GameManager.Instance.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Connecting) {
			_startbutton.interactable = false;
			_startButtonText.text = "連線中...";
		} else if (GameManager.Instance.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Connected) {
			_startbutton.interactable = true;
			_startButtonText.text = "開始遊戲";
		}
	}

	public void OnStartButtonClick(){
		SoundManager.Instance.PlayClickSound ();
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

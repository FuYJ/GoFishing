using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{

	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;

	private Button _startbutton;
	private Text _startButtonText;

    void Awake (){
        if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);
	}

	void Start () {
        SoundManager.Instance.PlayBackgroundMusic2 ();
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

	void OnDestroy(){
		//SoundManager.Instance.StopBackgroundMusic2 ();
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

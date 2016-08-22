using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{


	void Update (){
		if (GameManager.Instance.SportStatus == XBikeEventReceiver.SportStatus.Start) {
			Debug.Log (GameManager.Instance.SportStatus);
			GameManager.Instance.LoadScene((int)SceneLoader.Scenes.SelectPlayer);
		}
	}

	public void OnStartButtonClick(){
		#if UNITY_EDITOR
		GameManager.Instance.SendMessageForEachListener("OnXBikeConnectionStatusChange", "1");
		#elif UNITY_ANDROID
		GameManager.Instance.XBikeEventReceiver.Connect();
		#endif
		GameManager.Instance.StartSport();
	}
}

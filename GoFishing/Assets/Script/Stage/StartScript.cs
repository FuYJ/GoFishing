using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    public float resistanceValue = 1.0f;
    public static ScenesData _data = new ScenesData();

    private int DEADLINE = 100;

    private int _buttonWide = Screen.width / 8;
    private int _buttonHeight = Screen.height / 16;

    static StartScript()
    {
		DontDestroyOnLoad(_data);
    }

    void OnEnable()
    {
    }

    void Update()
    {
		#if UNITY_EDITOR
		Debug.Log (_data.ConnectionStatus);

        // Pressed mouse left button call OnXBikeLeftPressed true
        if (Input.GetMouseButtonDown(0))
        {
            _data.SendMessageForEachListener("OnXBikeLeftPressed", "True");
        }
        // Release mouse left button call OnXBikeLeftPressed false
        else if (Input.GetMouseButtonUp(0))
        {
            _data.SendMessageForEachListener("OnXBikeLeftPressed", "False");
        }
        // Pressed mouse right button call OnXBikeRightPressed true
        if (Input.GetMouseButtonDown(1))
        {
            _data.SendMessageForEachListener("OnXBikeRightPressed", "True");
        }
        // Release mouse right button call OnXBikeRightPressed false
        else if (Input.GetMouseButtonUp(1))
        {
            _data.SendMessageForEachListener("OnXBikeRightPressed", "False");
        }
#endif
    }

    void OnGUI()
    {
        if (_data.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Disconnected)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - _buttonWide / 2, Screen.height - _buttonHeight * 3, _buttonWide, _buttonHeight), "Connect"))
            {
#if UNITY_EDITOR
                _data.SendMessageForEachListener("OnXBikeConnectionStatusChange", "1");
#elif UNITY_ANDROID
                XBikeEventReceiver.Connect();
#endif
//                _data.SendMessageForEachListener("OnXBikeConnectionStatusChange", "1");
            }
        }
        else if (_data.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Connecting)
        {
            _data.test = true;
            Application.LoadLevel("Stage1Scene");
        }
        else if (_data.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Connected)
        {
            Application.LoadLevel("Stage1Scene");
            GUI.BeginGroup(new Rect(Screen.width / 2 - _buttonWide / 2, Screen.height - _buttonHeight * 4, _buttonWide, _buttonHeight * 4));
            GUILayout.BeginVertical();

            if (_data.SportStatus == XBikeEventReceiver.SportStatus.Stop)
            {
                if (GUILayout.Button("Start sport", GUILayout.Width(_buttonWide), GUILayout.Height(_buttonHeight)))
                {
                    _data.StartSport();
                }
                if (GUILayout.Button("Disconnected", GUILayout.Width(_buttonWide), GUILayout.Height(_buttonHeight)))
                {
#if UNITY_EDITOR
                    _data.SendMessageForEachListener("OnXBikeConnectionStatusChange", "0");
#elif UNITY_ANDROID
					XBikeEventReceiver.Disconnect();
#endif
                }
            }
            GUILayout.EndVertical();
            GUI.EndGroup();
        }
    }

#if UNITY_EDITOR
    private IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
        _data.SendMessageForEachListener("OnXBikeConnectionStatusChange", "2");
    }
#endif
}

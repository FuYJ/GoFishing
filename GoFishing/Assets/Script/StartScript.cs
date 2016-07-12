using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
	/// <summary>
	/// current resistance value.
	/// resistance has eight level. (range level1~level8)
	/// </summary>
	public float resistanceValue = 1.0f;

//	private XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
//	private XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;
    ScenesData _data = new ScenesData();
    private Timer _timer = new Timer();

    private int DEADLINE = 100;

    private int _buttonWide = Screen.width / 8;
    private int _buttonHeight = Screen.height / 16;

    private Loading _load;

    public Image _image;
    public Material[] _material;
    private int _imageCount = 0;
    private int MATERIAL_SIZE = 4;

	/// <summary>
	/// set connection status change event and sport status change event
	/// </summary>
	void OnEnable()
	{
		XBikeEventReceiver.connectStatusChangeEvent		+= OnXBikeConnectionStatusChange;
		XBikeEventReceiver.sportStatusChangeEvent		+= OnXBikeSportStatusChange;
	}

	void Update ()
	{
		#if UNITY_EDITOR

		// Pressed mouse left button call OnXBikeLeftPressed true
		if( Input.GetMouseButtonDown(0) )
		{
            _data.SendMessageForEachListener("OnXBikeLeftPressed", "True");
		}
		// Release mouse left button call OnXBikeLeftPressed false
		else if( Input.GetMouseButtonUp(0) )
		{
            _data.SendMessageForEachListener("OnXBikeLeftPressed", "False");
		}
		// Pressed mouse right button call OnXBikeRightPressed true
		if( Input.GetMouseButtonDown(1) )
		{
            _data.SendMessageForEachListener("OnXBikeRightPressed", "True");
		}
		// Release mouse right button call OnXBikeRightPressed false
		else if( Input.GetMouseButtonUp(1) )
		{
            _data.SendMessageForEachListener("OnXBikeRightPressed", "False");
		}

        if (Input.GetKeyDown(KeyCode.P))
        {

            _image.material = _material[_imageCount];
            _imageCount++;
            if (_imageCount >= 4)
                _imageCount = 0;
        }
        #endif
    }

	void OnGUI ()
	{
        if (_data.ConnectionStatus ==  XBikeEventReceiver.ConnectionStatus.Disconnected)
		{
            _timer.Start_Time = false;
            _image.material = null;

            if (GUI.Button(new Rect(Screen.width / 2 - _buttonWide / 2, Screen.height - _buttonHeight * 3, _buttonWide, _buttonHeight), "Connect"))
			{
#if UNITY_EDITOR
                _data.SendMessageForEachListener("OnXBikeConnectionStatusChange", "1");
#elif UNITY_ANDROID
                XBikeEventReceiver.Connect();
#endif
            }
		}
		else if (_data.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Connecting)
		{
            if (_timer.Start_Time)
            {
                _image.material = _material[_timer.Time_i % 4];
                _timer.Update();
            }
            else
            {
                _timer.Start_Time = true;
            }
        }
		else if (_data.ConnectionStatus == XBikeEventReceiver.ConnectionStatus.Connected)
		{
            _timer.Start_Time = false;
            _image.material = null;
            GUI.BeginGroup(new Rect(Screen.width/2 - _buttonWide / 2, Screen.height - _buttonHeight * 4, _buttonWide, _buttonHeight * 4));
			GUILayout.BeginVertical();

			if (_data.SportStatus == XBikeEventReceiver.SportStatus.Stop)
			{
				if (GUILayout.Button("Start sport", GUILayout.Width(_buttonWide), GUILayout.Height(_buttonHeight)))
				{
					StartSport();
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

	/// <summary>
	/// called when the connectivity changes
	/// </summary>
	/// <param name='status'>
	/// "0" = disconnect / "1" = connecting / "2" = connected
	/// </param>
	private void OnXBikeConnectionStatusChange(string status)
	{
        _data.ConnectionStatus = (XBikeEventReceiver.ConnectionStatus)int.Parse(status);
		switch (_data.ConnectionStatus)
		{
		case XBikeEventReceiver.ConnectionStatus.Connecting:
			{
				#if UNITY_EDITOR
				StartCoroutine(WaitTime(3.0f));
				#endif
				break;
			}
		case XBikeEventReceiver.ConnectionStatus.Connected:
			{
				break;
			}
		}
	}
	/// <summary>
	/// called when there is a change in status
	/// </summary>
	/// <param name='status'>
	/// "0" = stop / "1" = start / "2" = pause
	/// </param>
	private void OnXBikeSportStatusChange(string status)
	{
        _data.SportStatus = (XBikeEventReceiver.SportStatus)int.Parse(status);
		switch (_data.SportStatus)
		{
		case XBikeEventReceiver.SportStatus.Stop:
			{
                _data.SportStatus = XBikeEventReceiver.SportStatus.Stop;
				break;
			}
		case XBikeEventReceiver.SportStatus.Pause:
			{
                _data.SportStatus = XBikeEventReceiver.SportStatus.Pause;
				break;
			}
		case XBikeEventReceiver.SportStatus.Start:
			{
                _data.SportStatus = XBikeEventReceiver.SportStatus.Start;
				break;
			}
		}
	}

	private void StopSport()
	{
        #if UNITY_EDITOR
        _data.SendMessageForEachListener("OnXBikeSportStatusChange", "0");
		#elif UNITY_ANDROID
		XBikeEventReceiver.StopSport();
		#endif
	}

	private void StartSport()
	{
        #if UNITY_EDITOR
        _data.SendMessageForEachListener("OnXBikeSportStatusChange", "1");
		#elif UNITY_ANDROID
		XBikeEventReceiver.StartSport();
		#endif
	}

	private void PauseSport()
	{
        #if UNITY_EDITOR
        _data.SendMessageForEachListener("OnXBikeSportStatusChange", "2");
		#elif UNITY_ANDROID
		XBikeEventReceiver.PauseSport();
		#endif
	}
}

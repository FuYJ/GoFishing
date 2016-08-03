using UnityEngine;
using System.Collections;

public class ScenesData : MonoBehaviour {
    /// <summary>
    /// current resistance value.
    /// resistance has eight level. (range level1~level8)
    /// </summary>

    private XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
    private XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;
    public bool test = false;

    public ScenesData()
    {
        XBikeEventReceiver.connectStatusChangeEvent += OnXBikeConnectionStatusChange;
        XBikeEventReceiver.sportStatusChangeEvent += OnXBikeSportStatusChange;
    }

#if UNITY_EDITOR
    public void SendMessageForEachListener(string method, string arg)
    {
        XBikeEventReceiver.Listener.ForEach((x) =>
        {
            x.SendMessage(method, arg, SendMessageOptions.DontRequireReceiver);
        });
    }
#endif

    /// <summary>
    /// set connection status change event and sport status change event
    /// </summary>
    void OnEnable()
    {
    }

    void Update()
    {

    }

    public XBikeEventReceiver.ConnectionStatus ConnectionStatus
    {
        get
        {
            return connectionStatus;
        }
        set
        {
            connectionStatus = value;
        }
    }

    public XBikeEventReceiver.SportStatus SportStatus
    {
        get
        {
            return sportStatus;
        }
        set
        {
            sportStatus = value;
        }
    }

    public void StopSport()
    {
#if UNITY_EDITOR
        SendMessageForEachListener("OnXBikeSportStatusChange", "0");
#elif UNITY_ANDROID
		XBikeEventReceiver.StopSport();
#endif
    }

    public void StartSport()
    {
#if UNITY_EDITOR
        SendMessageForEachListener("OnXBikeSportStatusChange", "1");
#elif UNITY_ANDROID
		XBikeEventReceiver.StartSport();
#endif
    }

    public void PauseSport()
    {
#if UNITY_EDITOR
        SendMessageForEachListener("OnXBikeSportStatusChange", "2");
#elif UNITY_ANDROID
		XBikeEventReceiver.PauseSport();
#endif
    }

    //要測試是否可以成為事件
    /// <summary>
    /// called when there is a change in status
    /// </summary>
    /// <param name='status'>
    /// "0" = stop / "1" = start / "2" = pause
    /// </param>
    public void OnXBikeSportStatusChange(string status)
    {
        sportStatus = (XBikeEventReceiver.SportStatus)int.Parse(status);
        switch (sportStatus)
        {
            case XBikeEventReceiver.SportStatus.Stop:
                {
                    sportStatus = XBikeEventReceiver.SportStatus.Stop;
                    break;
                }
            case XBikeEventReceiver.SportStatus.Pause:
                {
                    sportStatus = XBikeEventReceiver.SportStatus.Pause;
                    break;
                }
            case XBikeEventReceiver.SportStatus.Start:
                {
                    sportStatus = XBikeEventReceiver.SportStatus.Start;
                    break;
                }
        }
    }

    /// <summary>
	/// called when the connectivity changes
	/// </summary>
	/// <param name='status'>
	/// "0" = disconnect / "1" = connecting / "2" = connected
	/// </param>
	private void OnXBikeConnectionStatusChange(string status)
    {
        connectionStatus = (XBikeEventReceiver.ConnectionStatus)int.Parse(status);
        switch (connectionStatus)
        {
            case XBikeEventReceiver.ConnectionStatus.Connecting:
                {
#if UNITY_EDITOR
                    //				StartCoroutine(WaitTime(3.0f));
#endif
                    break;
                }
            case XBikeEventReceiver.ConnectionStatus.Connected:
                {
                    break;
                }
        }
    }

    public bool Test
    {
        get
        {
            return test;
        }
        set
        {
            test = true;
        }
    }
}

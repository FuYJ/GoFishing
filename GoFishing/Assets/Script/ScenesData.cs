using UnityEngine;
using System.Collections;

public class ScenesData : MonoBehaviour {
    /// <summary>
    /// current resistance value.
    /// resistance has eight level. (range level1~level8)
    /// </summary>

    private XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
    private XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;

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

    private void StopSport()
    {
#if UNITY_EDITOR
        SendMessageForEachListener("OnXBikeSportStatusChange", "0");
#elif UNITY_ANDROID
		XBikeEventReceiver.StopSport();
#endif
    }

    private void StartSport()
    {
#if UNITY_EDITOR
        SendMessageForEachListener("OnXBikeSportStatusChange", "1");
#elif UNITY_ANDROID
		XBikeEventReceiver.StartSport();
#endif
    }

    private void PauseSport()
    {
#if UNITY_EDITOR
        SendMessageForEachListener("OnXBikeSportStatusChange", "2");
#elif UNITY_ANDROID
		XBikeEventReceiver.PauseSport();
#endif
    }
}

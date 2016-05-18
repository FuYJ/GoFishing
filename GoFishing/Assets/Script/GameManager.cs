using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public static XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
	public static XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;
	public float resistanceValue = 1.0f;

	#if UNITY_EDITOR
	public void SendMessageForEachListener(string method, string arg)
	{
		XBikeEventReceiver.Listener.ForEach((x) =>
			{
				x.SendMessage(method, arg, SendMessageOptions.DontRequireReceiver);
			});
	}
	#endif

	void Awake ()
	{
		Instance = this;
	}

	/// <summary>
	/// set connection status change event and sport status change event
	/// </summary>
	void OnEnable()
	{
		XBikeEventReceiver.connectStatusChangeEvent	+= OnXBikeConnectionStatusChange;
		XBikeEventReceiver.sportStatusChangeEvent += OnXBikeSportStatusChange;
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	public void Update () {
	}


		#if UNITY_EDITOR
		public IEnumerator WaitTime(float time)
		{
			yield return new WaitForSeconds(time);
			SendMessageForEachListener("OnXBikeConnectionStatusChange", "2");
		}
		#endif

		/// <summary>
		/// called when the connectivity changes
		/// </summary>
		/// <param name='status'>
		/// "0" = disconnect / "1" = connecting / "2" = connected
		/// </param>
		public void OnXBikeConnectionStatusChange(string status)
		{
			connectionStatus = (XBikeEventReceiver.ConnectionStatus)int.Parse(status);
			switch (connectionStatus)
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
}

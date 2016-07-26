using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;
	public float resistanceValue = 1.0f;
	ScenesData _data = StartScript._data;

	void Awake ()
	{
		Instance = this;
	}

	/// <summary>
	/// set connection status change event and sport status change event
	/// </summary>
	void OnEnable()
	{
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
			_data.SendMessageForEachListener("OnXBikeConnectionStatusChange", "2");
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
		public void OnXBikeSportStatusChange(string status)
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
	
}

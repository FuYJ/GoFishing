using UnityEngine;
using System.Collections;

public class StartScript : MonoBehaviour
{
	/// <summary>
	/// current resistance value.
	/// resistance has eight level. (range level1~level8)
	/// </summary>
	public float resistanceValue = 1.0f;

	private XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
	private XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;
	
#if UNITY_EDITOR
	void SendMessageForEachListener(string method, string arg)
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
		XBikeEventReceiver.connectStatusChangeEvent		+= OnXBikeConnectionStatusChange;
		XBikeEventReceiver.sportStatusChangeEvent		+= OnXBikeSportStatusChange;
	}
	
	void Update ()
	{
#if UNITY_EDITOR
		// Fill fake sport data
		string dataString = "{" +
			"\"rpmDirection\":1," +
			"\"distance\":0," + 
			"\"speed\":0," + 
			"\"watt\":0," + 
			"\"rpm\":150," + 
			"\"resistance\":1," + 
			"\"calories\":0," + 
			"\"leftRightSensor\":186," +
			"\"upDownSensor\":186," +
			"\"bpm\":0}";
		SendMessageForEachListener("OnXBikeDataChange", dataString);
		
		// Pressed mouse left button call OnXBikeLeftPressed true
		if( Input.GetMouseButtonDown(0) )
		{
			SendMessageForEachListener("OnXBikeLeftPressed", "True");
		}
		// Release mouse left button call OnXBikeLeftPressed false
		else if( Input.GetMouseButtonUp(0) )
		{
			SendMessageForEachListener("OnXBikeLeftPressed", "False");
		}
		// Pressed mouse right button call OnXBikeRightPressed true
		if( Input.GetMouseButtonDown(1) )
		{
			SendMessageForEachListener("OnXBikeRightPressed", "True");
		}
		// Release mouse right button call OnXBikeRightPressed false
		else if( Input.GetMouseButtonUp(1) )
		{
			SendMessageForEachListener("OnXBikeRightPressed", "False");
		}
#endif
	}
	
	void OnGUI ()
	{
		if (connectionStatus ==  XBikeEventReceiver.ConnectionStatus.Disconnected)
		{
			if (GUI.Button(new Rect(Screen.width/2 - 50.0f, Screen.height - 50.0f, 100.0f, 30.0f), "Connect"))
			{
#if UNITY_EDITOR
				SendMessageForEachListener("OnXBikeConnectionStatusChange", "1");
#elif UNITY_ANDROID
				XBikeEventReceiver.Connect();
#endif
			}
		}
		else if (connectionStatus == XBikeEventReceiver.ConnectionStatus.Connecting)
		{
			GUI.Label(new Rect(Screen.width/2 - 30.0f, Screen.height/2 - 15.0f, 65.0f, 30.0f), "Connecting");
			if (GUI.Button(new Rect(Screen.width/2 - 50.0f, Screen.height - 50.0f, 100.0f, 30.0f), "Disconnected"))
			{
				Application.LoadLevel ("Stage1Scene");
#if UNITY_EDITOR
				SendMessageForEachListener("OnXBikeConnectionStatusChange", "0");
#elif UNITY_ANDROID
				XBikeEventReceiver.Disconnect();
#endif
			}
		}
		else if (connectionStatus == XBikeEventReceiver.ConnectionStatus.Connected)
		{
			Application.LoadLevel ("Stage1Scene");
			GUI.BeginGroup(new Rect(Screen.width/2 - 50.0f, Screen.height - 100.0f, 100.0f, 100.0f));
			GUILayout.BeginVertical();
			
			if (sportStatus == XBikeEventReceiver.SportStatus.Stop)
			{
				if (GUILayout.Button("Start sport"))
				{
					StartSport();
				}
				if (GUILayout.Button("Disconnected"))
				{
#if UNITY_EDITOR
					SendMessageForEachListener("OnXBikeConnectionStatusChange", "0");
#elif UNITY_ANDROID
					XBikeEventReceiver.Disconnect();
#endif
				}
			}
			else if (sportStatus == XBikeEventReceiver.SportStatus.Start)
			{
				if (GUILayout.Button("Pause sport"))
				{
					PauseSport();
				}
				if (GUILayout.Button("Stop sport"))
				{
					StopSport();
				}
				if (GUILayout.Button("Disconnected"))
				{
#if UNITY_EDITOR
					SendMessageForEachListener("OnXBikeConnectionStatusChange", "0");
#elif UNITY_ANDROID
					XBikeEventReceiver.Disconnect();
#endif
				}
			}
			else if (sportStatus == XBikeEventReceiver.SportStatus.Pause)
			{
				if (GUILayout.Button("Start sport"))
				{
					StartSport();
				}
				if (GUILayout.Button("Disconnected"))
				{
#if UNITY_EDITOR
					SendMessageForEachListener("OnXBikeConnectionStatusChange", "0");
#elif UNITY_ANDROID
					XBikeEventReceiver.Disconnect();
#endif
				}
			}
			GUILayout.EndVertical();
			GUI.EndGroup();
			
			if (sportStatus == XBikeEventReceiver.SportStatus.Start)
			{
				// Show current sport data
				GUI.Label(new Rect(50.0f, 30.0f, 200.0f, Screen.height - 30.0f), XBikeEventReceiver.Data.ToString());
				// Show left an buuton status
				GUI.Label(new Rect(500.0f, 30.0f, 150.0f, 200.0f), "Left Button : " + XBikeEventReceiver.Left.ToString());
				GUI.Label(new Rect(500.0f, 60.0f, 150.0f, 200.0f), "Right Button : " + XBikeEventReceiver.Right.ToString());
				
				resistanceValue = GUI.HorizontalSlider(new Rect(Screen.width/2 - 50.0f, 20.0f, 100.0f, 30.0f), resistanceValue, 1.0f, 8.0f);
				GUI.Label(new Rect(Screen.width/2 - 50.0f, 60.0f, 100.0f, 30.0f), ((int)resistanceValue).ToString());
				if (GUI.Button(new Rect(Screen.width/2 - 50.0f, 90.0f, 100.0f, 30.0f), "Set resistance"))
				{
					XBikeEventReceiver.SetResistance((int)resistanceValue);
				}
			}
		}
	}
#if UNITY_EDITOR
	private IEnumerator WaitTime(float time)
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
	private void OnXBikeConnectionStatusChange(string status)
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
	private void OnXBikeSportStatusChange(string status)
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

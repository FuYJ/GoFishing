using UnityEngine;
using System;
using System.Collections;

public class XBikeLogger : MonoBehaviour
{
	string szLog = string.Empty;
	public float wheelSpeed = 10f;
	
#if UNITY_EDITOR
	float halfScreenWidth = Screen.width >> 1;
#endif

	string output = "";
	string stack= "";
	string Logstring = "";
	void OnEnable ()
	{
		Application.RegisterLogCallback(HandleLog);
	}
	void OnDisable ()
	{
		// Remove callback when object goes out of scope
		Application.RegisterLogCallback(null);
	}
	void HandleLog ( string logString , string stackTrace, LogType type )
	{
		if( type == LogType.Error || type == LogType.Exception)
		{
			output = logString + "\n" + stackTrace + ";\n" + output;
			stack = stackTrace;
		}
	}

	void Start()
	{

	}

	void OnGUI()
	{
//		if( !enabled ) return;
	
		Rect r;
		
		
		r = new Rect(10, 120, Screen.width,(Screen.height-120)/2);
		GUI.color = Color.black;
		GUI.Label(r, output);
		//r.x += (Screen.width/2);
		r.y += (120 + (Screen.height-120)/2);
		GUI.color = Color.white;
		GUI.Label(r, szLog);
	}

	
#if UNITY_EDITOR
	float currentScroll = 0f;
	void Update()
	{
		if( Input.GetMouseButtonDown(0) )
		{
			SendMessageForEachListener("OnXBikeLeftPressed", "True");
		}
		else if( Input.GetMouseButtonUp(0) )
		{
			SendMessageForEachListener("OnXBikeLeftPressed", "False");
		}
		if( Input.GetMouseButtonDown(1) )
		{
			SendMessageForEachListener("OnXBikeRightPressed", "True");
		}
		else if( Input.GetMouseButtonUp(1) )
		{
			SendMessageForEachListener("OnXBikeRightPressed", "False");
		}
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		currentScroll += Mathf.Abs(scroll) * wheelSpeed;
		currentScroll *= 0.99f;
		if(currentScroll > 1f)
		{
			SendMessageForEachListener("OnXBikeDataChange", "{\"rpm\":" + currentScroll + "}");
		}
	}
	
	void SendMessageForEachListener(string method, string arg)
	{
		XBikeEventReceiver.Listener.ForEach(x => {
			//if( x.gameObject.activeInHierarchy )
			{
				x.SendMessage(method, arg, SendMessageOptions.DontRequireReceiver);
			}
		});
	}
#endif
	
	void OnXBikeMessage(string message)
	{
		szLog = "Msg = " + message + "\n" + szLog;
	}
	
	void OnBluetoothStatusChange(string status)
	{
		szLog = "OnBluetoothStatusChange = " + status + "\n" + szLog;
	}
	
	void OnXBikeConnectionStatusChange(string status)
	{
		szLog = "OnXBikeConnectionStatusChange = " + status + "\n" + szLog;
	}
	
	void OnXBikeError(string error)
	{
		szLog = "OnXBikeError = " + error + "\n" + szLog;
	}
	
	void OnXBikeSportStatusChange(string status)
	{
		szLog = "OnXBikeSportStatusChange = " + status + "\n" + szLog;
	}

	void OnXBikeHorizontalSensor( string status )
	{
		szLog = "OnXBikeHorizontalSensor = " + status + "\n" + szLog;
	}

	void OnXBikeDataChange(string data)
	{
		szLog = "Data = " + data;
	}
}

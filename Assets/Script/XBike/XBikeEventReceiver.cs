using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class XBikeEventReceiver : MonoBehaviour
{
	public delegate void ConnectStatusChange(string status);
	public static ConnectStatusChange connectStatusChangeEvent;
	
	public delegate void SportStatusChange(string status);
	public static SportStatusChange sportStatusChangeEvent;
	
	public enum ConnectionStatus
	{
		Disconnected, Connecting, Connected
	}
	
	public enum SportStatus
	{
		Stop, Start, Pause
	}
	
	public class SportData
	{
		/// <summary>
		/// heart beats per minute
		/// </summary>
		[JsonName("bpm")]
		public int BPM { get; private set; }

		/// <summary>
		/// calories burnt (since start)
		/// </summary>
		[JsonName("calories")]
		public int Calories { get; private set; }

		/// <summary>
		/// distance tarveled, km (since start)
		/// </summary>
		[JsonName("distance")]
		public double Distance { get; private set; }

		/// <summary>
		/// Left/Right offset
		/// </summary>
		[JsonName("leftRightSensor")]
		public int LeftRightSensor { get; private set; }

		/// <summary>
		/// resistance
		/// </summary>
		[JsonName("resistance")]
		public int Resistance { get; private set; }

		/// <summary>
		/// raw_RPM_data
		/// </summary>
		[JsonName("rpm")]
		public int RPM { get; private set; }
		
		/// <summary>
		/// Rotational direction, 0 = forward, 1 = reverse
		/// </summary>
		[JsonName("rpmDirection")]
		public int RPMDirection { get; private set; }

		/// <summary>
		/// hour/km
		/// </summary>
		[JsonName("speed")]
		public double Speed { get; private set; }

		/// <summary>
		/// Up/Down offset
		/// </summary>
		[JsonName("upDownSensor")]
		public int UpDownSensor { get; private set; }

		/// <summary>
		/// Watts accumulated (since start)
		/// </summary>
		[JsonName("watt")]
		public int Watt { get; private set; }
		
		override public string ToString()
		{
			return string.Format(
				"BPM = {0}\nRPM = {1}\nRPMDirection = {2}\nDistance = {3}\nSpeed = {4}\nLeftRightSensor = {5}\nUpDownSensor = {6}\nResistance = {7}\nCalories = {8}\nWatt = {9}",
				BPM, RPM, RPMDirection, Distance, Speed, LeftRightSensor, UpDownSensor, Resistance, Calories, Watt
			);
		}
	}
	public static string Temp = "";
//#if UNITY_ANDROID	
	static AndroidJavaObject s_XbikeEventReceiver = null;
//#endif
	static bool isDisconnected = false;
	
	/// <summary>
	/// Connected device name
	/// </summary>
	public static string ConnectedDeviceName
	{
#if UNITY_EDITOR
		get { return "XBIKE9999"; }
#else
		get { return s_XbikeEventReceiver.Call<string>("getConnectedDeviceName"); }
#endif
	}
	
	/// <summary>
	/// 
	/// </summary>
	public static bool IsConnected { get; private set; }
	
	/// <summary>
	/// Get current sport data
	/// </summary>
	public static SportData Data { get; private set; }
	
	/// <summary>
	/// Resistance
	/// </summary>
	public static int Resistance
	{
		get { return Data.Resistance; }
#if UNITY_EDITOR
		set { Debug.Log("Set Resistance = " + value); }
#else
		set 
		{ 
			s_XbikeEventReceiver.Call("setResistance", value); 
		}
#endif
	}
	
#if UNITY_EDITOR
	public static bool Up { get; set; }
	public static bool Down { get; set; }
	public static bool Right { get; set; }
	public static bool Left { get; set; }
	public static bool Key01 { get; set; }
	public static bool Key02 { get; set; }
#elif UNITY_ANDROID
	/// <summary>
	/// XBike panel to the left in the second
	/// </summary>
	public static bool Up { get; private set; }
	/// <summary>
	/// XBike panel to the left in the first
	/// </summary>
	public static bool Down { get; private set; }
	/// <summary>
	/// right red button
	/// </summary>
	public static bool Right { get; private set; }
	/// <summary>
	/// left red button
	/// </summary>
	public static  bool Left { get; private set; }
	/// <summary>
	/// XBike panel to the right in the second
	/// </summary>
	public static bool Key01 { get; private set; }
	/// <summary>
	/// XBike panel to the right in the first
	/// </summary>
	public static bool Key02 { get; private set; }
#endif
	
	/// <summary>
	/// list of devices
	/// </summary>
	public static BluetoothDeviceInfo[] Device
	{
		get
		{
			string json = s_XbikeEventReceiver.Call<string>("getBluetoothDevice");
			return JsonFx.Json.JsonReader.Deserialize<BluetoothDeviceInfo[]>(json);
		}
	}
	
#if UNITY_EDITOR
	static List<XBikeEventReceiver> listener = new List<XBikeEventReceiver>();
	public static List<XBikeEventReceiver> Listener { get { return listener; } }
#endif
	/// <summary>
	/// opens up the list of devices
	/// </summary>
	public static void Connect ()
	{
#if UNITY_EDITOR
		Debug.Log("Connect");
#elif UNITY_ANDROID
		s_XbikeEventReceiver.Call("connect");
#endif
	}
	/// <summary>
	/// setting resistance
	/// </summary>
	/// <param name='value'>
	/// Value.
	/// </param>
	public static void SetResistance ( int value )
	{
#if UNITY_EDITOR
		Debug.Log("Set resistance : " + value);
#elif UNITY_ANDROID
		s_XbikeEventReceiver.Call("setResistance", value);
#endif
	}
	
	/// <summary>
	/// connect XBike bluetooth address
	/// </summary>
	/// <param name='address'>
	/// XBike bluetooth address
	/// </param>
	public static void Connect (string address)
	{
#if UNITY_EDITOR
		Debug.Log("Connect");
#elif UNITY_ANDROID
		s_XbikeEventReceiver.Call("connect");
#endif
	}
	
	/// <summary>
	/// Disconnect XBike bluetooth
	/// </summary>
	public static void Disconnect ()
	{
		if( isDisconnected == false )
		{
			isDisconnected = true;
#if UNITY_EDITOR
			Debug.Log("XBikeEventReceiver disconnected !!");
#elif UNITY_ANDROID
			s_XbikeEventReceiver.Call("disconnect");
#endif
		}
	}
	
	/// <summary>
	/// Start XBike sport data
	/// </summary>
	public static void StartSport()
	{
#if UNITY_EDITOR
		Debug.Log("Start sport");
#elif UNITY_ANDROID
		s_XbikeEventReceiver.Call("start");
#endif
	}
	/// <summary>
	/// Pause XBike sport data
	/// </summary>
	public static void PauseSport()
	{
#if UNITY_EDITOR
		Debug.Log("Pause sport");
#elif UNITY_ANDROID
		s_XbikeEventReceiver.Call("pause");
#endif
	}
	/// <summary>
	/// stop XBike sport data
	/// </summary>
	public static void StopSport()
	{
#if UNITY_EDITOR
		Debug.Log("Stop sport");
#elif UNITY_ANDROID
		s_XbikeEventReceiver.Call("stop");
#endif
	}
	
	void Awake ()
	{
		if ( Data == null ) Data = new SportData();
#if UNITY_EDITOR
		listener.Add(this);
#elif UNITY_ANDROID
		if ( s_XbikeEventReceiver == null )
		{
			s_XbikeEventReceiver = new AndroidJavaObject("com.gamebike.inc.XBikeEventReceiver");
		}
		AddReceiverObject(this.name);
#endif
	}
	/// <summary>
	/// The object name to the list to receive the response data.
	/// </summary>
	/// <param name='objectName'>
	/// Object name.
	/// </param>
	public void AddReceiverObject(string objectName)
	{
		s_XbikeEventReceiver.Call<bool>("addGameObject", objectName);
	}
	
	void OnDestroy()
	{
#if UNITY_EDITOR
		listener.Remove(this);
#elif UNITY_ANDROID
		s_XbikeEventReceiver.Call<bool>("removeGameObject", name);
#endif
	}
	
	void OnApplicationQuit()
	{
        StopSport();
		Disconnect();
	}
	/// <summary>
	/// saves data based on the bike positon every 0.25 seconds
	/// </summary>
	void OnXBikeDataChange(string data)
	{
		Data = JsonFx.Json.JsonReader.Deserialize<SportData>(data);
//		Debug.LogError ( "Data Change" );
	}
	/// <summary>
	/// function called when there is an error
	/// </summary>
	void OnXBikeError(string errorCode)
	{
		Debug.LogError("Bike get error : " + errorCode);
	}
	
	void OnXBikeNoResistance()
	{
		Debug.LogError("Bike get error");
	}
	
	/// <summary>
	/// Pressing up on the ui Panel
	/// </summary>
	/// <param name='press'>
	/// "true" / "false"
	/// </param>
	void OnXBikeUpPressed(string press)
	{
		bool value = bool.Parse(press);
		if ( Up != value )
		{
			Up = value;
		}
	}
	/// <summary>
	/// Pressing down on the ui Panel
	/// </summary>
	/// <param name='press'>
	/// "true" / "false"
	/// </param>
	void OnXBikeDownPressed(string press)
	{
		bool value = bool.Parse(press);
		if ( Down != value )
		{
			Down = value;
		}
	}
	/// <summary>
	/// Pressing Key01 on the ui Panel
	/// </summary>
	/// <param name='press'>
	/// "true" / "false"
	/// </param>
	void OnXBikeKey01Pressed(string press)
	{
		bool value = bool.Parse(press);
		if ( Key01 != value )
		{
			Key01 = value;
		}
	}
	/// <summary>
	/// Pressing Key02 on the ui Panel
	/// </summary>
	/// <param name='press'>
	/// "true" / "false"
	/// </param>
	void OnXBikeKey02Pressed(string press)
	{
		bool value = bool.Parse(press);
		if ( Key02 != value )
		{
			Key02 = value;
		}
	}
	/// <summary>
	/// Right red button pressed
	/// </summary>
	/// <param name='press'>
	/// "true" / "false"
	/// </param>
	void OnXBikeRightPressed(string press)
	{
		bool value = bool.Parse(press);
		if( Right != value )
		{
			Right = value;
		}
	}
	/// <summary>
	/// Left red button pressed
	/// </summary>
	/// <param name='press'>
	/// "true" / "false"
	/// </param>
	void OnXBikeLeftPressed(string press)
	{
		bool value = bool.Parse(press);
		if( Left != value )
		{
			Left = value;
		}
	}
	/// <summary>
	/// called when the connectivity changes
	/// </summary>
	/// <param name='status'>
	/// disconnect/connecting/connected
	/// </param>
	void OnXBikeConnectionStatusChange(string status)
	{
		IsConnected = ((int)ConnectionStatus.Connected == int.Parse(status));
		if (connectStatusChangeEvent != null)
		{
			connectStatusChangeEvent(status);
		}
	}
	/// <summary>
	/// called when there is a change in status
	/// </summary>
	/// <param name='status'>
	/// start/pause/stop
	/// </param>
	void OnXBikeSportStatusChange(string status)
	{
		XBikeEventReceiver.SportStatus sportStatus = (XBikeEventReceiver.SportStatus)int.Parse(status);
		if (sportStatusChangeEvent != null)
		{
			sportStatusChangeEvent(status);
		}
	}
}

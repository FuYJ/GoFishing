using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {


	/*XBike parameters setup*/
	private XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
	private XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;
	public float resistanceValue = 1.0f;

	Transform m_transform;
	public GameObject m_rod;
	public Transform m_rodTransform;
	public CharacterController m_CharacterController;

	/*Player parameter*/
	//private float eyeHeight = 12f;
	private float speed = 5f;
	private PlayerState m_playerState;
	private string playerMode = "boating";


	/*Rod parameter*/
	private Vector3 rodDistance = new Vector3(0, 8, 0);
	private Vector3 rodInitialAngles = new Vector3 (60, 0, 0);
	private Vector3 rodAngles = new Vector3 (60, 0, 0);

	/*Fishing parameters*/
	private bool isRodReady = false;
	private bool isFishing = false;
	private float reelingSpeed = 0f;
	private float timeSlice = 0.1f;
	private float depth = 0f;
	private int fishNumber = 0;
	private int rodPull = 0;

	/*Button Style*/
	public Texture2D buttonTexture;

	/*UI control*/
	public TextMesh meterTitle;
	public TextMesh meterData;


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


	// Use this for initialization
	void Start ()
	{
		m_transform = this.transform;
		m_rodTransform.rotation = m_transform.rotation;

		m_rodTransform.Translate (10000, 10000, 10000);
		m_rod.SetActive (false);

		m_playerState = this.GetComponentsInChildren<PlayerState>()[0];
		m_playerState.PlayerModeChanged += ChangePlayerState;
	}

	// Update is called once per frame
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
		if (playerMode == "boating") {
			Move ();
		} else {
			Fish ();
		}
	}

	void OnGUI ()
	{
		//GUI style setting
		GUIStyle labelStyle = new GUIStyle();
		#if UNITY_EDITOR
		labelStyle.fontSize = 20;
		float buttonWidth = 150.0f;
		#elif UNITY_ANDROID
		labelStyle.fontSize = 50;
		float buttonWidth = 300.0f;
		#endif
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;

		GUIStyle buttonStyle = new GUIStyle();
		#if UNITY_EDITOR
		buttonStyle.fontSize = 20;
		buttonStyle.fixedWidth = 120;
		#elif UNITY_ANDROID
		buttonStyle.fontSize = 50;
		buttonStyle.fixedWidth = 300;
		#endif
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		//buttonStyle.wordWrap = true;
		buttonStyle.stretchWidth = true;
		buttonStyle.normal.textColor = Color.black;
		buttonStyle.normal.background = buttonTexture;

		if (connectionStatus ==  XBikeEventReceiver.ConnectionStatus.Disconnected)
		{
			if (GUI.Button(new Rect(Screen.width/2 - buttonWidth / 2, Screen.height - 100.0f, buttonWidth, 40.0f), "Connect", buttonStyle))
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
			GUI.Label(new Rect(Screen.width/2 - buttonWidth / 2, Screen.height/2 - 30.0f, buttonWidth, 40.0f), "Connecting", labelStyle);
			if (GUI.Button(new Rect(Screen.width/2 - buttonWidth / 2, Screen.height - 100.0f, buttonWidth, 40.0f), "Disconnected", buttonStyle))
			{
				#if UNITY_EDITOR
				SendMessageForEachListener("OnXBikeConnectionStatusChange", "0");
				#elif UNITY_ANDROID
				XBikeEventReceiver.Disconnect();
				#endif
			}
		}
		else if (connectionStatus == XBikeEventReceiver.ConnectionStatus.Connected)
		{
			GUI.BeginGroup(new Rect(Screen.width/2 - buttonWidth / 2, Screen.height - 200.0f, buttonWidth, 100.0f));
			GUILayout.BeginVertical();

			if (sportStatus == XBikeEventReceiver.SportStatus.Stop)
			{
				if (GUILayout.Button("Start sport", buttonStyle))
				{
					StartSport();
				}
				if (GUILayout.Button("Disconnected", buttonStyle))
				{
					#if UNITY_EDITOR
					SendMessageForEachListener("OnXBikeConnectionStatusChange", "0");
					#elif UNITY_ANDROID
					XBikeEventReceiver.Disconnect();
					#endif
				}
			}
			/*else if (sportStatus == XBikeEventReceiver.SportStatus.Start)
		{
				if (GUILayout.Button("Pause sport", buttonStyle))
			{
				PauseSport();
			}
				if (GUILayout.Button("Stop sport", buttonStyle))
			{
				StopSport();
			}
				if (GUILayout.Button("Disconnected", buttonStyle))
			{
				#if UNITY_EDITOR
				SendMessageForEachListener("OnXBikeConnectionStatusChange", "0");
				#elif UNITY_ANDROID
				XBikeEventReceiver.Disconnect();
				#endif
			}
		}*/
			else if (sportStatus == XBikeEventReceiver.SportStatus.Pause)
			{
				if (GUILayout.Button("Start sport", buttonStyle))
				{
					StartSport();
				}
				if (GUILayout.Button("Disconnected", buttonStyle))
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
				//GUI.Label(new Rect(30.0f, 30.0f, 200.0f, Screen.height - 30.0f), XBikeEventReceiver.Data.ToString(), buttonStyle);
				// Show left an buuton status
				//GUI.Label(new Rect(500.0f, 30.0f, 150.0f, 200.0f), "Left Button : " + XBikeEventReceiver.Left.ToString(), style);
				//GUI.Label(new Rect(500.0f, 60.0f, 150.0f, 200.0f), "Right Button : " + XBikeEventReceiver.Right.ToString(), style);

				/*if(GUI.Button(new Rect(Screen.width - 200.0f, 30.0f, buttonWidth, 50.0f), playerMode, buttonStyle)){
					if (playerMode == "划船中") {
						playerMode = "釣魚中";
						Vector3 pos = m_transform.position + rodDistance;
						m_rodTransform.position = pos;
						m_rodTransform.rotation = m_transform.rotation;
						m_rodTransform.eulerAngles += rodInitialAngles;
						m_rod.SetActive (true);
					} else {
						playerMode = "划船中";
						m_rodTransform.Translate (10000, 10000, 10000);
						m_rod.SetActive (false);
					}
			}
				GUI.Label (new Rect (Screen.width / 2 - 50.0f, 30.0f, 100.0f, 30.0f), "已經釣到了 " + fishNumber.ToString () + " 隻魚", labelStyle);
				GUI.Label (new Rect (Screen.width / 2 - 50.0f, 90.0f, 100.0f, 30.0f), "魚餌深度 : " + depth.ToString (), labelStyle);
				GUI.Label (new Rect (Screen.width / 2 - 50.0f, 150.0f, 100.0f, 30.0f), "捲線速度 : " + reelingSpeed.ToString (), labelStyle);
				GUI.Label (new Rect (Screen.width - 200.0f, 120.0f, 100.0f, 30.0f), "拋線準備 : " + isRodReady.ToString (), labelStyle);
				GUI.Label (new Rect (Screen.width - 200.0f, 170.0f, 100.0f, 30.0f), "正在釣魚 : " + isFishing.ToString (), labelStyle);
				resistanceValue = GUI.HorizontalSlider(new Rect(Screen.width/2 - 50.0f, 20.0f, 100.0f, 30.0f), resistanceValue, 1.0f, 8.0f);
				GUI.Label(new Rect(Screen.width/2 - 50.0f, 60.0f, 100.0f, 30.0f), "Resistance Value : " + ((int)resistanceValue).ToString(), labelStyle);*/
				/*if (GUI.Button(new Rect(Screen.width/2 - 50.0f, 90.0f, 100.0f, 30.0f), "Set resistance"))
			{
				XBikeEventReceiver.SetResistance((int)resistanceValue);
			}*/
			}
		}
	}

	void Move ()
	{
		float move = 0;
		float rot = 0;
		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.W)) {
			move += speed * 5 * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.S)) {
			move -= speed * 5 * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.D)) {
			rot += speed * 5 * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.A)) {
			rot -= speed * 5 * Time.deltaTime;
		}
		#elif UNITY_ANDROID
		if(sportStatus == XBikeEventReceiver.SportStatus.Start){
			if((int)XBikeEventReceiver.Data.RPMDirection == 1)
				move += speed * (float)XBikeEventReceiver.Data.Speed * Time.deltaTime;
			else
				move -= speed * (float)XBikeEventReceiver.Data.Speed * Time.deltaTime;
			rot += speed * ((((float)XBikeEventReceiver.Data.LeftRightSensor - 180)) / 10) * Time.deltaTime;
		}
		#endif
		move = (move < 8) ? move : 8;

		m_CharacterController.Move (m_transform.TransformDirection (new Vector3 (0, 0, move)));
		m_transform.eulerAngles += new Vector3 (0, rot, 0);

		/*Update meter data*/
		meterData.text = ((int)Mathf.Abs((move * 10))).ToString();
	}

	void Fish ()
	{

		/*
		 	Fish would escape according to its weight
		*/
		timeSlice -= Time.deltaTime;
		if (timeSlice <= 0) {
			timeSlice = 0.1f;
			if (isFishing) {
				reelingSpeed -= 5f;
				depth -= reelingSpeed;
			}
		}

		/*
			If depth <= 0, then player got a fish,
			else if depth > 30000, then the fish escaped.
		*/
		if (depth <= 0 && isFishing) {
			isFishing = false;
			reelingSpeed = 0f;
			m_rodTransform.rotation = m_transform.rotation;
			m_rodTransform.eulerAngles += rodInitialAngles;
			fishNumber++;
			rodPull = 0;
			depth = 0f;
		} else if (depth > 30000f && isFishing) {
			isFishing = false;
			reelingSpeed = 0f;
			m_rodTransform.rotation = m_transform.rotation;
			m_rodTransform.eulerAngles += rodInitialAngles;
			rodPull = 0;
			depth = 0f;
		}

		rodAngles.x = 0;
		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.W) && !isRodReady && !isFishing) {
			rodPull++;
			rodAngles.x = -1;
			//Debug.Log(m_rodTransform.eulerAngles.x);
			if(m_rodTransform.eulerAngles.x > 270)
				isRodReady = true;
		}
		if (Input.GetKey (KeyCode.S) && isRodReady && !isFishing) {
			rodPull--;
			rodAngles.x = 2;
			if(rodPull < 0){
				isRodReady = false;
				isFishing = true;
				depth = 300f;
				reelingSpeed = 0f;
			}
		}
		if (Input.GetKey (KeyCode.D) && isFishing) {
			reelingSpeed += 2f;
		}
		if (Input.GetKey (KeyCode.A) && isFishing) {
			reelingSpeed += 2f;
		}
		#elif UNITY_ANDROID
		if(sportStatus == XBikeEventReceiver.SportStatus.Start){
			if(!isRodReady && !isFishing){
				rodAngles.x = (Mathf.Abs((int)XBikeEventReceiver.Data.UpDownSensor - 180) > 5) ? (((int)XBikeEventReceiver.Data.UpDownSensor > 0) ? 185 - (int)XBikeEventReceiver.Data.UpDownSensor : 175 - (int)XBikeEventReceiver.Data.UpDownSensor) : 0;
			}
			if(m_rodTransform.eulerAngles.x > 270 && !isRodReady && !isFishing){
				isRodReady = true;
			}
			if((bool)XBikeEventReceiver.Right && isRodReady && !isFishing){
				isRodReady = false;
				isFishing = true;
				depth = 300f;
			}
			if((int)XBikeEventReceiver.Data.RPMDirection == 1 && isFishing)
				reelingSpeed += (float)XBikeEventReceiver.Data.Speed/10;
			else if((int)XBikeEventReceiver.Data.RPMDirection == 0 && isFishing)
				reelingSpeed -= (float)XBikeEventReceiver.Data.Speed/10;
		}
		#endif
		m_rodTransform.eulerAngles += rodAngles;
	}

	void ChangePlayerState(){
		if (playerMode == "boating") {
			playerMode = "fishing";
			Vector3 pos = m_transform.position + rodDistance;
			m_rodTransform.position = pos;
			m_rodTransform.rotation = m_transform.rotation;
			m_rodTransform.eulerAngles += rodInitialAngles;
			m_rod.SetActive (true);
		} else {
			playerMode = "boating";
			m_rodTransform.Translate (10000, 10000, 10000);
			m_rod.SetActive (false);
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

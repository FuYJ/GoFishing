using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
	/*XBike parameters setup*/
	private XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
	private XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;
	public float resistanceValue = 1.0f;

	Transform m_transform;
	public Transform m_camTransform;
	public Transform m_boatTransform;
	public GameObject m_rod;
	public Transform m_rodTransform;
	public CharacterController m_boatCharacterController;

	/*Player parameter*/
	private float eyeHeight = 12f;
	private float speed = 5f;
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

		Vector3 pos = m_transform.position;
		m_boatTransform.position = pos;
		pos.y += eyeHeight;
		m_camTransform.position = pos;

		m_camTransform.rotation = m_transform.rotation;
		m_boatTransform.rotation = m_transform.rotation;
		m_rodTransform.rotation = m_transform.rotation;

		m_rodTransform.Translate (10000, 10000, 10000);
		m_rod.SetActive (false);
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
		#elif UNITY_ANDROID
		labelStyle.fontSize = 50;
		#endif
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;

		GUIStyle buttonStyle = new GUIStyle();
		#if UNITY_EDITOR
		buttonStyle.fontSize = 20;
		#elif UNITY_ANDROID
		buttonStyle.fontSize = 50;
		#endif
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		//buttonStyle.wordWrap = true;
		buttonStyle.fixedWidth = 120;
		buttonStyle.stretchWidth = true;
		buttonStyle.normal.textColor = Color.black;
		buttonStyle.normal.background = buttonTexture;

		if (connectionStatus ==  XBikeEventReceiver.ConnectionStatus.Disconnected)
		{
			if (GUI.Button(new Rect(Screen.width/2 - 75.0f, Screen.height - 50.0f, 150.0f, 30.0f), "Connect", buttonStyle))
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
			GUI.Label(new Rect(Screen.width/2 - 75.0f, Screen.height/2 - 15.0f, 150.0f, 30.0f), "Connecting", labelStyle);
			if (GUI.Button(new Rect(Screen.width/2 - 75.0f, Screen.height - 60.0f, 150.0f, 30.0f), "Disconnected", buttonStyle))
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
		GUI.BeginGroup(new Rect(Screen.width/2 - 60.0f, Screen.height - 100.0f, 120.0f, 100.0f));
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

				if(GUI.Button(new Rect(500.0f, 90.0f, 100.0f, 30.0f), playerMode, buttonStyle)){
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
				GUI.Label (new Rect (Screen.width / 2 - 50.0f, 60.0f, 100.0f, 30.0f), "Fish Number" + fishNumber.ToString (), labelStyle);
				GUI.Label (new Rect (Screen.width / 2 - 50.0f, 100.0f, 100.0f, 30.0f), "Depth" + depth.ToString (), labelStyle);
				GUI.Label (new Rect (Screen.width / 2 - 50.0f, 140.0f, 100.0f, 30.0f), "Reeling speed" + reelingSpeed.ToString (), labelStyle);
				GUI.Label (new Rect (500.0f, 70.0f, 100.0f, 30.0f), "isRodReady" + isRodReady.ToString (), labelStyle);
				GUI.Label (new Rect (500.0f, 130.0f, 100.0f, 30.0f), "isFishing" + isFishing.ToString (), labelStyle);
			//resistanceValue = GUI.HorizontalSlider(new Rect(Screen.width/2 - 50.0f, 20.0f, 100.0f, 30.0f), resistanceValue, 1.0f, 8.0f);
				GUI.Label(new Rect(Screen.width/2 - 50.0f, 60.0f, 100.0f, 30.0f), "Resistance Value : " + ((int)resistanceValue).ToString(), labelStyle);
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

		m_boatCharacterController.Move (m_boatTransform.TransformDirection (new Vector3 (0, 0, move)));
		Vector3 pos = m_boatTransform.position;
		m_transform.position = pos;
		m_rodTransform.position = pos + rodDistance;
		pos.y += eyeHeight;
		m_camTransform.position = pos;
		m_rodTransform.eulerAngles += new Vector3 (0, rot, 0);
		m_transform.eulerAngles += new Vector3 (0, rot, 0);
		m_camTransform.eulerAngles += new Vector3 (0, rot, 0);
		m_boatTransform.eulerAngles += new Vector3 (0, rot, 0);
	}

	void Fish ()
	{
		timeSlice -= Time.deltaTime;
		if (timeSlice <= 0) {
			timeSlice = 0.1f;
			if (isFishing) {
				reelingSpeed -= 5f;
				depth -= reelingSpeed;
			}
		}
		if (depth <= 0 && isFishing) {
			isFishing = false;
			reelingSpeed = 0f;
			m_rodTransform.rotation = m_transform.rotation;
			m_rodTransform.eulerAngles += rodInitialAngles;
			fishNumber++;
			rodPull = 0;
			depth = 0f;
		} else if (depth > 3000f && isFishing) {
			isFishing = false;
			reelingSpeed = 0f;
			m_rodTransform.rotation = m_transform.rotation;
			m_rodTransform.eulerAngles += rodInitialAngles;
			rodPull = 0;
			depth = 0f;
		}
		#if UNITY_EDITOR
		rodAngles.x = 0;
		if (Input.GetKey (KeyCode.W) && !isRodReady && !isFishing) {
			rodPull++;
			rodAngles.x = -1;
			if(rodPull > 60)
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
			if(!isRodReady){
				m_rodTransform.rotation = m_transform.rotation;
				Vector3 rot = rodInitialAngles;
				rot.x -= 180 * ((int)XBikeEventReceiver.Data.UpDownSensor - 180);
				m_rodTransform.eulerAngles += rot; 
			}
			if((int)XBikeEventReceiver.Data.UpDownSensor / 10 > 19 && !isRodReady && !isFishing){
				isRodReady = true;
			}
			if((bool)XBikeEventReceiver.Right && isRodReady && !isFishing){
				isRodReady = false;
				isFishing = true;
				rodAngles.x = 2;
				depth = 300f;
				reelingSpeed = 0f;
			}
			/*if((int)XBikeEventReceiver.Data.UpDownSensor / 10 < 17 && isRodReady && !isFishing){
				isRodReady = false;
				isFishing = true;
				depth = 300f;
				isRodReady = true; 
				rodAngles.x = 3;
			}*/
		if((int)XBikeEventReceiver.Data.RPMDirection == 1 && isFishing)
				reelingSpeed += (float)XBikeEventReceiver.Data.Speed / 10;
			else
				reelingSpeed -= (float)XBikeEventReceiver.Data.Speed / 10;
		}
		#endif
		m_rodTransform.eulerAngles += rodAngles;
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

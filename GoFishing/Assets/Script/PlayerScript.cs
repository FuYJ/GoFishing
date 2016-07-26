using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	/*Constants*/
	public const string BOATING_STATE = "boating";
	public const string WATING_FISH_STATE = "wating fish";
	public const string FISHING_STATE = "fishing";
	public const float MAX_MOVE_SPEED = 0.8f;
	public const float MAX_ROD_ANGLE = 30f;
	public const float MAX_REEL_SPEED = 100f;

	/*XBike parameters setup*/
	public float resistanceValue = 1.0f;

	/*Player's gameobjects*/
	Transform m_transform;
	public GameObject m_rod;
	public Transform m_rodTransform;
	public CharacterController m_CharacterController;
	public AudioSource m_waterFlowSound;

	/*Player parameter*/
	//private float eyeHeight = 12f;
	private float _speed = 2.5f;
	private PlayerState _playerState;
	private string _playerMode = "boating";

	/*Rod parameter*/
	private Vector3 _rodDistance = new Vector3(0, 8, 0);
	private Vector3 _rodInitialAngles = new Vector3 (60, 0, 0);
	private Vector3 _rodAngles = new Vector3 (60, 0, 0);

	/*Bait parameter*/
	public GameObject m_bait;
	public Transform m_baitTransform;
	public Rigidbody m_rodRigidbody;
	public Rigidbody m_baitRigidbody;

	/*Boating parameters*/
	private float _moveSpeed = 0f;
	private float _rotSpeed = 0f;

	/*Fishing parameters*/
	private bool _isRodReady = false;
	private bool _isFishing = false;
	private float _reelingSpeed = 0f;
	private float _timeSlice = 0.1f;
	private float _depth = 0f;
	private int _cachesNumber = 0;
	private float _rodPull = 0f;
	private float _fishEscapeSpeed = 1f;

	/*Button Style*/
	public Texture2D m_buttonTexture;

	/*UI control*/
	public TextMesh m_meterTitle;
	public TextMesh m_meterData;

	/*Fish gotten event*/
	public delegate void FishGottenEventHandler();
	public event FishGottenEventHandler FishGotten;

	/*Meter value changed event*/
	public delegate void MeterValueChangedEventHandler();
	public event MeterValueChangedEventHandler MeterValueChanged;

	/*Player mode changed event*/
	public delegate void PlayerModeChangedEventHandler();
	public event PlayerModeChangedEventHandler PlayerModeChanged;

	public ScenesData _data = StartScript._data;

	/* Player Properties*/
	public int CachesNumber{
		get{
			return _cachesNumber;
		}
	}

	public string PlayerMode{
		get{
			return _playerMode;
		}
	}

	public float MoveSpeed{
		get{
			return _moveSpeed;
		}
	}

	public float RodPullAngle{
		get{
			return _rodPull;
		}
	}

	public float ReelingSpeed{
		get{
			return _reelingSpeed;
		}
	}

	/// <summary>
	/// set connection status change event and sport status change event
	/// </summary>
	void OnEnable()
	{
	}


	// Use this for initialization
	void Start ()
	{
		m_transform = this.transform;
		m_rodTransform.rotation = m_transform.rotation;

		m_rodTransform.Translate (10000, 10000, 10000);
		m_rod.SetActive (false);

		_playerState = this.GetComponentsInChildren<PlayerState>()[0];
		_playerState.PlayerModeChanged += ChangePlayerState;
	}

	// Update is called once per frame
	void Update ()
	{
		if (_playerMode == BOATING_STATE) {
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
		buttonStyle.fontSize = 200;
		buttonStyle.fixedWidth = 120;
		#elif UNITY_ANDROID
		buttonStyle.fontSize = 500;
		buttonStyle.fixedWidth = 300;
		#endif
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		//buttonStyle.wordWrap = true;
		buttonStyle.stretchWidth = true;
		buttonStyle.normal.textColor = Color.black;
		buttonStyle.normal.background = m_buttonTexture;


			//if (_data.SportStatus == XBikeEventReceiver.SportStatus.Start)
			//{
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
			//}
		//}
	}

	void Move ()
	{
		_moveSpeed = 0f;
		_rotSpeed = 0f;
		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.W)) {
			_moveSpeed += _speed * 5 * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.S)) {
			_moveSpeed -= _speed * 5 * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.D)) {
			_rotSpeed += _speed * 5 * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.A)) {
			_rotSpeed -= _speed * 5 * Time.deltaTime;
		}
		#elif UNITY_ANDROID
		if(_data.SportStatus == XBikeEventReceiver.SportStatus.Start){
			if((int)XBikeEventReceiver.Data.RPMDirection == 1)
				move += speed * (float)XBikeEventReceiver.Data.Speed * Time.deltaTime;
			else
				move -= speed * (float)XBikeEventReceiver.Data.Speed * Time.deltaTime;
			//rot += speed * ((((float)XBikeEventReceiver.Data.LeftRightSensor - 180)) / 10) * Time.deltaTime;
			rot += speed * Time.deltaTime * (float)((Mathf.Abs((int)XBikeEventReceiver.Data.LeftRightSensor - 180) > 5) ? (((int)XBikeEventReceiver.Data.LeftRightSensor > 0) ? 185 - (int)XBikeEventReceiver.Data.LeftRightSensor : 175 - (int)XBikeEventReceiver.Data.LeftRightSensor) : 0);
		}
		#endif
		if((_moveSpeed != 0f || _rotSpeed != 0f) && !m_waterFlowSound.isPlaying)
			m_waterFlowSound.Play();

		_moveSpeed = (_moveSpeed < MAX_MOVE_SPEED) ? _moveSpeed : MAX_MOVE_SPEED;
		m_CharacterController.Move (m_transform.TransformDirection (new Vector3 (0, 0, _moveSpeed)));
		m_transform.eulerAngles += new Vector3 (0, _rotSpeed, 0);

		/*Update meter data*/
		//m_meterData.text = ((int)Mathf.Abs((_moveSpeed * 10))).ToString();
		NotifyMeterValueChanged ();
	}

	void Fish ()
	{
		/*
		 	Fish would escape according to its weight
		*/
		_timeSlice -= Time.deltaTime;
		if (_timeSlice <= 0) {
			_timeSlice = 0.1f;
			if (_isFishing) {
				_fishEscapeSpeed = 25f;
				_depth += _fishEscapeSpeed;
				_depth -= _reelingSpeed;
			}
		}

		/*
			If depth <= 0, then player got a fish,
			else if depth > 30000, then the fish escaped.
		*/
		if (_depth <= 0 && _isFishing) {
			_cachesNumber++;
			ResetRod ();
			NotifyScoreChanged ();
		} else if (_depth > 30000f && _isFishing) {
			ResetRod ();
		}

		_rodAngles.x = 0;
		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.W) && !_isFishing) {
			if(_rodPull < MAX_ROD_ANGLE){
				_rodPull++;
				_rodAngles.x = -2;
				_isRodReady = true;
			}
		}
		if(Input.GetKey(KeyCode.S) && _isRodReady && !_isFishing){
			_isRodReady = false;
			_isFishing = true;
			_depth = 300f;
			_playerMode = FISHING_STATE;
			_rodPull = Mathf.Max(Mathf.Min(_rodPull, 30), 0);
			m_baitRigidbody.AddForce(new Vector3(0, 500, 6000 + _rodPull * 300));
			NotifyPlayerModeChanged ();
		}
		if (Input.GetKey (KeyCode.D) && _isFishing) {
			_reelingSpeed = 50f;
		}
		if (Input.GetKey (KeyCode.A) && _isFishing) {
			_reelingSpeed = 50f;
		}
		#elif UNITY_ANDROID
		/*UpDown Sensor Region about 180 ~ 210*/
		if(_data.SportStatus == XBikeEventReceiver.SportStatus.Start){
			/*
			if(!isRodReady && !isFishing){
				rodAngles.x = (Mathf.Abs((int)XBikeEventReceiver.Data.UpDownSensor - 180) > 5) ? (((int)XBikeEventReceiver.Data.UpDownSensor > 0) ? 185 - (int)XBikeEventReceiver.Data.UpDownSensor : 175 - (int)XBikeEventReceiver.Data.UpDownSensor) : 0;
			}
			if(m_rodTransform.eulerAngles.x > 180 && !isRodReady && !isFishing){
				isRodReady = true;
			}
			if((bool)XBikeEventReceiver.Right && isRodReady && !isFishing){
				isRodReady = false;
				isFishing = true;
				depth = 300f;
				m_baitRigidbody.AddForce(new Vector3(0, 500, 15000));
				_rodPull = (float)XBikeEventReceiver.Data.UpDownSensor - 180;
				_playerMode = FISHING_STATE;
				NotifyPlayerModeChanged ();
			}
			*/
			
			/*No limit of whether rod is ready*/
			if(!isFishing){
				rodAngles.x = (Mathf.Abs((int)XBikeEventReceiver.Data.UpDownSensor - 180) > 5) ? (((int)XBikeEventReceiver.Data.UpDownSensor > 0) ? 185 - (int)XBikeEventReceiver.Data.UpDownSensor : 175 - (int)XBikeEventReceiver.Data.UpDownSensor) : 0;
			}
			if((bool)XBikeEventReceiver.Right && !isFishing){
				isFishing = true;
				depth = 300f;
				_rodPull = (float)XBikeEventReceiver.Data.UpDownSensor - 180;
				m_baitRigidbody.AddForce(new Vector3(0, 500, 6000 + Mathf.Max(Mathf.Min(_rodPull, 30), 0) * 300));
				_playerMode = FISHING_STATE;
				NotifyPlayerModeChanged ();
			}

			if((int)XBikeEventReceiver.Data.RPMDirection == 1 && isFishing)
				reelingSpeed += (float)XBikeEventReceiver.Data.Speed/10;
			else if((int)XBikeEventReceiver.Data.RPMDirection == 0 && isFishing)
				reelingSpeed -= (float)XBikeEventReceiver.Data.Speed/10;
		}
		#endif
		m_rodTransform.eulerAngles += _rodAngles;
		NotifyMeterValueChanged ();
	}

	/*Reset rod*/
	void ResetRod(){
		_isFishing = false;
		_isRodReady = false;
		_reelingSpeed = 0f;
		m_rodTransform.rotation = m_transform.rotation;
		m_rodTransform.eulerAngles += _rodInitialAngles;
		_rodPull = 0;
		_depth = 0f;
		ResetBait ();
		NotifyPlayerModeChanged ();
	}

	/*Reset bait*/
	void ResetBait(){
		m_baitTransform.localPosition = new Vector3 (0f, 18f, 1.04f);
		m_baitRigidbody.isKinematic = false;
		m_baitRigidbody.useGravity = true;
		if (!m_bait.GetComponent<HingeJoint> ()) {
			HingeJoint baitHingeJoint = m_bait.AddComponent<HingeJoint> ();
			baitHingeJoint.connectedBody = m_rodRigidbody;
			baitHingeJoint.anchor = new Vector3 (0f, 0.5f, 0f);
			baitHingeJoint.breakForce = 100f;
		}
	}

	void ChangePlayerState(){
		if (_playerMode == BOATING_STATE) {
			_playerMode = WATING_FISH_STATE;
			Vector3 pos = m_transform.position + _rodDistance;
			m_rodTransform.position = pos;
			m_rod.SetActive (true);
			ResetRod ();
		} else {
			_playerMode = BOATING_STATE;
			m_rodTransform.Translate (10000, 10000, 10000);
			m_rod.SetActive (false);
		}
		NotifyPlayerModeChanged ();
	}


	/*Notify Observers*/
	void NotifyScoreChanged(){
		if (FishGotten != null) {
			FishGotten ();
		}
	}

	void NotifyMeterValueChanged(){
		if (MeterValueChanged != null) {
			MeterValueChanged ();
		}
	}

	void NotifyPlayerModeChanged(){
		if (PlayerModeChanged != null) {
			PlayerModeChanged ();
		}
	}












	#if UNITY_EDITOR
	private IEnumerator WaitTime(float time)
	{
		yield return new WaitForSeconds(time);
		_data.SendMessageForEachListener("OnXBikeConnectionStatusChange", "2");
	}
	#endif
}

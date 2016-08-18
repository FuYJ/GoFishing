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
	public const float MAX_FISH_DEPTH = 2000f;

	/*XBike parameters setup*/
	public float resistanceValue = 1.0f;

	/*Player's gameobjects*/
	Transform m_transform;
	public GameObject m_rod;
	public GameObject m_fishingProgressBar;
	public GameObject m_fishHookedAnimation;
	public Transform m_rodTransform;
	public CharacterController m_CharacterController;

	/*Player parameter*/
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
	private float _journey = 0f;

	/*Fishing parameters*/
	private bool _isRodReady = false;
	private bool _isFishing = false;
	private bool _isBaitInWater = false;
	private float _reelingSpeed = 0f;
	private float _timeSlice = 0.1f;
	private float _timeSlicePerRound = 5f;
	private float _fishDepth = 0f;
	private int _cachesNumber = 0;
	private float _rodPull = 0f;
	private float _fishEscapeSpeed = 1f;
	private float _hookProbability = 0.3f;
	private float _fishWeight = 0f;
	private Block[] _blocks;
	private Bait _bait;

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

	/*Hook Probability changed event*/
	public delegate void HookProbabilityChangedEventHandler();
	public event HookProbabilityChangedEventHandler HookProbabilityChanged;

	/*Fish depth changed event*/
	public delegate void FishDepthChangedEventHandler();
	public event FishDepthChangedEventHandler FishDepthChanged;

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

	public float HookProbability{
		get{
			return _hookProbability * 100;
		}
	}

	public float FishDepth{
		get{
			return _fishDepth;
		}
	}

	public float FishWeight{
		get{
			return _fishWeight;
		}
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

		GameObject[] _blocksObject;
		_blocksObject = GameObject.FindGameObjectsWithTag ("Block");
		_blocks = new Block[_blocksObject.Length];
		for (int i = 0; i < _blocksObject.Length; i++) {
			_blocks [i] = _blocksObject [i].GetComponent<Block> ();
			_blocks [i].HookProbabilityChanged += ChangeHookProbability;
		}

		_bait = m_bait.GetComponent<Bait> ();
		_bait.BaitTouchedWater += BaitTouchWater;
		_bait.BaitTouchedWater += NotifyHookProbabilityChanged;
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
		if(GameManager.Instance.SportStatus == XBikeEventReceiver.SportStatus.Start){
			if((int)XBikeEventReceiver.Data.RPMDirection == 1)
				_moveSpeed += _speed * Mathf.Log((float)XBikeEventReceiver.Data.Speed) * Time.deltaTime;
			else
				_moveSpeed -= _speed * Mathf.Log((float)XBikeEventReceiver.Data.Speed) * Time.deltaTime;
			//rot += _speed * ((((float)XBikeEventReceiver.Data.LeftRightSensor - 180)) / 10) * Time.deltaTime;
			_rotSpeed -= _speed * Time.deltaTime * (float)((Mathf.Abs((int)XBikeEventReceiver.Data.LeftRightSensor - 180) > 5) ? (((int)XBikeEventReceiver.Data.LeftRightSensor > 0) ? 185 - (int)XBikeEventReceiver.Data.LeftRightSensor : 175 - (int)XBikeEventReceiver.Data.LeftRightSensor) : 0);
		}
		#endif
		if(_moveSpeed != 0f || _rotSpeed != 0f)
			SoundManager.Instance.PlayWaterFlowSound();

		_moveSpeed = (_moveSpeed < MAX_MOVE_SPEED) ? _moveSpeed : MAX_MOVE_SPEED;
		m_CharacterController.Move (m_transform.TransformDirection (new Vector3 (0, 0, _moveSpeed)));
		m_transform.eulerAngles += new Vector3 (0, _rotSpeed, 0);

		/*Update meter data*/
		//m_meterData.text = ((int)Mathf.Abs((_moveSpeed * 10))).ToString();
		NotifyMeterValueChanged ();
	}

	void Fish ()
	{
		HookFish ();
		UpdateFishDepth ();
		CheckFishHookedOrEscaped ();

		_rodAngles.x = 0;
		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.W) && !_isRodReady && !_isFishing) {
			if(_rodPull < MAX_ROD_ANGLE){
				_rodPull++;
				_rodAngles.x = -2;
			}
		}
		if(Input.GetKey(KeyCode.S) && !_isRodReady && !_isFishing){
			_isRodReady = true;
			_rodPull = Mathf.Max(Mathf.Min(_rodPull, 30), 0);
			m_baitRigidbody.AddForce(new Vector3(0, 500, 6000 + _rodPull * 300));
		}
		if (Input.GetKey (KeyCode.D) && _isFishing) {
			SoundManager.Instance.PlayReelingSound();
			_reelingSpeed += 0.5f;
		}
		if (Input.GetKey (KeyCode.A) && _isFishing) {
			SoundManager.Instance.PlayReelingSound();
			_reelingSpeed -= 0.5f;
		}
		#elif UNITY_ANDROID
		/*UpDown Sensor Region about 180 ~ 210*/
		if(GameManager.Instance.SportStatus == XBikeEventReceiver.SportStatus.Start){
			if(!_isFishing){
				_rodAngles.x = (Mathf.Abs((int)XBikeEventReceiver.Data.UpDownSensor - 180) > 5) ? (((int)XBikeEventReceiver.Data.UpDownSensor > 0) ? 185 - (int)XBikeEventReceiver.Data.UpDownSensor : 175 - (int)XBikeEventReceiver.Data.UpDownSensor) : 0;
			}
			if((bool)XBikeEventReceiver.Right && !_isRodReady && !_isFishing){
				_isRodReady = true;
				_rodPull = (float)XBikeEventReceiver.Data.UpDownSensor - 180;
				m_baitRigidbody.AddForce(new Vector3(0, 500, 6000 + Mathf.Max(Mathf.Min(_rodPull, 30), 0) * 300));
			}

			if((int)XBikeEventReceiver.Data.RPMDirection == 1 && _isFishing){
				SoundManager.Instance.PlayReelingSound();
				_reelingSpeed += Mathf.Log((float)XBikeEventReceiver.Data.Speed);
			}
			else if((int)XBikeEventReceiver.Data.RPMDirection == 0 && _isFishing){
				SoundManager.Instance.PlayReelingSound();
				_reelingSpeed -= Mathf.Log((float)XBikeEventReceiver.Data.Speed);
			}
		}
		#endif
		m_rodTransform.eulerAngles += _rodAngles;
		NotifyMeterValueChanged ();
		NotifyFishDepthChanged ();
	}

	/*Reset rod*/
	void ResetRod(){
		_isFishing = false;
		_isBaitInWater = false;
		_isRodReady = false;
		_reelingSpeed = 0f;
		_hookProbability = 0.3f;
		m_fishingProgressBar.SetActive (false);
		m_rodTransform.rotation = m_transform.rotation;
		m_rodTransform.eulerAngles += _rodInitialAngles;
		_rodPull = 0;
		_fishDepth = 0f;
		ResetBait ();
		NotifyPlayerModeChanged ();
		NotifyHookProbabilityChanged ();
	}

	/*Reset bait*/
	void ResetBait(){
		m_baitTransform.localPosition = new Vector3 (0f, 18f, 0.56f);
		m_baitRigidbody.isKinematic = false;
		m_baitRigidbody.useGravity = true;
		if (!m_bait.GetComponent<HingeJoint> ()) {
			HingeJoint baitHingeJoint = m_bait.AddComponent<HingeJoint> ();
			baitHingeJoint.connectedBody = m_rodRigidbody;
			baitHingeJoint.anchor = new Vector3 (0f, 0.5f, 0f);
			baitHingeJoint.breakForce = 100f;
		}
	}

	void HookFish(){
		_timeSlicePerRound -= Time.deltaTime;
		if (_timeSlicePerRound <= 0) {
			_timeSlicePerRound = 5f;
			if (_isBaitInWater && !_isFishing) {
				if (Random.value <= _hookProbability) {
					_isRodReady = false;
					_isFishing = true;
					_fishDepth = Random.Range (200, 1000);
					_fishWeight = Random.Range (200, 1599);
					#if UNITY_ANDROID
					resistanceValue = _fishWeight / 200;
					#endif
					_playerMode = FISHING_STATE;
					NotifyPlayerModeChanged ();
					m_fishingProgressBar.SetActive (true);
					/*隔3秒魚才開始逃*/
					StartCoroutine ("SetFishEscapeSpeed");
				}
			}
		}
	}

	IEnumerator SetFishEscapeSpeed(){
		_fishEscapeSpeed = 0f;
		yield return new WaitForSeconds (3);
		_fishEscapeSpeed = 25f;
	}

	void CheckFishHookedOrEscaped(){
		/*
			If depth <= 0, then player got a fish,
			else if depth > 30000, then the fish escaped.
		*/
		if (_fishDepth <= 0 && _isFishing) {
			_playerMode = WATING_FISH_STATE;
			_cachesNumber++;
			ResetRod ();
			SoundManager.Instance.PlaySuccessSound ();
			StartCoroutine ("PlayFishHookedAnimation");
		} else if (_reelingSpeed > MAX_REEL_SPEED * 0.9 && _isFishing) {
			_playerMode = WATING_FISH_STATE;
			ResetRod ();
			SoundManager.Instance.PlayFailSound ();
		} else if (_fishDepth > MAX_FISH_DEPTH && _isFishing) {
			_playerMode = WATING_FISH_STATE;
			ResetRod ();
			SoundManager.Instance.PlayFailSound ();
		}
	}

	IEnumerator PlayFishHookedAnimation(){
		m_fishHookedAnimation.SetActive (true);
		NotifyScoreChanged ();
		yield return new WaitForSeconds (3);
		m_fishHookedAnimation.SetActive (false);
	}

	void UpdateFishDepth(){
		/*
		 	Fish would escape 25m per sec.
		*/
		_timeSlice -= Time.deltaTime;
		if (_timeSlice <= 0) {
			_timeSlice = 0.1f;
			if (_isFishing) {
				_fishDepth += _fishEscapeSpeed;
				_fishDepth -= _reelingSpeed;
				NotifyFishDepthChanged ();
			}
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

	void ChangeHookProbability(Block e){
		_hookProbability = e.m_hookProbability;
	}

	void BaitTouchWater(){
		_isBaitInWater = true;
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

	void NotifyHookProbabilityChanged(){
		if (HookProbabilityChanged != null) {
			HookProbabilityChanged ();
		}
	}

	void NotifyFishDepthChanged(){
		if (FishDepthChanged != null) {
			FishDepthChanged ();
		}
	}












	#if UNITY_EDITOR
	private IEnumerator WaitTime(float time)
	{
		yield return new WaitForSeconds(time);
		GameManager.Instance.SendMessageForEachListener("OnXBikeConnectionStatusChange", "2");
	}
	#endif
}

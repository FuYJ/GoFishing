using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	/*Constants*/
	public const string BOATING_STATE = "boating";
	public const string WATING_FISH_STATE = "wating fish";
	public const string FISHING_STATE = "fishing";
	public const float MAX_MOVE_SPEED = 50f;
	public const float MAX_ROD_ANGLE = 60f;
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
	private float _speed = 30f;
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
	private double _journey = 0;
	private double _lastDistance = 0;

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
	//public Texture2D m_buttonTexture;

	/*UI control*/
	public TextMesh m_meterTitle;
	public TextMesh m_meterData;
	public Scrollbar m_BGMScrollbar;
	public Scrollbar m_soundScrollbar;

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

	public double Journey{
		get{ 
			return _journey;
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
			return _rodPull + 30f;
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
		m_BGMScrollbar.value = SoundManager.Instance.BGMVolume;
		m_soundScrollbar.value = SoundManager.Instance.SoundVolume;

		m_transform = this.transform;
		m_rodTransform.rotation = m_transform.rotation;

		m_rodTransform.Translate (10000, 10000, 10000);
		m_rod.SetActive (false);

		_playerState = this.GetComponentInChildren<PlayerState>();
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

		_lastDistance = XBikeEventReceiver.Data.Distance;
	}

	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.SportStatus == XBikeEventReceiver.SportStatus.Start) {
			if (_playerMode == BOATING_STATE) {
				Move ();
			} else {
				Fish ();
			}
		}
	}

	void Move ()
	{
		_speed = 30f;
		_moveSpeed = 0f;
		_rotSpeed = 0f;
		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.W)) {
			_moveSpeed += _speed;
		}
		if (Input.GetKey (KeyCode.S)) {
			_moveSpeed -= _speed;
		}
		if (Input.GetKey (KeyCode.D)) {
			_rotSpeed += 12.5f;
		}
		if (Input.GetKey (KeyCode.A)) {
			_rotSpeed -= 12.5f;
		}
		#elif UNITY_ANDROID
		/*Speed range: 0 ~ 50, avg: 30*/
		_speed = (float)XBikeEventReceiver.Data.Speed;
		_speed = (_speed < MAX_MOVE_SPEED) ? _speed : MAX_MOVE_SPEED;
		if((int)XBikeEventReceiver.Data.RPMDirection == 1)
			_moveSpeed += _speed;
		else
			_moveSpeed -= _speed;
		_rotSpeed -= 2.5f * (float)((Mathf.Abs((int)XBikeEventReceiver.Data.LeftRightSensor - 180) > 5) ? (((int)XBikeEventReceiver.Data.LeftRightSensor > 0) ? 185 - (int)XBikeEventReceiver.Data.LeftRightSensor : 175 - (int)XBikeEventReceiver.Data.LeftRightSensor) : 0);
		#endif
		if(_moveSpeed != 0f || _rotSpeed != 0f)
			SoundManager.Instance.PlayWaterFlowSound();

		m_CharacterController.Move (m_transform.TransformDirection (new Vector3 (0, 0, _moveSpeed / 2.4f * Time.deltaTime)));
		m_transform.Rotate(Vector3.up, _rotSpeed * Time.deltaTime);

		/*Update meter data*/
		UpdateJourney ();
		NotifyMeterValueChanged ();
	}

	void UpdateJourney () {
		_journey += XBikeEventReceiver.Data.Distance - _lastDistance;
		_lastDistance = XBikeEventReceiver.Data.Distance;
	}

	void Fish ()
	{
		HookFish ();
		UpdateFishDepth ();
		CheckFishHookedOrEscaped ();

		_rodAngles.x = 0;
		_reelingSpeed = 0f;
		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.W) && !_isRodReady && !_isFishing) {
			if(_rodPull < MAX_ROD_ANGLE - 30f){
				_rodPull++;
				_rodAngles.x = -1;
			}
		}
		if(Input.GetKey(KeyCode.S) && !_isRodReady && !_isFishing){
			_isRodReady = true;
			_rodPull = Mathf.Max(Mathf.Min(_rodPull, 30), 0);
			m_baitRigidbody.AddForce(new Vector3(0, 500, 6000 + _rodPull * 300));
		}
		if (Input.GetKey (KeyCode.D) && _isFishing) {
			SoundManager.Instance.PlayReelingSound();
			_reelingSpeed = Mathf.Log(60);
		}
		if (Input.GetKey (KeyCode.A) && _isFishing) {
			SoundManager.Instance.PlayReelingSound();
			_reelingSpeed = -3.689f;
		}
		#elif UNITY_ANDROID
		/*UpDown Sensor Region about 150 ~ 210*/
		if(!_isFishing){
			m_rodTransform.rotation = m_transform.rotation;
			m_rodTransform.eulerAngles += _rodInitialAngles;
			_rodAngles.x = ((int)XBikeEventReceiver.Data.UpDownSensor - 185 > 0) ? 185 - (int)XBikeEventReceiver.Data.UpDownSensor : ((175 - (int)XBikeEventReceiver.Data.UpDownSensor > 0) ? 175 - (int)XBikeEventReceiver.Data.UpDownSensor : 0);
			/*_rodAngles.x = ((int)XBikeEventReceiver.Data.UpDownSensor - 180 > 10) ? -2 : ((int)XBikeEventReceiver.Data.UpDownSensor - 180 < -10) ? 2 : 0;
			if(_rodAngles.x > 0){
				if(_rodPull <= 0)
					_rodAngles.x  = 0;
				else
					_rodPull--;
			}
			else if(_rodAngles.x < 0){
				if(_rodPull >= 30)
					_rodAngles.x = 0;
				else
					_rodPull++;
			}*/
		}
		if((bool)XBikeEventReceiver.Right && !_isRodReady && !_isFishing){
			_isRodReady = true;
			_rodPull = (float)XBikeEventReceiver.Data.UpDownSensor - 180;
			m_baitRigidbody.AddForce(new Vector3(0, 500, 6000 + Mathf.Max(Mathf.Min(_rodPull, 30), 0) * 300));
		}
		/*Speed Region about 0 ~ 50*/
		if((int)XBikeEventReceiver.Data.RPMDirection == 1 && _isFishing){
			SoundManager.Instance.PlayReelingSound();
			_reelingSpeed = Mathf.Log((float)XBikeEventReceiver.Data.RPM + 1);
		}
		else if((int)XBikeEventReceiver.Data.RPMDirection == 0 && _isFishing){
			SoundManager.Instance.PlayReelingSound();
			_reelingSpeed = -Mathf.Log((float)XBikeEventReceiver.Data.RPM + 1);
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
			baitHingeJoint.axis = new Vector3 (0, 0, 0);
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
					GameManager.Instance.PrintAlarmMessage ("魚上鉤了!!");
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
		_fishEscapeSpeed = 1f;
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
			GameManager.Instance.PrintAlarmMessage ("已切換至釣魚模式");
			_playerMode = WATING_FISH_STATE;
			Vector3 pos = m_transform.position + _rodDistance;
			m_rodTransform.position = pos;
			m_rod.SetActive (true);
			ResetRod ();
		} else {
			GameManager.Instance.PrintAlarmMessage ("已切換至划船模式");
			_playerMode = BOATING_STATE;
			_lastDistance = XBikeEventReceiver.Data.Distance;
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

	public void SetBGMVolume(float value){
		SoundManager.Instance.SetBGMVolume (value);
	}

	public void SetSoundVolume(float value){
		SoundManager.Instance.SetSoundVolume (value);
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
}

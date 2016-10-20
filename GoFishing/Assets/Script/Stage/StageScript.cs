using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StageScript : MonoBehaviour {

	/*Constants*/
	private float FISH_SPEED = 0.25f;
	private float FISH_TURNAROUND_SPEED = 0.05f;
	private float FISH_TURNAROUND_TIME = 5;

	/*Stage parameters*/
	protected int STAGE_TIME = 60;	//Time = 1 min
	protected float TERRAIN_SIZE = 200;
	private int _fishNumber;
	private bool _isStagePlaying = true;

	/*Fishes in the scene*/
	public Transform[] _fishTransform;
	public CharacterController[] _fishCharacterController;
	private float _fishTurnaroundTime;

	/*Pause*/
	private GameObject _basicUI;
	private GameObject _playerInformation;
	private GameObject _pauseMenu;
	private GameObject _timer;
	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;
	private TextMesh _timerText;
	private float _stageTime;
	protected bool _isPause = false;

	/*Grid*/
	public GameObject m_grid;
	public TextMesh m_gridState;
	private bool _isGridOn = false;


	public void Awake () {
		if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);
	}

	// Use this for initialization
	public void Start () {
		if (GameManager.Instance.SportStatus != XBikeEventReceiver.SportStatus.Start) {
			GameManager.Instance.StartSport ();
		}

		_basicUI = GameObject.Find ("PlayerGroup/BasicUI");
		_pauseMenu = GameObject.Find ("PlayerGroup/PauseUI/PauseMenu");
		_timerText = GameObject.Find ("PlayerGroup/BasicUI/Timer/TimerText").GetComponent<TextMesh> ();
		_playerInformation = GameObject.Find ("PlayerGroup/PauseUI/PlayerInformation");
		_timer = GameObject.Find ("PlayerGroup/BasicUI/Timer");

		ShowBasicUI ();

		SoundManager.Instance.StopBackgroundMusic2 ();
		SoundManager.Instance.PlayBackgroundMusic1 ();
		_stageTime = STAGE_TIME;
		if (STAGE_TIME == 1e9)
			_timer.SetActive (false);

		GameObject[] _fish = GameObject.FindGameObjectsWithTag ("Fish");
		_fishNumber = _fish.Length;
		_fishTransform = new Transform[_fishNumber];
		_fishCharacterController = new CharacterController[_fishNumber];
		for (int i = 0; i < _fishNumber; i++) {
			_fishTransform [i] = _fish[i].transform;
			_fishCharacterController [i] = (CharacterController)_fish [i].GetComponent<CharacterController> ();
			_fishTransform [i].Translate (new Vector3(Random.Range(-TERRAIN_SIZE, TERRAIN_SIZE), 0, Random.Range(-TERRAIN_SIZE, TERRAIN_SIZE)));
			_fishTransform [i].eulerAngles = new Vector3 (0, Random.Range(0, 360), 0);
		}
	}

	// Update is called once per frame
	public void Update () {
		if (_isStagePlaying) {
			/*Control timer*/
			if (!_isPause && _timer.activeSelf)
				_stageTime = CountTime (_stageTime);
			if (_stageTime <= 0)
				MoveToGameOver ();

			if(_timer.activeSelf)
				_timerText.text = ((int)_stageTime).ToString ();

			/*Control fishes' moving*/
			_fishTurnaroundTime = CountTime (_fishTurnaroundTime);
			if (_fishTurnaroundTime < 0) {
				_fishTurnaroundTime = FISH_TURNAROUND_TIME;
				for (int i = 0; i < _fishNumber; i++) {
					Quaternion fromRot = _fishTransform [i].rotation;
					Quaternion toRot = Quaternion.Euler (new Vector3 (0, Random.value * 360, 0));
					_fishTransform [i].rotation = Quaternion.Slerp (fromRot, toRot, Time.time * FISH_TURNAROUND_SPEED);
				}
			}
			for (int i = 0; i < _fishNumber; i++) {
				_fishCharacterController [i].Move (FISH_SPEED * _fishTransform [i].forward);
			}
		}
	}

	private void ShowBasicUI(){
		_basicUI.SetActive (true);
		_pauseMenu.SetActive (false);
		_playerInformation.SetActive (false);
	}

	protected float CountTime (float time){
		time -= Time.deltaTime;
		return time;
	}

	public void SetPause(bool value){
		_isPause = value;
		if (value) {
			GameManager.Instance.PauseSport ();
			_basicUI.SetActive (false);
			_pauseMenu.SetActive (true);
		} else {
			GameManager.Instance.StartSport ();
			_basicUI.SetActive (true);
			_playerInformation.SetActive (false);
			_pauseMenu.SetActive (false);
		}
	}

	public void ToggleGrid(){
		if (_isGridOn) {
			_isGridOn = false;
			m_grid.SetActive (false);
			m_gridState.text = "格線關閉";
		}
		else {
			_isGridOn = true;
			m_grid.SetActive (true);
			m_gridState.text = "格線開啟";
		}
	}

	public void MoveToGameOver(){
		_isStagePlaying = false;
		SoundManager.Instance.StopBackgroundMusic1 ();
		PlayerScript _playerScript = GameObject.Find ("PlayerGroup").GetComponent<PlayerScript> ();
		GameManager.Instance.InsertRecord (_playerScript.CachesNumber, _playerScript.Journey, (int)(STAGE_TIME - _stageTime));

		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.StageOver);
	}

	public void ShowPlayerInformation(){
		_playerInformation.SetActive (true);
		_pauseMenu.SetActive (false);
		LoadPlayerInformation ();
	}

	private void LoadPlayerInformation(){
		GameObject player = GameObject.Find ("PlayerGroup");
		PlayerScript playerScript = player.GetComponent<PlayerScript> ();
		GameObject playerName = GameObject.Find ("Canvas/PlayerName/PlayerNameText");
		playerName.GetComponent<Text> ().text = GameManager.Instance.Player.Name;
		GameObject dailyCaches = GameObject.Find ("Canvas/PlayerDailyCaches/PlayerDailyCachesText");
		dailyCaches.GetComponent<Text> ().text = (GameManager.Instance.DailyRecord.Caches + playerScript.CachesNumber).ToString();
		GameObject dailyJourney = GameObject.Find ("Canvas/PlayerDailyJourney/PlayerDailyJourneyText");
		dailyJourney.GetComponent<Text> ().text = (GameManager.Instance.DailyRecord.Journey + playerScript.Journey).ToString();
	}
}

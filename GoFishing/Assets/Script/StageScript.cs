using UnityEngine;
using System.Collections;

public class StageScript : MonoBehaviour {

	/*Constants*/
	private float FISH_SPEED = 0.25f;
	private float FISH_TURNAROUND_SPEED = 0.05f;
	private float FISH_TURNAROUND_TIME = 5;

	/*Stage parameters*/
	protected int STAGE_TIME = 60;	//Time = 1 min
	protected float TERRAIN_SIZE = 200;
	private int _fishNumber;

	/*Fishes in the scene*/
	public Transform[] _fishTransform;
	public CharacterController[] _fishCharacterController;
	private float _fishTurnaroundTime;

	/*Pause*/
	public GameObject m_basicUI;
	public GameObject m_playerInformation;
	public GameObject m_pauseMenu;
	//public GameObject m_timer;
	public TextMesh m_timerText;
	private float _time;
	private bool _isPause = false;

	/*Grid*/
	public GameObject m_grid;
	public TextMesh m_gridState;
	private bool _isGridOn = false;


	// Use this for initialization
	public void Start () {
		if(SoundManager.Instance != null)
			SoundManager.Instance.PlayBackgroundMusic ();
		
		_time = STAGE_TIME;

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
		/*Control timer*/
		if(!_isPause)
			_time = CountTime (_time);
		if (_time <= 0)
			MoveToGameOver ();
		m_timerText.text = ((int)_time).ToString ();

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
			_fishCharacterController [i].Move (FISH_SPEED * _fishTransform[i].forward);
		}
	}

	protected virtual float CountTime (float time){
		time -= Time.deltaTime;
		return time;
	}

	public void SetPause(bool value){
		_isPause = value;
		if (value) {
			m_basicUI.SetActive (false);
			//m_timer.SetActive (false);
			m_pauseMenu.SetActive (true);
		} else {
			m_basicUI.SetActive (true);
			//m_timer.SetActive (true);
			m_pauseMenu.SetActive (false);
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

	public virtual void MoveToGameOver(){
		UnityEngine.SceneManagement.SceneManager.LoadScene("StageOverScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
	}

	public void ShowPlayerInformation(){
		m_playerInformation.SetActive (true);
		m_pauseMenu.SetActive (false);
	}

	public void ClosePlayerInformation(){
		m_playerInformation.SetActive (false);
		m_basicUI.SetActive (true);
	}

	public void SetBGMVolume(float value){
		SoundManager.Instance.SetBGMVolume (value);
	}

	public void SetSoundVolume(float value){
		SoundManager.Instance.SetSoundVolume (value);
	}
}

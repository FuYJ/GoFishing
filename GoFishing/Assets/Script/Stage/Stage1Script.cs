using UnityEngine;
using System.Collections;

public class Stage1Script : MonoBehaviour {

	/*Stage parameters*/
	private int TIME = 60;	//Time = 1 min
	private int FISH_NUMBER = 10;
	private float FISH_SPEED = 0.25f;
	private float TERRAIN_SIZE = 200;
	private float FISH_TURNAROUND_SPEED = 0.05f;
	private float FISH_TURNAROUND_TIME = 5;

	private float _time;
	public Transform m_redBlockTransform;
	public Transform m_greenBlockTransform;
	public Transform m_yellowBlockTransform;

	/*Fishes in the scene*/
	public Transform[] m_fish;
	public CharacterController[] m_fishCharacterController;
	private float _fishTurnaroundTime;

	public TextMesh m_timerText;


	// Use this for initialization
	void Start () {
		_time = TIME;
		m_fish = new Transform[FISH_NUMBER];
		m_fishCharacterController = new CharacterController[FISH_NUMBER];
		for (int i = 0; i < FISH_NUMBER; i++) {
			m_fish [i] = GameObject.Find ("cruscarp (" + i + ")").transform;
			m_fishCharacterController [i] = (CharacterController)m_fish [i].GetComponent<CharacterController> ();
			m_fish [i].Translate (new Vector3(Random.Range(-TERRAIN_SIZE, TERRAIN_SIZE), 0, Random.Range(-TERRAIN_SIZE, TERRAIN_SIZE)));
			m_fish [i].eulerAngles = new Vector3 (0, Random.Range(0, 360), 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
		_time = CountTime (_time);
		//change to gameover scence if time's up
		if (_time <= 0) {
			UnityEngine.SceneManagement.SceneManager.LoadScene("StageOverScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
		}
		m_timerText.text = ((int)_time).ToString ();
		_fishTurnaroundTime = CountTime (_fishTurnaroundTime);
		if (_fishTurnaroundTime < 0) {
			_fishTurnaroundTime = FISH_TURNAROUND_TIME;
			for (int i = 0; i < FISH_NUMBER; i++) {
				Quaternion fromRot = m_fish [i].rotation;
				Quaternion toRot = Quaternion.Euler (new Vector3 (0, Random.value * 360, 0));
				m_fish [i].rotation = Quaternion.Slerp (fromRot, toRot, Time.time * FISH_TURNAROUND_SPEED);
			}
		}
			for (int i = 0; i < FISH_NUMBER; i++) {
			m_fishCharacterController [i].Move (FISH_SPEED * m_fish[i].forward);
		}
	}

	float CountTime (float time){
		time -= Time.deltaTime;
		return time;
	}
}

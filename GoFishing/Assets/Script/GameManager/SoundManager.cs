using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	//Parameters
	private float _BGMVolume = 0.5f;
	private float _soundVolume = 0.5f;

	//BGM
	public AudioSource m_backgroundMusic1;
	public AudioSource m_backgroundMusic2;

	//Sounds
	public AudioSource m_waterFlowSound;
	public AudioSource m_reelingSound;
	public AudioSource m_successSound;
	public AudioSource m_failSound;
	public AudioSource m_clickSound;
	public AudioSource m_waterCollisionSound;
	public AudioSource m_fishHookedSound;

	public static SoundManager Instance = null;

	public float BGMVolume{
		get{ 
			return _BGMVolume;
		}
	}

	public float SoundVolume{
		get{ 
			return _soundVolume;
		}
	}

	void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		SetBGMVolume (_BGMVolume);
		SetSoundVolume (_soundVolume);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetBGMVolume(float value){
		_BGMVolume = value;
		m_backgroundMusic1.volume = _BGMVolume;
		m_backgroundMusic2.volume = _BGMVolume;
	}

	public void SetSoundVolume(float value){
		_soundVolume = value;
		m_waterFlowSound.volume = _soundVolume;
		m_reelingSound.volume = _soundVolume;
		m_successSound.volume = _soundVolume;
		m_failSound.volume = _soundVolume;
		m_clickSound.volume = _soundVolume;
		m_waterCollisionSound.volume = _soundVolume;
		m_fishHookedSound.volume = _soundVolume;
	}

	public void PlayWaterCollisionSound (){
		if(!m_waterCollisionSound.isPlaying)
			m_waterCollisionSound.Play ();
	}

	public void PlayFailSound (){
		if(!m_failSound.isPlaying)
			m_failSound.Play ();
	}

	public void PlaySuccessSound (){
		if(!m_successSound.isPlaying)
			m_successSound.Play ();
	}

	public void PlayClickSound (){
		if(!m_clickSound.isPlaying)
			m_clickSound.Play ();
	}

	public void PlayFishHookedSound (){
		if(!m_fishHookedSound.isPlaying)
			m_fishHookedSound.Play ();
	}

	public void PlayWaterFlowSound (){
		if(!m_waterFlowSound.isPlaying)
			m_waterFlowSound.Play ();
	}

	public void PlayReelingSound (){
		if(!m_reelingSound.isPlaying)
			m_reelingSound.Play ();
	}

	public void PlayBackgroundMusic1 (){
		if(!m_backgroundMusic1.isPlaying)
			m_backgroundMusic1.Play ();
	}

	public void StopBackgroundMusic1 (){
		if(m_backgroundMusic1.isPlaying)
			m_backgroundMusic1.Stop ();
	}

	public void PlayBackgroundMusic2 (){
		if(!m_backgroundMusic2.isPlaying)
			m_backgroundMusic2.Play ();
	}

	public void StopBackgroundMusic2 (){
		if(m_backgroundMusic2.isPlaying)
			m_backgroundMusic2.Stop ();
	}
}

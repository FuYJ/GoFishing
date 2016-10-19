using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	static public SceneLoader Instance;
	private Scenes _own = 0;
	private Scenes _nowStage = 0;

	public enum Scenes{
		Start = 0,
        DailyTask,
        SelectPlayer,
		Menu,
		SelectStage,
		Stage1,
		Stage2,
		StageOver,
		GameRecord,
		Setting
	}

	public Scenes NowStage{
		get{ 
			return _nowStage;
		}
	}

	void Awake(){
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);
			GameManager.Instance.SceneOwn = (int)_own;
		} else {
			Destroy (gameObject);
		}
	}

	public void LoadLevel(Scenes target){
		GameManager.Instance.LoadScene((int)target);
	}

	public void SetNowStage(Scenes nowStage){
		_nowStage = nowStage;
	}
}

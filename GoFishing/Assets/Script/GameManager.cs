using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;

public class GameManager : MonoBehaviour {

	/*XBike parameters setup*/
	private XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
	private XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;
	private int _sceneIndex = 0;
	public float resistanceValue = 1.0f;

	/*Database*/
	public static string PLAYER_INFO = "PlayerInfo";
	private string[] _playerInfoCol = {"Name", "Age", "Gender", "Height", "Weight"};
	private string[] _playerInfoType = {"TEXT", "INTEGER", "INTEGER", "INTEGER", "INTEGER"};
	public static string GAME_RECORD = "GameRecord";
	private string[] _gameRecordCol = {"StageIndex", "Caches", "Journey", "Time"};
	private string[] _gameRecordType = {"INTEGER", "INTEGER", "FLOAT", "INTEGER"};
	private Database _db;
	private string _databaseName = "GoFishing.db";
	private SqliteDataReader _reader;

	/*Player information register*/
	private string _playerName = "未選擇玩家";
	private int _playerAge = 0;
	private int _playerGender = 0; 
	private int _playerHeight = 0;
	private int _playerWeight = 0;

	public static GameManager Instance = null;

	public enum Gender
	{
		Null,
		Male,
		Female
	}


	public int SceneIndex{
		get{ 
			return _sceneIndex;
		}
		set{ 
			_sceneIndex = value;
		}
	}

	public string PlayerName{
		get{ 
			return _playerName;
		}
	}

	public int PlayerAge{
		get{ 
			return _playerAge;
		}
	}

	public int PlayerGender{
		get{ 
			return _playerGender;
		}
	}

	public int PlayerHeight{
		get{ 
			return _playerHeight;
		}
	}

	public int PlayerWeight{
		get{ 
			return _playerWeight;
		}
	}

	public XBikeEventReceiver.ConnectionStatus ConnectionStatus{
		get{
			return connectionStatus;
		}
	}

	public XBikeEventReceiver.SportStatus SportStatus{
		get{ 
			return sportStatus;
		}
	}

	void Awake (){
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}

		_db = new Database (_databaseName);
		_db.openDatabaseConnecting ();
		if (!_db.isTableExists (PLAYER_INFO))
			_db.createTable (PLAYER_INFO, _playerInfoCol, _playerInfoType);
		if (!_db.isTableExists (GAME_RECORD))
			_db.createTable (GAME_RECORD, _gameRecordCol, _gameRecordType);
	}

	void Start (){
	}

	#if UNITY_EDITOR
	public void SendMessageForEachListener(string method, string arg)
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

	void Update (){
		if (sportStatus == XBikeEventReceiver.SportStatus.Start && _sceneIndex == 0) {
			Debug.Log (GameManager.Instance.SportStatus);
			_sceneIndex = 1;
			UnityEngine.SceneManagement.SceneManager.LoadScene("Stage1Scene", UnityEngine.SceneManagement.LoadSceneMode.Single);
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

	public void OnStartButtonClicked(){
		#if UNITY_EDITOR
		SendMessageForEachListener("OnXBikeConnectionStatusChange", "1");
		StartSport();
		#elif UNITY_ANDROID
		XBikeEventReceiver.Connect();
		StartSport();
		#endif
	}
}

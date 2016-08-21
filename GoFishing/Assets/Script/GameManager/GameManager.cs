using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class GameManager : MonoBehaviour {

	/*XBike parameters setup*/
	private XBikeEventReceiver.ConnectionStatus connectionStatus = XBikeEventReceiver.ConnectionStatus.Disconnected;
	private XBikeEventReceiver.SportStatus sportStatus = XBikeEventReceiver.SportStatus.Stop;
	private int _sceneIndex = 0;
	public float resistanceValue = 1.0f;

	/*Database*/
	public static string PLAYER_INFO = "PlayerInfo";
	private string[] _playerInfoCol = {"Name", "Age", "Gender", "Height", "Weight", "Avatar", "FootTrainingLevel", "ArmTrainingLevel"};
	private string[] _playerInfoType = {"TEXT", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER", "INTEGER"};
	public static string GAME_RECORD = "GameRecord";
	private string[] _gameRecordCol = {"PlayerName", "StageIndex", "Caches", "Journey", "Time"};
	private string[] _gameRecordType = {"TEXT", "INTEGER", "INTEGER", "FLOAT", "INTEGER"};
	private Database _db;
	private string _databaseName = "GoFishing.db";
	private SqliteDataReader _reader;

	/*Player information register*/
	private PlayerData _player;

	public GameObject m_errorMessage;

	public static GameManager Instance = null;



	public int SceneIndex{
		get{ 
			return _sceneIndex;
		}
		set{ 
			_sceneIndex = value;
		}
	}

	public PlayerData Player{
		get{ 
			return _player;
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

		//Test
		_db.deleteTable (PLAYER_INFO);
		_db.deleteTable (GAME_RECORD);
		//Test

		if (!_db.isTableExists (PLAYER_INFO))
			_db.createTable (PLAYER_INFO, _playerInfoCol, _playerInfoType);
		if (!_db.isTableExists (GAME_RECORD))
			_db.createTable (GAME_RECORD, _gameRecordCol, _gameRecordType);

		//_db.insertInto (PLAYER_INFO, new string[]{ "'1'", "1", "1", "1", "1", "1", "1", "1" });
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

	void OnDisable (){
		_db.closeDatabaseConnecting ();
	}

	void OnDestroy (){
		_db.releaseDatabaseAllResources ();
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

	public List<PlayerData> ReadPlayersInformation(){
		_reader = _db.searchFullTable (PLAYER_INFO);
		string[] name = _db.readStringData (_reader, "Name");
		_reader = _db.searchFullTable (PLAYER_INFO);
		int[] age = _db.readIntData (_reader, "Age");
		_reader = _db.searchFullTable (PLAYER_INFO);
		int[] gender = _db.readIntData (_reader, "Gender");
		_reader = _db.searchFullTable (PLAYER_INFO);
		int[] height = _db.readIntData (_reader, "Height");
		_reader = _db.searchFullTable (PLAYER_INFO);
		int[] weight = _db.readIntData (_reader, "Weight");
		_reader = _db.searchFullTable (PLAYER_INFO);
		int[] avatar = _db.readIntData (_reader, "Avatar");
		_reader = _db.searchFullTable (PLAYER_INFO);
		int[] footTrainingLevel = _db.readIntData (_reader, "FootTrainingLevel");
		_reader = _db.searchFullTable (PLAYER_INFO);
		int[] armTrainingLevel = _db.readIntData (_reader, "ArmTrainingLevel");
		List<PlayerData> playersInfo = new List<PlayerData>();
		for (int i = 0; i < name.Length; i++)
			playersInfo.Add(new PlayerData (name[i], age[i], (PlayerData.Genders)gender[i], height[i], weight[i], (PlayerData.Avatars)avatar[i], footTrainingLevel[i], armTrainingLevel[i]));
		return playersInfo;
	}

	public PlayerData AddPlayer(string name, int age, PlayerData.Genders gender, int height, int weight, PlayerData.Avatars avatar, int footTrainingLevel, int armTrainingLevel){
		PlayerData newPlayer = new PlayerData ();
		newPlayer.Name = name;
		newPlayer.Age = age;
		newPlayer.Gender = gender;
		newPlayer.Height = height;
		newPlayer.Weight = weight;
		newPlayer.Avatar = avatar;
		newPlayer.FootTrainingLevel = footTrainingLevel;
		newPlayer.ArmTrainingLevel = armTrainingLevel;
		InsertPlayerInformation (newPlayer);
		return newPlayer;
	}

	private void InsertPlayerInformation(PlayerData newPlayer){
		_db.insertInto (PLAYER_INFO, new string[]{ newPlayer.Name, newPlayer.Age.ToString(), ((int)newPlayer.Gender).ToString(), newPlayer.Height.ToString(), newPlayer.Weight.ToString(), ((int)newPlayer.Avatar).ToString(), newPlayer.FootTrainingLevel.ToString(), newPlayer.ArmTrainingLevel.ToString() });
	}

	public void EditPlayer(string playerName, PlayerData player){
		_db.updateInto (PLAYER_INFO, _playerInfoCol, new string[] {
			player.Name,
			player.Age.ToString (),
			((int)player.Gender).ToString (),
			player.Height.ToString (),
			player.Weight.ToString (),
			((int)player.Avatar).ToString (),
			player.FootTrainingLevel.ToString (),
			player.ArmTrainingLevel.ToString ()
		}, "Name", playerName);
		_db.updateInto (GAME_RECORD, new string[]{ "PlayerName" }, new string[]{ player.Name }, "PlayerName", playerName);
	}

	public void DeletePlayer(string playerName){
		_db.deleteAccordData (PLAYER_INFO, new string[]{ "Name" }, new string[]{ playerName });
		_db.deleteAccordData (GAME_RECORD, new string[]{ "PlayerName" }, new string[]{ playerName });
	}

	public void PrintErrorMessage(string msg){
		StartCoroutine (PopUpErrorMessage (msg));
	}

	private IEnumerator PopUpErrorMessage(string msg){
		UnityEngine.UI.Text errorText = m_errorMessage.GetComponentInChildren<UnityEngine.UI.Text> ();
		errorText.text = msg;
		GameObject newMsg = Instantiate (m_errorMessage);
		yield return new WaitForSeconds (3);
		Destroy (newMsg);
	}
}

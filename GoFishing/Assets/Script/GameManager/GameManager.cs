using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine.SceneManagement;

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
	private string[] _gameRecordCol = {"PlayerName", "StageIndex", "Caches", "Journey", "Duration", "Date", "Time"};
	private string[] _gameRecordType = {"TEXT", "INTEGER", "INTEGER", "FLOAT", "INTEGER", "TEXT", "TEXT"};
	private Database _db;
	private string _databaseName = "GoFishing.db";
	private SqliteDataReader _reader;

	/*Information register*/
	private PlayerData _player;
	private GameRecord _dailyRecord;
	private GameRecord _stageRecordRigister;
	public SceneNames m_sceneNames;
	private int _own;
	private Status _status;
	private AsyncOperation _loadOperation;

	public GameObject m_errorMessage;
	public GameObject m_alarmMessage;

	public static GameManager Instance = null;

	private enum Status{
		None,
		Prepare,
		Start,
		Loading,
		Complete
	}

	public int SceneOwn{
		set{ 
			_own = Mathf.Clamp (value, 0, m_sceneNames.scenes.Length - 1);	
		}
	}

	public float Progress{
		get{
			if(this._loadOperation == null){
				return 0;
			}else{
				return this._loadOperation.progress;
			}
		}
	}
		
	public int SceneIndex{
		get{ 
			return _sceneIndex;
		}
		set{ 
			_sceneIndex = value;
		}
	}

	public GameRecord DailyRecord{
		get{ 
			return _dailyRecord;
		}
		set{ 
			_dailyRecord = value;
		}
	}

	public GameRecord StageRecordRegister{
		get{ 
			return _stageRecordRigister;
		}
		set{ 
			_stageRecordRigister = value;
		}
	}

	public PlayerData Player{
		get{ 
			return _player;
		}
		set{ 
			_player = value;
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
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);

			if(!string.IsNullOrEmpty(this.m_sceneNames.initScene))
				SceneManager.LoadScene(this.m_sceneNames.initScene);
		} else {
			Destroy (gameObject);
		}

		_db = new Database (_databaseName);
		_db.openDatabaseConnecting ();

		//Test
		//_db.deleteTable (PLAYER_INFO);
		//_db.deleteTable (GAME_RECORD);
		//Test

		if (!_db.isTableExists (PLAYER_INFO))
			_db.createTable (PLAYER_INFO, _playerInfoCol, _playerInfoType);
		if (!_db.isTableExists (GAME_RECORD))
			_db.createTable (GAME_RECORD, _gameRecordCol, _gameRecordType);

		//_db.insertInto (PLAYER_INFO, new string[]{ "'1'", "1", "1", "1", "1", "1", "1", "1" });

		_dailyRecord = new GameRecord ();
		_stageRecordRigister = new GameRecord ();
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

	/*void Update (){
		if (sportStatus == XBikeEventReceiver.SportStatus.Start && _sceneIndex == 0) {
			Debug.Log (GameManager.Instance.SportStatus);
			_sceneIndex = 1;
			SceneManager.LoadScene("SelectPlayerScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
		}
	}*/

	void OnDisable (){
		_db.closeDatabaseConnecting ();
	}

	void OnDestroy (){
		_db.releaseDatabaseAllResources ();
	}

	public void LoadScene(int index){
		if (_status != Status.None && _status != Status.Complete)
			return;
		if (index >= m_sceneNames.scenes.Length)
			return;
		StartCoroutine (AsyncLoadScene (m_sceneNames.scenes [index]));
	}

	private IEnumerator AsyncLoadScene(SceneNameHolder holder){
		yield return StartCoroutine (LoadLoadingScene (holder));	
		yield return StartCoroutine (LoadTargetScene (holder));	
	}

	private IEnumerator LoadLoadingScene(SceneNameHolder holder){
		if (string.IsNullOrEmpty (holder.loading))
			yield break;
		_status = Status.Prepare;
		_loadOperation = holder.isAdditiveLoading ? SceneManager.LoadSceneAsync (holder.loading, LoadSceneMode.Additive) : SceneManager.LoadSceneAsync (holder.loading, LoadSceneMode.Single);
		yield return _loadOperation;
		while (_status == Status.Prepare) {
			yield return null;
		}
	}

	private IEnumerator LoadTargetScene(SceneNameHolder holder){
		_status = Status.Loading;
		if (string.IsNullOrEmpty (holder.own)) {
			_status = Status.None;
			yield break;
		}
		_loadOperation = SceneManager.LoadSceneAsync (holder.own);
		yield return _loadOperation;

		_status = Status.Complete;
	}

	public void StartLoadTargetScene(){
		_status = Status.Start;
	}
		
	public void LoadOwnScene(){
		LoadScene (_own);
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

	public void StopSport()
	{
		#if UNITY_EDITOR
		SendMessageForEachListener("OnXBikeSportStatusChange", "0");
		#elif UNITY_ANDROID
		XBikeEventReceiver.StopSport();
		#endif
	}

	public void StartSport()
	{
		#if UNITY_EDITOR
		SendMessageForEachListener("OnXBikeSportStatusChange", "1");
		#elif UNITY_ANDROID
		XBikeEventReceiver.StartSport();
		#endif
	}

	public void PauseSport()
	{
		#if UNITY_EDITOR
		SendMessageForEachListener("OnXBikeSportStatusChange", "2");
		#elif UNITY_ANDROID
		XBikeEventReceiver.PauseSport();
		#endif
	}

	/*public void OnStartButtonClicked(){
		#if UNITY_EDITOR
		SendMessageForEachListener("OnXBikeConnectionStatusChange", "1");
		StartSport();
		#elif UNITY_ANDROID
		XBikeEventReceiver.Connect();
		StartSport();
		#endif
	}*/

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
		_db.insertInto (PLAYER_INFO, new string[]{ "'" + newPlayer.Name + "'", newPlayer.Age.ToString(), ((int)newPlayer.Gender).ToString(), newPlayer.Height.ToString(), newPlayer.Weight.ToString(), ((int)newPlayer.Avatar).ToString(), newPlayer.FootTrainingLevel.ToString(), newPlayer.ArmTrainingLevel.ToString() });
	}

	public void EditPlayer(string playerName, PlayerData player){
		_db.updateInto (PLAYER_INFO, _playerInfoCol, new string[] {
			"'" + player.Name + "'",
			player.Age.ToString (),
			((int)player.Gender).ToString (),
			player.Height.ToString (),
			player.Weight.ToString (),
			((int)player.Avatar).ToString (),
			player.FootTrainingLevel.ToString (),
			player.ArmTrainingLevel.ToString ()
		}, "Name", "'" + playerName + "'");
		_db.updateInto (GAME_RECORD, new string[]{ "PlayerName" }, new string[]{ "'" + player.Name + "'" }, "PlayerName", "'" + playerName + "'");
	}

	public void DeletePlayer(string playerName){
		_db.deleteAccordData (PLAYER_INFO, new string[]{ "Name" }, new string[]{ "'" + playerName + "'" });
		_db.deleteAccordData (GAME_RECORD, new string[]{ "PlayerName" }, new string[]{ "'" + playerName + "'" });
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

	public void PrintAlarmMessage(string msg){
		StartCoroutine (PopUpAlarmMessage (msg));
	}

	private IEnumerator PopUpAlarmMessage(string msg){
		UnityEngine.UI.Text alarmText = m_alarmMessage.GetComponentInChildren<UnityEngine.UI.Text> ();
		alarmText.text = msg;
		GameObject newMsg = Instantiate (m_alarmMessage);
		yield return new WaitForSeconds (0.5f);
		Destroy (newMsg);
	}

	public void InsertRecord(int caches, double journey, int duration){
		_stageRecordRigister.Caches = caches;
		_stageRecordRigister.Journey = journey;
		_stageRecordRigister.Duration = duration;
		_dailyRecord.Caches += caches;
		_dailyRecord.Journey += journey;
		_dailyRecord.Duration += duration;
		_db.insertInto (GAME_RECORD, new string[] {
			"'" + _player.Name + "'",
			((int)SceneLoader.Instance.NowStage).ToString(),
			caches.ToString(),
			journey.ToString(),
			duration.ToString(),
			"'" + System.DateTime.Now.ToString("MM/dd/yyyy") + "'",
			"'" + System.DateTime.Now.ToString("hh:mm:ss") + "'"
		});
	}

	public List<GameRecord> LoadGameRecords(){
		if (_player == null)
			return new List<GameRecord> ();
		_reader = _db.searchAccordData (GAME_RECORD, "PlayerName", "=", "'" + _player.Name + "'");
		int[] stageIndex = _db.readIntData (_reader, "StageIndex");
		_reader = _db.searchAccordData (GAME_RECORD, "PlayerName", "=", "'" + _player.Name + "'");
		int[] caches = _db.readIntData (_reader, "Caches");
		_reader = _db.searchAccordData (GAME_RECORD, "PlayerName", "=", "'" + _player.Name + "'");
		float[] journey = _db.readFloatData (_reader, "Journey");
		_reader = _db.searchAccordData (GAME_RECORD, "PlayerName", "=", "'" + _player.Name + "'");
		int[] duration = _db.readIntData (_reader, "Duration");
		_reader = _db.searchAccordData (GAME_RECORD, "PlayerName", "=", "'" + _player.Name + "'");
		string[] date = _db.readStringData (_reader, "Date");
		_reader = _db.searchAccordData (GAME_RECORD, "PlayerName", "=", "'" + _player.Name + "'");
		string[] time = _db.readStringData (_reader, "Time");
		List<GameRecord> gameRecord = new List<GameRecord> ();
		for (int i = 0; i < stageIndex.Length; i++)
			gameRecord.Add (new GameRecord (stageIndex [i], caches [i], journey [i], duration [i], date [i], time [i]));
		return gameRecord;
	}
}

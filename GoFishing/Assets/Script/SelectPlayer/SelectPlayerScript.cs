using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectPlayerScript : MonoBehaviour {
	
	public GameObject m_selectPlayerCanvas;
	public GameObject m_addPlayerCanvas;
	public GameObject m_selectAvatarCanvas;
	public GameObject m_downArrowButton;
	public GameObject m_upArrowButton;

	//Prefabs
	public GameObject m_gameManager;
	public GameObject m_soundManager;
	public GameObject m_sceneLoader;
	public GameObject m_playerButton;
	public GameObject m_addPlayerButton;
	private GameObject[] _playerButtons;

	//AddPlayerBoard
	public InputField m_name;
	public InputField m_age;
	public Dropdown m_gender;
	public InputField m_height;
	public InputField m_weight;
	public PlayerData.Avatars avatar = PlayerData.Avatars.Default;
	public Scrollbar m_footTrainingLevel;
	public Scrollbar m_armTrainingLevel;
	public Button m_finishButton;

	private States _nowState = States.SelectPlayerState;

	private List<PlayerData> _playersInfo;
	private int _selectPlayerBoardIndex = 0;
	private int _editingPlayerIndex = -1;

	private enum States{
		SelectPlayerState,
		AddPlayerState,
		EditPlayerState
	}

	void Awake (){
		
		if (SoundManager.Instance == null)
			Instantiate (m_soundManager);
		if (GameManager.Instance == null)
			Instantiate (m_gameManager);
		if (SceneLoader.Instance == null)
			Instantiate (m_sceneLoader);

		_playersInfo = GameManager.Instance.ReadPlayersInformation ();
	}

	void Start () {
		_playerButtons = new GameObject[3];
		ShowSelectPlayerBoard ();
	}

	private void ShowSelectPlayerBoard(){
		_nowState = States.SelectPlayerState;
		m_selectPlayerCanvas.SetActive (true);
		m_addPlayerCanvas.SetActive (false);
		for (int i = 0; i < 3; i++)
			Destroy (_playerButtons [i]);
		PrintPlayers ();
	}

	private void PrintPlayers(){
		if (_selectPlayerBoardIndex == 0)
			m_upArrowButton.SetActive (false);
		else
			m_upArrowButton.SetActive (true);
		if (_selectPlayerBoardIndex < _playersInfo.Count) {
			ShowSelectPlayerButton (_selectPlayerBoardIndex);
		} else {
			_playerButtons[0] = (GameObject)Instantiate (m_addPlayerButton);
			SetPlayerButtonPosition(0, new Vector3 (-65, 103, 0));
			m_downArrowButton.SetActive (false);
			return;
		}
		if (_selectPlayerBoardIndex + 1 < _playersInfo.Count) {
			ShowSelectPlayerButton (_selectPlayerBoardIndex + 1);
		} else {
			_playerButtons[1] = (GameObject)Instantiate (m_addPlayerButton);
			SetPlayerButtonPosition(1, new Vector3 (-65, -46, 0));
			m_downArrowButton.SetActive (false);
			return;
		}
		if (_selectPlayerBoardIndex + 2 < _playersInfo.Count) {
			ShowSelectPlayerButton (_selectPlayerBoardIndex + 2);
			m_downArrowButton.SetActive (true);
		} else {
			_playerButtons[2] = (GameObject)Instantiate (m_addPlayerButton);
			SetPlayerButtonPosition(2, new Vector3 (-65, -196, 0));
			m_downArrowButton.SetActive (false);
			return;
		}
	}

	private void ShowSelectPlayerButton(int playerIndex){
		int buttonIndex = playerIndex % 3;
		_playerButtons[buttonIndex] = (GameObject)Instantiate (m_playerButton);
		(_playerButtons[buttonIndex].GetComponentInParent<SelectPlayerButton> ()).PlayerIndex = playerIndex;
		switch(buttonIndex){
		case 0:	
			SetPlayerButtonPosition (buttonIndex, new Vector3 (-65, 103, 0));
			break;
		case 1:
			SetPlayerButtonPosition (buttonIndex, new Vector3 (-65, -46, 0));
			break;
		case 2:
			SetPlayerButtonPosition (buttonIndex, new Vector3 (-65, -196, 0));
			break;
		}
		Text playerButtonName = _playerButtons [buttonIndex].GetComponentInChildren<Text> ();
		playerButtonName.text = _playersInfo [playerIndex].Name;
		RawImage playerButtonAvatar = _playerButtons [buttonIndex].GetComponentInChildren<RawImage> ();
		playerButtonAvatar.texture = GameManager.Instance.GetComponent<Avatars>().Textures[(int)_playersInfo[playerIndex].Avatar];
	}

	private void SetPlayerButtonPosition(int playerButtonIndex, Vector3 pos){
		_playerButtons[playerButtonIndex].transform.SetParent(m_selectPlayerCanvas.transform);
		_playerButtons[playerButtonIndex].transform.localPosition = pos;
		_playerButtons[playerButtonIndex].transform.localScale = m_addPlayerButton.transform.localScale;
	}

	public void DeletePlayer(int playerIndex){
		GameManager.Instance.DeletePlayer (_playersInfo [playerIndex].Name);
		_playersInfo.RemoveAt (playerIndex);
		ShowSelectPlayerBoard ();
	}

	public void ShowAddPlayerBoard(){
		_nowState = States.AddPlayerState;
		m_selectPlayerCanvas.SetActive (false);
		m_addPlayerCanvas.SetActive (true);
		ClearAddPlayerBoard ();
		m_addPlayerCanvas.GetComponentInChildren<Text> ().text = "新增角色";
	}

	public void ShowEditPlayerBoard(int playerIndex){
		_nowState = States.EditPlayerState;
		m_selectPlayerCanvas.SetActive (false);
		m_addPlayerCanvas.SetActive (true);
		m_addPlayerCanvas.GetComponentInChildren<Text> ().text = "編輯角色";
		GameObject.Find ("AddPlayerCanvas/Name").GetComponent<InputField> ().text = _playersInfo [playerIndex].Name;
		GameObject.Find ("AddPlayerCanvas/Age").GetComponent<InputField> ().text = _playersInfo [playerIndex].Age.ToString();
		GameObject.Find ("AddPlayerCanvas/Height").GetComponent<InputField> ().text = _playersInfo [playerIndex].Height.ToString();
		GameObject.Find ("AddPlayerCanvas/Weight").GetComponent<InputField> ().text = _playersInfo [playerIndex].Weight.ToString();
		GameObject.Find ("AddPlayerCanvas/Gender").GetComponent<Dropdown> ().value = (int)_playersInfo [playerIndex].Gender;
		GameObject.Find ("AddPlayerCanvas/Avatar/RawImage").GetComponent<RawImage> ().texture = GameManager.Instance.GetComponent<Avatars> ().Textures [(int)_playersInfo [playerIndex].Avatar];
		GameObject.Find ("AddPlayerCanvas/FootTrainingLevel").GetComponent<Scrollbar> ().value = (float)_playersInfo [playerIndex].FootTrainingLevel / 3 + 0.2f;
		GameObject.Find ("AddPlayerCanvas/ArmTrainingLevel").GetComponent<Scrollbar> ().value = (float)_playersInfo [playerIndex].ArmTrainingLevel / 3 + 0.2f;
		_editingPlayerIndex = playerIndex;
	}

	public void OnFinishButtonClick(){
		if (_nowState == States.AddPlayerState) {
			PlayerData newPlayer;
			newPlayer = GameManager.Instance.AddPlayer (m_name.text, int.Parse (m_age.text), (PlayerData.Genders)m_gender.value, int.Parse (m_height.text), int.Parse (m_weight.text), (PlayerData.Avatars)avatar, Mathf.FloorToInt (m_footTrainingLevel.value * 3), Mathf.FloorToInt (m_armTrainingLevel.value * 3));
			_playersInfo.Add (newPlayer);
		} else if (_nowState == States.EditPlayerState) {
			string editingPlayerName = _playersInfo [_editingPlayerIndex].Name;
			_playersInfo[_editingPlayerIndex] = new PlayerData (m_name.text, int.Parse (m_age.text), (PlayerData.Genders)m_gender.value, int.Parse (m_height.text), int.Parse (m_weight.text), (PlayerData.Avatars)avatar, Mathf.FloorToInt (m_footTrainingLevel.value * 3), Mathf.FloorToInt (m_armTrainingLevel.value * 3));
			GameManager.Instance.EditPlayer (editingPlayerName, _playersInfo [_editingPlayerIndex]);
		}
		ShowSelectPlayerBoard ();
	}

	public void CheckIsFinishButtonEnable(){
		m_finishButton.interactable = false;
		if (m_name.text.Length > 0 && m_age.text.Length > 0 && m_height.text.Length > 0 && m_weight.text.Length > 0)
			m_finishButton.interactable = true;
	}

	public void CancelAddPlayer(){
		ShowSelectPlayerBoard ();
	}

	private void ClearAddPlayerBoard (){
		m_name.text = "";
		m_age.text = "";
		m_gender.value = 0;
		m_height.text = "";
		m_weight.text = "";
		m_footTrainingLevel.value = 0.5f;
		m_armTrainingLevel.value = 0.5f;
		avatar = PlayerData.Avatars.Default;
		GameObject.Find ("AddPlayerCanvas/Avatar/RawImage").GetComponent<RawImage> ().texture = GameManager.Instance.GetComponent<Avatars>().Textures[(int)avatar];
	}

	public void ShowSelectAvatarBoard(){
		m_selectAvatarCanvas.SetActive (true);
		Debug.Log ("select avatar");
	}

	public void SelectAvatar(PlayerData.Avatars index){
		avatar = index;
		GameObject.Find ("AddPlayerCanvas/Avatar/RawImage").GetComponent<RawImage> ().texture = GameManager.Instance.GetComponent<Avatars>().Textures[(int)index];
		m_selectAvatarCanvas.SetActive (false);
	}

	public void OnDownArrowClick(){
		SoundManager.Instance.PlayClickSound ();
		_selectPlayerBoardIndex += 3;
		ShowSelectPlayerBoard();
	}

	public void OnUpArrowClick(){
		SoundManager.Instance.PlayClickSound ();
		_selectPlayerBoardIndex -= 3;
		ShowSelectPlayerBoard();
	}

	public void LoadNextScene(int playerIndex){
		GameManager.Instance.Player = _playersInfo [playerIndex];
		SceneLoader.Instance.LoadLevel (SceneLoader.Scenes.Menu);
	}
}

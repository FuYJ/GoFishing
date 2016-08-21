using UnityEngine;
using System.Collections;

public class SelectPlayerButton : MonoBehaviour {

	private int _playerIndex;

	public int PlayerIndex{
		get{ 
			return _playerIndex;
		}
		set{ 
			_playerIndex = value;
		}
	}

	public void OnSelectPlayerButtonClick(){
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();

	}

	public void OnEditButtonClick(){
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.ShowEditPlayerBoard (_playerIndex);
	}

	public void OnDeleteButtonClick(){
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.DeletePlayer (_playerIndex);
	}
}

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
		SoundManager.Instance.PlayClickSound ();
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.LoadNextScene (_playerIndex);
	}

	public void OnEditButtonClick(){
		SoundManager.Instance.PlayClickSound ();
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.ShowEditPlayerBoard (_playerIndex);
	}

	public void OnDeleteButtonClick(){
		SoundManager.Instance.PlayClickSound ();
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.DeletePlayer (_playerIndex);
	}
}

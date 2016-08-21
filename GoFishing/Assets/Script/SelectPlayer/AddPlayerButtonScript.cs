using UnityEngine;
using System.Collections;

public class AddPlayerButtonScript : MonoBehaviour {

	public void OnButtonClick(){
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.ShowAddPlayerBoard ();
	}
}

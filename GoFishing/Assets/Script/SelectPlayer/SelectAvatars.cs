using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectAvatars : MonoBehaviour {

	public RawImage m_default;
	public RawImage m_female1;
	public RawImage m_female2;
	public RawImage m_female3;
	public RawImage m_male1;
	public RawImage m_male2;
	public RawImage m_male3;
	public RawImage m_custom;

	// Use this for initialization
	void Awake () {
		Avatars avatars;
		avatars = GameManager.Instance.GetComponent<Avatars> ();
		m_default.texture = avatars.m_default;
		m_female1.texture = avatars.m_female1;
		m_female2.texture = avatars.m_female2;
		m_female3.texture = avatars.m_female3;
		m_male1.texture = avatars.m_male1;
		m_male2.texture = avatars.m_male2;
		m_male3.texture = avatars.m_male3;
		m_custom.texture = avatars.m_custom;
	}

	public void OnDefaultButtonClick () {
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.SelectAvatar (PlayerData.Avatars.Default);

	}

	public void OnFemale1ButtonClick () {
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.SelectAvatar (PlayerData.Avatars.Female1);
	}

	public void OnFemale2ButtonClick () {
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.SelectAvatar (PlayerData.Avatars.Female2);
	}

	public void OnFemale3ButtonClick () {
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.SelectAvatar (PlayerData.Avatars.Female3);
	}

	public void OnMale1ButtonClick () {
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.SelectAvatar (PlayerData.Avatars.Male1);
	}

	public void OnMale2ButtonClick () {
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.SelectAvatar (PlayerData.Avatars.Male2);
	}

	public void OnMale3ButtonClick () {
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.SelectAvatar (PlayerData.Avatars.Male3);
	}

	public void OnCustomButtonClick () {
		SelectPlayerScript _selectPlayerScript;
		_selectPlayerScript = GameObject.Find ("SelectPlayerScript").GetComponent<SelectPlayerScript>();
		_selectPlayerScript.SelectAvatar (PlayerData.Avatars.Custom);
	}
}

using UnityEngine;
using System.Collections;

public class Meter : MonoBehaviour {

	/*Transform range: 0 ~ 0.95*/
	private const float MAX_RANGE = 0.95f;
	private const float MAX_MOVE_SPEED = PlayerScript.MAX_MOVE_SPEED * 10;

	public Transform m_anchorTransform;
	public PlayerScript m_player;
	public TextMesh m_meterData;
	public TextMesh m_meterTitle;

	private string _playerMode = PlayerScript.BOATING_STATE;
	private float _playerMoveSpeed = 0f;

	// Use this for initialization
	void Start () {
		m_player.MeterValueChanged += MeterValueChanged;
		m_player.PlayerModeChanged += MeterValueChanged;
		m_player.PlayerModeChanged += PlayerModeChanged;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void MeterValueChanged(){
		if (_playerMode == PlayerScript.BOATING_STATE) {
			_playerMoveSpeed = ((int)Mathf.Abs ((m_player.MoveSpeed * 10)));
			m_meterData.text = _playerMoveSpeed.ToString ();
			m_anchorTransform.localPosition = new Vector3 (0, _playerMoveSpeed / MAX_MOVE_SPEED * MAX_RANGE, 0);
		} else if (_playerMode == PlayerScript.WATING_FISH_STATE) {
			m_meterData.text = m_player.RodPullAngle.ToString ();
			m_anchorTransform.localPosition = new Vector3 (0, m_player.RodPullAngle / PlayerScript.MAX_ROD_ANGLE * MAX_RANGE, 0);
		} else if (_playerMode == PlayerScript.FISHING_STATE) {
			m_meterData.text = m_player.ReelingSpeed.ToString ();
			m_anchorTransform.localPosition = new Vector3 (0, m_player.ReelingSpeed / PlayerScript.MAX_REEL_SPEED * MAX_RANGE, 0);
		}
	}

	void PlayerModeChanged(){
		_playerMode = m_player.PlayerMode;
		switch (_playerMode) {
		case PlayerScript.BOATING_STATE:
			m_meterTitle.text = "目前船速";
			break;
		case PlayerScript.WATING_FISH_STATE:
			m_meterTitle.text = "釣竿角度";
			break;
		case PlayerScript.FISHING_STATE:
			m_meterTitle.text = "捲線速度";
			break;
		}
	}
}

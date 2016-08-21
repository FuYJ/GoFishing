using UnityEngine;
using System.Collections;

public class FishingProgressBar : MonoBehaviour {

	//Fish icon range: x: -0.25 ~ 0.25
	private float MIN_X = -0.25f;
	private float PROGRESS_BAR_RANGE = 0.5f;

	public PlayerScript m_player;
	public Transform m_fishIcon;
	public TextMesh m_fishDepthNumber;

	// Use this for initialization
	void Start () {
		m_player.FishDepthChanged += ChangeFishDepth;
	}

	void ChangeFishDepth() {
		m_fishDepthNumber.text = m_player.FishDepth.ToString();
		m_fishIcon.localPosition = new Vector3 (MIN_X + (PlayerScript.MAX_FISH_DEPTH - m_player.FishDepth) / PlayerScript.MAX_FISH_DEPTH * PROGRESS_BAR_RANGE, 0f, -0.02f);
	}
}

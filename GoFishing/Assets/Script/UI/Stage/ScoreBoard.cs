using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

	public TextMesh m_cachesNumber;
	public TextMesh m_hookProbabilityNumber;
	public PlayerScript m_player;

	// Update is called once per frame
	void Update () {
		m_player.FishGotten += GotFish;
		m_player.HookProbabilityChanged += HookProbabilityChanged;
	}

	void GotFish(){
		m_cachesNumber.text = m_player.CachesNumber.ToString();
	}

	void HookProbabilityChanged(){
		m_hookProbabilityNumber.text = m_player.HookProbability.ToString () + "%";
	}
}

using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

	public TextMesh m_cachesNumber;
	public PlayerScript m_player;

	// Update is called once per frame
	void Update () {
		m_player.FishGotten += GotFish;
	}

	void GotFish(){
		m_cachesNumber.text = m_player.CachesNumber.ToString();
	}
}

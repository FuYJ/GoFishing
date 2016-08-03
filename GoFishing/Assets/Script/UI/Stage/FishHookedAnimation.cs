using UnityEngine;
using System.Collections;

public class FishHookedAnimation : MonoBehaviour {

	public PlayerScript m_player;
	public TextMesh m_fishWeight;

	// Use this for initialization
	void Awake () {
		m_player.FishGotten += UpdateFishWeight;
	}

	void UpdateFishWeight(){
		m_fishWeight.text = m_player.FishWeight.ToString () + "kg";
	}
}

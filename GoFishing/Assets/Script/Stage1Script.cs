using UnityEngine;
using System.Collections;

public class Stage1Script : MonoBehaviour {

	private int TIME = 60;	//Time = 1 min

	private float _time;
	public Transform m_redBlockTransform;
	public Transform m_greenBlockTransform;
	public Transform m_yellowBlockTransform;


	// Use this for initialization
	void Start () {
		_time = TIME;

	}
	
	// Update is called once per frame
	void Update () {
		_time = CountTime (_time);

	}

	float CountTime (float time){
		time -= Time.deltaTime;
		//change to gameover scence if time's up
		if (time < 0) {

		}
		return time;
	}
}

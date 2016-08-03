using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public float m_hookProbability;

	public delegate void HookProbabilityChangedEventHandler(Block e);
	public event HookProbabilityChangedEventHandler HookProbabilityChanged;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		//Debug.Log (other.tag);
		if(other.CompareTag("Bait"))
			NotifyHookedProbabilityChanged ();
	}

	void NotifyHookedProbabilityChanged(){
		if (HookProbabilityChanged != null) {
			HookProbabilityChanged (this);
		}
	}
}

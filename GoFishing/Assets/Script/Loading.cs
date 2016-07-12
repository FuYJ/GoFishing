using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AnimationStart()
    {
        GetComponent<Animation>().Play();
    }

    void AnimationEnd()
    {
    }

    public void AnimationStop()
    {
        GetComponent<Animation>().Stop();
    }
}

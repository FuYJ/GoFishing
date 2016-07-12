using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
    private float _time_f = 0;
    private int _time_i = 0;
    private bool _start;
	// Use this for initialization
	void Start () {
        _time_f = 0;
        _time_i = 0;
        _start = false;
    }
	
	// Update is called once per frame
	public void Update () {
        _time_f += Time.deltaTime;
        _time_i = (int)_time_f;
    }

    public int Time_i
    {
        get
        {
            return _time_i;
        }
        set
        {
            _time_i = value;
        }
    }

    public bool Start_Time
    {
        get
        {
            return _start;
        }
        set
        {
            _start = value;
            _time_f = 0;
            _time_i = 0;
        }
    }
}

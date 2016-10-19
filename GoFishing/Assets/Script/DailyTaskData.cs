using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class DailyTaskData : MonoBehaviour {
    private string _player;
    private int[] _number;
    private float[] _complete;
    private DateTime _date;

    void Awake()
    {
        _number = new int[3];
        _complete = new float[3];
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string Player
    {
        get
        {
            return _player;
        }
        set
        {
            _player = value;
        }
    }

    public int[] Number
    {
        get
        {
            return _number;
        }
        set
        {
            _number = value;
        }
    }

    public float[] Complete
    {
        get
        {
            return _complete;
        }
        set
        {
            _complete = value;
        }
    }

    public DateTime Date
    {
        get
        {
            return _date;
        }
        set
        {
            _date = value;
        }
    }
}

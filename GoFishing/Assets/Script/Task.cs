using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Task : MonoBehaviour {

    public GameObject _goal;
    public GameObject _total;
    private int _number;
    private double _complete;
    private double _target;
    private string _goalText;

    void Awake()
    {
    }
    // Use this for initialization
    void Start () {
        Text goal = _goal.GetComponent<Text>();
        goal.text = _goalText;
    }
	
	// Update is called once per frame
	void Update () {
        Text total = _total.GetComponent<Text>();
        total.text = _complete + "/" + _target;
    }

    public void SetNumber(int number)
    {
        _number = number;
    }

    public void SetTarget(double target)
    {
        _target = target;
    }

    public void SetComplete(double complete)
    {
        _complete = complete;
    }

    public void SetGoalText(string goal)
    {
        _goalText = goal;
    }
}

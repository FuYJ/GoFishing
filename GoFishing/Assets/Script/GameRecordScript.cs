using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameRecordScript : MonoBehaviour {

    public GameObject _panel;
    public GameObject _task;
    private GameObject[] _taskTable;
    private int _taskNumber;
    private string[] TASK_CONTENT = {"釣五條魚", "往前划船5公里", "完成3個關卡"};
    private int TASK_TABLE_MAX = 9;
    private int TASK_CONTENT_MAX = 9;

	void Awake () {
        _taskNumber = 0;
        _taskTable = new GameObject[9];
        CreateTask(0, 0, 5);
        CreateTask(1, 2.5, 5);
        CreateTask(2, 1, 3);
    }

	void Start () {
		
	}

    private void CreateTaskRandomly()
    {

    }

    private void CreateTask(int number, double complete, double total)
    {
        if(_taskNumber < TASK_TABLE_MAX)
        {
            _taskTable[_taskNumber] = Instantiate(_task);
            Task taskScript = _taskTable[_taskNumber].GetComponent<Task>();
            taskScript.SetNumber(number);
            taskScript.SetTarget(total);
            taskScript.SetComplete(complete);
            taskScript.SetGoalText(TASK_CONTENT[number]);
            _taskTable[_taskNumber].transform.parent = _panel.transform;
            _taskNumber++;
        }
}

	private void LoadPlayerRecords(){
		
	}

	public void OnBackButtonClick(){
		
	}
}

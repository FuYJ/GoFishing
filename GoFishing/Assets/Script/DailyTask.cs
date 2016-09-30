using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DailyTask : MonoBehaviour {

    public GameObject[] _task;
    private int _taskNumber;
    private string[] TASK_CONTENT = {"請釣到五條魚", "划船5公里", "完成3個關卡"};
    private int TASK_TABLE_MAX = 3;

	void Awake () {
        _taskNumber = 0;
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

    }

	private void LoadPlayerRecords(){

	}

	public void OnBackButtonClick(){
		
	}
}

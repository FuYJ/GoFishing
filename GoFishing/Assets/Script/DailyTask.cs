using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LitJson;
using System.IO;
using System;

public class DailyTask : MonoBehaviour {

    public GameObject[] _task;
    private string _name;
    private int _taskSize;
    private int[] _taskNumber;
    private float[] _complete;
    private int TASK_TABLE_MAX;
    private int TASK_NUMBER;
    private DateTime _date;

    private string jsonString;
    private JsonData jsonData;

    private DailyTask _data;

    void Awake () {
        jsonString = File.ReadAllText(Application.dataPath + "/DailyTask.json");
        jsonData = JsonMapper.ToObject(jsonString);
        _taskSize = 0;
        TASK_TABLE_MAX = 3;
        TASK_NUMBER = jsonData["Task"].Count;
        _taskNumber = new int[3] {-1, -1, -1 };
        _complete = new float[3];
    }

	void Start () {
        
	}

    public void CreateTaskRandomly()
    {
        _taskSize = 0;
        while (_taskSize < TASK_TABLE_MAX)
        {
            _taskNumber[_taskSize] = UnityEngine.Random.Range(0, TASK_NUMBER);
            _complete[_taskSize] = 0;
            _taskSize++;
        }
    }

    public void MarkTime()
    {
        _date = DateTime.Now;
    }

    private void CreateTask(int number, float complete)
    {
        if(_taskSize < TASK_TABLE_MAX)
        {
            _taskNumber[_taskSize] = number;
            _complete[_taskSize] = complete;
            _taskSize++;
        }
    }

    public void ShowTask()
    {
        for(int i = 0; i < TASK_TABLE_MAX; i++)
        {
            if(_taskNumber[i] >= 0)
            {
                Text[] text = _task[i].GetComponentsInChildren<Text>();
                text[0].text = jsonData["Task"][_taskNumber[i]]["context"].ToString();
                text[1].text = _complete[i] + "/" + jsonData["Task"][i]["total"];
            }
        }
    }

    public void CheckTask(GameRecord record)
    {
        for(int i = 0; i < TASK_TABLE_MAX; i++)
        {
            switch (_taskNumber[i])
            {
                case 0:
                    _complete[i] += record.Caches;
                    break;
                case 1:
                    _complete[i] += 1;
                    break;
                case 2:
                    _complete[i] += (float)record.Journey;
                    break;
            }
        }
    }

    private void LoadPlayerRecords(){

	}

	public void OnBackButtonClick(){
		
	}

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    public int[] TaskNumber
    {
        get
        {
            return _taskNumber;
        }
        set
        {
            _taskNumber = value;
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

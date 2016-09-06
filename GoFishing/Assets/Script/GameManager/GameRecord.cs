using UnityEngine;
using System.Collections;

public class GameRecord {

	private int _stageIndex = 0;
	private int _caches = 0;
	private double _journey = 0;
	private int _duration = 0;
	private string _date = "";
	private string _time = "";

	public GameRecord(){
	}

	public GameRecord(int stageIndex, int caches, double journey, int duration, string date, string time){
		_stageIndex = stageIndex;
		_caches = caches;
		_journey = journey;
		_duration = duration;
		_date = date;
		_time = time;
	}

	public int StageIndex{
		get{ 
			return _stageIndex;
		}
		set{ 
			_stageIndex = value;
		}
	}

	public int Caches{
		get{ 
			return _caches;
		}
		set{ 
			_caches = value;
		}
	}

	public double Journey{
		get{ 
			return _journey;
		}
		set{ 
			_journey = value;
		}
	}

	public int Duration{
		get{ 
			return _duration;
		}
		set{ 
			_duration = value;
		}
	}

	public string Date{
		get{ 
			return _date;
		}
		set{ 
			_date = value;
		}
	}

	public string Time{
		get{ 
			return _time;
		}
		set{ 
			_time = value;
		}
	}
}

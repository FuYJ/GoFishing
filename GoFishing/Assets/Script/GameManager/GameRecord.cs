using UnityEngine;
using System.Collections;

public class GameRecord {

	private int _caches = 0;
	private double _journey = 0;
	private int _time = 0;

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

	public int Time{
		get{ 
			return _time;
		}
		set{ 
			_time = value;
		}
	}
}

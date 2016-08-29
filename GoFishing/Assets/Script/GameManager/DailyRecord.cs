using UnityEngine;
using System.Collections;

public class DailyRecord : MonoBehaviour {

	private int _dailyCaches;
	private int _dailyJourney;

	public int DailyCaches{
		get{ 
			return _dailyCaches;
		}
		set{ 
			_dailyCaches = value;
		}
	}
	public int DailyJourney{
		get{ 
			return _dailyJourney;
		}
		set{ 
			_dailyJourney = value;
		}
	}
}

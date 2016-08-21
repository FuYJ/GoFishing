using UnityEngine;
using System.Collections;

public class PlayerData {

	private string _name;
	private int _age;
	private Genders _gender;
	private int _height;
	private int _weight;
	private Avatars _avatar;
	private int _footTrainingLevel;
	private int _armTrainingLevel;

	public enum Genders
	{
		Unknown,
		Male,
		Female
	}

	public enum Avatars
	{
		Default,
		Female1,
		Female2,
		Female3,
		Male1,
		Male2,
		Male3,
		Custom
	}

	public PlayerData(){
	}

	public PlayerData(string name, int age, Genders gender, int height, int weight, Avatars avatar, int footTrainingLevel, int armTrainingLevel){
		_name = name;
		_age = age;
		_gender = gender;
		_height = height;
		_weight = weight;
		_avatar = avatar;
		_footTrainingLevel = footTrainingLevel;
		_armTrainingLevel = armTrainingLevel;
	}

	public string Name{
		get{ 
			return _name;
		}
		set{ 
			if (value.Length <= 10) {
				_name = value;
			} else if (value.Length <= 0) {
				GameManager.Instance.PrintErrorMessage ("請輸入您的名稱");
			} else{
				GameManager.Instance.PrintErrorMessage ("字數過多\n請低於10個字");
			}
		}
	}

	public int Age{
		get{ 
			return _age;
		}
		set{ 
			if (value < 150) {
				_age = value;
			} else {
				GameManager.Instance.PrintErrorMessage ("請填寫真實年齡");
			}
		}
	}

	public Genders Gender{
		get{ 
			return _gender;
		}
		set{ 
			_gender = (Genders)Mathf.Clamp ((float)value, (float)Genders.Unknown, (float)Genders.Female);
		}
	}

	public int Height{
		get{ 
			return _height;
		}
		set{ 
			if (_height < 500) {
				_height = value;
			} else {
				GameManager.Instance.PrintErrorMessage ("請填寫真實身高");
			}
		}
	}

	public int Weight{
		get{ 
			return _weight;
		}
		set{ 
			if (_weight < 500) {
				_weight = value;
			} else {
				GameManager.Instance.PrintErrorMessage ("請填寫真實體重");
			}
		}
	}

	public Avatars Avatar{
		get{ 
			return _avatar;
		}
		set{ 
			_avatar = (Avatars)Mathf.Clamp ((float)value, (float)Avatars.Default, (float)Avatars.Custom);
		}
	}

	public int FootTrainingLevel{
		get{ 
			return _footTrainingLevel;
		}
		set{ 
			_footTrainingLevel = Mathf.Clamp (value, 0, 2);
		}
	}

	public int ArmTrainingLevel{
		get{ 
			return _armTrainingLevel;
		}
		set{ 
			_armTrainingLevel = Mathf.Clamp (value, 0, 2);
		}
	}
}

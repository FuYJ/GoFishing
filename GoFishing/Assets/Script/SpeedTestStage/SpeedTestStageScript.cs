using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpeedTestStageScript : StageScript {

	private Transform m_transform;

	//Record
	public List<Vector2> m_rpmData;
	private int _time;
	private int _speed;
	private int _width = 400;
	private int _height = 300;
	private Texture2D _texture;


	public void Awake () {
		base.Awake ();
		STAGE_TIME = 180;
		TERRAIN_SIZE = 500f;
	}

	void Start () {
		base.Start ();
		_time = 0;
		_speed = 0;
		m_transform = this.transform;
		ChartDrawer.Width = _width;
		ChartDrawer.Height = _height;

		InitCanvas (_width, _height);
		StartCoroutine (DrawLineChart ());
	}

	void Update () {
		base.Update ();
		if (Input.GetKey (KeyCode.UpArrow)) {
			_speed++;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			_speed--;
		}
	}

	void OnGUI () {
		GUI.DrawTexture (ResizeGUI(new Rect (m_transform.position.x, m_transform.position.y, _width, _height)), _texture);
	}

	private Rect ResizeGUI(Rect _rect){
		float FilScreenWidth = _rect.width / 800;
		float rectWidth = FilScreenWidth * Screen.width;
		float FilScreenHeight = _rect.height / 600;
		float rectHeight = FilScreenHeight * Screen.height;
		float rectX = (_rect.x / 800) * Screen.width;
		float rectY = (_rect.y / 600) * Screen.height;

		return new Rect(rectX,rectY,rectWidth,rectHeight);
	}

	public void InitCanvas(int w, int h){
		_texture = new Texture2D (w, h);
	}

	public IEnumerator DrawLineChart(){
		while (!_isPause) {
			m_rpmData.Add (new Vector2 (_time, _speed));
			_time += 1;
			StartCoroutine (ChartDrawer.InitLineChart(_texture));
			StartCoroutine (ChartDrawer.DrawGrid(_texture));
			StartCoroutine (ChartDrawer.DrawLineChart(_texture, m_rpmData));
			_texture.Apply ();
			yield return new WaitForSeconds (1);
		}
	}

}

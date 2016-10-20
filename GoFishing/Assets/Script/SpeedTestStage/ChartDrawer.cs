using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChartDrawer : MonoBehaviour {

	//Record
	//public List<Vector2> points;
	public static Color m_gridColor;
	public static Color m_lineColor;
	private static int xAxisInterval = 5;
	private static int _xGridInterval = 50;
	private static int _yGridInterval = 50;
	private static int _width = 400;
	private static int _height = 300;
	private Texture2D _texture;

	public static int Width{
		get{
			return _width;
		}
		set{
			_width = value;
		}
	}
		
	public static int Height{
		get{
			return _height;
		}
		set{
			_height = value;
		}
	}

	public static IEnumerator InitLineChart(Texture2D texture){
		for (int i = 0; i < _width; i++) {
			for (int j = 0; j < _height; j++) {
				texture.SetPixel (i, j, Color.white);
			}
		}

		yield return texture;
	}

	public static IEnumerator DrawGrid(Texture2D texture){
		for(int i = 0; i < _width; i += _xGridInterval){
			for (int j = 0; j < _height; j++) {
				texture.SetPixel (i, j, m_gridColor);
			}
		}
		for(int i = 0; i < _width; i++){
			for (int j = 0; j < _height; j += _yGridInterval) {
				texture.SetPixel (i, j, m_gridColor);
			}
		}

		yield return texture;
	}

	public static IEnumerator DrawLineChart(Texture2D texture, List<Vector2> points){
		//Vector2 currentPoint = points [0];
		for (int i = 1; i < points.Count; i++) {
			for (float j = 0; j < 1; j = j + 0.001f) {
				Vector2 temp = Vector2.Lerp (points [i - 1], points [i], j);
				texture.SetPixel (Convert.ToInt32 (temp.x * xAxisInterval), Convert.ToInt32 (temp.y), m_lineColor);
			}
			//currentPoint = points [i];
		}

		yield return texture;
	}
}

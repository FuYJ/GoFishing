using UnityEngine;
using System.Collections;

public class Avatars : MonoBehaviour {

	public Texture m_default;
	public Texture m_female1;
	public Texture m_female2;
	public Texture m_female3;
	public Texture m_male1;
	public Texture m_male2;
	public Texture m_male3;
	public Texture m_custom;

	public Texture[] Textures{
		get{
			return new Texture[]{ m_default, m_female1, m_female2, m_female3, m_male1, m_male2, m_male3, m_custom };
		}
	}
}

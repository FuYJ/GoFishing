using UnityEngine;
using System.Collections;

public class Bait : MonoBehaviour {

	public Transform m_baitTransform;
	public Rigidbody m_baitRigidbody;
	public AudioSource m_waterCollisionSound;

	public delegate void BaitTouchedWaterEventHandler();
	public event BaitTouchedWaterEventHandler BaitTouchedWater;

	void Start(){
		m_baitTransform = this.transform;
		m_baitRigidbody = this.GetComponent<Rigidbody>();
	}

	void OnTriggerEnter(Collider other){
		m_waterCollisionSound.Play ();
		if (other.CompareTag ("Water")) {
			m_baitRigidbody.isKinematic = true;
			m_baitRigidbody.useGravity = false;
			m_baitRigidbody.velocity = new Vector3 (0, 0, 0);
			NotifyBaitTouchedWater ();
		}
	}

	void NotifyBaitTouchedWater(){
		if (BaitTouchedWater != null) {
			BaitTouchedWater ();
		}
	}
}

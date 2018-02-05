using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling_Tile : MonoBehaviour {
	private Vector4 currentPosition;
	private Quaternion originalRotation;
 

	// Use this for initialization
	void Start () {
		this.currentPosition = transform.position;
		originalRotation = transform.rotation;
	}


	public void ReturnToPosition(){
		transform.position = currentPosition; 
		transform.rotation = originalRotation;
		this.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll; 
	} 

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("UnderSea")) {
			this.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
		}
	}
	// Update is called once per frame
}

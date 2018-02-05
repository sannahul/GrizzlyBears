using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	
	public Vector3 position;
	
	void Start()
	{
		position = transform.position;
	}
	
	/*public void reset()
	{
		gameObject.SetActive(true);
		transform.position = position;
	}*/
	
	void Update() 
	{
		transform.Rotate (new Vector3 (0,45,0) * Time.deltaTime);
	}
}
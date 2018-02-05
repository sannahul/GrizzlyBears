using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour {

	public BlockType type;
	public bool current;
	public Sprite off;
	public Sprite on;
	public Image image;

	// Use this for initialization
	void Start () 
	{
		current = false;
		image = gameObject.GetComponent<Image>();
		on = Resources.Load<Sprite>(type.ToString()+"_on");
		off = Resources.Load<Sprite>(type.ToString());
	}

	public void setCurrent()
	{
		//Debug.Log (type.ToString());
		//Debug.Log ("on = " + on);
		//Debug.Log ("image = " + image);
		image.sprite = on;
		current = true;
	}

	public void notCurrent()
	{
		current = false;
		image.sprite = off;
	}

	public void setType(BlockType t)
	{
		type = t;
	}

	public BlockType getType()
	{
		return type;
	}

/*	// Update is called once per frame
	void Update ()
	{
		if (current)
		{
			cr = on;
		}
		else cr = off;

	} */
}

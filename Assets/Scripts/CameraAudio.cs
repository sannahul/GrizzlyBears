using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAudio : MonoBehaviour {

	public AudioClip clicksound;
	public AudioClip movesound;
	public AudioClip jumpsound;
	public AudioClip fishsound;
	public AudioClip endtilesound;
	public AudioClip splashsound;
	public AudioSource source;

	void Start()
	{

	}

	// Use this for initialization
	public void click()
	{
		source.PlayOneShot (clicksound, 0.5F);
	}

	public void move()
	{
		source.PlayOneShot (movesound, 1F);
	}

	public void jump()
	{
		source.PlayOneShot (jumpsound, 1F);
	}

	public void fish()
	{
		source.PlayOneShot (fishsound, 1F);
	}

	public void endtile()
	{
		source.PlayOneShot (endtilesound, 1F);
	}

	public void splash()
	{
		source.PlayOneShot (splashsound, 1F);
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterPersist : MonoBehaviour {

	private static int? activeChapter;

	void Start () {
		activeChapter = GlobalControl.getInstance ().getActiveChapter ();
			if (activeChapter != null) {
			GameObject.Find ("WelcomeScreen").SetActive (false);	
			GameObject.Find ("Canvas").transform.GetChild ((int)activeChapter).gameObject.SetActive (true);
		}
	}

	public static void setActiveChapter () {
		GameObject go1 = GameObject.Find ("Chapter1");
		GameObject go2 = GameObject.Find ("Chapter2");
		GameObject go3 = GameObject.Find ("Chapter3");

		if(go1.activeSelf) {
			activeChapter = 1;
		} else if (go2.activeSelf) {
			activeChapter = 2;
		} else if (go3.activeSelf) {
			activeChapter = 3;
		}

		GlobalControl.getInstance ().setActiveChapter (activeChapter);
	}
}

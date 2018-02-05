using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour {

	static GlobalControl instance;

	public static int? activeChapter;

	public static GlobalControl getInstance () {
		return instance;
	}

	void Awake () {
		if (instance == null) {
			GameObject.DontDestroyOnLoad(gameObject);
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
		
	public int? getActiveChapter () {
		if (activeChapter != null) {
			return activeChapter;
		} else {
			return null;
		}
	}

	public void setActiveChapter (int? chapter) {
		activeChapter = chapter;
	}
}

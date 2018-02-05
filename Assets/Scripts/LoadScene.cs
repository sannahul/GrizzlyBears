using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {



	public void changeScene(string sceneName) {
		SceneManager.LoadScene(sceneName);

		if (SceneManager.GetActiveScene ().name == "Menu") {
			ChapterPersist.setActiveChapter ();
		}
	}
}

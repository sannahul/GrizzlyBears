using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateMenu : MonoBehaviour {

	GameObject[] scores;
	int i;
	string levelName;
	int chapter;

	// Use this for initialization
	void Start () {
		i = 1;
		chapter = int.Parse (this.gameObject.name.Split ('_') [1]);
		Debug.Log (chapter);
		scores = GameObject.FindGameObjectsWithTag ("Score");
		Debug.Log (scores);
		foreach (GameObject score in scores) 
		{
			levelName = "Level" + "1" + "_" + i.ToString();
			int savedScore = PlayerPrefs.GetInt (levelName, 0);
			Debug.Log ("Score found for level " + i.ToString () + " = " + savedScore.ToString ());
			string current = score.GetComponent<Text> ().text;
			string[] curtext = current.Split ('/');
			score.GetComponent<Text> ().text = savedScore.ToString() + "/" + curtext [1];
			i++;
		}
	}

	public void deleteProgress ()
	{
		i = 1;
		chapter = int.Parse (this.gameObject.name.Split ('_') [1]);
		scores = GameObject.FindGameObjectsWithTag ("Score");
		foreach (GameObject score in scores) 
		{
			levelName = "Level" + "1" + "_" + i.ToString();
			PlayerPrefs.SetInt (levelName, 0);
			i++;
		}
		Start ();
	}
	// Update is called once per frame
	void Update () {
		
	}
}

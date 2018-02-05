using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public GameObject dBox;
	public Text dText;
	string[] textList = { "Sup dawg", "How is it going?" };
	int currentText = 0;

	// Use this for initialization
	void Start () {
		dBox.SetActive (true);
		populateDialogue (textList[currentText]);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown (0)) {
			currentText++;
		}
		
		if (currentText < textList.Length) {
			populateDialogue (textList[currentText]);
		} else {
			dBox.SetActive (false);
		}
	}

	private void populateDialogue (string dialogue) {
		dText.text = dialogue;
	}
}

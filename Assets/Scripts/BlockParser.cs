using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParser : MonoBehaviour {

	private GameObject main;
	private GameObject a;
	private GameObject b;
	private GameObject c;
	private GameObject player;
	GameObject[] fallingTiles;
	private bool playable;
	
	// Which blockspace are we in (by int) so 0 is main, 1 is a, 2 is b etc
	private Stack<int> stack = new Stack<int>();
	
	// Which blocks are in the blockspaces, so spaces[0][1] is the 2nd block in main, spaces[2][3] is the 4th block in f2 
	private Block[][] spaces = new Block[4][];
	
	// Which position we are at in each blockspaces
	private int[] current = new int[4];
	private Block currentBlock = null;
	public bool isRunning;
	
	private static readonly float TIME_DELAY = 0.8f;
	private float timer;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		fallingTiles = GameObject.FindGameObjectsWithTag("Falling_1");
		stack.Push (0);
		for(int i = 0; i<4; i++)
		{
			current [i] = 0;
		}
		isRunning = false;
		timer = 0f;
		playable = true;
	}
	
	// Update is called once per frame
	void Update () {
	/*	BlockType move = nextMove ();
		if (move != BlockType.INVALID) {	
			Debug.Log (move);
		} */

		timer += Time.deltaTime;
		if (timer >= TIME_DELAY)
		{
			timer = 0f;
			if (isRunning)
			{
				PlayerMovement pl = (PlayerMovement) player.GetComponent(typeof(PlayerMovement));
				if (pl.commandComplete())
				{
					callMove();
				}
			}
		}
	}

	public void callMove()
	{
		BlockType move = nextMove();
		if (move == BlockType.INVALID) {
			playable = false;
		}
		else player.SendMessage ("makeMove", move);
	}
	
	public void resetPress()
	{
		currentBlock.SendMessage ("notCurrent");
		isRunning = false;
		player.SendMessage("reset");
		// call reset on collectibles
		stack.Clear();
		stack.Push (0);

		foreach (GameObject ft in fallingTiles) {

			ft.GetComponent<Falling_Tile>().ReturnToPosition();

		} 
		 

		for(int i = 0; i<4; i++)
		{
			current [i] = 0;
		}
		GameObject[] fish = GameObject.FindGameObjectsWithTag("Pick Up");
		foreach (GameObject f in fish)
		{
			f.GetComponent<Renderer>().enabled = (true);
			//if (t.tag = 
			//collects[i].SendMessage("reset");
		}
		playable = true;
	}

	public void buttonPress()
	{
		if (!playable) {
			resetPress ();
		}
		if(!isRunning & playable)
		{
			main = GameObject.Find ("BlockSpaceMain");
			a = GameObject.Find("BlockSpaceA");
			b = GameObject.Find("BlockSpaceB");
			c = GameObject.Find("BlockSpaceC");
			spaces [0] = main.GetComponentsInChildren<Block> ();
			if(a!=null) spaces [1] = a.GetComponentsInChildren<Block> ();
			if(b!=null) spaces [2] = b.GetComponentsInChildren<Block> ();
			if(c!=null) spaces [3] = c.GetComponentsInChildren<Block> ();
			isRunning = true;
			//InvokeRepeating ("callMove", 0.1f, 0.8f);
		}
		return;
	}
		
	BlockType nextMove()
	{
		if (currentBlock != null)
		{
			currentBlock.SendMessage ("notCurrent");
		}
		// while current position in current stack is greater than 
		//  number of spaces in current stack, jump back one "scope"
		while(current[stack.Peek()]>= spaces[stack.Peek()].Length)
		{
			if (stack.Peek() == 0)
			{
				isRunning = false;
				return BlockType.INVALID;
			}
			current [stack.Peek()] = 0;
			stack.Pop ();
		}

		currentBlock = spaces [stack.Peek()] [current [stack.Peek()]];
		currentBlock.SendMessage ("setCurrent");
		BlockType move = currentBlock.getType ();

		current [stack.Peek()] = current [stack.Peek()] + 1;

		switch(move)
		{
		case BlockType.FunctionA:
			stack.Push (1);
			break;
		case BlockType.FunctionB:
			stack.Push (2);
			break;
		case BlockType.FunctionC:
			stack.Push (3);
			break;
		}
			
		return move;
	}
}

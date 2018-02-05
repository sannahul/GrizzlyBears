// Contains all blockspaces and keeps track of which is currently being read

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceControl {
	/*
	private List<BlockSpace> spaces;
	private Stack<int> stack;

	public SpaceControl()
	{
		spaces = new List<BlockSpace>();
		stack = new Stack<int> ();
		stack.Push(0);
	}	

	 // Resets all blockspaces and returns to main
	public void ResetCurrent()
	{
		foreach (BlockSpace x in spaces)
		{
			x.Restart ();
		}
		stack.Clear ();
		stack.Push (0);
	}

	// Returns next instruction from current space
	// Moves between spaces as required
	public BlockType NextBlock()
	{
		BlockType move = spaces [stack.Peek()].NextBlock ();
		while(move == BlockType.INVALID)
		{
			if (stack.Peek() == 0)
				return BlockType.INVALID;
			stack.Pop ();
			move = spaces[stack.Peek()].NextBlock();
		}
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

	public void CreateSpaces(int x)
	{
		for (int i = 0; i < x; i++) {
			BlockSpace newspace = new BlockSpace();
				spaces.Add (newspace);
		}
	}

	public int GetNumSpaces()
	{
		return spaces.Count;
	}

	public void AddBlock(Block b, int sindex, int index)
	{
		spaces [sindex].AddBlock (b, index);
	}

	public Block RemoveBlock(int sindex, int index)
	{
		return spaces [sindex].RemoveBlock (index);
	}*/
}

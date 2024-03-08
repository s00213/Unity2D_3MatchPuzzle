using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
	Board board;

	private void Awake()
	{
		board = FindObjectOfType<Board>();
	}

	public void ShuffleButton()
	{
		board.ShufflePuzzles();
	}
}

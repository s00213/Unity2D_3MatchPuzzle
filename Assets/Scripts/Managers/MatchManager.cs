using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchManager : MonoBehaviour
{
	public List<Puzzle> matchStatus = new List<Puzzle>();

	Board board;
	Puzzle curPuzzle, leftPuzzle, rightPuzzle, upPuzzle, downPuzzle;
	
	void Awake()
	{
		board = FindObjectOfType<Board>();
	}

	public void MatchPuzzleType()
	{
		for (int x = 0; x < board.width; x++)
		{
			for (int y = 0; y < board.height; y++)
			{
				curPuzzle = board.allPuzzles[x, y];
				if (curPuzzle != null)
				{
					if (x > 0 && x < board.width - 1)
					{
						leftPuzzle = board.allPuzzles[x - 1, y];
						rightPuzzle = board.allPuzzles[x + 1, y];
						if (leftPuzzle != null && rightPuzzle != null)
						{
							if (leftPuzzle.type == curPuzzle.type && rightPuzzle.type == curPuzzle.type)
							{
								curPuzzle.isMatched = true;
								leftPuzzle.isMatched = true;
								rightPuzzle.isMatched = true;

								matchStatus.Add(curPuzzle);
								matchStatus.Add(leftPuzzle);
								matchStatus.Add(rightPuzzle);
							}
						}
					}
				}

				if (y > 0 && y < board.height - 1)
				{
					upPuzzle = board.allPuzzles[x, y + 1];
					downPuzzle = board.allPuzzles[x, y - 1];
					if (upPuzzle != null && downPuzzle != null)
					{
						if (upPuzzle.type == curPuzzle.type && downPuzzle.type == curPuzzle.type)
						{
							curPuzzle.isMatched = true;
							upPuzzle.isMatched = true;
							downPuzzle.isMatched = true;

							matchStatus.Add(curPuzzle);
							matchStatus.Add(upPuzzle);
							matchStatus.Add(downPuzzle);
						}
					}
				}
			}
		}

		if (matchStatus.Count > 0)
		{
			matchStatus = matchStatus.Distinct().ToList();
		}
	}	
}


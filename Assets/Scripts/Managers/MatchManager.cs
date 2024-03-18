using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
		matchStatus.Clear();

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
							if (leftPuzzle.type == curPuzzle.type && rightPuzzle.type == curPuzzle.type && curPuzzle.type != Puzzle.PuzzleType.Brick)
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
						if (upPuzzle.type == curPuzzle.type && downPuzzle.type == curPuzzle.type && curPuzzle.type != Puzzle.PuzzleType.Brick)
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

		CheckBomb();
	}

	public void CheckBomb()
	{
		for (int i = 0; i < matchStatus.Count; i++)
		{
			Puzzle puzzle = matchStatus[i];

			int x = puzzle.posIndex.x;
			int y = puzzle.posIndex.y;

			// 폭탄 퍼즐 주변의 인접한 퍼즐을 확인함
			for (int dx = -1; dx <= 1; dx++)
			{
				for (int dy = -1; dy <= 1; dy++)
				{
					int checkX = x + dx;
					int checkY = y + dy;

					// 보드 내부에 있는지 확인함
					if (checkX >= 0 && checkX < board.width && checkY >= 0 && checkY < board.height)
					{
						// 해당 위치에 퍼즐이 있는지 확인하고 폭탄인지 검사함
						Puzzle neighborPuzzle = board.allPuzzles[checkX, checkY];
						if (neighborPuzzle != null && neighborPuzzle.type == Puzzle.PuzzleType.Bomb)
						{
							// 폭탄 퍼즐을 기준으로 폭발 범위 내의 퍼즐들을 처리함
							BombExplosionRange(new Vector2Int(checkX, checkY), neighborPuzzle);
						}
					}
				}
			}
		}
	}

	public void BombExplosionRange(Vector2Int bombPos, Puzzle _Bomb)
	{
		for (int x = bombPos.x - 1; x <= bombPos.x + 1; x++)
		{
			for (int y = bombPos.y - 1; y <= bombPos.y + 1; y++)
			{
				// 보드 내에 있는 유효한 좌표인지 확인함
				if (x >= 0 && x < board.width && y >= 0 && y < board.height)
				{
					if (board.allPuzzles[x, y] != null)
					{
						// 폭발 범위 내에 있는 퍼즐들을 매치 상태로 표시함
						board.allPuzzles[x, y].isMatched = true;
						matchStatus.Add(board.allPuzzles[x, y]);
					}
				}
			}
		}

		// 중복된 매치 상태를 제거합함
		matchStatus = matchStatus.Distinct().ToList();
	}
}


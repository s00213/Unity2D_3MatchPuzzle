using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
	[Header("Board")]
	public int width;
	public int height;
	public float puzzleSpeed;
	public GameObject BackgroundTilePrefab;
	[Space]
	public Puzzle[] puzzles;
	public Puzzle[,] allPuzzles;

	MatchManager matchManager;

	int iteration = 0;
	int nullCount = 0; // 연속된 빈 공간의 수

	void Awake()
	{
		matchManager = FindObjectOfType<MatchManager>();
	}

	void Start()
	{
		allPuzzles = new Puzzle[width, height];
		
		BoardSetUp();
	}

	void Update()
	{
		matchManager.MatchPuzzleType();
	}

	void BoardSetUp()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{ 
				Vector2 pos = new Vector2(x, y);
				GameObject backgroundTile = Instantiate(BackgroundTilePrefab, pos, Quaternion.identity);
				backgroundTile.transform.parent = transform;
				backgroundTile.name = "BackgroundTile : " + x + ", " + y;

				int puzzleToUse = Random.Range(0, puzzles.Length);

				while (MatchSamePuzzles(new Vector2Int(x, y), puzzles[puzzleToUse]) && iteration < 100)
				{
					puzzleToUse = Random.Range(0, puzzles.Length);
					iteration++;

					if (iteration > 0)
					{
						Debug.Log(iteration);
					}
				}

				SpawnPuzzle(new Vector2Int(x, y), puzzles[puzzleToUse]);
			}
		}
	}

	void SpawnPuzzle(Vector2Int pos, Puzzle puzzleToSpawn)
	{
		Puzzle puzzle = Instantiate(puzzleToSpawn, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
		puzzle.transform.parent = transform;
		puzzle.name = "Puzzle - " + pos.x + ", " + pos.y;
		allPuzzles[pos.x, pos.y] = puzzle;

		puzzle.PuzzleSetUp(pos, this);
	}

	// 가로 또는 세로 방향으로 일치하는 보석이 있는지를 확인
	bool MatchSamePuzzles(Vector2Int checkPos, Puzzle checkPuzzle)
	{
		if (checkPos.x > 1)
		{
			if (allPuzzles[checkPos.x - 1, checkPos.y].type == checkPuzzle.type && allPuzzles[checkPos.x - 2, checkPos.y].type == checkPuzzle.type)
			{ 
				return true;
			}
		}

		if (checkPos.y > 1)
		{
			if (allPuzzles[checkPos.x, checkPos.y - 1].type == checkPuzzle.type && allPuzzles[checkPos.x, checkPos.y - 2].type == checkPuzzle.type)
			{
				return true;
			}
		}

		return false;
	}

	// 매치된 상태인 퍼즐 오브젝트를 삭제하고 배열에서 해당 퍼즐 위치를 null이 되도록 함
	void DestroySamePuzzles(Vector2Int pos)
	{
		if (allPuzzles[pos.x, pos.y] != null)
		{
			if (allPuzzles[pos.x, pos.y].isMatched)
			{
				Destroy(allPuzzles[pos.x, pos.y].gameObject);
				allPuzzles[pos.x, pos.y] = null;
			}
		}
	}

	// Match Status 리스트에 저장된 위치를 기반으로 퍼즐 오브젝트를 삭제함
	public void DestroyMatch()
	{ 
		for (int i = 0; i < matchManager.matchStatus.Count; i++) 
		{
			if (matchManager.matchStatus[i] != null)
			{
				DestroySamePuzzles(matchManager.matchStatus[i].posIndex);
			}
		}

		StartCoroutine(FallPuzzleRoutine());
	}

	// 빈 공간이 생기면 해당 열의 모든 퍼즐을 아래로 이동
	IEnumerator FallPuzzleRoutine()
	{
		yield return new WaitForSeconds(0.1f);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// 현재 위치에 퍼즐이 없다면
				if (allPuzzles[x, y] == null)
				{
					// nullCount 증가
					nullCount++;
				}
				// nullCount가 있다면
				else if (nullCount > 0)
				{
					// nullCount만큼 해당 y 위치의 퍼즐 오브젝트를 내려보냄
					allPuzzles[x, y].posIndex.y -= nullCount;
					// 내린 위치에 퍼즐의 위치 정보도 내려보냄
					allPuzzles[x, y - nullCount] = allPuzzles[x, y];
					// 원래 위치의 퍼즐은 null이 되도록 함
					allPuzzles[x, y] = null;
				}
			}

			// nullCount 0으로 초기화
			nullCount = 0;
		}
	}
}

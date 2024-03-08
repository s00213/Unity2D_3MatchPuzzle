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

	int iteration = 0; // 반복
	int nullCount = 0; // 연속된 빈 공간의 수
	int puzzleToUse; // 사용할 퍼즐

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

				puzzleToUse = Random.Range(0, puzzles.Length);

				// 현재 위치와 선택된 퍼즐이 일치하는지 확인하고 100미만안 동안에 반복함
				while (MatchSamePuzzles(new Vector2Int(x, y), puzzles[puzzleToUse]) && iteration < 100)
				{
					// 퍼즐 배열에서 랜덤으로 퍼즐 선택
					puzzleToUse = Random.Range(0, puzzles.Length);
					iteration++;

					if (iteration > 0)
					{
						Debug.Log(iteration);
					}
				}

				SpawnPuzzles(new Vector2Int(x, y), puzzles[puzzleToUse]);
			}
		}
	}

	void SpawnPuzzles(Vector2Int pos, Puzzle puzzleToSpawn)
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
			// 가로 방향에서 2칸 떨어진 퍼즐과 비교할 떄
			if (allPuzzles[checkPos.x - 1, checkPos.y].type == checkPuzzle.type && allPuzzles[checkPos.x - 2, checkPos.y].type == checkPuzzle.type)
			{
				// 같다면 true
				return true;
			}
		}

		if (checkPos.y > 1)
		{
			// 세로 방향에서 2칸 떨어진 퍼즐과 비교할 떄
			if (allPuzzles[checkPos.x, checkPos.y - 1].type == checkPuzzle.type && allPuzzles[checkPos.x, checkPos.y - 2].type == checkPuzzle.type)
			{
				// 같다면 true
				return true;
			}
		}

		// 그 외의 경우에는 항상 false를 반환
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

		StartCoroutine(FallPuzzlesRoutine());
	}

	// 빈 공간이 생기면 해당 y 위치의 모든 퍼즐을 아래로 이동
	IEnumerator FallPuzzlesRoutine()
	{
		yield return new WaitForSeconds(0.2f);

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

		StartCoroutine(RefillPuzzlesRoutine());
	}

	IEnumerator RefillPuzzlesRoutine()
	{
		yield return new WaitForSeconds(0.2f);
		RefillPuzzles();

		yield return new WaitForSeconds(0.2f);

		matchManager.MatchPuzzleType();

		// matchStatus 리스트에 일치하는 퍼즐이 하나 이상 있는 경우
		if (matchManager.matchStatus.Count > 0)
		{
			yield return new WaitForSeconds(1.0f);
			// 매치된 퍼즐들 삭제
			DestroyMatch();
		}
	}

	void RefillPuzzles()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// 비어 있는 위치에 퍼즐이 없는 경우
				if (allPuzzles[x, y] == null)
				{
					// puzzles 배열의 길이를 기반으로 무작위로 인덱스를 선택함
					puzzleToUse = Random.Range(0, puzzles.Length);
					// 선택된 퍼즐을 현재 위치에 생성함
					SpawnPuzzles(new Vector2Int(x, y), puzzles[puzzleToUse]);
				}
			}
		}
	}
}

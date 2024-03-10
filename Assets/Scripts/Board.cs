using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
	public enum BoardStatus { Idle, Move }
	public BoardStatus curStatus = BoardStatus.Move; //  ���� ���� �ÿ��� Move���� ��

	List<Puzzle> findPuzzles = new List<Puzzle>();
	List<Puzzle> tempPuzzles = new List<Puzzle>();

	[Header("Board")]
	public int width;
	public int height;
	public float puzzleSpeed;
	public GameObject BackgroundTilePrefab;
	public Button shuffleButton;
	[Space]
	public Puzzle[] puzzles;
	public Puzzle[,] allPuzzles;

	MatchManager matchManager;
	RoundManager roundManager;

	int iteration = 0; // �ݺ�
	int nullCount = 0; // ���ӵ� �� ������ ��
	int puzzleToUse; // ����� ����
	int shuffleCount = 0; // ���� Ƚ���� �����ϴ� ����

	void Awake()
	{
		matchManager = FindObjectOfType<MatchManager>();
		roundManager = FindObjectOfType<RoundManager>();
	}

	void Start()
	{
		allPuzzles = new Puzzle[width, height];

		BoardSetUp();
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

				// ���� ��ġ�� ���õ� ������ ��ġ�ϴ��� Ȯ���ϰ� 100�̸��� ���ȿ� �ݺ���
				while (MatchSamePuzzles(new Vector2Int(x, y), puzzles[puzzleToUse]) && iteration < 100)
				{
					// ���� �迭���� �������� ���� ����
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
		Puzzle puzzle = Instantiate(puzzleToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);

		puzzle.transform.parent = transform;
		puzzle.name = "Puzzle - " + pos.x + ", " + pos.y;
		allPuzzles[pos.x, pos.y] = puzzle;

		puzzle.PuzzleSetUp(pos, this);
	}

	// ���� �Ǵ� ���� �������� ��ġ�ϴ� ������ �ִ����� Ȯ��
	bool MatchSamePuzzles(Vector2Int checkPos, Puzzle checkPuzzle)
	{
		if (checkPos.x > 1)
		{
			// ���� ���⿡�� 2ĭ ������ ����� ���� ��
			if (allPuzzles[checkPos.x - 1, checkPos.y].type == checkPuzzle.type && allPuzzles[checkPos.x - 2, checkPos.y].type == checkPuzzle.type)
			{
				// ���ٸ� true
				return true;
			}
		}

		if (checkPos.y > 1)
		{
			// ���� ���⿡�� 2ĭ ������ ����� ���� ��
			if (allPuzzles[checkPos.x, checkPos.y - 1].type == checkPuzzle.type && allPuzzles[checkPos.x, checkPos.y - 2].type == checkPuzzle.type)
			{
				// ���ٸ� true
				return true;
			}
		}

		// �� ���� ��쿡�� �׻� false�� ��ȯ
		return false;
	}

	// ��ġ�� ������ ���� ������Ʈ�� �����ϰ� �迭���� �ش� ���� ��ġ�� null�� �ǵ��� ��
	void DestroySamePuzzles(Vector2Int pos)
	{
		if (allPuzzles[pos.x, pos.y] != null)
		{
			if (allPuzzles[pos.x, pos.y].isMatched)
			{
				Instantiate(allPuzzles[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);

				Destroy(allPuzzles[pos.x, pos.y].gameObject);
				allPuzzles[pos.x, pos.y] = null;
			}
		}
	}

	// Match Status ����Ʈ�� ����� ��ġ�� ������� ���� ������Ʈ�� ������
	public void DestroyMatch()
	{
		for (int i = 0; i < matchManager.matchStatus.Count; i++)
		{
			if (matchManager.matchStatus[i] != null)
			{
				CheckScore(matchManager.matchStatus[i]);

				DestroySamePuzzles(matchManager.matchStatus[i].posIndex);
			}
		}

		StartCoroutine(FallPuzzlesRoutine());
	}

	// �� ������ ����� �ش� y ��ġ�� ��� ������ �Ʒ��� �̵�
	IEnumerator FallPuzzlesRoutine()
	{
		yield return new WaitForSeconds(0.3f);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// ���� ��ġ�� ������ ���ٸ�
				if (allPuzzles[x, y] == null)
				{
					// nullCount ����
					nullCount++;
				}
				// nullCount�� �ִٸ�
				else if (nullCount > 0)
				{
					// nullCount��ŭ �ش� y ��ġ�� ���� ������Ʈ�� ��������
					allPuzzles[x, y].posIndex.y -= nullCount;
					// ���� ��ġ�� ������ ��ġ ������ ��������
					allPuzzles[x, y - nullCount] = allPuzzles[x, y];
					// ���� ��ġ�� ������ null�� �ǵ��� ��
					allPuzzles[x, y] = null;
				}
			}

			// nullCount 0���� �ʱ�ȭ
			nullCount = 0;
		}

		StartCoroutine(RefillPuzzlesRoutine());
	}

	IEnumerator RefillPuzzlesRoutine()
	{
		yield return new WaitForSeconds(0.3f);
		
		RefillPuzzles();

		yield return new WaitForSeconds(0.3f);

		matchManager.MatchPuzzleType();

		// matchStatus ����Ʈ�� ��ġ�ϴ� ������ �ϳ� �̻� �ִ� ���
		if (matchManager.matchStatus.Count > 0)
		{
			yield return new WaitForSeconds(1.0f);
			// ��ġ�� ����� ����
			DestroyMatch();
		}
		else 
		{
			yield return new WaitForSeconds(0.3f);

			curStatus = Board.BoardStatus.Move;
		}
	}

	// ����ִ� ��ġ�� ������ ä��
	void RefillPuzzles()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// ��� �ִ� ��ġ�� ������ ���� ���
				if (allPuzzles[x, y] == null)
				{
					// puzzles �迭�� ���̸� ������� �������� �ε����� ������
					puzzleToUse = Random.Range(0, puzzles.Length);
					// ���õ� ������ ���� ��ġ�� ������
					SpawnPuzzles(new Vector2Int(x, y), puzzles[puzzleToUse]);
				}
			}
		}

		CheckMisplacePuzzles();
	}

	// �߸��� ��ġ�� ��ġ�� ������ Ȯ���Ͽ� ������
	void CheckMisplacePuzzles()
	{
		// findPuzzles ����Ʈ�� ã�� ��� Puzzle ������Ʈ�� �߰���
		findPuzzles.AddRange(FindObjectsOfType<Puzzle>());

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// ���� ��ġ�� �ִ� ������ findPuzzles ����Ʈ�� ���ԵǾ� �ִٸ� ������
				if (findPuzzles.Contains(allPuzzles[x, y]))
				{
					findPuzzles.Remove(allPuzzles[x, y]);
				}
			}
		}

		// findPuzzles ����Ʈ�� �����ִٸ� ���� ������Ʈ�� ������ �ݺ���
		foreach (Puzzle g in findPuzzles)
		{
			Destroy(g.gameObject);
		}
	}

	public void ShufflePuzzles()
	{
		if (curStatus != BoardStatus.Idle)
		{
			curStatus = BoardStatus.Idle;

			// �� �������� ����ǵ��� ������
			if (shuffleCount < 3)
			{
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						// ���� ��ġ�� ������ ����Ʈ�� �߰��ϰ� �迭���� ���� ��
						tempPuzzles.Add(allPuzzles[x, y]);
						allPuzzles[x, y] = null;
					}
				}

				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						// �������� ������ ������
						puzzleToUse = Random.Range(0, tempPuzzles.Count);

						// ������ ������ ������ �����ϴ� ��� �ִ� 100������ ��õ���
						iteration = 0;
						while (MatchSamePuzzles(new Vector2Int(x, y), tempPuzzles[puzzleToUse]) && iteration < 100 && tempPuzzles.Count > 1)
						{
							puzzleToUse = Random.Range(0, tempPuzzles.Count);
							iteration++;
						}

						// ���õ� ������ ���� ��ġ�� ��ġ�ϰ� ������ ������
						tempPuzzles[puzzleToUse].PuzzleSetUp(new Vector2Int(x, y), this);
						allPuzzles[x, y] = tempPuzzles[puzzleToUse];

						// ���� ���Ŀ� �ش� ������ tempPuzzles ����Ʈ���� �����Ͽ� �ߺ��� ������
						tempPuzzles.RemoveAt(puzzleToUse);
					}
				}

				shuffleCount++;

				if (shuffleCount >= 3)
				{
					shuffleButton.gameObject.SetActive(false);
				}
			}

			StartCoroutine(RefillPuzzlesRoutine());
		}
	}

	// ������ ���ھ Ȯ���ϰ� ���� ���ھ ������Ʈ��
	public void CheckScore(Puzzle puzzleToCheck)
	{
		roundManager.curScore += puzzleToCheck.scoreValue;
	}
}


﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
	public enum BoardStatus { Idle, Move }
	public BoardStatus curStatus = BoardStatus.Move; //  게임 시작 시에는 Move여야 함

	List<Puzzle> findPuzzles = new List<Puzzle>();
	List<Puzzle> tempPuzzles = new List<Puzzle>();

	[Header("Board")]
	public int width;
	public int height;
	public float puzzleSpeed;
	public GameObject BackgroundTilePrefab;
	public Button shuffleButton;
	[Space]
	[Header("Bonus")]
	public float combo;
	public float comboAmount = 1.5f;
	public TextMeshProUGUI comboText;
	public float displayComboTime = 2.0f;
	[Space]
	public Puzzle[] puzzles;
	public Puzzle[,] allPuzzles;

	MatchManager matchManager;
	RoundManager roundManager;

	int iteration = 0; // 반복
	int nullCount = 0; // 연속된 빈 공간의 수
	int puzzleToUse; // 사용할 퍼즐
	int shuffleCount = 0; // 셔플 횟수를 저장하는 변수

	void Awake()
	{
		matchManager = FindObjectOfType<MatchManager>();
		roundManager = FindObjectOfType<RoundManager>();
	}

	void Start()
	{
		allPuzzles = new Puzzle[width, height];

		comboText.gameObject.SetActive(false);

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
		Puzzle puzzle = Instantiate(puzzleToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);

		puzzle.transform.parent = transform;
		puzzle.name = "Puzzle - " + pos.x + ", " + pos.y;
		allPuzzles[pos.x, pos.y] = puzzle;

		puzzle.PuzzleSetUp(pos, this);
	}

	// 가로 또는 세로 방향으로 일치하는 퍼즐이 있는지를 확인
	bool MatchSamePuzzles(Vector2Int checkPos, Puzzle checkPuzzle)
	{
		if (checkPos.x > 1)
		{
			// 가로 방향에서 2칸 떨어진 퍼즐과 비교할 때
			if (allPuzzles[checkPos.x - 1, checkPos.y].type == checkPuzzle.type
				&& allPuzzles[checkPos.x - 2, checkPos.y].type == checkPuzzle.type)
			{
				// 같다면 true
				return true;
			}
		}

		if (checkPos.y > 1)
		{
			// 세로 방향에서 2칸 떨어진 퍼즐과 비교할 때
			if (allPuzzles[checkPos.x, checkPos.y - 1].type == checkPuzzle.type
				&& allPuzzles[checkPos.x, checkPos.y - 2].type == checkPuzzle.type)
			{
				// 같다면 true
				return true;
			}
		}

		// 그 외의 경우에는 항상 false를 반환함
		return false;
	}

	// 매치된 상태인 퍼즐 오브젝트를 삭제하고 배열에서 해당 퍼즐 위치를 null이 되도록 함
	void DestroySamePuzzles(Vector2Int pos)
	{
		if (allPuzzles[pos.x, pos.y] != null)
		{
			if (allPuzzles[pos.x, pos.y].isMatched)
			{
				if (allPuzzles[pos.x, pos.y].type == Puzzle.PuzzleType.Brick)
				{
					SoundManager.Sound.PlayBrick();
				}
				else
				{
					SoundManager.Sound.PlayMatch();
				}

				Instantiate(allPuzzles[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);


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
				CheckScore(matchManager.matchStatus[i]);

				DestroySamePuzzles(matchManager.matchStatus[i].posIndex);
			}
		}

		StartCoroutine(FallPuzzlesRoutine());
	}


	// 빈 공간이 생기면 해당 y 위치의 모든 퍼즐을 아래로 이동
	IEnumerator FallPuzzlesRoutine()
	{
		yield return new WaitForSeconds(0.3f);

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
		yield return new WaitForSeconds(0.3f);

		RefillPuzzles();

		yield return new WaitForSeconds(0.3f);

		matchManager.MatchPuzzleType();

		// matchStatus 리스트에 일치하는 퍼즐이 하나 이상 있는 경우
		if (matchManager.matchStatus.Count > 0)
		{
			// 일치하는 퍼즐이 있으면 combo를 증가
			combo++;

			yield return new WaitForSeconds(1.0f);
			// 매치된 퍼즐들 삭제
			DestroyMatch();
		}
		else
		{
			yield return new WaitForSeconds(0.3f);

			curStatus = Board.BoardStatus.Move;

			// combo를 초기화함
			combo = 0f;
		}
	}

	// 비어있는 위치에 퍼즐을 채움
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

		CheckMisplacePuzzles();
	}

	// 잘못된 위치에 배치된 퍼즐을 확인하여 제거함
	void CheckMisplacePuzzles()
	{
		// findPuzzles 리스트에 찾은 모든 Puzzle 오브젝트를 추가함
		findPuzzles.AddRange(FindObjectsOfType<Puzzle>());

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				// 현재 위치에 있는 퍼즐이 findPuzzles 리스트에 포함되어 있다면 삭제함
				if (findPuzzles.Contains(allPuzzles[x, y]))
				{
					findPuzzles.Remove(allPuzzles[x, y]);
				}
			}
		}

		// findPuzzles 리스트에 남아있다면 게임 오브젝트를 삭제를 반복함
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

			// 세 번까지만 실행되도록 제한함
			if (shuffleCount < 3)
			{
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						// 현재 위치의 퍼즐을 리스트에 추가하고 배열에서 비우게 함
						tempPuzzles.Add(allPuzzles[x, y]);
						allPuzzles[x, y] = null;
					}
				}

				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						// 랜덤으로 퍼즐을 선택함
						puzzleToUse = Random.Range(0, tempPuzzles.Count);

						// 동일한 종류의 퍼즐을 선택하는 경우 최대 100번까지 재시도함
						iteration = 0;
						while (MatchSamePuzzles(new Vector2Int(x, y), tempPuzzles[puzzleToUse]) && iteration < 100 && tempPuzzles.Count > 1)
						{
							puzzleToUse = Random.Range(0, tempPuzzles.Count);
							iteration++;
						}

						// 선택된 퍼즐을 현재 위치에 배치하고 정보를 설정함
						tempPuzzles[puzzleToUse].PuzzleSetUp(new Vector2Int(x, y), this);
						allPuzzles[x, y] = tempPuzzles[puzzleToUse];

						// 셔플 이후에 해당 퍼즐을 tempPuzzles 리스트에서 제거하여 중복을 방지함
						tempPuzzles.RemoveAt(puzzleToUse);
					}
				}

				shuffleCount++;
				SoundManager.Sound.PlayShuffle();

				if (shuffleCount >= 3)
				{
					shuffleButton.gameObject.SetActive(false);
				}
			}

			StartCoroutine(RefillPuzzlesRoutine());
		}
	}

	// 교착 상태 확인
	bool IsStuck()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Puzzle puzzle = allPuzzles[x, y];

				// 가로에 있는 퍼즐과 비교함
				if (x < width - 1)
				{
					// 오른쪽 퍼즐과 퍼즐 타입이 같다면 교착 상태가 아님
					Puzzle rightPuzzle = allPuzzles[x + 1, y];
					if (puzzle.type == rightPuzzle.type)
					{
						return false;
					}
				}

				// 세로에 있는 퍼즐과 비교함
				if (y > 0)
				{
					// 아래 퍼즐과 퍼즐 타입이 같다면 교착 상태가 아님
					Puzzle downPuzzle = allPuzzles[x, y - 1];
					if (puzzle.type == downPuzzle.type)
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	// 퍼즐의 스코어를 확인하고 현재 스코어를 업데이트함
	public void CheckScore(Puzzle puzzleToCheck)
	{
		roundManager.curScore += puzzleToCheck.scoreValue;

		// combo 적용된 경우
		if (combo > 0)
		{
			// combo 따라 추가될 bonusAmount를 계산함
			float comboToAdd = puzzleToCheck.scoreValue * combo * comboAmount;
			// 보너스 값을 반올림하여 curScore에 더함
			roundManager.curScore += Mathf.RoundToInt(comboToAdd);

			UpdateComboText(comboToAdd);
			Debug.Log("Combo Amount : " + comboToAdd);
		}
	}

	void UpdateComboText(float combo)
	{
		comboText.text = "COMBO " + combo.ToString("0");

		comboText.gameObject.SetActive(true);
		SoundManager.Sound.PlayCombo();

		StartCoroutine(DisplayComboTextRoutine());
	}

	IEnumerator DisplayComboTextRoutine()
	{
		yield return new WaitForSeconds(displayComboTime);
		comboText.gameObject.SetActive(false);
	}
}

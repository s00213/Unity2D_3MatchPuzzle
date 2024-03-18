using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Puzzle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public enum PuzzleType 
	{ 
		Bear, Chick, Crocodile, Narwhal, Panda, Parrot, Penguin, Pig,
		Brick, Bomb
	}
	public PuzzleType type;

	[Header("Puzzle")]
	public Vector2Int posIndex;
	public Board board;
	public bool isMatched;
	public GameObject destroyEffect;
	public int scoreValue = 10;

	[HideInInspector] public Vector2Int prePos;

	Vector2 firstTouchPos;
	Vector2 finalTouchPos;

	MatchManager matchManager;
	RoundManager roundManager;
	Puzzle otherPuzzle;

	float swipeAngle = 0;

	void Awake()
	{
		matchManager = FindObjectOfType<MatchManager>();
		roundManager = FindObjectOfType<RoundManager>();
	}

	void Update()
	{
		ExchangePuzzles();
	}

	public void PuzzleSetUp(Vector2Int pos, Board _board)
	{
		posIndex = pos;
		board = _board;
	}

	// 포인터가 오브젝트 위에서 눌렸을 때 호출됨
	public void OnPointerDown(PointerEventData eventData)
	{
		if (board.curStatus == Board.BoardStatus.Move && roundManager.roundTime > 0)
		{
			firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	// 포인터를 뗄 때 호출됨
	public void OnPointerUp(PointerEventData eventData)
	{
		if (board.curStatus == Board.BoardStatus.Move && roundManager.roundTime > 0)
		{
			finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			CalculateSwipeAngle();
		}
	}

	void CalculateSwipeAngle()
	{
		swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x);
		swipeAngle = swipeAngle * 180 / Mathf.PI;
		Debug.Log("Swipe Angle : " + swipeAngle);

		if (Vector3.Distance(firstTouchPos, finalTouchPos) > 0.5f)
		{
			MovePuzzles();
		}
	}

	void MovePuzzles()
	{
		// Brick 퍼즐일 경우에만 교환을 막음
		if (type == Puzzle.PuzzleType.Brick)
		{
			return;
		}

		prePos = posIndex;

		// 1. 오른쪽 방향으로 교환할 경우
		// 터치의 각도가 -45에서 45도 사이일 때
		// 퍼즐이 보드의 오른쪽 끝에 위치하지 않았을 때
		// 퍼즐은 오른쪽으로 이동하게 되며, 오른쪽의 퍼즐은 현재 퍼즐의 위치로 이동함
		// Brick 퍼즐일 경우에만 교환을 막음
		if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)
		{
			otherPuzzle = board.allPuzzles[posIndex.x + 1, posIndex.y];
			if (otherPuzzle.type == Puzzle.PuzzleType.Brick) 
			{
				return;
			}
			otherPuzzle.posIndex.x--;
			posIndex.x++;
		}
		// 2. 왼쪽 방향으로 교환할 경우
		// 터치의 각도가 135보다 크거나 -135보다 작을 때
		// 퍼즐이 보드의 왼쪽 끝에 위치하지 않았을 때
		// 퍼즐은 왼쪽으로 이동하게 되며, 왼쪽의 퍼즐은 현재 퍼즐의 위치로 이동함
		// Brick 퍼즐일 경우에만 교환을 막음
		else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
		{
			if (posIndex.x <= 0)
			{
				return;
			}

			otherPuzzle = board.allPuzzles[posIndex.x - 1, posIndex.y];
			if (otherPuzzle.type == Puzzle.PuzzleType.Brick)
			{
				return;
			}
			otherPuzzle.posIndex.x++;
			posIndex.x--;
		}
		// 3. 위쪽 방향으로 교환할 경우
		// 터치의 각도가 45에서 135도 사이일 때
		// 퍼즐이 보드의 맨 위에 위치하지 않았을 때
		// 퍼즐은 위쪽으로 이동하게 되며, 위쪽의 퍼즐은 현재 퍼즐의 위치로 이동함
		// Brick 퍼즐일 경우에만 교환을 막음
		else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)
		{
			otherPuzzle = board.allPuzzles[posIndex.x, posIndex.y + 1];
			if (otherPuzzle.type == Puzzle.PuzzleType.Brick)
			{
				return;
			}
			otherPuzzle.posIndex.y--;
			posIndex.y++;
		}
		// 4. 아래쪽 방향으로 교환할 경우
		// 터치의 각도가 -45에서 -135도 사이일 때
		// 퍼즐이 보드의 맨 아래에 위치하지 않았을 때
		// 퍼즐은 아래쪽으로 이동하게 되며, 아래쪽의 퍼즐은 현재 퍼즐의 위치로 이동함
		// Brick 퍼즐일 경우에만 교환을 막음
		else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
		{
			otherPuzzle = board.allPuzzles[posIndex.x, posIndex.y - 1];
			if (otherPuzzle.type == Puzzle.PuzzleType.Brick)
			{
				return;
			}
			otherPuzzle.posIndex.y++;
			posIndex.y--;
		}

		// 교환할 퍼즐이 없는 경우 리턴
		if (otherPuzzle == null)
		{
			return;
		}
			
		board.allPuzzles[posIndex.x, posIndex.y] = this;
		board.allPuzzles[otherPuzzle.posIndex.x, otherPuzzle.posIndex.y] = otherPuzzle;

		StartCoroutine(CheckMoveRoutine());
		SoundManager.Sound.PlayPuzzleClick();
	}

	void ExchangePuzzles()
	{
		if (Vector2.Distance(transform.position, posIndex) > 0.01f)
		{
			// 현재 위치에서 목표 위치로 일정 속도(board.puzzleSpeed)로 이동함
			transform.position = Vector2.Lerp(transform.position, posIndex, board.puzzleSpeed * Time.deltaTime);
		}
		else
		{
			transform.position = new Vector2(posIndex.x, posIndex.y);
			board.allPuzzles[(int)posIndex.x, (int)posIndex.y] = this;
		}
	}

	// 유효 교환인지 체크하는 코루틴
	IEnumerator CheckMoveRoutine()
	{
		board.curStatus = Board.BoardStatus.Idle;

		yield return new WaitForSeconds(0.5f);

		matchManager.MatchPuzzleType();

		if (otherPuzzle != null)
		{
			// 현재 퍼즐과 이동된 위치에 있는 다른 퍼즐이 모두 일치하지 않은 경우
			if (!isMatched && !otherPuzzle.isMatched)
			{
				// 퍼즐의 위치를 서로 바꿈
				otherPuzzle.posIndex = posIndex;
				// 퍼즐의 위치를 원래대로 함
				posIndex = prePos;

				// 보드 배열에서 퍼즐의 위치 정보를 갱신함
				board.allPuzzles[posIndex.x, posIndex.y] = this;
				// 보드 배열에서 이동된 위치에 있는 다른 퍼즐의 위치 정보를 갱신함
				board.allPuzzles[otherPuzzle.posIndex.x, otherPuzzle.posIndex.y] = otherPuzzle;

				yield return new WaitForSeconds(0.5f);

				board.curStatus = Board.BoardStatus.Move;
			}
			else
			{
				board.DestroyMatch();
			}
		}
	}
}
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

	// �����Ͱ� ������Ʈ ������ ������ �� ȣ���
	public void OnPointerDown(PointerEventData eventData)
	{
		if (board.curStatus == Board.BoardStatus.Move && roundManager.roundTime > 0)
		{
			firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	// �����͸� �� �� ȣ���
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
		// Brick ������ ��쿡�� ��ȯ�� ����
		if (type == Puzzle.PuzzleType.Brick)
		{
			return;
		}

		prePos = posIndex;

		// 1. ������ �������� ��ȯ�� ���
		// ��ġ�� ������ -45���� 45�� ������ ��
		// ������ ������ ������ ���� ��ġ���� �ʾ��� ��
		// ������ ���������� �̵��ϰ� �Ǹ�, �������� ������ ���� ������ ��ġ�� �̵���
		// Brick ������ ��쿡�� ��ȯ�� ����
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
		// 2. ���� �������� ��ȯ�� ���
		// ��ġ�� ������ 135���� ũ�ų� -135���� ���� ��
		// ������ ������ ���� ���� ��ġ���� �ʾ��� ��
		// ������ �������� �̵��ϰ� �Ǹ�, ������ ������ ���� ������ ��ġ�� �̵���
		// Brick ������ ��쿡�� ��ȯ�� ����
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
		// 3. ���� �������� ��ȯ�� ���
		// ��ġ�� ������ 45���� 135�� ������ ��
		// ������ ������ �� ���� ��ġ���� �ʾ��� ��
		// ������ �������� �̵��ϰ� �Ǹ�, ������ ������ ���� ������ ��ġ�� �̵���
		// Brick ������ ��쿡�� ��ȯ�� ����
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
		// 4. �Ʒ��� �������� ��ȯ�� ���
		// ��ġ�� ������ -45���� -135�� ������ ��
		// ������ ������ �� �Ʒ��� ��ġ���� �ʾ��� ��
		// ������ �Ʒ������� �̵��ϰ� �Ǹ�, �Ʒ����� ������ ���� ������ ��ġ�� �̵���
		// Brick ������ ��쿡�� ��ȯ�� ����
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

		// ��ȯ�� ������ ���� ��� ����
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
			// ���� ��ġ���� ��ǥ ��ġ�� ���� �ӵ�(board.puzzleSpeed)�� �̵���
			transform.position = Vector2.Lerp(transform.position, posIndex, board.puzzleSpeed * Time.deltaTime);
		}
		else
		{
			transform.position = new Vector2(posIndex.x, posIndex.y);
			board.allPuzzles[(int)posIndex.x, (int)posIndex.y] = this;
		}
	}

	// ��ȿ ��ȯ���� üũ�ϴ� �ڷ�ƾ
	IEnumerator CheckMoveRoutine()
	{
		board.curStatus = Board.BoardStatus.Idle;

		yield return new WaitForSeconds(0.5f);

		matchManager.MatchPuzzleType();

		if (otherPuzzle != null)
		{
			// ���� ����� �̵��� ��ġ�� �ִ� �ٸ� ������ ��� ��ġ���� ���� ���
			if (!isMatched && !otherPuzzle.isMatched)
			{
				// ������ ��ġ�� ���� �ٲ�
				otherPuzzle.posIndex = posIndex;
				// ������ ��ġ�� ������� ��
				posIndex = prePos;

				// ���� �迭���� ������ ��ġ ������ ������
				board.allPuzzles[posIndex.x, posIndex.y] = this;
				// ���� �迭���� �̵��� ��ġ�� �ִ� �ٸ� ������ ��ġ ������ ������
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
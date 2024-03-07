using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Puzzle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public enum PuzzleType { Bear, Chick, Crocodile, Narwhal, Panda, Parrot, Penguin, Pig }
	public PuzzleType type;
	 
	[Header("Puzzle")]
	public Vector2Int posIndex;
	public Board board;
	public bool isMatched;

	[HideInInspector]
	public Vector2Int prePos;

	Vector2 firstTouchPos;
	Vector2 finalTouchPos;
	
	Puzzle otherPuzzle;
	float swipeAngle = 0;

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
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Debug.Log("Down");
	}

	// �����͸� �� �� ȣ���
	public void OnPointerUp(PointerEventData eventData)
    {
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateSwipeAngle();
    }

    void CalculateSwipeAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;
        Debug.Log(swipeAngle);

        if (Vector3.Distance(firstTouchPos, finalTouchPos) > 0.5f)
        {
			MovePuzzles();
		}
	}

    void MovePuzzles()
    {
		prePos = posIndex;

		// ������ �������� ��ȯ�� ��
		if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)
		{
			otherPuzzle = board.allPuzzles[posIndex.x + 1, posIndex.y];
			otherPuzzle.posIndex.x--;
			posIndex.x++;
		}
		// ���� �������� ��ȯ�� ��
		else if (swipeAngle > 135 && swipeAngle < -135 && posIndex.x > 0)
		{
			otherPuzzle = board.allPuzzles[posIndex.x - 1, posIndex.y];
			otherPuzzle.posIndex.x++;
			posIndex.x--;
		}
		// ���� �������� ��ȯ�� ��
		else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)
		{
			otherPuzzle = board.allPuzzles[posIndex.x, posIndex.y + 1];
			otherPuzzle.posIndex.y--;
			posIndex.y++;
		}
		// �Ʒ��� �������� ��ȯ�� ��
		else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
		{
			otherPuzzle = board.allPuzzles[posIndex.x, posIndex.y - 1];
			otherPuzzle.posIndex.y++;
			posIndex.y--;
		}	

		// ��ȯ�� ������ ���� ���
		if (otherPuzzle == null)
		{
			return;
		}

		board.allPuzzles[posIndex.x, posIndex.y] = this;
		board.allPuzzles[otherPuzzle.posIndex.x, otherPuzzle.posIndex.y] = otherPuzzle;

		StartCoroutine(CheckMoveRoutine());
	}

	void ExchangePuzzles()
	{		
		if (Vector2.Distance(transform.position, posIndex) > .01f)
		{
			transform.position = Vector2.Lerp(transform.position, posIndex, board.puzzleSpeed * Time.deltaTime);
		}
		else
		{
			transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
			board.allPuzzles[posIndex.x, posIndex.y] = this;
		}
	}

	
	IEnumerator CheckMoveRoutine()
	{
		yield return new WaitForSeconds(0.5f);
	}
}


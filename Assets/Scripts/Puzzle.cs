using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Puzzle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Header("Puzzle")]
	public Vector2Int posIndex;
	public Board board;

	Vector2 firstTouchPos;
	Vector2 finalTouchPos;

	float swipeAngle = 0;
	Puzzle otherPuzzle;

	void Update()
	{ 
		// ���� ������Ʈ ��ȯ
		transform.position = Vector2.Lerp(transform.position, posIndex, board.puzzleSpeed * Time.deltaTime);
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
		// ������ �������� ��ȯ�� ��
		if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)
		{
			otherPuzzle = board.allPuzzles[posIndex.x + 1, posIndex.y];
			otherPuzzle.posIndex.x--;
			posIndex.x++;
			//updateArray();
		}
		// ���� �������� ��ȯ�� ��
		else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
		{
			otherPuzzle = board.allPuzzles[posIndex.x - 1, posIndex.y];
			otherPuzzle.posIndex.x++;
			posIndex.x--;
			//updateArray();
		}
		// ���� �������� ��ȯ�� ��
		else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)
		{
			otherPuzzle = board.allPuzzles[posIndex.x, posIndex.y + 1];
			otherPuzzle.posIndex.y--;
			posIndex.y++;
			//updateArray();
		}
		// �Ʒ��� �������� ��ȯ�� ��
		else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
		{
			otherPuzzle = board.allPuzzles[posIndex.x, posIndex.y - 1];
			otherPuzzle.posIndex.y++;
			posIndex.y--;
			//updateArray();
		}	

		if (otherPuzzle == null)
		{
			return;
		}

		board.allPuzzles[posIndex.x, posIndex.y] = this;
		board.allPuzzles[otherPuzzle.posIndex.x, otherPuzzle.posIndex.y] = otherPuzzle;
	}

	//void updateArray()
	//{
	//	board.allPuzzles[posIndex.x, posIndex.y] = this;
	//	board.allPuzzles[otherPuzzle.posIndex.x, otherPuzzle.posIndex.y] = otherPuzzle;
	//}
}


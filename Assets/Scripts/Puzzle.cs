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
		// 퍼즐 오브젝트 교환
		transform.position = Vector2.Lerp(transform.position, posIndex, board.puzzleSpeed * Time.deltaTime);
	}

    public void PuzzleSetUp(Vector2Int pos, Board _board)
    {
        posIndex = pos;
        board = _board;
    }

    // 포인터가 오브젝트 위에서 눌렸을 때 호출됨
    public void OnPointerDown(PointerEventData eventData)
    {
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Debug.Log("Down");
	}

	// 포인터를 뗄 때 호출됨
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
		// 오른쪽 방향으로 교환할 때
		if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)
		{
			otherPuzzle = board.allPuzzles[posIndex.x + 1, posIndex.y];
			otherPuzzle.posIndex.x--;
			posIndex.x++;
			//updateArray();
		}
		// 왼쪽 방향으로 교환할 때
		else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
		{
			otherPuzzle = board.allPuzzles[posIndex.x - 1, posIndex.y];
			otherPuzzle.posIndex.x++;
			posIndex.x--;
			//updateArray();
		}
		// 위쪽 방향으로 교환할 때
		else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)
		{
			otherPuzzle = board.allPuzzles[posIndex.x, posIndex.y + 1];
			otherPuzzle.posIndex.y--;
			posIndex.y++;
			//updateArray();
		}
		// 아래쪽 방향으로 교환할 때
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


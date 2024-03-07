using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Puzzle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2Int posIndex;
    public Board board;

    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;

    float swipeAngle = 0;

    public void PuzzleSetUp(Vector2Int pos, Board _board)
    {
        posIndex = pos;
        board = _board;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		CalculateSwipeAngle();
	}

    public void OnPointerUp(PointerEventData eventData)
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateSwipeAngle();
    }

    void CalculateSwipeAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;
        Debug.Log(swipeAngle);
    }

    void MovePuzzles()
    {

    }
}


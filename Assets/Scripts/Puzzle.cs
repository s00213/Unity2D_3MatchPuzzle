using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public Vector2Int posIndex;
    public Board board;

	Vector2 firstTouchPosition;
	Vector2 finalTouchPosition;
	bool mousePressed;
	float swipeAngle = 0;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;

			finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			CalculateSwipeAngle();
		}
    }

	public void PuzzleSetUp(Vector2Int pos, Board _board)
    { 
        posIndex = pos;
        board = _board;
    }

	void OnMouseDown()
	{
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePressed = true;
	}

    void CalculateSwipeAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
		swipeAngle = swipeAngle * 180 / Mathf.PI;
		Debug.Log(swipeAngle);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public Vector2Int posIndex;
    public Board board;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PuzzleSetUp(Vector2Int pos, Board _board)
    { 
        posIndex = pos;
        board = _board;
    }
}

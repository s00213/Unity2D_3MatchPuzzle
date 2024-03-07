using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
	[Header("Board")]
	public int width;
	public int height;
	public float puzzleSpeed;
	public GameObject BackgroundTilePrefab;
	[Space]
	public Puzzle[] puzzles;
	public Puzzle[,] allPuzzles;

	MatchManager matchManager;

	void Awake()
	{
		matchManager = FindObjectOfType<MatchManager>();
	}

	void Start()
	{
		allPuzzles = new Puzzle[width, height];
		
		BoardSetUp();
	}

	void Update()
	{
		matchManager.MatchPuzzleType();
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
				backgroundTile.name = "BackgroundTile - " + x + ", " + y;

				int puzzleToUse = Random.Range(0, puzzles.Length);

				SpawnPuzzle(new Vector2Int(x, y), puzzles[puzzleToUse]);
			}
		}
	}

	void SpawnPuzzle(Vector2Int pos, Puzzle puzzleToSpawn)
	{
		Puzzle puzzle = Instantiate(puzzleToSpawn, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
		puzzle.transform.parent = transform;
		puzzle.name = "Puzzle - " + pos.x + ", " + pos.y;
		allPuzzles[pos.x, pos.y] = puzzle;

		puzzle.PuzzleSetUp(pos, this);
	}
}

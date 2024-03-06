using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ���� ���� �� ���� ����
public class Board : MonoBehaviour
{
	public int width;
	public int height;

	public GameObject BackgroundTilePrefab;

	void Start()
	{
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
				backgroundTile.name = "BackgroundTile - " + x + ", " + y;
			}
		}
	}	
}

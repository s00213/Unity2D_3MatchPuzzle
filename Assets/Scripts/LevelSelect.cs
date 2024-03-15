using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class LevelSelect : MonoBehaviour
{
	public Button[] levelButtons;

	private void Awake()
	{
		int unlockLevel = PlayerPrefs.GetInt("unlockLevel", 1);
		for (int i = 1; i < levelButtons.Length; i++)
		{
			if (i + 1 > unlockLevel)
			{
				levelButtons[i].interactable = false;
			}
		}
	}

	public void Level1()
	{
		UnitySceneManager.LoadScene(UnitySceneManager.GetActiveScene().buildIndex + 1);
	}

	public void Level2()
	{
		UnitySceneManager.LoadScene(UnitySceneManager.GetActiveScene().buildIndex + 2);
	}

	public void Level3()
	{
		UnitySceneManager.LoadScene(UnitySceneManager.GetActiveScene().buildIndex + 3);
	}
}


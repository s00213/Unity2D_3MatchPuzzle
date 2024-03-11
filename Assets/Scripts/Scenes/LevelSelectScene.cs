using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class LevelSelectScene : MonoBehaviour
{
	public Button[] levelButtons;

	public string titleScene = "TitleScene";

	public void ReturnTitleScene()
	{
		//GameManager.Scene.LoadScene("LevelSelectScene");
		//GameManager.Scene.LoadScene("GameScene");

		//TODO : �ε� �߰� �� SceneManager�� ����
		UnitySceneManager.LoadScene(titleScene);
	}

	private void Awake()
	{
		int unlockLevel = PlayerPrefs.GetInt("unlockLevel", 1);
		for (int i = 0; i < levelButtons.Length; i++)
		{
			levelButtons[i].interactable = false;
		}

		for (int i = 0; i < unlockLevel; i++)
		{
			levelButtons[i].interactable = true;
		}
	}
}


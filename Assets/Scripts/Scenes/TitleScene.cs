using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class TitleScene : MonoBehaviour
{
	public string GameScene;

	public void StartGame()
	{
		//GameManager.Scene.LoadScene("LevelSelectScene");
		//GameManager.Scene.LoadScene("GameScene");

		//TODO : �ε� �߰� �� SceneManager�� ����
		UnitySceneManager.LoadScene(GameScene);
	}

	public void LoadGame()
	{
		Debug.Log("Load Game");
	}

	public void QuitGame()
	{
		Application.Quit();
		Debug.Log("Quit Game");
	}
}

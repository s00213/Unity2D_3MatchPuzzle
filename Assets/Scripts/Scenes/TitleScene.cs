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

		//TODO : 로딩 추가 후 SceneManager로 변경
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class TitleScene : MonoBehaviour
{
	public string GameScene;

	public void StartGame()
	{
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

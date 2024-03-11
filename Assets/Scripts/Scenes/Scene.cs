using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class Scene : MonoBehaviour
{
	public string levelSelectScene = "LevelSelectScene";
	public string replayScene;

	public void ReturnLevelSelectScene()
	{
		//GameManager.Scene.LoadScene("LevelSelectScene");
		//GameManager.Scene.LoadScene("GameScene");

		UnitySceneManager.LoadScene(levelSelectScene);
	}

	public void Replay()
	{
		//GameManager.Scene.LoadScene("LevelSelectScene");
		//GameManager.Scene.LoadScene("GameScene");

		UnitySceneManager.LoadScene(replayScene);
	}
}

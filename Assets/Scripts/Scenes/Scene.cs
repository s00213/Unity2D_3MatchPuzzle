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

		//TODO : 로딩 추가 후 SceneManager로 변경
		UnitySceneManager.LoadScene(levelSelectScene);
	}

	public void Replay()
	{
		//GameManager.Scene.LoadScene("LevelSelectScene");
		//GameManager.Scene.LoadScene("GameScene");

		//TODO : 로딩 추가 후 SceneManager로 변경
		UnitySceneManager.LoadScene(replayScene);
	}
}

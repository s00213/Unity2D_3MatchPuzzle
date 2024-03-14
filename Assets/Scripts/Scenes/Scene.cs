using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class Scene : MonoBehaviour
{
	public string replayScene;

	private int Level = 1;
	
	public void Back(int sceneID)
	{
		UnitySceneManager.LoadScene(sceneID);
	}

	public void Replay()
	{
		//GameManager.Scene.LoadScene("LevelSelectScene");
		//GameManager.Scene.LoadScene("GameScene");

		UnitySceneManager.LoadScene(replayScene);
	}
}

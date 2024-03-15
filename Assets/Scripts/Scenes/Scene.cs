using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class Scene : MonoBehaviour
{
	public string replayScene;
	public string sceneName;

	public void Back(int sceneID)
	{
		Time.timeScale = 1f;

		UnitySceneManager.LoadScene(sceneName);
	}

	public void Replay()
	{
		Time.timeScale = 1f;

		UnitySceneManager.LoadScene(replayScene);
	}

	public void Next()
	{
		UnitySceneManager.LoadScene(UnitySceneManager.GetActiveScene().buildIndex + 1);
	}
}

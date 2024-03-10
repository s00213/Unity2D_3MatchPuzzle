using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class LevelSelectButton : MonoBehaviour
{
	public string levelToLoad;

	public void LoadLevel()
	{
		UnitySceneManager.LoadScene(levelToLoad);
	}
}

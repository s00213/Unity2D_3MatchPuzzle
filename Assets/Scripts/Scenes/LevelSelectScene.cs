using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class LevelSelectScene : MonoBehaviour
{
	public string titleScene = "TitleScene";

	public void ReturnTitleScene()
	{
		//GameManager.Scene.LoadScene("LevelSelectScene");
		//GameManager.Scene.LoadScene("GameScene");

		//TODO : �ε� �߰� �� SceneManager�� ����
		UnitySceneManager.LoadScene(titleScene);
	}
}


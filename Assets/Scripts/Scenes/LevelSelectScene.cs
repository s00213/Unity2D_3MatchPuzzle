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

		//TODO : 로딩 추가 후 SceneManager로 변경
		UnitySceneManager.LoadScene(titleScene);
	}
}


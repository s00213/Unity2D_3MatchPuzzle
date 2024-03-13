using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class TitleScene : MonoBehaviour
{
	public static TitleScene Title;
	public string GameScene;
	public GameObject loginUI;
	public GameObject registerUI;
	public GameObject titleUI;

	private void Awake()
	{
		if (Title == null)
		{
			Title = this;
		}
		else if (Title != null)
		{
			Debug.Log("Instance already exists, destroying object!");
			Destroy(this);
		}
	}

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

	public void ClearUI()
	{
		loginUI.SetActive(false);
		registerUI.SetActive(false);
	}

	public void LoginUI()
	{
		ClearUI();
		loginUI.SetActive(true);
		registerUI.SetActive(false);
	}
	public void RegisterUI()
	{
		ClearUI();
		loginUI.SetActive(false);
		registerUI.SetActive(true);
	}

	public void LoginSucces()
	{
		ClearUI();
		titleUI.SetActive(true);
	}
}

using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Resources;

public class TitleScene : MonoBehaviour
{
	public static TitleScene instance;

	public static TitleScene Instance { get { return instance; } }

	public GameObject loginUI;
	public GameObject registerUI;
	public GameObject titleUI;
	public GameObject levelSelectUI;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(this);
			return;
		}

		instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	private void Start()
	{
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

	public void LevelSelectUI()
	{
		ClearUI();
		titleUI.SetActive(true);
		levelSelectUI.SetActive(true);
	}
}

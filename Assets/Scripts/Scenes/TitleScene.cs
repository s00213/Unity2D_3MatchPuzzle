using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
	public static TitleScene instance;

	public GameObject loginUI;
	public GameObject registerUI;
	public GameObject titleUI;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(this);
			return;
		}

		instance = this;

		AudioSource audioSource = GetComponent<AudioSource>();
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

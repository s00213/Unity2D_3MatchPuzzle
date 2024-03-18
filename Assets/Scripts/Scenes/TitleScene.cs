using UnityEngine;

public class TitleScene : MonoBehaviour
{
	public static TitleScene instance;

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
		levelSelectUI.SetActive(true);
	}
}
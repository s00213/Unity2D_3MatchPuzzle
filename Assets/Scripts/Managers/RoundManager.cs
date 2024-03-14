using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using System;

public class RoundManager : MonoBehaviour
{
	[Header("Text")]
	public TextMeshProUGUI timeText;
	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI resultScoreText;
	public TextMeshProUGUI levelText;
	public GameObject star0, star1, stars2, stars3;

	[Header("Timer")]
	public float roundTime;
	public Slider timerSlider;

	[Header("Score")]
	public int curScore;
	public float displayScore;
	public float scoreSpeed;
	public int scoreTarget1, scoreTarget2, scoreTarget3;

	[Header("Pause")]
	public string TitleSceneToLoad;
	public GameObject pauseUI;

    [Header("Game Over")]
	public GameObject ResultUI;

	Board board;

	bool roundEnd = false;

	private int level = 1;
	private int point;
	private int nextSceneToLoad;

	void Awake()
	{
		board = FindObjectOfType<Board>();
	}

	private void Start()
	{
		star0.SetActive(false);
		star1.SetActive(false);
		stars2.SetActive(false);
		stars3.SetActive(false);

		timerSlider.maxValue = roundTime;
		timerSlider.value = roundTime;	
	}

	void Update()
	{
		Timer();

		DisplayScore();
	}

	public void ShuffleButton()
	{
		board.ShufflePuzzles();
	}

	public void PauseButton()
	{
		if (!pauseUI.activeInHierarchy)
		{
			pauseUI.SetActive(true);
			Time.timeScale = 0f;
		}
		else
		{
			pauseUI.SetActive(false);
			Time.timeScale = 1f;
		}
	}

	public void LevelSelectButton()
	{
		Time.timeScale = 1f;
		UnitySceneManager.LoadScene(TitleSceneToLoad);
		TitleScene.Instance.LoginSucces();
	}

	public void QuitButton()
	{
		Application.Quit();
		Debug.Log("Quit Game");
	}

	void Timer()
	{
		if (roundTime > 0 && !roundEnd)
		{
			roundTime -= Time.deltaTime;

			timerSlider.value = roundTime;

			if (roundTime <= 0)
			{
				roundTime = 0;
				roundEnd = true;
			}
		}

		if (roundEnd && board.curStatus == Board.BoardStatus.Move)
		{
			CheckResult();

			roundEnd = false;
		}

		timeText.text = "Time : " + roundTime.ToString("0.0") + "s";
	}

	void DisplayScore()
	{
		displayScore = Mathf.Lerp(displayScore, curScore, scoreSpeed * Time.deltaTime);

		scoreText.text = "Score : " + displayScore.ToString("0");
	}

	void CheckResult()
	{
		ResultUI.SetActive(true);

		resultScoreText.text = curScore.ToString();

		UpdateLevelData();
		UpdateScoreData();

		if (curScore >= scoreTarget3)
		{
			stars3.SetActive(true);
			if (UnitySceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ScoreTarget3"))
			{
				PlayerPrefs.SetInt("ScoreTarget3", UnitySceneManager.GetActiveScene().buildIndex + 1);
				PlayerPrefs.SetInt("unlockLevel", PlayerPrefs.GetInt("unlockLevel", 1) + 1);
				PlayerPrefs.Save();
			}
		}
		else if (curScore >= scoreTarget2)
		{
			stars2.SetActive(true);
			if (UnitySceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ScoreTarget2"))
			{
				PlayerPrefs.SetInt("ScoreTarget2", UnitySceneManager.GetActiveScene().buildIndex + 1);
				PlayerPrefs.SetInt("unlockLevel", PlayerPrefs.GetInt("unlockLevel", 1) + 1);
				PlayerPrefs.Save();
			}
		}
		else if (curScore >= scoreTarget1)
		{
			star1.SetActive(true);
			if (UnitySceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ScoreTarget1"))
			{
				PlayerPrefs.SetInt("ScoreTarget1", UnitySceneManager.GetActiveScene().buildIndex + 1);
				PlayerPrefs.SetInt("unlockLevel", PlayerPrefs.GetInt("unlockLevel", 1) + 1);
				PlayerPrefs.Save();
			}
		}
		else
		{
			star0.SetActive(true);
		}

		SoundManager.Sound.PlayResult();
	}

	void UpdateLevelData()
	{
		var user = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;
		if (user != null)
		{
			var DBReference = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;

			int level;
			string levelNumberStr = levelText.text.Substring(5);

			if (int.TryParse(levelNumberStr, out level))
			{
				DBReference.Child("User").Child(user.UserId).Child("Level").SetValueAsync(level);
			}
			else
			{
				Debug.LogError("Level text is not a valid integer.");
			}
		}
	}

	void UpdateScoreData()
	{
		var user = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;
		if (user != null)
		{
			var DBReference = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;

			DBReference.Child("User").Child(user.UserId).Child("Point").SetValueAsync(curScore);
		}
	}
}
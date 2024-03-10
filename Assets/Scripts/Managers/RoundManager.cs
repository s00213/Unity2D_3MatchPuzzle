using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
	[Header("Text")]
	public TextMeshProUGUI timeText;
	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI resultScoreText;
	public GameObject star0, star1, stars2, stars3;
	[Header("Timer")]
	public float roundTime = 60f;
	public Slider timerSlider;
	[Header("Score")]
	public int curScore;
	public float displayScore;
	public float scoreSpeed; //스코어가 업데이트되는 속도
	public int scoreTarget1, scoreTarget2, scoreTarget3;
	[Header("Game Over")]
	public GameObject ResultUI;

	Board board;

	bool roundEnd = false;

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

	void Timer()
	{
		if (roundTime > 0 && !roundEnd)
		{
			roundTime -= Time.deltaTime;

			// roundTime을 0에서 1로 정규화하여 슬라이더 값에 할당함
			timerSlider.value = Mathf.Clamp01(roundTime / 60.0f);

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

		if (curScore >= scoreTarget3)
		{
			stars3.SetActive(true);
		}
		else if (curScore >= scoreTarget2)
		{
			stars2.SetActive(true);
		}
		else if (curScore >= scoreTarget1)
		{
			star1.SetActive(true);
		}
		else
		{
			star0.SetActive(true);
		}
	}
}
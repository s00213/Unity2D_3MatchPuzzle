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
	[Header("Timer")]
	public float roundTime = 60f;
	public Slider timerSlider;
	[Header("Score")]
	public int curScore;
	public float displayScore;
	public float scoreSpeed; //스코어가 업데이트되는 속도
	[Header("Game Over")]
	public GameObject ResultUI;

	Board board;

	bool roundEnd = false;

	void Awake()
	{
		board = FindObjectOfType<Board>();
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

	void CheckResult()
	{
		ResultUI.SetActive(true);
	}

	void DisplayScore()
	{
		displayScore = Mathf.Lerp(displayScore, curScore, scoreSpeed * Time.deltaTime);

		scoreText.text = "Score : " + displayScore.ToString("0");
	}
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
	[Header("UI")]
	public TextMeshProUGUI timeText;
	public TextMeshProUGUI scoreText;
	[Header("Timer")]
	public float roundTime = 60f;
	public Slider timerSlider;

	Board board;

	bool endingRound = false;

	void Awake()
	{
		board = FindObjectOfType<Board>();
	}

	void Update()
	{
		Timer();
	}

	public void ShuffleButton()
	{
		board.ShufflePuzzles();
	}

	private void Timer()
	{
		if (roundTime > 0 && !endingRound)
		{
			roundTime -= Time.deltaTime;

			// roundTime을 0에서 1로 정규화하여 슬라이더 값에 할당
			timerSlider.value = Mathf.Clamp01(roundTime / 60.0f);

			if (roundTime <= 0)
			{
				roundTime = 0;
				endingRound = true;
			}
		}

		if (endingRound && board.curStatus == Board.BoardStatus.Move)
		{
			endingRound = false;
		}

		timeText.text = "Time : " + roundTime.ToString("0.0") + "s";
	}
}

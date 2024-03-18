using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Sound;

	public AudioSource MatchSound, puzzleClickSound, BrickSound, resultSound, comboSound, shuffleSound;

	void Awake()
	{
		Sound = this;
	}

	public void PlayMatch()
	{
		MatchSound.Stop();
		MatchSound.Play();
	}

	public void PlayPuzzleClick()
	{
		puzzleClickSound.Stop();
		puzzleClickSound.Play();
	}

	public void PlayBrick()
	{
		BrickSound.Stop();
		BrickSound.Play();
	}

	public void PlayResult()
	{
		resultSound.Stop();
		resultSound.Play();
	}

	public void PlayCombo()
	{
		comboSound.Stop();
		comboSound.Play();
	}

	public void PlayShuffle()
	{
		shuffleSound.Stop();
		shuffleSound.Play();
	}
}

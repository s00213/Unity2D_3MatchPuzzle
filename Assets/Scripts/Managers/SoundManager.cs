using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;


public class SoundManager : MonoBehaviour
{
	public static SoundManager sound;

	public AudioSource MatchSound, puzzleClickSound, BrickSound, BombSound, resultSound, comboSound, shuffleSound;

	
	void Awake()
	{
		sound = this;
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

	public void PlayBomb()
	{
		BombSound.Stop();
		BombSound.Play();
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

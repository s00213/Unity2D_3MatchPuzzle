using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSceneUI : SceneUI
{
	protected override void Awake()
	{
		base.Awake();

		buttons["SettingButton"].onClick.AddListener(() => { OpenPausePopUpUI(); });
	}

	public void OpenPausePopUpUI()
	{
		GameManager.UI.ShowPopUpUI("UI/SettingPopUpUI");
	}
}
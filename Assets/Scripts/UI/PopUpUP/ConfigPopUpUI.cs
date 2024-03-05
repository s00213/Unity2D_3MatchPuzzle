using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigPopUpUI : PopUpUI
{
	protected override void Awake()
	{
		base.Awake();

		buttons["CloseButton"].onClick.AddListener(() => { GameManager.UI.ClosePopUpUI(); });
		buttons["SaveButton"].onClick.AddListener(() => { GameManager.UI.ClosePopUpUI(); });
		buttons["CancelButton"].onClick.AddListener(() => { GameManager.UI.ClosePopUpUI(); });
	}
}

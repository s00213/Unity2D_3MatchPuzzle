using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
	EventSystem eventSystem;

	Canvas popUpCanvas;
	Stack<PopUpUI> popUpStack; // 편리하게 UI 관리를 위힌 Stack 구조 사용

	Canvas windowCanvas;
	Canvas inGameCanvas;
	
	private void Awake()
	{
		eventSystem = GameManager.Resource.Instantiate<EventSystem>("UI/EventSystem");
		eventSystem.transform.parent = transform;

		popUpCanvas = GameManager.Resource.Instantiate<Canvas>("UI/Canvas");
		popUpCanvas.gameObject.name = "PopUpCanvas";
		popUpCanvas.sortingOrder = 100;
		popUpStack = new Stack<PopUpUI>();

		windowCanvas = GameManager.Resource.Instantiate<Canvas>("UI/Canvas");
		windowCanvas.gameObject.name = "WindowCanvas";
		windowCanvas.sortingOrder = 10;

		//gameSceneCanvas.sortingOrder = 1;

		inGameCanvas = GameManager.Resource.Instantiate<Canvas>("UI/Canvas");
		inGameCanvas.gameObject.name = "InGameCanvas";
		inGameCanvas.sortingOrder = 0;
	}

	public void UIRestart()
	{
		Destroy(eventSystem.gameObject);
		Awake();
	}

	// 만든 ShowPopUpUI은 일반화를 적용하면 PopUpUI를 상속받는 다른 팝업 UI를 반환받게끔 함
	public void ShowPopUpUI(PopUpUI popUpUI)
	{
		// 이전의 팝업 UI가 있다면 잠깐 안 보이게 함
		if (popUpStack.Count > 0)
		{
			PopUpUI prevUI = popUpStack.Peek();
			prevUI.gameObject.SetActive(false);
		}

		PopUpUI ui = GameManager.Pool.GetUI(popUpUI);
		ui.transform.SetParent(popUpCanvas.transform, false);

		// UI 관리를 위힌 Stack 구조 사용
		popUpStack.Push(ui);

		// 팝업이 있을 때 시간 멈추게 함
		Time.timeScale = 0;
	}

	public void ShowPopUpUI(string path)
	{
		PopUpUI ui = GameManager.Resource.Load<PopUpUI>(path);
		ShowPopUpUI(ui);
	}

	public void ClosePopUpUI()
	{
		PopUpUI ui = popUpStack.Pop();
		// 풀 매니저를 통해서 UI 반납함
		GameManager.Pool.ReleaseUI(ui.gameObject);

		// 가장 위에 있는 현재 UI를 활성화시켜서 보이게 함
		if (popUpStack.Count > 0)
		{
			PopUpUI curUI = popUpStack.Peek();
			curUI.gameObject.SetActive(true);
		}
		else
		{
			// 팝업이 없을 때 시간 멈추게 함
			Time.timeScale = 1f;
		}
	}

	public void ClearPopUpUI()
	{
		while (popUpStack.Count > 0)
		{
			ClosePopUpUI();
		}
	}

	public void ShowWindowUI(WindowUI windowUI)
	{
		WindowUI ui = GameManager.Pool.GetUI(windowUI);
		ui.transform.SetParent(windowCanvas.transform, false);
	}

	public void ShowWindowUI(string path)
	{
		WindowUI ui = GameManager.Resource.Load<WindowUI>(path);
		ShowWindowUI(ui);
	}

	public void SelectWindowUI(WindowUI windowUI)
	{
		// 선택한 UI가 계층 구조 상에서 가장 아래로 내려가짐
		windowUI.transform.SetAsLastSibling();
	}

	public void CloseWindowUI(WindowUI windowUI)
	{
		GameManager.Pool.ReleaseUI(windowUI.gameObject);
	}

	public void ClearWindowUI()
	{
		WindowUI[] windows = windowCanvas.GetComponentsInChildren<WindowUI>();

		foreach (WindowUI windowUI in windows)
		{
			GameManager.Pool.ReleaseUI(windowUI.gameObject);
		}
	}

	// 일반화가 꼭 필요함
	public T ShowInGameUI<T>(T gameUi) where T : InGameUI
	{
		T ui = GameManager.Pool.GetUI(gameUi);
		ui.transform.SetParent(inGameCanvas.transform, false);

		return ui;
	}

	public T ShowInGameUI<T>(string path) where T : InGameUI
	{
		T ui = GameManager.Resource.Load<T>(path);
		return ShowInGameUI(ui);
	}

	public void CloseInGameUI<T>(T inGameUI) where T : InGameUI
	{
		GameManager.Pool.ReleaseUI(inGameUI.gameObject);
	}

	public void ClearInGameUI()
	{
		InGameUI[] inGames = inGameCanvas.GetComponentsInChildren<InGameUI>();

		foreach (InGameUI inGameUI in inGames)
		{
			GameManager.Pool.ReleaseUI(inGameUI.gameObject);
		}
	}
}
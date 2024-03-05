using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
	EventSystem eventSystem;

	Canvas popUpCanvas;
	Stack<PopUpUI> popUpStack; // ���ϰ� UI ������ ���� Stack ���� ���

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

	// ���� ShowPopUpUI�� �Ϲ�ȭ�� �����ϸ� PopUpUI�� ��ӹ޴� �ٸ� �˾� UI�� ��ȯ�ްԲ� ��
	public void ShowPopUpUI(PopUpUI popUpUI)
	{
		// ������ �˾� UI�� �ִٸ� ��� �� ���̰� ��
		if (popUpStack.Count > 0)
		{
			PopUpUI prevUI = popUpStack.Peek();
			prevUI.gameObject.SetActive(false);
		}

		PopUpUI ui = GameManager.Pool.GetUI(popUpUI);
		ui.transform.SetParent(popUpCanvas.transform, false);

		// UI ������ ���� Stack ���� ���
		popUpStack.Push(ui);

		// �˾��� ���� �� �ð� ���߰� ��
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
		// Ǯ �Ŵ����� ���ؼ� UI �ݳ���
		GameManager.Pool.ReleaseUI(ui.gameObject);

		// ���� ���� �ִ� ���� UI�� Ȱ��ȭ���Ѽ� ���̰� ��
		if (popUpStack.Count > 0)
		{
			PopUpUI curUI = popUpStack.Peek();
			curUI.gameObject.SetActive(true);
		}
		else
		{
			// �˾��� ���� �� �ð� ���߰� ��
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
		// ������ UI�� ���� ���� �󿡼� ���� �Ʒ��� ��������
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

	// �Ϲ�ȭ�� �� �ʿ���
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
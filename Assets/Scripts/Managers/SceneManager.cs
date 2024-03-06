using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

// �� �� ���� �ִ� BaseScene ã�Ƽ� ������ ��
public class SceneManager : MonoBehaviour
{
	LoadingUI loadingUI;

	BaseScene curScene;
	public BaseScene CurScene
	{
		get
		{
			// FindObjectOfType�� ���� ���� �δ��� �Ǵϱ� Null�� Ȯ����
			if (curScene == null)
				curScene = GameObject.FindObjectOfType<BaseScene>();

			return curScene;
		}
	}

	void Awake()
	{
		//LoadingUI ui = GameManager.Resource.Load<LoadingUI>("UI/LoadingUI");
		//loadingUI = Instantiate(ui);
		//loadingUI.transform.SetParent(transform, false);
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(LoadingRoutine(sceneName));
	}

	IEnumerator LoadingRoutine(string sceneName)
	{
		loadingUI.FadeOut();
		yield return new WaitForSeconds(0.5f);
		//�ε� �߿��� ������ �ð��� ������
		Time.timeScale = 0f;

		// �񵿱�� �ε�
		AsyncOperation oper = UnitySceneManager.LoadSceneAsync(sceneName);
		while (!oper.isDone)
		{
			loadingUI.SetProgress(Mathf.Lerp(0.0f, 0.5f, oper.progress));
			yield return null;
		}

		if (CurScene != null)
		{
			CurScene.LoadAsync();
			while (CurScene.progress < 1f)
			{
				loadingUI.SetProgress(Mathf.Lerp(0.5f, 1.0f, CurScene.progress));
				yield return null;
			}
		}

		loadingUI.SetProgress(1.0f);
		//�ε� �߿��� ������ �ð��� ���� �� ����
		Time.timeScale = 1f;
		loadingUI.FadeIn();
		yield return new WaitForSeconds(1f);
	}
}

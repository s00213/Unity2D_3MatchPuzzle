using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEditor.EditorTools;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class GameManager : MonoBehaviour
{
	static GameManager instance;
	static ResourceManager resourceManager;
	static PoolManager poolManager;
	static UIManager uiManager;
	static SceneManager sceneManager;

	public static GameManager Instance { get { return instance; } }
	public static ResourceManager Resource { get { return resourceManager; } }
	public static PoolManager Pool { get { return poolManager; } }
	public static UIManager UI { get { return uiManager; } }
	public static SceneManager Scene { get { return sceneManager; } }

	void Awake()
	{
		if (instance != null)
		{
			Destroy(this);
			return;
		}

		instance = this;
		DontDestroyOnLoad(this);

		InitManagers();
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	void InitManagers()
	{
		GameObject resourceObject = new GameObject();
		resourceObject.name = "ResourceManager";
		resourceObject.transform.parent = transform;
		resourceManager = resourceObject.AddComponent<ResourceManager>();

		GameObject poolObject = new GameObject();
		poolObject.name = "PoolManager";
		poolObject.transform.parent = transform;
		poolManager = poolObject.AddComponent<PoolManager>();

		GameObject uiObject = new GameObject();
		uiObject.name = "UIManager";
		uiObject.transform.parent = transform;
		uiManager = uiObject.AddComponent<UIManager>();

		GameObject sceneObject = new GameObject();
		sceneObject.name = "SceneManager";
		sceneObject.transform.parent = transform;
		sceneManager = sceneObject.AddComponent<SceneManager>();
	}
}

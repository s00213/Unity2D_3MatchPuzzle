using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
	Dictionary<string, ObjectPool<GameObject>> poolDic;
	Dictionary<string, Transform> poolContainer;
	Transform poolRoot;
	Canvas canvasRoot;

	void Awake()
	{
		InitPool();
	}

	public void InitPool()
	{
		poolDic = new Dictionary<string, ObjectPool<GameObject>>();
		poolContainer = new Dictionary<string, Transform>();
		poolRoot = new GameObject("PoolRoot").transform;
		canvasRoot = GameManager.Resource.Instantiate<Canvas>("UI/Canvas");
	}

	public T Get<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
	{
		if (original is GameObject)
		{
			GameObject prefab = original as GameObject;
			string key = prefab.name;

			if (!poolDic.ContainsKey(key))
				CreatePool(key, prefab);

			GameObject obj = poolDic[key].Get();
			obj.transform.SetParent(parent, false);
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			return obj as T;
		}
		else if (original is Component)
		{
			Component component = original as Component;
			string key = component.gameObject.name;

			if (!poolDic.ContainsKey(key))
				CreatePool(key, component.gameObject);

			GameObject obj = poolDic[key].Get();
			obj.transform.parent = parent;
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			return obj.GetComponent<T>();
		}
		else
		{
			return null;
		}
	}

	public T Get<T>(T original, Vector3 position, Quaternion rotation) where T : Object
	{
		return Get<T>(original, position, rotation, null);
	}

	public T Get<T>(T original, Transform parent) where T : Object
	{
		return Get<T>(original, Vector3.zero, Quaternion.identity, parent);
	}

	public T Get<T>(T original) where T : Object
	{
		return Get<T>(original, Vector3.zero, Quaternion.identity, null);
	}

	public bool Release<T>(T instance) where T : Object
	{
		if (instance is GameObject)
		{
			GameObject go = instance as GameObject;
			string key = go.name;

			if (!poolDic.ContainsKey(key))
				return false;

			poolDic[key].Release(go);
			return true;
		}
		else if (instance is Component)
		{
			Component component = instance as Component;
			string key = component.gameObject.name;

			if (!poolDic.ContainsKey(key))
				return false;

			poolDic[key].Release(component.gameObject);
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool IsContain<T>(T original) where T : Object
	{
		if (original is GameObject)
		{
			GameObject prefab = original as GameObject;
			string key = prefab.name;

			if (poolDic.ContainsKey(key))
				return true;
			else
				return false;

		}
		else if (original is Component)
		{
			Component component = original as Component;
			string key = component.gameObject.name;

			if (poolDic.ContainsKey(key))
				return true;
			else
				return false;
		}
		else
		{
			return false;
		}
	}

	void CreatePool(string key, GameObject prefab)
	{
		GameObject root = new GameObject();
		root.gameObject.name = $"{key}Container";
		root.transform.parent = poolRoot;
		poolContainer.Add(key, root.transform);

		ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
			createFunc: () =>
			{
				GameObject obj = Instantiate(prefab);
				obj.gameObject.name = key;
				return obj;
			},
			actionOnGet: (GameObject obj) =>
			{
				obj.gameObject.SetActive(true);
				obj.transform.SetParent(null);
			},
			actionOnRelease: (GameObject obj) =>
			{
				obj.gameObject.SetActive(false);
				obj.transform.SetParent(poolContainer[key]);
			},
			actionOnDestroy: (GameObject obj) =>
			{
				Destroy(obj);
			}
			);
		poolDic.Add(key, pool);
	}

	public T GetUI<T>(T original, Vector3 position) where T : Object
	{
		if (original is GameObject)
		{
			GameObject prefab = original as GameObject;
			string key = prefab.name;

			if (!poolDic.ContainsKey(key))
				CreateUIPool(key, prefab);

			GameObject obj = poolDic[key].Get();
			obj.transform.position = position;
			return obj as T;
		}
		else if (original is Component)
		{
			Component component = original as Component;
			string key = component.gameObject.name;

			if (!poolDic.ContainsKey(key))
				CreateUIPool(key, component.gameObject);

			GameObject obj = poolDic[key].Get();
			obj.transform.position = position;
			return obj.GetComponent<T>();
		}
		else
		{
			return null;
		}
	}

	public T GetUI<T>(T original) where T : Object
	{
		if (original is GameObject)
		{
			GameObject prefab = original as GameObject;
			string key = prefab.name;

			if (!poolDic.ContainsKey(key))
				CreateUIPool(key, prefab);

			GameObject obj = poolDic[key].Get();
			return obj as T;
		}
		else if (original is Component)
		{
			Component component = original as Component;
			string key = component.gameObject.name;

			if (!poolDic.ContainsKey(key))
				CreateUIPool(key, component.gameObject);

			GameObject obj = poolDic[key].Get();
			return obj.GetComponent<T>();
		}
		else
		{
			return null;
		}
	}

	public bool ReleaseUI<T>(T instance) where T : Object
	{
		if (instance is GameObject)
		{
			GameObject go = instance as GameObject;
			string key = go.name;

			if (!poolDic.ContainsKey(key))
				return false;

			poolDic[key].Release(go);
			return true;
		}
		else if (instance is Component)
		{
			Component component = instance as Component;
			string key = component.gameObject.name;

			if (!poolDic.ContainsKey(key))
				return false;

			poolDic[key].Release(component.gameObject);
			return true;
		}
		else
		{
			return false;
		}
	}

	void CreateUIPool(string key, GameObject prefab)
	{
		ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
			createFunc: () =>
			{
				GameObject obj = Instantiate(prefab);
				obj.gameObject.name = key;
				return obj;
			},
			actionOnGet: (GameObject obj) =>
			{
				obj.gameObject.SetActive(true);
			},
			actionOnRelease: (GameObject obj) =>
			{
				obj.gameObject.SetActive(false);
				obj.transform.SetParent(canvasRoot.transform, false);
			},
			actionOnDestroy: (GameObject obj) =>
			{
				Destroy(obj);
			}
			);
		poolDic.Add(key, pool);
	}
}

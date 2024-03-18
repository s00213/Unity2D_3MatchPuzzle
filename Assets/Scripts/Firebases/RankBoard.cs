using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class RankBoard : MonoBehaviour
{
	[Header("Firebase")]
	public DependencyStatus dependencyStatus;
	public FirebaseAuth auth; // �α���, ȸ������ � ���
	public FirebaseUser user; // ������ �Ϸ�� ����	
	public DatabaseReference DBreference;

	[Header("UserData")]
	public TextMeshProUGUI rankBoardText;

	void Start()
	{
		StartCoroutine(CheckAndFixDependenciesRoutine());
	}

	// FirebaseAuth �ν��Ͻ� ��ü ����
	private void InitializeFirebase()
	{
		DBreference = FirebaseDatabase.DefaultInstance.RootReference;

		GetTopUsersByLevel();
	}

	// Firebase ���Ӽ��� üũ�ϰ� �����ϴ� �ڷ�ƾ
	IEnumerator CheckAndFixDependenciesRoutine()
	{
		var checkAndFixDependenciesTask = FirebaseApp.CheckAndFixDependenciesAsync();

		yield return new WaitUntil(predicate: () => checkAndFixDependenciesTask.IsCompleted);

		var dependencyResult = checkAndFixDependenciesTask.Result;

		if (dependencyResult == DependencyStatus.Available)
		{
			InitializeFirebase();
		}
		else
		{
			Debug.LogError("��� Firebase ���Ӽ��� �ذ��� �� ���� : " + dependencyStatus);
		}
	}

	// Firebase���� �������� ���ĵ� ���� 5���� ����� �����͸� ������
	public void GetTopUsersByLevel()
	{
		// �������� ����ڸ� �����ϰ� ���� 5���� ������
		DBreference.Child("User").OrderByChild("Level").LimitToLast(5).GetValueAsync().ContinueWith(task =>
		{
			if (task.IsFaulted)
			{
				Debug.Log("���� ���� ������ �ε� ����");
			}
			else if (task.IsCompleted)
			{
				DataSnapshot snapshot = task.Result;
				ProcessData(snapshot);
			}
		});
	}

	private void ProcessData(DataSnapshot snapshot)
	{
		// ��ŷ ������ ���� ����Ʈ�� ������
		var rankList = new List<(string, int, int)>();

		Debug.Log("Snapshot Children Count: " + snapshot.ChildrenCount.ToString());

		// �� �ڽ� ��忡 ���� �ݺ���
		foreach (var child in snapshot.Children)
		{
			// �� ������� �̸�, ����, ����Ʈ�� ������ ������ ������
			string username = child.Child("Username").Value.ToString();
			int level = int.Parse(child.Child("Level").Value.ToString());
			int point = int.Parse(child.Child("Point").Value.ToString());

			Debug.Log("Username: " + username);
			Debug.Log("Level: " + level);
			Debug.Log("Point: " + point);

			// ����� ������ ��ŷ ����Ʈ�� �߰���
			rankList.Add((username, level, point));
		}

		// ������ ���� ������� ��������, ������ ���� ��쿡�� ������ ���� ������ �����͸� ������
		rankList = rankList.OrderByDescending(x => x.Item2).ThenByDescending(x => x.Item3).ToList();
		Debug.Log("Rank List Count: " + rankList.Count);

		// ��ŷ ���忡 ǥ���� ���ڿ��� ������
		string rankBoardString = "";
		
		for (int i = 0; i < rankList.Count; i++)
		{
			var user = rankList[i];
			int rank = i + 1;
			rankBoardString += $"{rank}. {user.Item1} Level{user.Item2} Point {user.Item3}\n\n";
		}

		rankBoardText.text = rankBoardString;
	}
}

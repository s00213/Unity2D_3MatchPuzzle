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
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			dependencyStatus = task.Result;
			if (dependencyStatus == DependencyStatus.Available)
			{
				// Firebase �ʱ�ȭ �� ������ Ȯ���� �Ϸ�� �Ŀ� DBreference �ʱ�ȭ
				DBreference = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;

				// DBreference�� �ùٸ��� �ʱ�ȭ�� �Ŀ� GetTopUsersByLevel() ȣ��
				GetTopUsersByLevel();
			}
			else
			{
				Debug.LogError("Firebase �ʱ�ȭ �� ������ Ȯ�� ����");
			}
		});		
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
		// ������ ���� ������ ������
		var rankList = new List<(string, int, int)>();

		Debug.Log("Snapshot Children Count: " + snapshot.ChildrenCount.ToString());

		// ������ ���� ������ �����͸� ó����
		foreach (var child in snapshot.Children.Reverse())
		{
			string username = child.Child("Username").Value.ToString();
			int level = int.Parse(child.Child("Level").Value.ToString());
			int point = int.Parse(child.Child("Point").Value.ToString());

			Debug.Log("Username: " + username);
			Debug.Log("Level: " + level);
			Debug.Log("Point: " + point);

			rankList.Add((username, level, point));
		}

		// ������ ���� ����ڸ� ������ ���� ������ ������
		rankList = rankList.OrderBy(x => x.Item2).ThenByDescending(x => x.Item3).ToList();
		Debug.Log("Rank List Count: " + rankList.Count);

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

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
	public FirebaseAuth auth; // 로그인, 회원가입 등에 사용
	public FirebaseUser user; // 인증이 완료된 유저	
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
				// Firebase 초기화 및 의존성 확인이 완료된 후에 DBreference 초기화
				DBreference = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;

				// DBreference가 올바르게 초기화된 후에 GetTopUsersByLevel() 호출
				GetTopUsersByLevel();
			}
			else
			{
				Debug.LogError("Firebase 초기화 및 의존성 확인 실패");
			}
		});		
	}

	// Firebase에서 레벨별로 정렬된 상위 5명의 사용자 데이터를 가져옴
	public void GetTopUsersByLevel()
	{
		// 레벨별로 사용자를 정렬하고 상위 5명을 제한함
		DBreference.Child("User").OrderByChild("Level").LimitToLast(5).GetValueAsync().ContinueWith(task =>
		{
			if (task.IsFaulted)
			{
				Debug.Log("순위 보드 데이터 로드 실패");
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
		// 레벨이 높은 순서로 정렬함
		var rankList = new List<(string, int, int)>();

		Debug.Log("Snapshot Children Count: " + snapshot.ChildrenCount.ToString());

		// 레벨이 높은 순서로 데이터를 처리함
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

		// 레벨이 같은 사용자를 점수가 높은 순서로 정렬함
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

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
		//FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		//{
		//	dependencyStatus = task.Result;
		//	if (dependencyStatus == DependencyStatus.Available)
		//	{
		//		// Firebase 초기화 및 의존성 확인이 완료된 후에 DBreference 초기화
		//		DBreference = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;

		//		// DBreference가 올바르게 초기화된 후에 GetTopUsersByLevel() 호출
		//		GetTopUsersByLevel();
		//	}
		//	else
		//	{
		//		Debug.LogError("Firebase 초기화 및 의존성 확인 실패");
		//	}
		//});

		StartCoroutine(CheckAndFixDependenciesRoutine());
	}

	// FirebaseAuth 인스턴스 객체 설정
	private void InitializeFirebase()
	{
		DBreference = FirebaseDatabase.DefaultInstance.RootReference;

		GetTopUsersByLevel();
	}

	// Firebase 종속성을 체크하고 수정하는 코루틴
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
			Debug.LogError("모든 Firebase 종속성을 해결할 수 없음 : " + dependencyStatus);
		}
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
		// 랭킹 정보를 담을 리스트를 생성함
		var rankList = new List<(string, int, int)>();

		Debug.Log("Snapshot Children Count: " + snapshot.ChildrenCount.ToString());

		// 각 자식 노드에 대해 반복함
		foreach (var child in snapshot.Children)
		{
			// 각 사용자의 이름, 레벨, 포인트를 가져와 변수에 저장함
			string username = child.Child("Username").Value.ToString();
			int level = int.Parse(child.Child("Level").Value.ToString());
			int point = int.Parse(child.Child("Point").Value.ToString());

			Debug.Log("Username: " + username);
			Debug.Log("Level: " + level);
			Debug.Log("Point: " + point);

			// 사용자 정보를 랭킹 리스트에 추가함
			rankList.Add((username, level, point));
		}

		// 레벨이 높은 순서대로 가져오되, 레벨이 같은 경우에는 점수가 높은 순서로 데이터를 가져옴
		rankList = rankList.OrderByDescending(x => x.Item2).ThenByDescending(x => x.Item3).ToList();
		Debug.Log("Rank List Count: " + rankList.Count);

		// 랭킹 보드에 표시할 문자열을 생성함
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

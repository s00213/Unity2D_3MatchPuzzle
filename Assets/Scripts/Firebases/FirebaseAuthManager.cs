using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class FirebaseAuthManager : MonoBehaviour
{
	FirebaseAuth user; // 인증이 완료된 유저	
	FirebaseAuth auth; // 로그인, 회원가입 등에 사용

	[Header("Login")]
	public TMP_InputField loginEmail;
	public TMP_InputField loginpassword;
	[Header("Regsiter")]
	public TMP_InputField createEmail;
	public TMP_InputField createPassword;

	void Start()
	{
		auth = FirebaseAuth.DefaultInstance;
	}

	// 신규 사용자 가입
	public void Create()
	{
		auth.CreateUserWithEmailAndPasswordAsync(createEmail.text, createPassword.text).ContinueWith(task =>
		{
			if (task.IsCanceled)
			{
				Debug.LogError("회원가입 취소");
				return;
			}
			if (task.IsFaulted)
			{
				Debug.LogError("회원가입 실패 : " + task.Exception);
				return;
			}

			Firebase.Auth.AuthResult result = task.Result;
			Debug.LogFormat("회원가입 성공: {0} ({1})", result.User.DisplayName, result.User.UserId);
		});
	}

	// 로그인
	public void Login()
	{
		auth.SignInWithEmailAndPasswordAsync(loginEmail.text, loginpassword.text).ContinueWith(task =>
		{
			if (task.IsCanceled)
			{
				Debug.LogError("로그인 취소");
				return;
			}
			if (task.IsFaulted)
			{
				Debug.LogError("로그인 실패 : " + task.Exception);
				return;
			}

			Firebase.Auth.AuthResult result = task.Result;
			Debug.LogFormat("로그인 성공 : {0} ({1})", result.User.DisplayName, result.User.UserId);

		});
	}

	// 로그아웃
	public void LogOut()
    { 
        auth.SignOut();
        Debug.Log("로그아웃");
    }
}
 

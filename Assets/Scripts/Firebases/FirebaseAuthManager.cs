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
	FirebaseAuth user; // ������ �Ϸ�� ����	
	FirebaseAuth auth; // �α���, ȸ������ � ���

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

	// �ű� ����� ����
	public void Create()
	{
		auth.CreateUserWithEmailAndPasswordAsync(createEmail.text, createPassword.text).ContinueWith(task =>
		{
			if (task.IsCanceled)
			{
				Debug.LogError("ȸ������ ���");
				return;
			}
			if (task.IsFaulted)
			{
				Debug.LogError("ȸ������ ���� : " + task.Exception);
				return;
			}

			Firebase.Auth.AuthResult result = task.Result;
			Debug.LogFormat("ȸ������ ����: {0} ({1})", result.User.DisplayName, result.User.UserId);
		});
	}

	// �α���
	public void Login()
	{
		auth.SignInWithEmailAndPasswordAsync(loginEmail.text, loginpassword.text).ContinueWith(task =>
		{
			if (task.IsCanceled)
			{
				Debug.LogError("�α��� ���");
				return;
			}
			if (task.IsFaulted)
			{
				Debug.LogError("�α��� ���� : " + task.Exception);
				return;
			}

			Firebase.Auth.AuthResult result = task.Result;
			Debug.LogFormat("�α��� ���� : {0} ({1})", result.User.DisplayName, result.User.UserId);

		});
	}

	// �α׾ƿ�
	public void LogOut()
    { 
        auth.SignOut();
        Debug.Log("�α׾ƿ�");
    }
}
 

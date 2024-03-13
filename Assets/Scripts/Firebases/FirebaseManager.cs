using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEditor.VersionControl;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance;

	[Header("Firebase")]
	public DependencyStatus dependencyStatus;
	public FirebaseAuth auth; // �α���, ȸ������ � ���
	public FirebaseUser user; // ������ �Ϸ�� ����	
	public DatabaseReference DBreference;

	[Header("Login")]
	public TMP_InputField loginEmail;
	public TMP_InputField loginpassword;
	public TextMeshProUGUI loginSuccessText;
	public TextMeshProUGUI loginErrorText;

	[Header("Regsiter")]
	public TMP_InputField regsiterUsername;
	public TMP_InputField regsiterEmail;
	public TMP_InputField regsiterPassword;
	public TextMeshProUGUI registerSuccessText;
	public TextMeshProUGUI registerErrorText;

	[Header("Reset Password")]
	public TMP_InputField resetPasswordEmail;
	public TMP_Text resetPasswordSuccessText;
	public TMP_Text resetPasswordErrorText;

	TitleScene titleScene;

	int level;
	int point;
	int nextSceneToLoad;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		StartCoroutine(CheckAndFixDependenciesRoutine());
		nextSceneToLoad = UnitySceneManager.GetActiveScene().buildIndex + 1;
	}

	// FirebaseAuth �ν��Ͻ� ��ü ����
	private void InitializeFirebase()
	{
		auth = FirebaseAuth.DefaultInstance;

		DBreference = FirebaseDatabase.DefaultInstance.RootReference;

		auth.StateChanged += AuthStateChanged;
		AuthStateChanged(this, null);
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

	// ���� �α����� ����� �������� �̺�Ʈ
	private void AuthStateChanged(object sender, System.EventArgs eventArgs)
	{
		Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
		Firebase.Auth.FirebaseUser user = null;
		if (auth.CurrentUser != user)
		{
			bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
			user = auth.CurrentUser;
		}
	}

	// �α��� �ʵ� �ؽ�Ʈ ���� 
	public void DeleteLoginFeild()
	{
		loginEmail.text = "";
		loginpassword.text = "";
	}

	// ���� �ʵ� �ؽ�Ʈ ����
	public void DeleteRegisterFeild()
	{
		regsiterUsername.text = "";
		regsiterEmail.text = "";
		regsiterPassword.text = "";
	}

	// �̸��� �缳�� �ʵ� �ؽ�Ʈ ����
	public void DeleteEmailFeild()
	{
		resetPasswordEmail.text = "";
	}

	// ȸ������
	public void Register()
	{
		StartCoroutine(RegisterRoutine(regsiterEmail.text, regsiterPassword.text, regsiterUsername.text));
	}

	// ȸ������ �ڷ�ƾ
	IEnumerator RegisterRoutine(string _email, string _password, string _username)
	{
		if (_username == "")
		{
			registerErrorText.text = "Missing Username";
			yield return new WaitForSeconds(3);
			registerErrorText.text = "";
		}
		else
		{
			// �̸��ϰ� ��й�ȣ�� �����Ͽ� Firebase ���� �α��� �Լ��� ȣ����
			var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

			// �۾��� �Ϸ�� ������ �����
			yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

			// ȸ������ ����
			if (RegisterTask.Exception != null)
			{         
				FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
				AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

				string message = "Register failed check internet connection!";
				switch (errorCode)
				{
					case AuthError.MissingEmail:
						message = "Missing Email";
						break;
					case AuthError.MissingPassword:
						message = "Missing Password";
						break;
					case AuthError.WeakPassword:
						message = "Weak Password";
						break;
					case AuthError.EmailAlreadyInUse:
						message = "Email Already In Use";
						break;
				}
				registerErrorText.text = message;
				yield return new WaitForSeconds(3);
				registerErrorText.text = "";
			}
			else
			{
				// ȸ������ �Ϸ�
				user = RegisterTask.Result.User;
				registerSuccessText.text = "Register Successfully";
				var One = DBreference.Child("User").Child(user.UserId).Child("Username").SetValueAsync(_username);
				yield return new WaitUntil(predicate: () => One.IsCompleted);

				var Two = DBreference.Child("User").Child(user.UserId).Child("Level").SetValueAsync(0);
				yield return new WaitUntil(predicate: () => Two.IsCompleted);

				var Tree = DBreference.Child("User").Child(user.UserId).Child("Point").SetValueAsync(0);
				yield return new WaitUntil(predicate: () => Tree.IsCompleted);

				yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);
				
				// TODO : �α��� UI Ȱ��ȭ
				//UIManager.Instance.LoginScreen();
				registerErrorText.text = "";
				registerErrorText.text = "";
				Debug.Log("ȸ������ ������");
				DeleteRegisterFeild();
			}
		}
	}

	// �α���
	public void Login()
	{
		StartCoroutine(LoginRoutine(loginEmail.text, loginpassword.text));
	}

	// �α��� �ڷ�ƾ
	IEnumerator LoginRoutine(string _email, string _password)
	{
		// �̸��ϰ� ��й�ȣ�� �����Ͽ� Firebase ���� �α��� �Լ��� ȣ����
		var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
		// �۾��� �Ϸ�� ������ �����
		yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

		// �α��� ����
		if (LoginTask.Exception != null)
		{       
			FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
			AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

			string message = "Login failed check internet connection!";
			switch (errorCode)
			{
				case AuthError.MissingEmail:
					message = "Missing Email";
					break;
				case AuthError.MissingPassword:
					message = "Missing Password";
					break;
				case AuthError.WrongPassword:
					message = "Wrong Password";
					break;
				case AuthError.InvalidEmail:
					message = "Invalid Email";
					break;
				case AuthError.UserNotFound:
					message = "Account does not exist";
					break;
			}

			loginErrorText.text = message;
			yield return new WaitForSeconds(3);
			loginErrorText.text = "";
		}
		else
		{
			// �α��� ����
			user = LoginTask.Result.User;
			Debug.LogFormat("�α��� ���� : {0} ({1})", user.DisplayName, user.Email);
			loginErrorText.text = "";
			loginSuccessText.text = "Success Log In";
			//StartCoroutine(LoadData());
			yield return new WaitForSeconds(1);

			TitleScene.Title.LoginSucces();
			//PlayerPrefs.SetString("Name", Username.text);
			loginSuccessText.text = "";

		}
	}

	// �α׾ƿ�
	public void LogOut()
	{
		auth.SignOut();
		Debug.Log("�α׾ƿ�");
	}
}

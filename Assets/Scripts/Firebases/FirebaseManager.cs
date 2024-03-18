using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance;

	[Header("Firebase")]
	public DependencyStatus dependencyStatus;
	public FirebaseAuth auth; // 로그인, 회원가입 등에 사용
	public FirebaseUser user; // 인증이 완료된 유저	
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
	public TMP_Text resetPasswordText;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		StartCoroutine(CheckAndFixDependenciesRoutine());			
	}

	// FirebaseAuth 인스턴스 객체 설정
	private void InitializeFirebase()
	{
		auth = FirebaseAuth.DefaultInstance;

		DBreference = FirebaseDatabase.DefaultInstance.RootReference;

		auth.StateChanged += AuthStateChanged;
		AuthStateChanged(this, null);
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

	// 현재 로그인한 사용자 가져오는 이벤트
	private void AuthStateChanged(object sender, System.EventArgs eventArgs)
	{
		Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
		Firebase.Auth.FirebaseUser user = null;
		if (auth.CurrentUser != user)
		{
			bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
			user = auth.CurrentUser;
		}
		else
		{
			TitleScene.instance.LoginUI();
		}
	}

	// 로그인 필드 텍스트 삭제 
	public void DeleteLoginFeild()
	{
		loginEmail.text = "";
		loginpassword.text = "";
	}

	// 가입 필드 텍스트 삭제
	public void DeleteRegisterFeild()
	{
		regsiterUsername.text = "";
		regsiterEmail.text = "";
		regsiterPassword.text = "";
	}

	// 이메일 재설정 필드 텍스트 삭제
	public void DeleteEmailFeild()
	{
		resetPasswordEmail.text = "";
	}

	// 회원가입
	public void Register()
	{
		StartCoroutine(RegisterRoutine(regsiterEmail.text, regsiterPassword.text, regsiterUsername.text));
	}

	// 회원가입 코루틴
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
			// 이메일과 비밀번호를 전달하여 Firebase 인증 로그인 함수를 호출함
			var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

			// 작업이 완료될 때까지 대기함
			yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

			// 회원가입 에러
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
				// 회원가입 완료
				user = RegisterTask.Result.User;
				registerSuccessText.text = "Register Successfully";
				var One = DBreference.Child("User").Child(user.UserId).Child("Username").SetValueAsync(_username);
				yield return new WaitUntil(predicate: () => One.IsCompleted);

				var Two = DBreference.Child("User").Child(user.UserId).Child("Level").SetValueAsync(0);
				yield return new WaitUntil(predicate: () => Two.IsCompleted);

				var Tree = DBreference.Child("User").Child(user.UserId).Child("Point").SetValueAsync(0);
				yield return new WaitUntil(predicate: () => Tree.IsCompleted);

				yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

				registerErrorText.text = "";
				registerErrorText.text = "";
				Debug.Log("회원가입 성공함");
				DeleteRegisterFeild();
			}
		}
	}

	// 로그인
	public void Login()
	{
		StartCoroutine(LoginRoutine(loginEmail.text, loginpassword.text));
	}

	// 로그인 코루틴
	IEnumerator LoginRoutine(string _email, string _password)
	{
		// 이메일과 비밀번호를 전달하여 Firebase 인증 로그인 함수를 호출함
		var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
		// 작업이 완료될 때까지 대기함
		yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

		// 로그인 에러
		if (LoginTask.Exception != null)
		{
			FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;

			loginErrorText.text = "Login Error";
			yield return new WaitForSeconds(3);
			loginErrorText.text = "";
		}
		else
		{
			// 로그인 성공
			user = LoginTask.Result.User;
			Debug.LogFormat("로그인 성공 : {0} ({1})", user.DisplayName, user.Email);
			loginErrorText.text = "";
			loginSuccessText.text = "Success LogIn";

			yield return new WaitForSeconds(1);

			TitleScene.instance.LoginSucces();
			loginSuccessText.text = "";
		}
	}

	// 로그아웃
	public void LogOut()
	{
		auth.SignOut();

		DeleteLoginFeild();
		DeleteRegisterFeild();

		TitleScene.instance.LoginUI();

		Debug.Log("로그아웃");
	}

	// 패스워드 재설정
	public void ResetPassowrd()
	{
		StartCoroutine(ResetPasswordRoutine(resetPasswordEmail.text));
	}

	// 패스워드 재설정 코루틴
	IEnumerator ResetPasswordRoutine(string _emailAddress)
	{	
		var LoginTask = auth.SendPasswordResetEmailAsync(_emailAddress);

		yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

		resetPasswordText.text = "Reset code send successfully";
		yield return new WaitForSeconds(3);
		resetPasswordText.text = "";
	}
}


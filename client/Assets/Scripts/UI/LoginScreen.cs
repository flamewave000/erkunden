using System.Collections.Generic;
using Assets.Scripts.Net;
using Assets.Scripts.Net.HTTP;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
	public class LoginScreen : Menu
	{
		public const string Name = "Login";
		private Button loginButton;
		private Button showCreateAccountButton;
		private Button createAccountButton;
		private Button backButton;
		private TextField username;
		private TextField password;
		private TextField passwordRepeat;
		private Label errorMessageLabel;
		private VisualElement loginForm;
		private VisualElement createAccountForm;

		private IDeferred<bool> onUsernameChange = null;

		public GameApi api;
		public override string MenuName => Name;

		private string errorMessage
		{
			get => errorMessageLabel.text;
			set
			{
				errorMessageLabel.text = value;
				errorMessageLabel.visible = value != null && value.Trim().Length > 0;
			}
		}

		protected override void InitializeLayout()
		{
			base.InitializeLayout();
			var root = document.rootVisualElement;

			loginButton = root.Q<Button>("LoginButton");
			showCreateAccountButton = root.Q<Button>("ShowCreateAccountButton");
			createAccountButton = root.Q<Button>("CreateAccountButton");
			backButton = root.Q<Button>("BackButton");
			username = root.Q<TextField>("Username");
			password = root.Q<TextField>("Password");
			passwordRepeat = root.Q<TextField>("RepeatPassword");
			errorMessageLabel = root.Q<Label>("ErrorMessage");

			loginForm = root.Q<VisualElement>("LoginForm");
			createAccountForm = root.Q<VisualElement>("CreateAccountForm");

			//#if DEBUG
			//		username.value = "admin";
			//		password.value = "admin";
			//		passwordRepeat.value = "admin";
			//#endif

			loginButton.clicked += Login;
			showCreateAccountButton.clicked += ShowCreateAccountButton_clicked;
			createAccountButton.clicked += CreateAccountButton_clicked;
			backButton.clicked += BackButton_clicked;

			root.Q<Button>("Settings").clicked += SettingsButton_clicked;

			username.RegisterValueChangedCallback(TextChanged);
		}

		protected override void OnResume(Dictionary<string, object> state)
		{
			base.OnResume(state);
			username.value = (string)state.GetValueOrDefault("username", PlayerPrefs.GetString("login.username"));
			password.value = (string)state.GetValueOrDefault("password", "");
			passwordRepeat.value = (string)state.GetValueOrDefault("passwordRepeat", "");
		}
		protected override void OnPause(Dictionary<string, object> state)
		{
			base.OnResume(state);
			state["username"] = username.value;
			state["password"] = password.value;
			state["passwordRepeat"] = passwordRepeat.value;
		}

		private void SettingsButton_clicked()
		{
			GoTo(SettingsScreen.Name);
		}

		private void TextChanged(ChangeEvent<string> text)
		{
			if (createAccountForm.style.display == DisplayStyle.None) return;
			if (onUsernameChange != null) return;
			onUsernameChange = api.CheckAccountExists(text.newValue)
				.OnResult(x => { errorMessage = "Username Already Exists!"; })
				.OnError(x => { errorMessage = ""; })
				.OnComplete((_, _) => onUsernameChange = null);
		}

		private void BackButton_clicked()
		{
			loginForm.style.display = DisplayStyle.Flex;
			createAccountForm.style.display = DisplayStyle.None;
		}

		private void ShowCreateAccountButton_clicked()
		{
			loginForm.style.display = DisplayStyle.None;
			createAccountForm.style.display = DisplayStyle.Flex;
		}

		private void CreateAccountButton_clicked()
		{
			if (username.value.Trim().Length == 0)
			{
				errorMessage = "Please provide a Username";
				return;
			}
			if (password.value.Length >= 10)
			{
				errorMessage = "Please provide a Password\nMiniumum 8 characters";
				return;
			}
			if (passwordRepeat.value.Length == 0)
			{
				errorMessage = "Please repeat the Password";
				return;
			}
			if (password.value != passwordRepeat.value)
			{
				errorMessage = "Password don't match!";
				return;
			}

			backButton.SetEnabled(false);
			createAccountButton.SetEnabled(false);
			username.SetEnabled(false);
			password.SetEnabled(false);
			passwordRepeat.SetEnabled(false);

			api.CreateAccount(username.text, password.text)
				.OnResult(userId =>
				{
					errorMessage = $"Account created!\n({userId})";
					PlayerPrefs.SetString("login.username", username.value);
					PlayerPrefs.Save();
					BackButton_clicked();
				})
				.OnError(error =>
				{
					var httpError = error as HttpException;
					if (httpError != null)
					{
						switch (httpError.code)
						{
							case 400:
								switch (httpError.error)
								{
									case "missing_username":
										errorMessage = "Please enter a username";
										break;
									case "missing_password":
										errorMessage = "Please enter a password";
										break;
									default:
										errorMessage = httpError.error;
										break;
								}
								return;
							case 406:
								errorMessage = "User already exists";
								return;
						}
					}
					errorMessage = error.Message;
				})
				.OnComplete((_, _) =>
				{
					backButton.SetEnabled(true);
					createAccountButton.SetEnabled(true);
					username.SetEnabled(true);
					password.SetEnabled(true);
					passwordRepeat.SetEnabled(true);
				});
		}

		void Login()
		{
			if (username.value.Trim().Length == 0)
			{
				errorMessage = "User ID Required";
				return;
			}
			if (password.value.Length == 0)
			{
				errorMessage = "Password Required";
				return;
			}

			loginButton.SetEnabled(false);
			showCreateAccountButton.SetEnabled(false);
			username.SetEnabled(false);
			password.SetEnabled(false);

			api.Login(username.value, password.value)
				.OnResult(token =>
				{
					errorMessage = "";
					password.value = "";
					passwordRepeat.value = "";
					PlayerPrefs.SetString("login.username", username.value);
					PlayerPrefs.Save();
					//Navigation.Load("CharacterSelection");
				})
				.OnError(error =>
				{
					var httpError = error as HttpException;
					if (httpError != null)
					{
						switch (httpError.code)
						{
							case 400:
								errorMessage = httpError.error == "missing_username" ? "Please enter a username" : "Please enter a password";
								return;
							case 404:
								errorMessage = httpError.error == "invalid_credentials" ? "Invalid Username or Password" : httpError.error;
								return;
						}
					}
					errorMessage = error.Message;
				})
				.OnComplete((_, _) =>
				{
					loginButton.SetEnabled(true);
					showCreateAccountButton.SetEnabled(true);
					username.SetEnabled(true);
					password.SetEnabled(true);
				});
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
	public class MenuNavItem
	{
		public string name;
		public UIDocument document;
		public Menu menu;
	}
	public abstract class Menu : MonoBehaviour
	{
		private static Dictionary<string, Dictionary<string, MenuNavItem>> menus = new Dictionary<string, Dictionary<string, MenuNavItem>>();
		private static Dictionary<string, Stack<string>> history = new Dictionary<string, Stack<string>>();
		private static MenuNavItem root;
		private Dictionary<string, object> state = new Dictionary<string, object>();
		protected UIDocument document;

		public abstract string MenuName { get; }
		public string MenuGroup;
		public bool IsRoot;

		protected bool CanGoBack => history[MenuGroup].Count > 1;

		protected virtual void Start()
		{
			document = GetComponent<UIDocument>();
			if (!menus.ContainsKey(MenuGroup))
			{
				menus.Add(MenuGroup, new Dictionary<string, MenuNavItem>());
				history.Add(MenuGroup, new Stack<string>());
			}
			var group = menus[MenuGroup];
			group.Add(MenuName, new MenuNavItem
			{
				name = MenuName,
				document = document,
				menu = this
			});
			if (IsRoot)
			{
				if (root != null) throw new Exception($"{MenuName}.IsRoot conflicts with {root.name}.IsRoot");
				root = group[MenuName];
				history[MenuGroup].Push(MenuName);
				OnResume(state);
			}
			else
				gameObject.SetActive(false);
		}

		protected void GoTo(string menuName)
		{
			var group = menus[MenuGroup];
			if (!group.ContainsKey(menuName)) throw new Exception($"'{menuName}' is not a member of the '{MenuGroup}' group");
			var groupHistory = history[MenuGroup];

			var current = group[groupHistory.Peek()];
			current.menu.OnPause(current.menu.state);
			current.document.gameObject.SetActive(false);

			groupHistory.Push(menuName);
			group[menuName].document.gameObject.SetActive(true);
			group[menuName].menu.OnResume(group[menuName].menu.state);
		}

		protected void GoBack()
		{
			if (!CanGoBack) throw new Exception("Cannot go back as this is the root menu");
			var group = menus[MenuGroup];
			var groupHistory = history[MenuGroup];

			var current = group[groupHistory.Pop()];
			current.menu.OnPause(current.menu.state);
			current.document.gameObject.SetActive(false);

			var previous = groupHistory.Peek();
			group[previous].document.gameObject.SetActive(true);
			group[previous].menu.OnResume(group[previous].menu.state);
		}

		protected virtual void InitializeLayout() { }
		protected virtual void OnPause(Dictionary<string, object> state) { }
		protected virtual void OnResume(Dictionary<string, object> state) { InitializeLayout(); }
	}
}

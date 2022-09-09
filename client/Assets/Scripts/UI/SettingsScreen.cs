using Assets.Scripts.UI;
using UnityEngine.UIElements;

public class SettingsScreen : Menu
{
	public const string Name = "Settings";

	private Button saveButton;
	private Button backButton;

	public override string MenuName => Name;

	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();
	}

	protected override void InitializeLayout()
	{
		base.InitializeLayout();
		var root = document.rootVisualElement;
		saveButton = root.Q<Button>("Save");
		saveButton.clicked += SaveButton_clicked;
		backButton = root.Q<Button>("Back");
		backButton.clicked += BackButton_clicked;
		saveButton.SetEnabled(false);
	}

	private void SaveButton_clicked()
	{
		throw new System.NotImplementedException();
	}
	private void BackButton_clicked()
	{
		GoBack();
	}
}

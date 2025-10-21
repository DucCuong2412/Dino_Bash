using UnityEngine;

public class LocalizedText : MonoBase
{
	private tk2dTextMesh _textMesh;

	[SerializeField]
	private bool AutoLocalisation = true;

	[SerializeField]
	private string KeyOverride = string.Empty;

	private bool isInitialized;

	private string _Key;

	public tk2dTextMesh textMesh
	{
		get
		{
			if (!isInitialized)
			{
				Start();
			}
			return _textMesh;
		}
		private set
		{
			_textMesh = value;
		}
	}

	public string Key
	{
		get
		{
			return _Key;
		}
		set
		{
			if (_Key != value)
			{
				_Key = value;
				Localize();
			}
		}
	}

	public void Localize()
	{
		if (!isInitialized)
		{
			Start();
		}
		string text = _Key.Localize();
		if (text.Length > textMesh.maxChars)
		{
			textMesh.maxChars = text.Length;
		}
		textMesh.text = text;
		textMesh.Commit();
		textMesh.ForceBuild();
	}

	public void Start()
	{
		if (isInitialized)
		{
			return;
		}
		isInitialized = true;
		_textMesh = GetComponent<tk2dTextMesh>();
		if (textMesh == null)
		{
			Debug.LogError("no TextMesh Component found! - " + base.gameObject.name);
			Object.Destroy(this);
		}
		else if (AutoLocalisation)
		{
			if (KeyOverride == string.Empty)
			{
				string text = textMesh.text;
				text = text.Replace("\n", string.Empty);
				text = text.Replace("\r", string.Empty);
				Key = text;
			}
			else
			{
				Key = KeyOverride;
			}
		}
	}
}

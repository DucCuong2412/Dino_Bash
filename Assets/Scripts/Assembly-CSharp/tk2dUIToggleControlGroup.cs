using System;
using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/tk2dUIToggleControlGroup")]
public class tk2dUIToggleControlGroup : MonoBehaviour
{
	private tk2dUIToggleButton[] toggleBtns;

	[SerializeField]
	private tk2dUIToggleControl[] toggleControls;

	public int initSelectedIndex;

	private tk2dUIToggleButton currSelectedToggleButton;

	public tk2dUIToggleControl[] ToggleControls
	{
		get
		{
			return toggleControls;
		}
	}

	public tk2dUIToggleButton CurrSelectedToggleButton
	{
		get
		{
			return currSelectedToggleButton;
		}
	}

	public tk2dUIToggleControl CurrSelectedToggleControl
	{
		get
		{
			for (int i = 0; i < toggleBtns.Length; i++)
			{
				if (toggleBtns[i] == currSelectedToggleButton)
				{
					return toggleControls[i];
				}
			}
			return null;
		}
	}

	public event Action<tk2dUIToggleControlGroup> OnChange;

	private void Awake()
	{
		BuildToggleBtnList();
		Setup();
	}

	private void BuildToggleBtnList()
	{
		toggleBtns = new tk2dUIToggleButton[toggleControls.Length];
		for (int i = 0; i < toggleControls.Length; i++)
		{
			toggleBtns[i] = toggleControls[i];
		}
	}

	protected void Setup()
	{
		if (initSelectedIndex >= 0 && initSelectedIndex < toggleBtns.Length)
		{
			currSelectedToggleButton = toggleBtns[initSelectedIndex];
		}
		else if (toggleBtns.Length <= 0)
		{
			currSelectedToggleButton = null;
		}
		else
		{
			initSelectedIndex = 0;
			currSelectedToggleButton = toggleBtns[initSelectedIndex];
		}
		tk2dUIToggleButton[] array = toggleBtns;
		foreach (tk2dUIToggleButton tk2dUIToggleButton2 in array)
		{
			if (tk2dUIToggleButton2 == currSelectedToggleButton)
			{
				tk2dUIToggleButton2.IsOn = true;
			}
			else
			{
				tk2dUIToggleButton2.IsOn = false;
			}
			tk2dUIToggleButton2.IsInToggleGroup = true;
			tk2dUIToggleButton2.OnToggle += ButtonToggle;
		}
	}

	public void AddNewToggleControls(tk2dUIToggleControl[] newToggleControls)
	{
		ClearExistingToggleBtns();
		toggleControls = newToggleControls;
		BuildToggleBtnList();
		Setup();
	}

	private void ClearExistingToggleBtns()
	{
		if (toggleBtns != null && toggleBtns.Length > 0)
		{
			tk2dUIToggleButton[] array = toggleBtns;
			foreach (tk2dUIToggleButton tk2dUIToggleButton2 in array)
			{
				tk2dUIToggleButton2.IsInToggleGroup = false;
				tk2dUIToggleButton2.OnToggle -= ButtonToggle;
				tk2dUIToggleButton2.IsOn = false;
			}
		}
	}

	private void ButtonToggle(tk2dUIToggleButton toggleButton)
	{
		if (!toggleButton.IsOn || !(toggleButton != currSelectedToggleButton))
		{
			return;
		}
		currSelectedToggleButton = toggleButton;
		tk2dUIToggleButton[] array = toggleBtns;
		foreach (tk2dUIToggleButton tk2dUIToggleButton2 in array)
		{
			if (tk2dUIToggleButton2 != toggleButton)
			{
				tk2dUIToggleButton2.IsOn = false;
			}
		}
		if (this.OnChange != null)
		{
			this.OnChange(this);
		}
	}
}

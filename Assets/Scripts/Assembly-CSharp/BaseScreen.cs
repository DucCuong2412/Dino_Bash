using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScreen : MonoBase
{
	public const float depthOffset = 512f;

	protected const float showDuration = 0.3f;

	protected const float hideDuration = 0.3f;

	public Action OnEscapeUp;

	private bool isvisible;

	private List<tk2dUIItem> ui_item_set = new List<tk2dUIItem>();

	private bool interactive;

	protected Vector3 top
	{
		get
		{
			return new Vector3(0f, 1536f, 0f);
		}
	}

	protected Vector3 bottom
	{
		get
		{
			return new Vector3(0f, -1536f, 0f);
		}
	}

	protected Vector3 left
	{
		get
		{
			return new Vector3(-2800f, 0f, 0f);
		}
	}

	protected Vector3 right
	{
		get
		{
			return new Vector3(2800f, 0f, 0f);
		}
	}

	public bool isVisible
	{
		get
		{
			return isvisible;
		}
		protected set
		{
			isvisible = value;
		}
	}

	public bool Interactive
	{
		get
		{
			return interactive;
		}
		set
		{
			if (value == interactive)
			{
				return;
			}
			interactive = value;
			if (interactive)
			{
				foreach (tk2dUIItem item in ui_item_set)
				{
					if (!(item == null) && !(item.tag == "UI_Disabled"))
					{
						item.enabled = true;
					}
				}
				return;
			}
			ui_item_set.Clear();
			tk2dUIItem[] componentsInChildren = GetComponentsInChildren<tk2dUIItem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ui_item_set.Add(componentsInChildren[i]);
				componentsInChildren[i].enabled = false;
			}
		}
	}

	public int SortingLayerID { get; private set; }

	public event Action OnScreenShow;

	public event Action OnScreenHide;

	protected void ShowFrom(Vector3 position, Action callback = null, float speed = 0.3f, GoUpdateType goUpdateType = GoUpdateType.Update)
	{
		base.transform.localPosition = new Vector3(position.x, position.y, base.transform.localPosition.z);
		Go.to(base.transform, speed, new GoTweenConfig().localPosition(new Vector3(0f, 0f, base.transform.localPosition.z)).setEaseType(GoEaseType.CircOut).setUpdateType(goUpdateType)
			.onComplete(delegate
			{
				if (callback != null)
				{
					callback();
				}
			}));
	}

	protected void HideTo(Vector3 position, Action callback = null, float speed = 0.3f, GoUpdateType goUpdateType = GoUpdateType.Update)
	{
		position.z = base.transform.localPosition.z;
		Go.to(base.transform, speed, new GoTweenConfig().localPosition(position).setEaseType(GoEaseType.CircIn).setUpdateType(goUpdateType)
			.onComplete(delegate
			{
				if (callback != null)
				{
					callback();
				}
			}));
	}

	public virtual void Show()
	{
		SetAnchors();
		Interactive = true;
		isVisible = true;
		GameInput.OnEscapeKeyUp += HandleOnEscapeKeyUp;
		if (this.OnScreenShow != null)
		{
			this.OnScreenShow();
		}
	}

	private void HandleOnEscapeKeyUp()
	{
		if (OnEscapeUp != null)
		{
			OnEscapeUp();
		}
	}

	public virtual void Hide()
	{
		OnEscapeUp = null;
		Interactive = false;
		isVisible = false;
		GameInput.OnEscapeKeyUp -= HandleOnEscapeKeyUp;
		if (this.OnScreenHide != null)
		{
			this.OnScreenHide();
		}
	}

	protected virtual void OnDestroy()
	{
		ScreenManager.Remove(this);
	}

	public void internal_SetSortingLayerID(int sortingID)
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.z = 23552f - 512f * (float)sortingID;
		base.transform.localPosition = localPosition;
		string text = base.gameObject.name;
		if (char.IsDigit(text[0]))
		{
			text = text.Substring(5);
		}
		base.gameObject.name = sortingID.ToString("00") + " - " + text;
		SpriteTools.SetSortingLayerID(this, sortingID);
		SortingLayerID = sortingID;
	}

	protected void SetAnchors()
	{
		tk2dCameraAnchor[] componentsInChildren = GetComponentsInChildren<tk2dCameraAnchor>();
		tk2dCameraAnchor[] array = componentsInChildren;
		foreach (tk2dCameraAnchor tk2dCameraAnchor2 in array)
		{
			tk2dCameraAnchor2.AnchorCamera = ScreenManager.Camera;
		}
	}
}

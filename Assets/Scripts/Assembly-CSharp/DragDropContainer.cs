using System;
using System.Collections.Generic;
using UnityEngine;

public class DragDropContainer : MonoBehaviour
{
	public float mDepthOffset = -1f;

	public Transform[] mSlots;

	public Transform[] mSelectionSlots;

	private Dictionary<Transform, tk2dUIItem> mSlotsMapping = new Dictionary<Transform, tk2dUIItem>();

	private Transform dragStartTransform;

	private void Start()
	{
		GatherSlotPositions();
	}

	private void OnEnable()
	{
		if (mSlotsMapping.Count <= 0)
		{
			return;
		}
		foreach (tk2dUIItem value in mSlotsMapping.Values)
		{
			if (!(value == null))
			{
				value.OnDownUIItem += StartDrag;
				value.OnReleaseUIItem += EndDrag;
			}
		}
	}

	private void OnDisable()
	{
		if (mSlotsMapping.Count <= 0)
		{
			return;
		}
		foreach (tk2dUIItem value in mSlotsMapping.Values)
		{
			if (!(value == null))
			{
				value.OnDownUIItem -= StartDrag;
				value.OnReleaseUIItem -= EndDrag;
			}
		}
	}

	public void GatherSlotPositions()
	{
		mSlotsMapping = new Dictionary<Transform, tk2dUIItem>();
		Transform[] array = mSlots;
		foreach (Transform transform in array)
		{
			if (!(transform == null))
			{
				mSlotsMapping.Add(transform, null);
			}
		}
		Transform[] array2 = mSelectionSlots;
		foreach (Transform transform2 in array2)
		{
			if (!(transform2 == null))
			{
				mSlotsMapping.Add(transform2, null);
			}
		}
	}

	public void AddElementsToSlots(tk2dUIItem[] pElements)
	{
		if (pElements.Length > mSlots.Length)
		{
			throw new Exception("too many elements");
		}
		for (int i = 0; i < pElements.Length; i++)
		{
			tk2dUIItem tk2dUIItem2 = pElements[i];
			mSlotsMapping[mSlots[i]] = tk2dUIItem2;
			tk2dUIItem2.transform.position = mSlots[i].position;
			tk2dUIItem2.transform.PosZ(tk2dUIItem2.transform.position.z + mDepthOffset);
			tk2dUIItem2.OnDownUIItem += StartDrag;
			tk2dUIItem2.OnReleaseUIItem += EndDrag;
		}
	}

	public void AddElementsToSelectionSlots(tk2dUIItem[] pElements)
	{
		if (pElements.Length > mSelectionSlots.Length)
		{
			throw new Exception("too many elements");
		}
		for (int i = 0; i < pElements.Length; i++)
		{
			tk2dUIItem tk2dUIItem2 = pElements[i];
			mSlotsMapping[mSelectionSlots[i]] = tk2dUIItem2;
			tk2dUIItem2.transform.position = mSelectionSlots[i].position;
			tk2dUIItem2.transform.PosZ(tk2dUIItem2.transform.position.z + mDepthOffset);
			tk2dUIItem2.OnDownUIItem += StartDrag;
			tk2dUIItem2.OnReleaseUIItem += EndDrag;
		}
		foreach (KeyValuePair<Transform, tk2dUIItem> item in mSlotsMapping)
		{
			if (item.Value != null)
			{
				Debug.Log(item.Key.ToString() + " - " + item.Value.ToString());
			}
		}
	}

	public tk2dUIItem[] GetSelectedElements()
	{
		List<tk2dUIItem> list = new List<tk2dUIItem>();
		Transform[] array = mSelectionSlots;
		foreach (Transform key in array)
		{
			if (mSlotsMapping.ContainsKey(key))
			{
				tk2dUIItem tk2dUIItem2 = mSlotsMapping[key];
				if (tk2dUIItem2 != null)
				{
					list.Add(tk2dUIItem2);
				}
			}
		}
		return list.ToArray();
	}

	private void StartDrag(tk2dUIItem pItem)
	{
		pItem.transform.PosZ(pItem.transform.position.z + mDepthOffset);
		dragStartTransform = GetClosestTransform(pItem.transform.position);
	}

	private void EndDrag(tk2dUIItem pItem)
	{
		pItem.transform.PosZ(pItem.transform.position.z - mDepthOffset);
		Transform closestTransform = GetClosestTransform(pItem.transform.position);
		if (mSlotsMapping[closestTransform] != null)
		{
			tk2dUIItem tk2dUIItem2 = mSlotsMapping[closestTransform];
			tk2dUIItem2.transform.positionTo(0.2f, dragStartTransform.position);
			mSlotsMapping[dragStartTransform] = mSlotsMapping[closestTransform];
		}
		else
		{
			mSlotsMapping[dragStartTransform] = null;
		}
		pItem.transform.position = closestTransform.position;
		mSlotsMapping[closestTransform] = pItem;
		dragStartTransform = null;
	}

	private Transform GetClosestTransform(Vector3 pPosition)
	{
		Transform transform = null;
		float num = float.PositiveInfinity;
		foreach (Transform key in mSlotsMapping.Keys)
		{
			float num2 = Vector3.Distance(pPosition, key.position);
			transform = ((!(num2 < num)) ? transform : key);
			num = Mathf.Min(num2, num);
		}
		return transform;
	}
}

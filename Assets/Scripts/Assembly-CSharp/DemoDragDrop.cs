using UnityEngine;

[RequireComponent(typeof(DragDropContainer))]
public class DemoDragDrop : MonoBehaviour
{
	public tk2dUIItem[] mItems;

	public tk2dUIItem[] mSelectedItems;

	private void Start()
	{
		DragDropContainer component = GetComponent<DragDropContainer>();
		component.AddElementsToSlots(mItems);
		component.AddElementsToSelectionSlots(mSelectedItems);
	}
}

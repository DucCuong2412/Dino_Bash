using UnityEngine;

public class FX_CreateOnStart : MonoBehaviour
{
	[SerializeField]
	private GameObject FX_Prefab;

	[SerializeField]
	private string FX_TransformParent;

	private GameObject fxInstance;

	private void Start()
	{
		if (FX_TransformParent != null)
		{
			fxInstance = (GameObject)Object.Instantiate(FX_Prefab);
		}
		fxInstance.transform.parent = base.gameObject.transform.Search(FX_TransformParent);
		fxInstance.transform.localPosition = Vector3.zero;
	}
}

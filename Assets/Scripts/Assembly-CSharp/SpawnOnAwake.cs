using UnityEngine;

public class SpawnOnAwake : MonoBehaviour
{
	[SerializeField]
	private GameObject spawnPrefab;

	[SerializeField]
	private Transform[] transformTargets;

	[SerializeField]
	private AnimatorOverrideController animatorOverride;

	[SerializeField]
	private AnimatorUpdateMode updateMode;

	[SerializeField]
	private Vector3 scale = Vector3.one;

	[SerializeField]
	private string spawnedTag;

	[SerializeField]
	private bool changeLayer;

	[SerializeField]
	private bool destroySpawner;

	private void Awake()
	{
		if (transformTargets == null || transformTargets.Length == 0)
		{
			Spawn(base.transform);
		}
		for (int i = 0; i < transformTargets.Length; i++)
		{
			Spawn(transformTargets[i]);
		}
		if (destroySpawner)
		{
			Object.Destroy(this);
		}
	}

	private void Spawn(Transform pParent)
	{
		if (spawnPrefab == null)
		{
			Debug.LogError("object to spawn is null: " + base.gameObject.name);
			return;
		}
		if (spawnPrefab == base.gameObject)
		{
			Debug.LogError("self referece error: " + base.gameObject.name);
			return;
		}
		GameObject gameObject = null;
		gameObject = Object.Instantiate(spawnPrefab) as GameObject;
		ApplyThemeSwitcher(ref gameObject);
		gameObject.transform.parent = pParent;
		gameObject.transform.localScale = scale;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		if (spawnedTag != null || spawnedTag == string.Empty)
		{
			gameObject.tag = spawnedTag;
		}
		if (changeLayer)
		{
			gameObject.transform.SetLayer(base.gameObject.layer);
		}
		ReplaceAnimatorController(gameObject);
	}

	private void ReplaceAnimatorController(GameObject go)
	{
		if (animatorOverride != null)
		{
			Animator component = go.GetComponent<Animator>();
			if (component != null)
			{
				component.runtimeAnimatorController = animatorOverride;
				component.updateMode = updateMode;
			}
		}
	}

	private void ApplyThemeSwitcher(ref GameObject go)
	{
		ThemeSwitcher component = go.GetComponent<ThemeSwitcher>();
		if (component != null)
		{
			go = component.SetTheme();
		}
	}
}

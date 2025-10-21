using UnityEngine;

public class Meteor : GenericCombatBehaviour<BaseEntity>
{
	[SerializeField]
	private Transform meteor_prefab;

	[SerializeField]
	private int meteor_amount;

	public override void Init()
	{
		if (!isInititalized)
		{
			base.Init();
			ObjectPool.CreatePool(meteor_prefab);
		}
	}

	private void OnEnable()
	{
		if (!isInititalized)
		{
			Init();
		}
		if (Level.Instance.state != Level.State.playing)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < meteor_amount; i++)
		{
			AbstractShot meteor = ObjectPool.Spawn(meteor_prefab).GetComponent<AbstractShot>();
			meteor.ResetSettings();
			meteor.transform.parent = base.transform.parent;
			meteor.gameObject.SetActive(false);
			Vector3 position = GameCamera.Instance.transform.position + new Vector3(256f + (float)i * Random.Range(400f, 600f), 1800f, 0f);
			meteor.transform.position = position;
			WaitThen(num, delegate
			{
				meteor.gameObject.SetActive(true);
				meteor.Fire(false);
			});
			num += 0.2f;
		}
		WaitThen(num, delegate
		{
			EntityFactory.Free(base.Entity);
		});
	}

	private void OnDisable()
	{
	}
}

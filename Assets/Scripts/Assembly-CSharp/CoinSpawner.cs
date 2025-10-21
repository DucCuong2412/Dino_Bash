using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
	private ParticleSystem ps;

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
		ps.emissionRate = 0f;
		EntityFactory.OnNeanderBuilt -= HandleOnNeanderBuilt;
		EntityFactory.OnNeanderBuilt += HandleOnNeanderBuilt;
	}

	private void OnDestroy()
	{
		EntityFactory.OnNeanderBuilt -= HandleOnNeanderBuilt;
	}

	private void HandleOnNeanderBuilt(Unit neander)
	{
		neander.OnStateChanged -= emitCoins;
		neander.OnStateChanged += emitCoins;
	}

	private void emitCoins(Unit unit, Unit.State state)
	{
		if (unit.unitType != UnitType.Neander_Bush && state == Unit.State.die)
		{
			base.transform.position = unit.transform.position;
			ps.Emit(Mathf.RoundToInt(3f));
		}
	}
}

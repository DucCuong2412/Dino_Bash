using System.Collections.Generic;
using LeanplumSDK;

public class ShotDataVar
{
	private Var<List<object>> damageLevels;

	private Var<List<object>> cooldownLevels;

	private Var<List<object>> costLevels;

	private Var<float> stunDuration;

	private Var<List<object>> level_requirements;

	public ShotDataVar(ShotData data)
	{
		string text = string.Format("balancing.shot.{0}.", data.type);
		damageLevels = Var.Define(text + "damageLevels", LeanplumHelper.toList<object>(data.damageLevels));
		cooldownLevels = Var.Define(text + "cooldownLevels", LeanplumHelper.toList<object>(data.cooldownLevels));
		costLevels = Var.Define(text + "costLevels", LeanplumHelper.toList<object>(data.costLevels));
		stunDuration = Var.Define(text + "stunDuration", data.stunDuration);
		level_requirements = Var.Define(text + "level_requirements", LeanplumHelper.toList<object>(data.level_requirements));
	}

	public ShotData apply(ShotData data)
	{
		data.damageLevels = LeanplumHelper.toIntArray(damageLevels);
		data.cooldownLevels = LeanplumHelper.toFloatArray(cooldownLevels);
		data.costLevels = LeanplumHelper.toIntArray(costLevels);
		data.stunDuration = stunDuration.Value;
		data.level_requirements = LeanplumHelper.toStringArray(level_requirements);
		return data;
	}
}

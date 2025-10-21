using System.Collections.Generic;
using LeanplumSDK;

public class EntityDataVar
{
	private Var<bool> isUnique;

	private Var<int> attackRange;

	private Var<int> appleCost;

	private Var<int> premiumCost;

	private Var<float> walkspeed;

	private Var<float> buildcooldown;

	private Var<float> dropChance;

	private Var<List<object>> costLevels;

	private Var<List<object>> attackStrenghLevels;

	private Var<List<object>> healthpointslevels;

	private Var<List<object>> upgrade_skip_costs;

	private Var<List<object>> upgrade_durations;

	private Var<int> minutes_available;

	private Var<List<object>> level_requirements;

	public EntityDataVar(EntityData data)
	{
		string text = string.Format("balancing.entity.{0}.", data.unit);
		isUnique = Var.Define(text + "isUnique", data.isUnique);
		attackRange = Var.Define(text + "attackRange", data.attackRange);
		appleCost = Var.Define(text + "appleCost", data.appleCost);
		premiumCost = Var.Define(text + "premiumCost", data.premiumCost);
		walkspeed = Var.Define(text + "walkspeed", data.walkspeed);
		buildcooldown = Var.Define(text + "buildcooldown", data.buildcooldown);
		dropChance = Var.Define(text + "dropChance", data.dropChance);
		costLevels = Var.Define(text + "costLevels", LeanplumHelper.toList<object>(data.costLevels));
		attackStrenghLevels = Var.Define(text + "attackStrenghLevels", LeanplumHelper.toList<object>(data.attackStrenghLevels));
		healthpointslevels = Var.Define(text + "healthpointslevels", LeanplumHelper.toList<object>(data.healthpointslevels));
		upgrade_skip_costs = Var.Define(text + "upgrade_skip_costs", LeanplumHelper.toList<object>(data.upgrade_skip_costs));
		upgrade_durations = Var.Define(text + "upgrade_durations", LeanplumHelper.toList<object>(data.upgrade_durations));
		minutes_available = Var.Define(text + "minutes_available", data.minutes_available);
		level_requirements = Var.Define(text + "level_requirements", LeanplumHelper.toList<object>(data.level_requirements));
	}

	public EntityData apply(EntityData data)
	{
		data.isUnique = isUnique.Value;
		data.attackRange = attackRange.Value;
		data.appleCost = appleCost.Value;
		data.premiumCost = premiumCost.Value;
		data.walkspeed = walkspeed.Value;
		data.dropChance = dropChance.Value;
		data.buildcooldown = buildcooldown.Value;
		data.costLevels = LeanplumHelper.toIntArray(costLevels);
		data.attackStrenghLevels = LeanplumHelper.toIntArray(attackStrenghLevels);
		data.healthpointslevels = LeanplumHelper.toIntArray(healthpointslevels);
		data.upgrade_skip_costs = LeanplumHelper.toIntArray(upgrade_skip_costs);
		data.upgrade_durations = LeanplumHelper.toIntArray(upgrade_durations);
		data.minutes_available = minutes_available.Value;
		data.level_requirements = LeanplumHelper.toStringArray(level_requirements);
		return data;
	}
}

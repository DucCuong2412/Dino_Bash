using System.Collections.Generic;


public class EntityDataVar
{
    private bool isUnique;

    private int attackRange;

    private int appleCost;

    private int premiumCost;

    private float walkspeed;

    private float buildcooldown;

    private float dropChance;

    private List<object> costLevels;

    private List<object> attackStrenghLevels;

    private List<object> healthpointslevels;

    private List<object> upgrade_skip_costs;

    private List<object> upgrade_durations;

    private int minutes_available;

    private List<object> level_requirements;

    public EntityDataVar(EntityData data)
    {
        string text = string.Format("balancing.entity.{0}.", data.unit);
        isUnique = data.isUnique;
        attackRange = data.attackRange;
        appleCost = data.appleCost;
        premiumCost = data.premiumCost;
        walkspeed = data.walkspeed;
        buildcooldown = data.buildcooldown;
        dropChance = data.dropChance;
        costLevels = LeanplumHelper.toList<object>(data.costLevels);
        attackStrenghLevels = LeanplumHelper.toList<object>(data.attackStrenghLevels);
        healthpointslevels = LeanplumHelper.toList<object>(data.healthpointslevels);
        upgrade_skip_costs = LeanplumHelper.toList<object>(data.upgrade_skip_costs);
        upgrade_durations = LeanplumHelper.toList<object>(data.upgrade_durations);
        minutes_available = data.minutes_available;
        level_requirements = LeanplumHelper.toList<object>(data.level_requirements);
    }

    public EntityData apply(EntityData data)
    {
        data.isUnique = isUnique;
        data.attackRange = attackRange;
        data.appleCost = appleCost;
        data.premiumCost = premiumCost;
        data.walkspeed = walkspeed;
        data.dropChance = dropChance;
        data.buildcooldown = buildcooldown;
        data.costLevels = LeanplumHelper.toIntArray(costLevels);
        data.attackStrenghLevels = LeanplumHelper.toIntArray(attackStrenghLevels);
        data.healthpointslevels = LeanplumHelper.toIntArray(healthpointslevels);
        data.upgrade_skip_costs = LeanplumHelper.toIntArray(upgrade_skip_costs);
        data.upgrade_durations = LeanplumHelper.toIntArray(upgrade_durations);
        data.minutes_available = minutes_available;
        data.level_requirements = LeanplumHelper.toStringArray(level_requirements);
        return data;
    }
}

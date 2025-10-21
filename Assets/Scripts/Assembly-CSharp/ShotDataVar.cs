using System.Collections.Generic;

public class ShotDataVar
{
    private List<int> damageLevels; // Đổi thành List<int>
    private List<float> cooldownLevels; // Đổi thành List<float>
    private List<int> costLevels; // Đổi thành List<int>
    private float stunDuration;
    private List<string> level_requirements; // Đổi thành List<string>

    public ShotDataVar(ShotData data)
    {
        // Giả sử ShotData có các thuộc tính: damageLevels (int[]), cooldownLevels (float[]), 
        // costLevels (int[]), level_requirements (string[])
        damageLevels = new List<int>(data.damageLevels); // Chuyển int[] thành List<int>
        cooldownLevels = new List<float>(data.cooldownLevels); // Chuyển float[] thành List<float>
        costLevels = new List<int>(data.costLevels); // Chuyển int[] thành List<int>
        stunDuration = data.stunDuration; // Giữ nguyên
        level_requirements = new List<string>(data.level_requirements); // Chuyển string[] thành List<string>
    }

    public ShotData apply(ShotData data)
    {
        data.damageLevels = damageLevels.ToArray(); // Chuyển List<int> thành int[]
        data.cooldownLevels = cooldownLevels.ToArray(); // Chuyển List<float> thành float[]
        data.costLevels = costLevels.ToArray(); // Chuyển List<int> thành int[]
        data.stunDuration = stunDuration; // Giữ nguyên
        data.level_requirements = level_requirements.ToArray(); // Chuyển List<string> thành string[]
        return data;
    }
}
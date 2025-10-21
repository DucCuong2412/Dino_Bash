using System;

public class Helper
{
	public static float MapRange(float a1, float a2, float b1, float b2, float s)
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}

	public static int EnumArrayToBitMask(Array enumArray)
	{
		int num = 0;
		int length = enumArray.Length;
		if (length > 32)
		{
			throw new Exception("Cant create bitmask - too many values!");
		}
		for (int i = 0; i < length; i++)
		{
			num |= 1 << (int)enumArray.GetValue(i);
		}
		return num;
	}

	public static string formatTime(int seconds)
	{
		int num = seconds % 60;
		int num2 = seconds / 60;
		return string.Format("{0:0}:{1:00}", num2, num);
	}
}

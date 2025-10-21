using UnityEngine;

public static class Vector3Extensions
{
	public static Vector3 SetX(this Vector3 vec, float x)
	{
		return new Vector3(x, vec.y, vec.z);
	}

	public static Vector3 SetY(this Vector3 vec, float y)
	{
		return new Vector3(vec.x, y, vec.z);
	}

	public static Vector3 SetZ(this Vector3 vec, float z)
	{
		return new Vector3(vec.x, vec.y, z);
	}

	public static Vector3 RotateX(this Vector3 v, float angle)
	{
		float num = Mathf.Sin(angle);
		float num2 = Mathf.Cos(angle);
		float y = v.y;
		float z = v.z;
		return new Vector3(v.x, num2 * y - num * z, num2 * z + num * y);
	}

	public static Vector3 RotateY(this Vector3 v, float angle)
	{
		float num = Mathf.Sin(angle);
		float num2 = Mathf.Cos(angle);
		float x = v.x;
		float z = v.z;
		return new Vector3(num2 * x + num * z, v.y, num2 * z - num * x);
	}

	public static Vector3 RotateZ(this Vector3 v, float angle)
	{
		float num = Mathf.Sin(angle);
		float num2 = Mathf.Cos(angle);
		float x = v.x;
		float y = v.y;
		return new Vector3(num2 * x - num * y, num2 * y + num * x, v.z);
	}
}

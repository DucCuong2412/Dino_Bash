using UnityEngine;

public static class RectExtensions
{
	public static bool Intersect(this Rect rectA, Rect rectB)
	{
		return Mathf.Abs(rectA.x - rectB.x) < Mathf.Abs(rectA.width + rectB.width) / 2f && Mathf.Abs(rectA.y - rectB.y) < Mathf.Abs(rectA.height + rectB.height) / 2f;
	}
}

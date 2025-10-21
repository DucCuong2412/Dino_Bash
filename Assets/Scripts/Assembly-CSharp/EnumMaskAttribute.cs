using System;
using UnityEngine;

public class EnumMaskAttribute : PropertyAttribute
{
	public Type enumType;

	public EnumMaskAttribute(Type pEnumType)
	{
		enumType = pEnumType;
	}
}

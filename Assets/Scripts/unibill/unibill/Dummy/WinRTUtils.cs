using System;
using System.Reflection;

namespace unibill.Dummy
{
	public static class WinRTUtils
	{
		public static MethodInfo GetMethod(Type t, string name, Type[] paramTypes)
		{
			return t.GetMethod(name, paramTypes);
		}
	}
}

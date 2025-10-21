using System;
using System.Collections;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK
{
	public static class SharedUtil
	{
		public static void FillInValues(object source, object destination)
		{
			if (source == null || Json.Serialize(source) == Json.Serialize(destination))
			{
				return;
			}
			if (source is IDictionary)
			{
				if (destination is IDictionary)
				{
					bool flag = false;
					{
						foreach (object key2 in ((IDictionary)source).Keys)
						{
							object key = Convert.ChangeType(key2, destination.GetType().GetGenericArguments()[0]);
							if (((IDictionary)source)[key2] is IDictionary || ((IDictionary)source)[key2] is IList)
							{
								if (((IDictionary)destination).Contains(key))
								{
									FillInValues(((IDictionary)source)[key2], ((IDictionary)destination)[key]);
								}
								continue;
							}
							if (!flag)
							{
								flag = true;
								((IDictionary)destination).Clear();
							}
							((IDictionary)destination)[key] = Convert.ChangeType(((IDictionary)source)[key2], destination.GetType().GetGenericArguments()[1]);
						}
						return;
					}
				}
				if (!(destination is IList))
				{
					return;
				}
				{
					foreach (object key3 in ((IDictionary)source).Keys)
					{
						string text = (string)key3;
						int index = Convert.ToInt32(text.Substring(1, text.Length - 1 - 1));
						FillInValues(((IDictionary)source)[key3], ((IList)destination)[index]);
					}
					return;
				}
			}
			if (source is IList || source is Array)
			{
				int num = 0;
				IList list = (IList)source;
				for (int i = 0; i < list.Count; i++)
				{
					object obj = list[i];
					if (obj is IDictionary || obj is IList)
					{
						FillInValues(obj, ((IList)destination)[num]);
					}
					else
					{
						((IList)destination)[num] = Convert.ChangeType(obj, (!destination.GetType().IsArray) ? destination.GetType().GetGenericArguments()[0] : destination.GetType().GetElementType());
					}
					num++;
				}
			}
			else
			{
				destination = Convert.ChangeType(source, source.GetType());
			}
		}
	}
}

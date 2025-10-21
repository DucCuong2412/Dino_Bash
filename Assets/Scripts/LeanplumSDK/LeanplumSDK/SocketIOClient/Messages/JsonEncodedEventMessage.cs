using System;
using System.Collections.Generic;
using System.Linq;
using LeanplumSDK.MiniJSON;

namespace LeanplumSDK.SocketIOClient.Messages
{
	internal class JsonEncodedEventMessage
	{
		public string name { get; set; }

		public object[] args { get; set; }

		public JsonEncodedEventMessage()
		{
		}

		public JsonEncodedEventMessage(string name, object payload)
			: this(name, new object[1] { payload })
		{
		}

		public JsonEncodedEventMessage(string name, object[] payloads)
		{
			this.name = name;
			args = payloads;
		}

		public T GetFirstArgAs<T>()
		{
			//Discarded unreachable code: IL_0030
			try
			{
				object obj = args.FirstOrDefault();
				if (obj != null)
				{
					return (T)Json.Deserialize(obj.ToString());
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return default(T);
		}

		public IEnumerable<T> GetArgsAs<T>()
		{
			List<T> list = new List<T>();
			object[] array = args;
			foreach (object obj in array)
			{
				list.Add((T)Json.Deserialize(obj.ToString()));
			}
			return list.AsEnumerable();
		}

		public string ToJsonString()
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["name"] = name;
			dictionary["args"] = args;
			return Json.Serialize(dictionary);
		}

		public static JsonEncodedEventMessage Deserialize(string jsonString)
		{
			JsonEncodedEventMessage result = null;
			try
			{
				result = (JsonEncodedEventMessage)Json.Deserialize(jsonString);
			}
			catch (Exception)
			{
			}
			return result;
		}
	}
}

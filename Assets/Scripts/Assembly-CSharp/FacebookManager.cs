using System;
using System.Collections;
using System.Collections.Generic;
using Facebook;
using Facebook.MiniJSON;
using UnityEngine;

public class FacebookManager : ScriptableObject
{
	public class Profile
	{
		public string id;

		public string first_name;

		public string middle_name;

		public string last_name;

		public string email;
	}

	public struct Friend
	{
		public string name;

		public string first_name;

		public string middle_name;

		public string last_name;

		public string id;
	}

	public enum AppRequestType
	{
		UNKNOWN = 0,
		ASK_FOR_LIVES = 1,
		ASK_FOR_FRIENDGATE = 2,
		HELP_FRIENDGATE = 3,
		GIVE_LIVE = 4
	}

	public struct AppRequestData
	{
		public AppRequestType type;

		public string from_id;

		public string from_first_name;

		public AppRequestData(AppRequestType type, string from_id, string from_first_name)
		{
			this.type = type;
			this.from_id = from_id;
			this.from_first_name = from_first_name;
		}

		public static AppRequestData fromJSON(string json)
		{
			AppRequestData result = default(AppRequestData);
			if (json == null)
			{
				Debug.Log("FB: Empty app request data");
				result.type = AppRequestType.UNKNOWN;
			}
			else
			{
				IDictionary dictionary = Json.Deserialize(json) as IDictionary;
				result.type = (AppRequestType)(int)Enum.Parse(typeof(AppRequestType), dictionary["type"] as string);
				result.from_id = dictionary["from_id"] as string;
				result.from_first_name = dictionary["from_name"] as string;
			}
			return result;
		}

		public string toJSON()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("type", type.ToString());
			dictionary.Add("from_id", from_id);
			dictionary.Add("from_name", from_first_name);
			return Json.Serialize(dictionary);
		}
	}

	public class AppRequestRecords : SerializableDictionary<string, DateTime>
	{
	}

	public struct AppRequest
	{
		public string id;

		public string application_name;

		public string application_namespace;

		public string application_id;

		public string to_id;

		public string to_name;

		public string message;

		public DateTime created_time;

		public AppRequestData data;

		public override string ToString()
		{
			return string.Concat("<AppRequest id='", id, "' type='", data.type, "' from='", data.from_first_name, "' to='", to_name, "' message='", message, "'/>");
		}
	}

	private List<string> required_permissions = new List<string>(new string[2] { "user_friends", "email" });

	private Profile profile;

	private bool isInitialized;

	public bool isLoggedIn
	{
		get
		{
			return FB.IsLoggedIn;
		}
	}

	public void Init(Action onInitComplete)
	{
		if (isInitialized)
		{
			onInitComplete();
			return;
		}
		FB.Init(delegate
		{
			isInitialized = true;
			onInitComplete();
		}, onHideUnity);
	}

	private void onHideUnity(bool isUnityShown)
	{
		Debug.Log("FBM.onHideUnity: " + isUnityShown);
	}

	public void checkPermissions(Action<bool> onDone)
	{
		if (!FB.IsLoggedIn)
		{
			Debug.Log("User is not logged in via facebook");
			onDone(false);
		}
		FB.API("/me/permissions", HttpMethod.GET, delegate(FBResult permissions_json)
		{
			Debug.Log("permissions: " + permissions_json.Text);
			IDictionary dictionary = Json.Deserialize(permissions_json.Text) as IDictionary;
			IList list = dictionary["data"] as IList;
			IDictionary dictionary2;
			if (list.Count == 1)
			{
				dictionary2 = list[0] as IDictionary;
			}
			else
			{
				dictionary2 = new Dictionary<string, object>();
				foreach (IDictionary item in list)
				{
					if (item["status"] as string == "granted")
					{
						dictionary2[item["permission"]] = 1;
					}
				}
			}
			bool flag = true;
			foreach (string required_permission in required_permissions)
			{
				if (!dictionary2.Contains(required_permission))
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				Debug.Log("User has not granted all required permissions");
				Debug.Log("required_permissions = " + string.Join(", ", required_permissions.ToArray()));
				Debug.Log("granted_permissions  = " + permissions_json.Text);
			}
			onDone(flag);
		});
	}

	public void getProfile(Action<Profile> onDone)
	{
		if (profile != null)
		{
			onDone(profile);
			return;
		}
		Login(delegate(bool success)
		{
			if (success)
			{
				FB.API("/me", HttpMethod.GET, delegate(FBResult profile_json)
				{
					IDictionary dictionary = Json.Deserialize(profile_json.Text) as IDictionary;
					profile = new Profile();
					profile.id = dictionary["id"] as string;
					profile.first_name = dictionary["first_name"] as string;
					profile.middle_name = dictionary["middle_name"] as string;
					profile.last_name = dictionary["last_name"] as string;
					string text = dictionary["email"] as string;
					profile.email = ((text == null) ? string.Empty : text);
					onDone(profile);
				});
			}
			else
			{
				onDone(null);
			}
		});
	}

	public void Logout()
	{
		FB.Logout();
		profile = null;
		App.Instance.cloudSaveGameManager.setUserProfile(profile);
	}

	public void Login(Action<bool> onLoggedIn)
	{
		Init(delegate
		{
			string scope = string.Join(", ", required_permissions.ToArray());
			if (FB.IsLoggedIn)
			{
				onLoggedIn(true);
			}
			else
			{
				FB.Login(scope, delegate(FBResult result)
				{
					Debug.Log("FB LOGIN RESULT:" + result.Text);
					if ("true" == (Json.Deserialize(result.Text) as IDictionary)["is_logged_in"].ToString().ToLower())
					{
						checkPermissions(delegate(bool has_all_required_permissions)
						{
							if (has_all_required_permissions)
							{
								getProfile(delegate(Profile profile)
								{
									App.Instance.cloudSaveGameManager.setUserProfile(profile);
									onLoggedIn(has_all_required_permissions && FB.IsLoggedIn);
									Tracking.FBConnect(profile);
								});
							}
							else
							{
								FB.Logout();
								Login(onLoggedIn);
							}
						});
					}
					else
					{
						Debug.Log("FB: User canceled login");
						onLoggedIn(false);
					}
				});
			}
		});
	}

	private List<Friend> parseFriends(string friends_json)
	{
		IDictionary dictionary = Json.Deserialize(friends_json) as IDictionary;
		IList list = dictionary["data"] as IList;
		List<Friend> list2 = new List<Friend>();
		foreach (IDictionary item2 in list)
		{
			Friend item = default(Friend);
			item.id = item2["id"] as string;
			item.first_name = item2["first_name"] as string;
			item.middle_name = item2["middle_name"] as string;
			item.last_name = item2["last_name"] as string;
			item.name = item2["name"] as string;
			if (item.first_name == null)
			{
				item.first_name = string.Empty;
			}
			if (item.middle_name == null)
			{
				item.middle_name = string.Empty;
			}
			if (item.last_name == null)
			{
				item.last_name = string.Empty;
			}
			list2.Add(item);
		}
		return list2;
	}

	private void parseAndcombineFriends(string friends_json, string inviteable_friends_json, AppRequestType requestType, Action<List<Friend>> onDone)
	{
		List<Friend> list = parseFriends(friends_json);
		List<Friend> collection = parseFriends(inviteable_friends_json);
		list.AddRange(collection);
		list = filterRecentRequestees(list, requestType);
		list.Shuffle();
		while (list.Count > 50)
		{
			list.RemoveAt(list.Count - 1);
		}
		onDone(list);
	}

	public void getFriends(Action<List<Friend>> onDone, AppRequestType requestType)
	{
		Login(delegate(bool success)
		{
			if (success)
			{
				FB.API("/me/invitable_friends/?fields=id,name,first_name,middle_name,last_name", HttpMethod.GET, delegate(FBResult inviteable_friends_json_)
				{
					if (string.IsNullOrEmpty(inviteable_friends_json_.Error))
					{
						string inviteable_friends_json = inviteable_friends_json_.Text;
						FB.API("/me/friends/?fields=id,name,first_name,middle_name,last_name", HttpMethod.GET, delegate(FBResult friends_json_)
						{
							if (string.IsNullOrEmpty(friends_json_.Error))
							{
								string text = friends_json_.Text;
								parseAndcombineFriends(text, inviteable_friends_json, requestType, onDone);
							}
							else
							{
								Debug.LogError("/me/friends/: " + friends_json_.Error);
								onDone(null);
							}
						});
					}
					else
					{
						Debug.LogError("/me/invitable_friends/: " + inviteable_friends_json_.Error);
						onDone(null);
					}
				});
			}
			else
			{
				onDone(null);
			}
		});
	}

	private List<Friend> filterRecentRequestees(List<Friend> friends, AppRequestType requestType)
	{
		AppRequestRecords appRequestRecords = Serializer.DeserializeFromPlayerPrefs<AppRequestRecords>("AppRequestRecords");
		if (appRequestRecords == null)
		{
			appRequestRecords = new AppRequestRecords();
		}
		AppRequestRecords appRequestRecords2 = new AppRequestRecords();
		foreach (KeyValuePair<string, DateTime> item in appRequestRecords)
		{
			if ((DateTime.UtcNow - item.Value).TotalHours < 8.0)
			{
				appRequestRecords2.Add(item.Key, item.Value);
			}
		}
		appRequestRecords = appRequestRecords2;
		Serializer.SerializeToPlayerPrefs(appRequestRecords, "AppRequestRecords");
		List<Friend> list = new List<Friend>();
		foreach (Friend friend in friends)
		{
			string key = friend.id + requestType;
			if (!appRequestRecords.ContainsKey(key))
			{
				list.Add(friend);
			}
		}
		return list;
	}

	private void recordAppRequests(AppRequestType request_type, string[] ids)
	{
		AppRequestRecords appRequestRecords = Serializer.DeserializeFromPlayerPrefs<AppRequestRecords>("AppRequestRecords");
		if (appRequestRecords == null)
		{
			appRequestRecords = new AppRequestRecords();
		}
		foreach (string text in ids)
		{
			appRequestRecords.Add(text + request_type, DateTime.UtcNow);
		}
		Serializer.SerializeToPlayerPrefs(appRequestRecords, "AppRequestRecords");
	}

	public void sendAppRequest(AppRequestType request_type, List<Friend> friends)
	{
		if (friends.Count == 0)
		{
			return;
		}
		getProfile(delegate(Profile profile)
		{
			if (profile != null)
			{
				string text = string.Format(i18n.Get(request_type.ToString() + ".message"), profile.first_name);
				AppRequestData appRequestData = new AppRequestData(request_type, profile.id, profile.first_name);
				string[] array = new string[friends.Count];
				for (int i = 0; i != friends.Count; i++)
				{
					array[i] = friends[i].id;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["message"] = text;
				dictionary["data"] = appRequestData.toJSON();
				string data = appRequestData.toJSON();
				FB.AppRequest(text, array, null, null, null, data, string.Empty);
				recordAppRequests(request_type, array);
				Tracking.FBAppRequest(request_type);
			}
			else
			{
				Debug.LogWarning("FB: app request canceled, bacause user not logged in");
			}
		});
	}

	public void getAppRequests(Action<List<AppRequest>> onDone)
	{
		Login(delegate(bool success)
		{
			if (success)
			{
				FB.API("/me/apprequests", HttpMethod.GET, delegate(FBResult apprequests_json)
				{
					Debug.Log("App Requests: " + apprequests_json.Text);
					IDictionary dictionary = Json.Deserialize(apprequests_json.Text) as IDictionary;
					IList list = dictionary["data"] as IList;
					List<AppRequest> list2 = new List<AppRequest>();
					foreach (IDictionary item in list)
					{
						AppRequest appRequest = default(AppRequest);
						appRequest.id = item["id"] as string;
						appRequest.application_name = (item["application"] as IDictionary)["name"] as string;
						appRequest.application_namespace = (item["application"] as IDictionary)["namespace"] as string;
						appRequest.application_id = (item["application"] as IDictionary)["id"] as string;
						appRequest.to_id = (item["to"] as IDictionary)["id"] as string;
						appRequest.to_name = (item["to"] as IDictionary)["name"] as string;
						appRequest.message = item["message"] as string;
						DateTime.TryParse(item["created_time"] as string, out appRequest.created_time);
						appRequest.data = AppRequestData.fromJSON(item["data"] as string);
						if (appRequest.data.type == AppRequestType.UNKNOWN)
						{
							Debug.LogWarning("FB: discarding App-Request of unknown type:" + appRequest);
						}
						else
						{
							list2.Add(appRequest);
						}
					}
					onDone(list2);
				});
			}
			else
			{
				onDone(null);
			}
		});
	}

	public void deleteAppRequest(AppRequest app_requests, Action onDone)
	{
		Login(delegate(bool success)
		{
			if (success)
			{
				Debug.Log("deleting app request " + app_requests.id);
				FB.API("/" + app_requests.id, HttpMethod.DELETE, delegate(FBResult result_json)
				{
					Debug.Log(result_json.Text);
					onDone();
				});
			}
			else
			{
				Debug.LogWarning("FB: app request deletion canceled, bacause user not logged in");
				onDone();
			}
		});
	}

	public void deleteAppRequests(List<AppRequest> app_requests, Action onDone)
	{
		if (app_requests.Count == 0)
		{
			onDone();
			return;
		}
		Login(delegate(bool success)
		{
			if (success)
			{
				string[] array = new string[app_requests.Count];
				for (int i = 0; i != app_requests.Count; i++)
				{
					array[i] = app_requests[i].id;
				}
				FB.API("/?ids=" + string.Join(",", array), HttpMethod.DELETE, delegate(FBResult apprequests_json)
				{
					Debug.LogWarning(apprequests_json.Text);
					onDone();
				});
			}
			else
			{
				Debug.LogWarning("FB: mass app request deletion canceled, bacause user not logged in");
				onDone();
			}
		});
	}

	public void OpenInvitePopup()
	{
		string title = "FACEBOOK_INVITE_TITLE".Localize();
		FB.AppRequest("FACEBOOK_INVITE_MESSAGE".Localize(), null, null, null, null, string.Empty, title);
	}
}

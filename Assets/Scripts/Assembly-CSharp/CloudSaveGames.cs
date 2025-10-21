using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using LeanplumSDK;
using UnityEngine;
using dinobash;

public class CloudSaveGames : MonoBehaviour
{
	public class CloudSaveGame
	{
		public class Wallet
		{
			[XmlAttribute]
			public bool isPayingUser;

			[XmlAttribute]
			public int coins;

			[XmlAttribute]
			public int diamonds;

			[XmlAttribute]
			public float total_spent;
		}

		public FacebookManager.Profile profile;

		public PlayerData player_data;

		public Wallet wallet = new Wallet();

		[XmlAttribute]
		public string device_name = string.Empty;

		[XmlAttribute]
		public string device_model = string.Empty;
	}

	public class CloudSaveGameProgress
	{
		[XmlAttribute]
		public long timestamp;

		[XmlIgnore]
		public bool newer_savegame_in_cloud;

		[XmlIgnore]
		public bool older_savegame_in_cloud;

		[XmlAttribute]
		public int max_level;

		[XmlAttribute]
		public int coins;

		[XmlAttribute]
		public int diamonds;

		[XmlAttribute]
		public string device_name = string.Empty;

		[XmlAttribute]
		public string device_model = string.Empty;
	}

	private static readonly string CLOUD_HOST = "https://gameservices.dinobash.com";

	private Var<string> var_cloud_host = Var.Define("Cloud Host", CLOUD_HOST);

	public void setUserProfile(FacebookManager.Profile profile)
	{
		Serializer.SerializeToPlayerPrefs(profile, "cloud_save_user_profile");
	}

	private FacebookManager.Profile getUserProfile()
	{
		FacebookManager.Profile profile = Serializer.DeserializeFromPlayerPrefs<FacebookManager.Profile>("cloud_save_user_profile");
		if (profile == null || string.IsNullOrEmpty(profile.id))
		{
			return null;
		}
		return profile;
	}

	public void loadFromCloudAndApply(long timestamp, Action<bool> onDone)
	{
		loadFromCloud(delegate(CloudSaveGame csg)
		{
			if (csg != null)
			{
				Player.ApplyCloudSaveGame(csg);
				PlayerPrefs.SetString("cloud_save_ts", timestamp.ToString());
				PlayerPrefs.Save();
				onDone(true);
			}
			else
			{
				onDone(false);
			}
		});
	}

	private void loadFromCloud(Action<CloudSaveGame> onDone)
	{
		FacebookManager.Profile userProfile = getUserProfile();
		if (userProfile == null)
		{
			onDone(null);
		}
		else
		{
			StartCoroutine(FetchSaveGame(onDone, userProfile));
		}
	}

	private static string hash_url_safe(string s)
	{
		SHA256 sHA = SHA256.Create();
		byte[] inArray = sHA.ComputeHash(Encoding.UTF8.GetBytes(s));
		string text = Convert.ToBase64String(inArray).Trim();
		return text.Replace('+', '-').Replace('/', '_').Replace("=", string.Empty);
	}

	private string sign(string profile_id)
	{
		string text = (UnityEngine.Random.value * 10f).ToString().Replace(".", string.Empty);
		string text2 = hash_url_safe(getPassword() + profile_id + text);
		return "salt=" + text + "&sign=" + text2;
	}

	private IEnumerator FetchSaveGame(Action<CloudSaveGame> onDone, FacebookManager.Profile profile)
	{
		string url = var_cloud_host.Value + "/savegames/" + profile.id + "/?" + sign(profile.id);
		WWW www = new WWW(url);
		yield return www;
		if (www.responseHeaders["CONTENT-TYPE"] != "application/zip")
		{
			Debug.LogWarning("Cloud Save: unexpected content type");
			onDone(null);
			yield break;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log("Error while fetching savegame: " + www.error);
			onDone(null);
			yield break;
		}
		using (MemoryStream ms = new MemoryStream(www.bytes))
		{
			using (ZipFile zip = new ZipFile(ms))
			{
				zip.Password = getPassword();
				ZipEntry entry = zip.GetEntry("savegame.xml");
				if (entry == null || !entry.IsFile)
				{
					Debug.LogError("savegame.xml not found in zipfile or it is not file");
					onDone(null);
					yield break;
				}
				Stream stream = zip.GetInputStream(entry);
				using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
				{
					string xml = reader.ReadToEnd();
					CloudSaveGame csg = Serializer.DeserializeString<CloudSaveGame>(xml);
					if (csg.profile.id != profile.id)
					{
						Debug.LogError("Temper Detection: The id in the cloud savegame and the one in the players profile do not match. csg.profile.id=" + csg.profile.id + ", profile.id=" + profile.id);
						onDone(null);
					}
					else
					{
						onDone(csg);
					}
				}
			}
		}
	}

	public void checkAndPrompt(bool promt_for_loading = true)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return;
		}
		getSaveGameProgress(delegate(CloudSaveGameProgress progress)
		{
			if (progress != null && progress.newer_savegame_in_cloud && promt_for_loading)
			{
				if (App.State == App.States.Map || App.State == App.States.StartScreen)
				{
					NewCloudSaveAvailableScreen.ShowPrompt(progress);
				}
			}
			else if (progress != null && progress.older_savegame_in_cloud)
			{
				saveToCloud(delegate
				{
				});
			}
		});
	}

	private void getSaveGameProgress(Action<CloudSaveGameProgress> onDone)
	{
		FacebookManager.Profile userProfile = getUserProfile();
		if (userProfile == null)
		{
			if (onDone != null)
			{
				onDone(null);
			}
		}
		else
		{
			StartCoroutine(FetchSaveGameProgress(onDone, userProfile));
		}
	}

	private IEnumerator FetchSaveGameProgress(Action<CloudSaveGameProgress> onDone, FacebookManager.Profile profile)
	{
		WWW www = new WWW(var_cloud_host.Value + "/savegames/" + profile.id + "/progress/?" + sign(profile.id));
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log("Error while fetching savegame-progress: " + www.error);
			if (onDone != null)
			{
				onDone(null);
			}
		}
		else if (www.responseHeaders["CONTENT-TYPE"] != "application/xml")
		{
			Debug.LogWarning("Cloud Save Progress: no new data");
			if (onDone != null)
			{
				onDone(null);
			}
		}
		else
		{
			CloudSaveGameProgress progress = Serializer.DeserializeString<CloudSaveGameProgress>(www.text);
			long local_ts = long.Parse(PlayerPrefs.GetString("cloud_save_ts", "0"));
			progress.newer_savegame_in_cloud = progress.timestamp > local_ts;
			progress.older_savegame_in_cloud = progress.timestamp <= local_ts;
			onDone(progress);
		}
	}

	public void saveToCloud(Action<int?> onDone)
	{
		FacebookManager.Profile userProfile = getUserProfile();
		if (userProfile == null)
		{
			onDone(null);
			return;
		}
		CloudSaveGame cloudSaveGame = new CloudSaveGame();
		cloudSaveGame.profile = userProfile;
		cloudSaveGame.device_name = SystemInfo.deviceModel;
		cloudSaveGame.device_model = SystemInfo.deviceModel;
		if (Player.Instance == null || Player.Instance.PlayerData == null)
		{
			onDone(null);
			return;
		}
		cloudSaveGame.player_data = Player.Instance.PlayerData;
		cloudSaveGame.wallet.coins = Wallet.Coins;
		cloudSaveGame.wallet.diamonds = Wallet.Diamonds;
		cloudSaveGame.wallet.isPayingUser = Wallet.IsPayingUser;
		cloudSaveGame.wallet.total_spent = Wallet.Total_spent;
		string s = Serializer.SerializeToString(cloudSaveGame);
		MemoryStream memoryStream = new MemoryStream();
		ZipOutputStream zipOutputStream = new ZipOutputStream(memoryStream);
		zipOutputStream.Password = getPassword();
		zipOutputStream.SetLevel(9);
		ZipEntry zipEntry = new ZipEntry("savegame.xml");
		zipEntry.DateTime = DateTime.Now;
		zipOutputStream.PutNextEntry(zipEntry);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		zipOutputStream.Write(bytes, 0, bytes.Length);
		zipOutputStream.CloseEntry();
		zipOutputStream.IsStreamOwner = false;
		zipOutputStream.Close();
		StartCoroutine(Upload(memoryStream.ToArray(), cloudSaveGame.profile.id, onDone));
	}

	private IEnumerator Upload(byte[] bytes, string facebook_id, Action<int?> onDone)
	{
		WWWForm form = new WWWForm();
		form.AddBinaryData("file", bytes);
		WWW www = new WWW(var_cloud_host.Value + "/savegames/" + facebook_id + "/?" + sign(facebook_id), form);
		yield return www;
		int server_side_ts = 0;
		if (string.IsNullOrEmpty(www.error) && int.TryParse(www.text, out server_side_ts))
		{
			onDone(server_side_ts);
			PlayerPrefs.SetString("cloud_save_ts", server_side_ts.ToString());
			yield break;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogWarning("CloudSave::Upload: " + www.error);
		}
		onDone(null);
	}

	private string getPassword()
	{
		return "eAAVAA0ACQAaADsAOAB+AD4AEgA9ACQAHgBBAGAAMQAMACsACwA8ADkANwAjABYAWgA5AAIAeQAMADgAfAAxAAMAXQAsAFIAUwAHACAAHAA=".DecodeFrom64().EncryptOrDecrypt("0dcDWOHLIFOwn13UAIrqJtVGcHeOXz1rIjK04dAH");
	}

	private void Awake()
	{
		base.gameObject.name = "Cloud Save Manager";
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}
}

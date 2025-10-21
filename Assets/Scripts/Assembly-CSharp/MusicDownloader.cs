using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class MusicDownloader : MonoBehaviour
{
	public static IEnumerator GetMusic(Music music, Action<AudioClip> onDone)
	{
		if (IsDownloaded(music))
		{
			WWW www = new WWW("file://" + GetCacheLocation(music));
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(string.Concat("Corrupted cache file: ", music, ", error: ", www.error));
				onDone(GetFallbackMusic(music));
			}
			else
			{
				AudioClip ac = www.GetAudioClip(false);
				if (ac == null)
				{
					Debug.LogError("Cached file is not an audio clip: " + music);
					onDone(GetFallbackMusic(music));
				}
				else
				{
					onDone(ac);
				}
			}
		}
		else
		{
			onDone(GetFallbackMusic(music));
		}
	}

	private static AudioClip GetFallbackMusic(Music music)
	{
		AudioClip audioClip = Resources.Load("Music/" + music, typeof(AudioClip)) as AudioClip;
		if ((bool)audioClip)
		{
			return audioClip;
		}
		return Resources.Load("Music/" + Music.theme_hills, typeof(AudioClip)) as AudioClip;
	}

	private void Start()
	{
		StartCoroutine(DownloadClips());
	}

	private IEnumerator DownloadClips()
	{
		yield return StartCoroutine(DownloadMusic(Music.theme_desert));
		yield return StartCoroutine(DownloadMusic(Music.theme_ice));
		yield return StartCoroutine(DownloadMusic(Music.theme_volcano));
	}

	private IEnumerator DownloadMusic(Music music)
	{
		while (!IsDownloaded(music))
		{
			WWW www = new WWW(GetDownloadUrl(music));
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogWarning("Failed to download: " + www.url + "error: " + www.error);
				yield return new WaitForSeconds(30f);
				continue;
			}
			AudioClip ac = www.GetAudioClip();
			if (ac == null)
			{
				Debug.LogError("Not And Audio Clip: " + music);
				yield return new WaitForSeconds(30f);
			}
			else
			{
				File.WriteAllBytes(GetCacheLocation(music), www.bytes);
			}
		}
		Debug.Log(string.Concat(">>> download of ", music, " complete."));
	}

	private static string GetDownloadUrl(Music music)
	{
		return "https://s3.amazonaws.com/db-assets/music/" + music.ToString() + ".mp3";
	}

	private static bool IsDownloaded(Music music)
	{
		FileInfo fileInfo = new FileInfo(GetCacheLocation(music));
		return fileInfo != null && fileInfo.Exists;
	}

	private static string GetCacheLocation(Music music)
	{
		return Application.persistentDataPath + "/" + music.ToString() + ".mp3";
	}
}

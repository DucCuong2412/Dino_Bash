using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBase
{
	private const int gameSpeakerCount = 16;

	private const float Max_Sound_Volume = 1f;

	private const float Max_Music_Volume = 0.6f;

	private const string music_volume_key = "MusicVolume";

	private const string sfx_volume_key = "SFXVolume";

	private static AudioPlayer instance;

	public int LoadedAudioClips;

	private AudioSource gui_speaker;

	private AudioSource music_speaker;

	private List<AudioSource> game_sources = new List<AudioSource>();

	private static float sfx_volume;

	private static float music_volume;

	public static bool IsAdPlaying;

	private IEnumerator change_music_routine;

	private bool isMusicFading;

	public static float SFXVolume
	{
		get
		{
			return sfx_volume;
		}
		set
		{
			sfx_volume = Mathf.Clamp01(value);
			instance.gui_speaker.volume = sfx_volume;
		}
	}

	public static float MusicVolume
	{
		get
		{
			return music_volume;
		}
		set
		{
			music_volume = Mathf.Clamp01(value);
			instance.music_speaker.volume = music_volume;
		}
	}

	public static void ToggleMusic()
	{
		MusicVolume = ((MusicVolume != 0f) ? 0f : 0.6f);
		PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
	}

	public static void ToggleSound()
	{
		SFXVolume = ((SFXVolume != 0f) ? 0f : 1f);
		PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
	}

	private void Awake()
	{
		instance = this;
		gui_speaker = base.gameObject.AddComponent<AudioSource>();
		SFXVolume = 1f;
		if (PlayerPrefs.HasKey("SFXVolume"))
		{
			SFXVolume = Mathf.Min(1f, PlayerPrefs.GetFloat("SFXVolume"));
		}
		music_speaker = base.gameObject.AddComponent<AudioSource>();
		music_speaker.loop = true;
		MusicVolume = 0.6f;
		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			MusicVolume = Mathf.Min(0.6f, PlayerPrefs.GetFloat("MusicVolume"));
		}
		StartCoroutine(UserMusicChecker());
	}

	private void Update()
	{
		if (game_sources.Count > 0)
		{
			List<AudioSource> list = game_sources.FindAll((AudioSource x) => !x.isPlaying);
			list.ForEach(delegate(AudioSource x)
			{
				x.clip = null;
				game_sources.Remove(x);
			});
		}
	}

	private void OnLevelWasLoaded()
	{
		game_sources.RemoveAll((AudioSource x) => x == null);
	}

	public static void Setup3DAudioSource(AudioSource source)
	{
		source.maxDistance = 8192f;
		source.dopplerLevel = 0f;
		source.rolloffMode = AudioRolloffMode.Custom;
		source.panLevel = 0.5f;
	}

	public static void PlayGameSFX(AudioSource source, AudioResources.SFX sfx, bool forcePlay = false)
	{
		if ((isInViewport(source) && instance.game_sources.Count < 16) || forcePlay)
		{
			AudioClip clip = AudioResources.GetClip(sfx.Path);
			source.volume = sfx.Volume * SFXVolume;
			source.clip = clip;
			source.Play();
			instance.game_sources.Add(source);
		}
	}

	private static bool isInViewport(Component c)
	{
		return Mathf.Abs(Camera.main.WorldToViewportPoint(c.transform.position).x) < 1.5f;
	}

	public static void PlayGuiSFX(AudioResources.SFX sfx, float delay = 0f, float pitch = 1f)
	{
		AudioClip clip = AudioResources.GetClip(sfx.Path);
		PlayGUISFX(clip, sfx.Volume, delay, pitch);
	}

	public static void PlayGuiSFX(Sounds sound, float delay = 0f, float pitch = 1f)
	{
		AudioResources.SFX sFX = AudioResources.GetSFX(sound);
		AudioClip clip = AudioResources.GetClip(sFX.Path);
		if (clip == null)
		{
			Debug.LogError("clip is null: " + sFX.Path);
		}
		PlayGUISFX(clip, sFX.Volume, delay, pitch);
	}

	private static void PlayGUISFX(AudioClip clip, float volume = 1f, float delay = 0f, float pitch = 1f)
	{
		instance.WaitThen(delay, delegate
		{
			instance.gui_speaker.volume = SFXVolume * volume;
			instance.gui_speaker.pitch = pitch;
			instance.gui_speaker.PlayOneShot(clip);
		});
	}

	public static void FadeTo(AudioSource source, float volume, float fade_time = 1f)
	{
		instance.StartCoroutine(instance.ChangeVolume(source, volume, fade_time));
	}

	private static bool IsUserMusicPlaying()
	{
		return false;
	}

	private static bool IsUserMusicOrAdVideoPlaying()
	{
		return IsUserMusicPlaying() || IsAdPlaying;
	}

	private IEnumerator UserMusicChecker()
	{
		while (instance != null)
		{
			bool userMusicPlaying = IsUserMusicOrAdVideoPlaying();
			if (!isMusicFading && instance.music_speaker.volume > 0f && userMusicPlaying)
			{
				SetMusicVolume(0f);
			}
			else if (!isMusicFading && instance.music_speaker.volume < 0.6f && (double)MusicVolume > 0.0 && !userMusicPlaying)
			{
				SetMusicVolume(MusicVolume);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public static Music GetTheme(int level_id)
	{
		ChapterData chapterData = Konfiguration.GetChapterData(level_id);
		switch (chapterData.theme)
		{
		case LevelTheme.map00:
			return Music.theme_hills;
		case LevelTheme.map01:
			return Music.theme_hills;
		case LevelTheme.map02:
			return Music.theme_volcano;
		case LevelTheme.map03:
			return Music.theme_ice;
		case LevelTheme.map04:
			return Music.theme_desert;
		default:
			return Music.theme_hills;
		}
	}

	public static void PlayMusic(Music music, bool loop = true)
	{
		App.Instance.StartCoroutine(MusicDownloader.GetMusic(music, delegate(AudioClip clip)
		{
			Debug.Log("Music: " + music);
			if (instance.change_music_routine != null)
			{
				instance.StopCoroutine(instance.change_music_routine);
			}
			instance.music_speaker.Stop();
			instance.music_speaker.volume = 0f;
			if (instance.music_speaker.clip != clip)
			{
				Resources.UnloadAsset(instance.music_speaker.clip);
				instance.music_speaker.clip = clip;
				instance.music_speaker.loop = loop;
			}
			instance.music_speaker.Play();
			SetMusicVolume(MusicVolume);
		}));
	}

	public static void StopMusic()
	{
		SetMusicVolume(0f);
	}

	public static void SetMusicVolume(float volume)
	{
		if (instance.change_music_routine != null)
		{
			instance.StopCoroutine(instance.change_music_routine);
		}
		instance.change_music_routine = instance.ChangeVolume(instance.music_speaker, volume, 0.5f);
		instance.StartCoroutine(instance.change_music_routine);
	}

	private IEnumerator ChangeVolume(AudioSource source, float targetVolume, float duration, Action callback = null)
	{
		isMusicFading = true;
		float start_volume = source.volume;
		targetVolume = Mathf.Clamp01(targetVolume);
		float time = duration;
		while (time > 0f)
		{
			float progress = 1f - time / duration;
			source.volume = Mathf.Lerp(start_volume, targetVolume, progress);
			time -= Time.deltaTime;
			yield return null;
		}
		source.volume = targetVolume;
		if (callback != null)
		{
			callback();
		}
		isMusicFading = false;
	}
}

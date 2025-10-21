using System;
using UnityEngine;

public class CutSceneScreen : MonoBase
{
	public Action onIntroEnd;

	public Music music;

	private HudScreen hud_screen;

	public static void PlayCutscene(string cutscene_path, Action callback)
	{
		CutSceneScreen cutSceneScreen = UnityEngine.Object.Instantiate(Resources.Load<CutSceneScreen>(cutscene_path)) as CutSceneScreen;
		cutSceneScreen.onIntroEnd = callback;
	}

	private void Start()
	{
		if (App.State == App.States.Game)
		{
			hud_screen = ScreenManager.GetScreen<HudScreen>();
			hud_screen.gameObject.SetActive(false);
		}
		LocalizedText[] componentsInChildren = GetComponentsInChildren<LocalizedText>(true);
		Array.ForEach(componentsInChildren, delegate(LocalizedText x)
		{
			x.Start();
		});
		AudioPlayer.PlayMusic(music, false);
		StandardButton skip_button = base.transform.Search("btn_skip").GetComponent<StandardButton>();
		skip_button.uiItem.OnClick += delegate
		{
			skip_button.uiItem.enabled = false;
			GetComponent<Animator>().enabled = false;
			base.transform.Search("zfiller").renderer.enabled = false;
			float duration = 0.3f;
			SpriteRenderer component = base.transform.Search("schwarze_balken_oben").GetComponent<SpriteRenderer>();
			SpriteRenderer component2 = base.transform.Search("schwarze_balken_unten").GetComponent<SpriteRenderer>();
			Go.to(component.transform, duration, new GoTweenConfig().localPosition(component.transform.localPosition.SetY(1000f)).setEaseType(GoEaseType.CubicOut));
			Go.to(component2.transform, duration, new GoTweenConfig().localPosition(component.transform.localPosition.SetY(-1000f)).setEaseType(GoEaseType.CubicOut));
			GoTweenConfig config = new GoTweenConfig().colorProp("color", Colors.Invisible);
			Renderer[] componentsInChildren2 = GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren2;
			foreach (Renderer renderer in array)
			{
				if (renderer.GetComponent<tk2dTextMesh>() != null)
				{
					renderer.enabled = false;
				}
				else
				{
					ParticleSystem component3 = renderer.GetComponent<ParticleSystem>();
					if (component3 != null)
					{
						component3.enableEmission = false;
						ParticleSystem.Particle[] array2 = new ParticleSystem.Particle[component3.particleCount];
						component3.GetParticles(array2);
						Array.ForEach(array2, delegate(ParticleSystem.Particle p)
						{
							p.lifetime = duration;
						});
						component3.SetParticles(array2, array2.Length);
					}
					else if (renderer != component && renderer != component2)
					{
						Go.to(renderer, duration, config);
					}
				}
			}
			OnAnimationEnd();
		};
	}

	private void OnAnimationEnd()
	{
		if (hud_screen != null)
		{
			hud_screen.gameObject.SetActive(true);
		}
		if (onIntroEnd != null)
		{
			onIntroEnd();
		}
		if (App.State == App.States.Game)
		{
			AudioPlayer.PlayMusic(AudioPlayer.GetTheme(Level.Instance.levelid));
		}
		else
		{
			AudioPlayer.PlayMusic(Music.map);
		}
		WaitThen(0.3f, delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		});
	}
}

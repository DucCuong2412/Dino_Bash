using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class AudioResources
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct SFX
	{
		public string Path { get; private set; }

		public float Volume { get; private set; }

		public SFX(string path, float volume = 1f)
		{
			Path = path;
			Volume = volume;
		}
	}

	private static Dictionary<UnitType, Dictionary<EntitySound, SFX>> entitiy_sounds = new Dictionary<UnitType, Dictionary<EntitySound, SFX>>
	{
		{
			UnitType.DinoEgg,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/Human Hit Egg 1")
				},
				{
					EntitySound.Die2,
					new SFX("GameSounds/Human Hit Egg 2")
				}
			}
		},
		{
			UnitType.Raptor,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_dinoDies01")
				},
				{
					EntitySound.Die2,
					new SFX("GameSounds/dinobash_SFX_game_dinoDies03")
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/Dino A Attack")
				},
				{
					EntitySound.Attack2,
					new SFX("GameSounds/Dino B Attack 2")
				},
				{
					EntitySound.Attack3,
					new SFX("GameSounds/Dino B Attack 3")
				}
			}
		},
		{
			UnitType.Spitty,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_dinoDies05")
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/dinobash_SFX_game_DINO_fight_Spitty")
				}
			}
		},
		{
			UnitType.Stego,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_dinoDies04")
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/dinobash_SFX_game_DINO_fight_stego", 0.75f)
				}
			}
		},
		{
			UnitType.Rambo,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Spawn,
					new SFX("dinobash_SFX_game_DINO_RamboLONG")
				},
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_dinoDies06")
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/Dino B Attack 3")
				}
			}
		},
		{
			UnitType.Tricer,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_dinoDies04")
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/Dino B Attack 2")
				}
			}
		},
		{
			UnitType.TRex,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_dinoDies04")
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/dinobash_SFX_game_DINO_fight_TREX")
				}
			}
		},
		{
			UnitType.Brachio,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_dinoDies02")
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/dinobash_SFX_game_DINO_fight_Brachio", 3f)
				}
			}
		},
		{
			UnitType.Neander_Weak,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_01", 0.8f)
				},
				{
					EntitySound.Die2,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_02", 0.8f)
				},
				{
					EntitySound.Die3,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_03", 0.8f)
				},
				{
					EntitySound.Die4,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_04", 0.8f)
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/Human Attack 1")
				},
				{
					EntitySound.Attack2,
					new SFX("GameSounds/Human Attack 2")
				},
				{
					EntitySound.Attack3,
					new SFX("GameSounds/Human Attack 3")
				},
				{
					EntitySound.Attack4,
					new SFX("GameSounds/Human Attack 4")
				}
			}
		},
		{
			UnitType.Neander_Shooter,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_05", 0.8f)
				},
				{
					EntitySound.Die2,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_06", 0.8f)
				},
				{
					EntitySound.Die3,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_07", 0.8f)
				},
				{
					EntitySound.Die4,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_08", 0.8f)
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/Human Attack 1")
				},
				{
					EntitySound.Attack2,
					new SFX("GameSounds/Human Attack 2")
				},
				{
					EntitySound.Attack3,
					new SFX("GameSounds/Human Attack 3")
				},
				{
					EntitySound.Attack4,
					new SFX("GameSounds/Human Attack 4")
				}
			}
		},
		{
			UnitType.PoisonTrap,
			new Dictionary<EntitySound, SFX> { 
			{
				EntitySound.Attack1,
				new SFX("GameSounds/dinobash_SFX_game_PoisonTrap")
			} }
		},
		{
			UnitType.Neander_Healer,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Spawn,
					new SFX("dinobash_SFX_game_Human_Healer")
				},
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_09", 0.8f)
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/dinobash_SFX_game_Human_Healing")
				}
			}
		},
		{
			UnitType.Neander_Bush,
			new Dictionary<EntitySound, SFX> { 
			{
				EntitySound.Die1,
				new SFX("GameSounds/dinobash_SFX_Bush_die")
			} }
		},
		{
			UnitType.Neander_Tank,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Spawn,
					new SFX("dinobash_SFX_game_Human_Opera")
				},
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinobash_SFX_game_Human_Dies_07", 0.8f)
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/Human Attack 1")
				},
				{
					EntitySound.Attack3,
					new SFX("GameSounds/Human Attack 3")
				}
			}
		},
		{
			UnitType.Neander_Catapult,
			new Dictionary<EntitySound, SFX>
			{
				{
					EntitySound.Die1,
					new SFX("GameSounds/dinoBash_SFX_catapult_kaputt", 0.8f)
				},
				{
					EntitySound.Attack1,
					new SFX("GameSounds/dinoBash_SFX_catapult_feuert")
				},
				{
					EntitySound.Move,
					new SFX("GameSounds/dinoBash_SFX_catapult_faehrt")
				}
			}
		}
	};

	public static Dictionary<Sounds, SFX> game_sounds = new Dictionary<Sounds, SFX>
	{
		{
			Sounds.main_loader_in,
			new SFX("dinobash_SFX_menu_StartLoadingScreen")
		},
		{
			Sounds.main_close_popup,
			new SFX("main_close_popup")
		},
		{
			Sounds.main_play_button,
			new SFX("main_play_button")
		},
		{
			Sounds.main_continue_button,
			new SFX("continue_button")
		},
		{
			Sounds.main_snappy_select,
			new SFX("snappy_select")
		},
		{
			Sounds.main_raptor_select,
			new SFX("raptor_select")
		},
		{
			Sounds.main_spitty_select,
			new SFX("spitty_select")
		},
		{
			Sounds.main_tricer_select,
			new SFX("tricer_select")
		},
		{
			Sounds.main_rambo_select,
			new SFX("dinobash_SFX_game_DINO_RamboSHORT")
		},
		{
			Sounds.main_stego_select,
			new SFX("stego_select")
		},
		{
			Sounds.main_trex_select,
			new SFX("trex_select")
		},
		{
			Sounds.main_trex_jr_select,
			new SFX("dinobash_SFX_game_DINO_TrexJr")
		},
		{
			Sounds.main_brachio_select,
			new SFX("tricer_select")
		},
		{
			Sounds.main_dino_unselect,
			new SFX("dino_unselect")
		},
		{
			Sounds.main_get_coins,
			new SFX("dinobash_SFX_menu_GETcoins")
		},
		{
			Sounds.main_get_diamonds,
			new SFX("dinobash_SFX_menu_GETdimonds")
		},
		{
			Sounds.main_buy_coins,
			new SFX("dinobash_SFX_menu_BUYcoins")
		},
		{
			Sounds.main_buy_diamods,
			new SFX("dinobash_SFX_menu_BUYdimonds")
		},
		{
			Sounds.main_get_lifes,
			new SFX("dinobash_SFX_menu_GETLife")
		},
		{
			Sounds.map_upgrade_btn,
			new SFX("upgrade_button")
		},
		{
			Sounds.main_upgrade,
			new SFX("dinoBash_SFX_DINO_Upgrade")
		},
		{
			Sounds.map_waypoint,
			new SFX("waypoint_select")
		},
		{
			Sounds.map_shot_unselect,
			new SFX("shot_unselect")
		},
		{
			Sounds.map_dino_unselect,
			new SFX("dino_unselect")
		},
		{
			Sounds.map_shot_select_ice,
			new SFX("dinobash_SFX_menu_SelectShotsICE")
		},
		{
			Sounds.map_shot_select_triple,
			new SFX("dinobash_SFX_menu_SelectShotsTRIPPLE")
		},
		{
			Sounds.map_shot_select_bomb,
			new SFX("dinobash_SFX_menu_SelectShotsVULCANO")
		},
		{
			Sounds.map_shot_select_standard,
			new SFX("dinobash_SFX_menu_SelectShotsROCK")
		},
		{
			Sounds.map_shot_select_poison,
			new SFX("dinobash_SFX_menu_SelectShotsPOISEN")
		},
		{
			Sounds.map_shot_select_heal,
			new SFX("dinobash_SFX_menu_SelectShotsHEAL")
		},
		{
			Sounds.map_shot_select_icebomb,
			new SFX("dinobash_SFX_menu_SelectShotsICE")
		},
		{
			Sounds.map_shot_select_wobble,
			new SFX("dinobash_SFX_menu_SelectShotsROCK")
		},
		{
			Sounds.map_shot_select_rocket,
			new SFX("dinobash_SFX_menu_SelectShotsVULCANO")
		},
		{
			Sounds.game_start_horn,
			new SFX("dinobash_SFX_game_gameStart")
		},
		{
			Sounds.game_finalwave_horn,
			new SFX("dinobash_SFX_game_finalWave_horn")
		},
		{
			Sounds.game_lost_music,
			new SFX("game_lost_music")
		},
		{
			Sounds.game_won_music,
			new SFX("game_won_music")
		},
		{
			Sounds.game_apple_spawn,
			new SFX("Apple Appear")
		},
		{
			Sounds.game_apple_collect,
			new SFX("Click on Apple")
		},
		{
			Sounds.bird_fly,
			new SFX("Bird wing flaps loop")
		},
		{
			Sounds.shoot_drop_shot,
			new SFX("dinobash_SFX_game_Shot_dropRANDOM")
		},
		{
			Sounds.shoot_miss,
			new SFX("shot_miss_hitground")
		},
		{
			Sounds.shoot_select,
			new SFX("shot_select")
		},
		{
			Sounds.shot_bomb_hit,
			new SFX("dinobash_SFX_game_Shot_TreffferVULCANO")
		},
		{
			Sounds.shot_frost_hit,
			new SFX("dinobash_SFX_game_Shot_TreffferICE")
		},
		{
			Sounds.shot_poison_hit,
			new SFX("dinobash_SFX_game_Shot_TreffferPOISON")
		},
		{
			Sounds.shot_tripple_hit,
			new SFX("dinobash_SFX_game_Shot_TreffferTRIPPLE")
		},
		{
			Sounds.shot_generic_hit,
			new SFX("dinobash_SFX_game_Shot_TreffferRANDOM")
		},
		{
			Sounds.shot_wobble_hit,
			new SFX("dinobash_SFX_game_Shot_TreffferHUNK")
		},
		{
			Sounds.shot_heal_hit,
			new SFX("dinobash_SFX_menu_SelectShotsHEAL")
		},
		{
			Sounds.shot_hit_human1,
			new SFX("Shot Hit Human 1")
		},
		{
			Sounds.shot_hit_human2,
			new SFX("Shot Hit Human 2")
		},
		{
			Sounds.unlock_dino,
			new SFX("dinobash_SFX_menu_UnlockDINO")
		},
		{
			Sounds.unlock_shot,
			new SFX("dinobash_SFX_menu_UnlockSHOT")
		},
		{
			Sounds.megaball,
			new SFX("dinobash_SFX_game_Shot_TreffferBIGROLL")
		},
		{
			Sounds.meteorstorm,
			new SFX("dinobash_SFX_game_Shot_dropRANDOM")
		},
		{
			Sounds.blizzard,
			new SFX("dinobash_SFX_game_thunder")
		}
	};

	private static Dictionary<string, AudioClip> game_audio = new Dictionary<string, AudioClip>();

	public static int LoadedAudioClips
	{
		get
		{
			return game_audio.Count;
		}
	}

	private static UnitType EntitiySoundMapping(UnitType entitiy)
	{
		if (entitiy_sounds.ContainsKey(entitiy))
		{
			return entitiy;
		}
		switch (entitiy)
		{
		case UnitType.TRex_Jr:
			return UnitType.TRex;
		case UnitType.Rocky:
			return UnitType.Rambo;
		case UnitType.Neander_Tank_Strong:
		case UnitType.Neander_Catapult:
			return UnitType.Neander_Tank;
		case UnitType.Neander_FrontShield:
		case UnitType.Neander_ShotShield:
		case UnitType.Neander_ShotShield_Fast:
		case UnitType.Neander_Shooter_Strong:
		case UnitType.Neander_Shooter_Heavy:
		case UnitType.Neander_Shooter_Fire:
		case UnitType.Neander_Flyer:
		case UnitType.Neander_Flyer_Fire:
			return UnitType.Neander_Shooter;
		case UnitType.Frosty:
			return UnitType.Stego;
		case UnitType.SmallBarricade:
		case UnitType.Stone:
		case UnitType.Dynamite:
		case UnitType.DynamiteSmall:
		case UnitType.Blizzard:
		case UnitType.MeteorStorm:
			return UnitType.None;
		default:
			return (!Konfiguration.isNeander(entitiy)) ? UnitType.Raptor : UnitType.Neander_Weak;
		}
	}

	public static Sounds[] GetShotSounds(ShotType shot)
	{
		switch (shot)
		{
		case ShotType.Normal:
			return new Sounds[2]
			{
				Sounds.map_shot_select_standard,
				Sounds.shot_generic_hit
			};
		case ShotType.Bomb:
			return new Sounds[2]
			{
				Sounds.map_shot_select_bomb,
				Sounds.shot_bomb_hit
			};
		case ShotType.Heal:
			return new Sounds[2]
			{
				Sounds.map_shot_select_heal,
				Sounds.shot_heal_hit
			};
		case ShotType.Ice:
			return new Sounds[2]
			{
				Sounds.map_shot_select_ice,
				Sounds.shot_frost_hit
			};
		case ShotType.IceBomb:
			return new Sounds[2]
			{
				Sounds.map_shot_select_icebomb,
				Sounds.shot_frost_hit
			};
		case ShotType.Poison:
			return new Sounds[2]
			{
				Sounds.map_shot_select_bomb,
				Sounds.shot_poison_hit
			};
		case ShotType.Wobble:
			return new Sounds[2]
			{
				Sounds.map_shot_select_wobble,
				Sounds.shot_wobble_hit
			};
		case ShotType.DoubleHunk:
			return new Sounds[2]
			{
				Sounds.map_shot_select_wobble,
				Sounds.shot_wobble_hit
			};
		case ShotType.Rocket:
			return new Sounds[2]
			{
				Sounds.map_shot_select_rocket,
				Sounds.shot_generic_hit
			};
		case ShotType.Triple:
			return new Sounds[2]
			{
				Sounds.map_shot_select_triple,
				Sounds.shot_generic_hit
			};
		case ShotType.Meteor:
			return new Sounds[2]
			{
				Sounds.map_shot_select_triple,
				Sounds.shot_bomb_hit
			};
		default:
			Debug.Log("no sound found for " + shot);
			return null;
		}
	}

	public static Sounds GetDinoSelectSound(UnitType dino)
	{
		switch (dino)
		{
		case UnitType.Raptor:
			return Sounds.main_raptor_select;
		case UnitType.Snappy:
			return Sounds.main_snappy_select;
		case UnitType.Spitty:
			return Sounds.main_spitty_select;
		case UnitType.Tricer:
			return Sounds.main_tricer_select;
		case UnitType.Rambo:
			return Sounds.main_rambo_select;
		case UnitType.Stego:
			return Sounds.main_stego_select;
		case UnitType.TRex:
			return Sounds.main_trex_select;
		case UnitType.TRex_Jr:
			return Sounds.main_trex_jr_select;
		case UnitType.Brachio:
			return Sounds.main_brachio_select;
		case UnitType.MegaBall:
			return Sounds.megaball;
		case UnitType.Blizzard:
			return Sounds.blizzard;
		case UnitType.MeteorStorm:
			return Sounds.meteorstorm;
		default:
			Debug.Log("no sound found for " + dino);
			return Sounds.main_raptor_select;
		}
	}

	public static void LoadSoundClips(Sounds start, Sounds end)
	{
		for (int i = (int)(start + 1); i < (int)end; i++)
		{
			Sounds key = (Sounds)i;
			if (game_sounds.ContainsKey(key))
			{
				LoadClip(game_sounds[key].Path);
			}
		}
	}

	public static void UnLoadSoundClips(Sounds start, Sounds end)
	{
		for (int i = (int)(start + 1); i < (int)end; i++)
		{
			Sounds key = (Sounds)i;
			if (game_sounds.ContainsKey(key))
			{
				UnloadClip(game_sounds[key].Path);
			}
		}
	}

	private static AudioClip LoadClip(string path)
	{
		if (!game_audio.ContainsKey(path))
		{
			AudioClip value = Resources.Load("SFX/" + path, typeof(AudioClip)) as AudioClip;
			game_audio.Add(path, value);
		}
		return game_audio[path];
	}

	public static void UnloadClip(string path)
	{
		if (game_audio.ContainsKey(path))
		{
			Resources.UnloadAsset(game_audio[path]);
			game_audio.Remove(path);
		}
	}

	public static SFX GetSFX(Sounds sound)
	{
		if (game_sounds.ContainsKey(sound))
		{
			return game_sounds[sound];
		}
		throw new Exception("No sound path defined: " + sound);
	}

	public static AudioClip GetClip(string key)
	{
		if (game_audio.ContainsKey(key))
		{
			return game_audio[key];
		}
		return null;
	}

	public static Dictionary<EntitySound, SFX> LoadAndGetEntitiySounds(UnitType entitiy)
	{
		UnitType unitType = EntitiySoundMapping(entitiy);
		if (unitType == UnitType.None)
		{
			return new Dictionary<EntitySound, SFX>();
		}
		if (entitiy_sounds.ContainsKey(unitType))
		{
			Dictionary<EntitySound, SFX> dictionary = entitiy_sounds[unitType];
			{
				foreach (SFX value in dictionary.Values)
				{
					LoadClip(value.Path);
				}
				return dictionary;
			}
		}
		Debug.Log("No sound paths found for entitiy: " + entitiy);
		return null;
	}

	public static void UnloadEntitiySounds()
	{
		foreach (int value in Enum.GetValues(typeof(UnitType)))
		{
			if (!entitiy_sounds.ContainsKey((UnitType)value))
			{
				continue;
			}
			Dictionary<EntitySound, SFX> dictionary = entitiy_sounds[(UnitType)value];
			foreach (SFX value2 in dictionary.Values)
			{
				UnloadClip(value2.Path);
			}
		}
	}
}

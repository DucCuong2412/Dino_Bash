using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfoContainer : MonoBase
{
	[Flags]
	private enum Attribute
	{
		None = 0,
		attack = 1,
		health = 2,
		defense = 4
	}

	private Transform attackAttribute;

	private Transform healthAttribute;

	private Transform emptyAttribute;

	private Attribute attributes;

	private DinoShotUpgradeAdapter adapter;

	private List<tk2dTextMesh> labels = new List<tk2dTextMesh>();

	private SpriteRenderer[] sprites;

	private bool init;

	private float left_entires_position;

	private float to_center_offset = 80f;

	private Dictionary<Transform, Transform> disabled_entries = new Dictionary<Transform, Transform>();

	private List<Action<float>> labelsToUpdate = new List<Action<float>>();

	public static InfoContainer Load(Transform target = null)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("GUI/InfoContainer")) as GameObject;
		if (target != null)
		{
			gameObject.transform.RepositionAndReparent(target);
		}
		return gameObject.GetComponent<InfoContainer>();
	}

	public void Set(UnitType entity, int sortingLayer, bool showLevelDifference = true, bool animate = false, bool showAttributeNames = false)
	{
		if (Konfiguration.isDinoUnit(entity))
		{
			adapter = new DinoAdapter(entity);
			attributes = Attribute.attack | Attribute.health;
		}
		else
		{
			attributes = Attribute.None;
		}
		Set(showLevelDifference, sortingLayer, animate, showAttributeNames);
	}

	public void Set(ShotType shot, int sortingLayer, bool showLevelDifference = true, bool animate = false, bool showAttributeNames = false)
	{
		if (shot != ShotType.None)
		{
			if (Konfiguration.ShotData[shot].damage > 0)
			{
				attributes = Attribute.attack;
			}
			else
			{
				attributes = Attribute.health;
			}
			adapter = new ShotAdapter(shot);
		}
		else
		{
			attributes = Attribute.None;
		}
		Set(showLevelDifference, sortingLayer, animate, showAttributeNames);
	}

	private void Set(bool showLevelDifference, int sortingLayer, bool animate, bool showAttributeNames)
	{
		if (!init)
		{
			attackAttribute = base.transform.Find("attribute_attack");
			healthAttribute = base.transform.Find("attribute_health");
			emptyAttribute = base.transform.Find("attribute_empty");
			labels = GetComponentsInChildren<tk2dTextMesh>(true).ToList();
			sprites = GetComponentsInChildren<SpriteRenderer>(true);
			left_entires_position = base.transform.Search("current_health").localPosition.x;
			init = true;
		}
		labelsToUpdate.Clear();
		tk2dTextMesh tk2dTextMesh2 = labels.Find((tk2dTextMesh x) => x.name == "label_upgrade_attack");
		tk2dTextMesh tk2dTextMesh3 = labels.Find((tk2dTextMesh x) => x.name == "label_upgrade_health");
		tk2dTextMesh2.gameObject.SetActive(true);
		tk2dTextMesh3.gameObject.SetActive(true);
		attackAttribute.gameObject.SetActive(true);
		healthAttribute.gameObject.SetActive(true);
		emptyAttribute.gameObject.SetActive(false);
		foreach (KeyValuePair<Transform, Transform> disabled_entry in disabled_entries)
		{
			disabled_entry.Value.gameObject.SetActive(false);
		}
		base.transform.Search("current_health").LocalPosX(left_entires_position);
		base.transform.Search("current_attack").LocalPosX(left_entires_position);
		if (showLevelDifference && attributes != 0)
		{
			Array.Find(sprites, (SpriteRenderer x) => x.name == "icon_upgrade_attack").enabled = true;
			Array.Find(sprites, (SpriteRenderer x) => x.name == "icon_upgrade_health").enabled = true;
			int to = adapter.AttackPowerNextLevel - adapter.AttackPower;
			int to2 = ((adapter.AttackPower < 0) ? (Mathf.Abs(adapter.AttackPowerNextLevel) - Mathf.Abs(adapter.AttackPower)) : (adapter.HealthNextLevel - adapter.Health));
			if (animate)
			{
				int from = int.Parse(tk2dTextMesh2.text);
				FromTo(tk2dTextMesh2, from, to, string.Empty);
				from = int.Parse(tk2dTextMesh3.text);
				FromTo(tk2dTextMesh3, from, to2, string.Empty);
			}
			else
			{
				tk2dTextMesh2.text = to.ToString();
				tk2dTextMesh3.text = to2.ToString();
			}
		}
		else
		{
			base.transform.Search("current_health").LocalPosX(left_entires_position + to_center_offset);
			base.transform.Search("current_attack").LocalPosX(left_entires_position + to_center_offset);
			tk2dTextMesh2.gameObject.SetActive(false);
			tk2dTextMesh3.gameObject.SetActive(false);
			Array.Find(sprites, (SpriteRenderer x) => x.name == "icon_upgrade_attack").enabled = false;
			Array.Find(sprites, (SpriteRenderer x) => x.name == "icon_upgrade_health").enabled = false;
		}
		if ((attributes & Attribute.attack) == Attribute.attack)
		{
			tk2dTextMesh tk2dTextMesh4 = labels.Find((tk2dTextMesh x) => x.name == "label_attribute_attack");
			tk2dTextMesh4.GetComponent<Renderer>().enabled = showAttributeNames;
			tk2dTextMesh tk2dTextMesh5 = labels.Find((tk2dTextMesh x) => x.name == "label_current_attack");
			if (animate)
			{
				int from2 = int.Parse(tk2dTextMesh5.text);
				FromTo(tk2dTextMesh5, from2, adapter.AttackPower, string.Empty);
			}
			else
			{
				tk2dTextMesh5.text = adapter.AttackPower.ToString();
			}
		}
		else
		{
			DisableEntry(attackAttribute);
		}
		if ((attributes & Attribute.health) == Attribute.health)
		{
			tk2dTextMesh tk2dTextMesh6 = labels.Find((tk2dTextMesh x) => x.name == "label_attribute_health");
			tk2dTextMesh6.GetComponent<Renderer>().enabled = showAttributeNames;
			int to3 = ((adapter.AttackPower < 0) ? Mathf.Abs(adapter.AttackPower) : adapter.Health);
			tk2dTextMesh tk2dTextMesh7 = labels.Find((tk2dTextMesh x) => x.name == "label_current_health");
			if (animate)
			{
				int from3 = int.Parse(tk2dTextMesh7.text);
				FromTo(tk2dTextMesh7, from3, to3, string.Empty);
			}
			else
			{
				tk2dTextMesh7.text = to3.ToString();
			}
		}
		else
		{
			DisableEntry(healthAttribute);
		}
		SpriteTools.SetSortingLayerID(this, sortingLayer);
		if (animate)
		{
			StartCoroutine(Animate(1f));
		}
	}

	private void DisableEntry(Transform entry)
	{
		if (!disabled_entries.ContainsKey(entry))
		{
			emptyAttribute.gameObject.SetActive(true);
			GameObject gameObject = UnityEngine.Object.Instantiate(emptyAttribute.gameObject) as GameObject;
			gameObject.transform.RepositionAndReparent(entry);
			gameObject.transform.localScale = Vector3.one;
			disabled_entries.Add(entry, gameObject.transform);
			emptyAttribute.gameObject.SetActive(false);
		}
		disabled_entries[entry].gameObject.SetActive(true);
		entry.gameObject.SetActive(false);
	}

	private IEnumerator Animate(float duration)
	{
		float progress2 = 0f;
		float time = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			progress2 = Mathf.Clamp01(time / duration);
			labelsToUpdate.ForEach(delegate(Action<float> x)
			{
				x(progress2);
			});
			yield return null;
		}
	}

	private void FromTo(tk2dTextMesh tm, int from, int to, string suffix = "")
	{
		labelsToUpdate.Add(delegate(float progress)
		{
			int num = Mathf.FloorToInt(Mathf.Lerp(from, to, progress));
			tm.text = suffix + num;
		});
	}
}

using System;
using UnityEngine;

namespace dinobash
{
	public class EntityUpgradeSetter : MonoBase
	{
		public Sprite[] sprites = new Sprite[10];

		private int override_level = -1;

		private UnitType unit_type;

		private UnitType get_entity()
		{
			Transform transform = this.GetComponentUpwards<Animator>().transform;
			string text = transform.name.Substring(0, transform.name.IndexOf("(Clone)"));
			text = char.ToUpper(text[0]) + text.Substring(1);
			return (UnitType)(int)Enum.Parse(typeof(UnitType), text);
		}

		private void Awake()
		{
			if (unit_type == UnitType.None)
			{
				unit_type = get_entity();
			}
			int unitLevel = Player.GetUnitLevel(unit_type);
			if (override_level != -1)
			{
				unitLevel = override_level;
			}
			for (int num = unitLevel; num > 0; num--)
			{
				if (sprites[num] != null)
				{
					GetComponent<SpriteRenderer>().sprite = sprites[num];
					break;
				}
			}
		}

		public void ForceUpdate(int level = -1)
		{
			if (level >= 0)
			{
				override_level = Mathf.Clamp(level, 0, 9);
			}
			Awake();
		}
	}
}

using UnityEngine;

public class ToggleButton : StandardButton
{
	public SpriteRenderer toggle_on;

	public SpriteRenderer toggle_off;

	private bool state = true;

	public bool toggle_state
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
			toggle_on.enabled = state;
			toggle_off.enabled = !state;
		}
	}
}

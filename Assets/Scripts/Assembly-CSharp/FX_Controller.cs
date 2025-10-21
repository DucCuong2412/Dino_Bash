using System;

public abstract class FX_Controller : MonoBase
{
	private bool _isDone;

	public bool isDone
	{
		get
		{
			return _isDone;
		}
		protected set
		{
			_isDone = value;
			if (_isDone && this.OnFxDone != null)
			{
				this.OnFxDone();
			}
		}
	}

	public event Action OnFxDone;

	protected void ResetOnFxDoneEvent()
	{
		this.OnFxDone = null;
	}
}

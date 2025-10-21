using mixpanel.detail;

namespace mixpanel
{
	public class Value : mixpanel.detail.Value
	{
		public Value(string v)
			: base(v)
		{
		}

		public Value(int v)
			: base(v)
		{
		}

		public Value(double v)
			: base(v)
		{
		}

		public Value(float v)
			: base(v)
		{
		}

		public Value(bool v)
			: base(v)
		{
		}

		public Value()
		{
		}

		public static implicit operator Value(string v)
		{
			return new Value(v);
		}

		public static implicit operator Value(int v)
		{
			return new Value(v);
		}

		public static implicit operator Value(double v)
		{
			return new Value(v);
		}

		public static implicit operator Value(float v)
		{
			return new Value(v);
		}

		public static implicit operator Value(bool v)
		{
			return new Value(v);
		}

		public static implicit operator string(Value v)
		{
			return v.asString();
		}

		public static implicit operator int(Value v)
		{
			return v.asInt();
		}

		public static implicit operator double(Value v)
		{
			return v.asDouble();
		}

		public static implicit operator float(Value v)
		{
			return v.asFloat();
		}

		public static implicit operator bool(Value v)
		{
			return v.asBool();
		}
	}
}

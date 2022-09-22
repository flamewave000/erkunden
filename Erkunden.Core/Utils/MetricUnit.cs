namespace Erkunden.Core.Utils
{
	public enum MetricUnit
	{
		Second,
		Metre,
		Gram,
		Ampere,
		Kelvin,
		Watt,
		Hertz,
		//Mole,
		//Candela
	}

	public struct Metric
	{
		public MetricUnit unit;
		public float milli => value * 1000f;
		public float centi => value * 100f;
		public float deci => value * 10f;
		public float value;
		public float kilo => value / 1000f;
		public float Mega => value / 1000_000f;
		public float Giga => value / 1000_000_000f;

		public Metric(float value, MetricUnit unit) { this.value = value; this.unit = unit; }

		public static Metric Millis(float m, MetricUnit unit) => new Metric(m / 1000f, unit);
		public static Metric Centis(float c, MetricUnit unit) => new Metric(c / 100f, unit);
		public static Metric Decis(float d, MetricUnit unit) => new Metric(d / 10f, unit);
		public static Metric Kilos(float k, MetricUnit unit) => new Metric(k * 1_000f, unit);
		public static Metric Megas(float M, MetricUnit unit) => new Metric(M * 1_000_000f, unit);
		public static Metric Gigas(float G, MetricUnit unit) => new Metric(G * 1_000_000_000f, unit);

		public override string ToString()
		{
			string result = "";
			if (milli < 10f) result = milli.ToString() + 'm';
			if (centi < 10f) result = centi.ToString() + 'c';
			if (deci < 10f) result = deci.ToString() + 'd';
			if (value < 1_000f) result = value.ToString();
			if (kilo < 1_000f) result = kilo.ToString() + 'k';
			if (Mega < 1_000f) result = kilo.ToString() + 'M';
			if (Giga < 1_000f) result = kilo.ToString() + 'G';
			switch (unit)
			{
				case MetricUnit.Second:
					return result + 's';
				case MetricUnit.Metre:
					return result + 'm';
				case MetricUnit.Gram:
					return result + 'g';
				case MetricUnit.Ampere:
					return result + 'A';
				case MetricUnit.Kelvin:
					return result + 'K';
				case MetricUnit.Watt:
					return result + 'W';
				case MetricUnit.Hertz:
					return result + "Hz";
				default:
					return result + 'u';
			}
		}
	}

	public struct Metre
	{
		private Metric unit;
		public float mm => unit.milli;
		public float cm => unit.centi;
		public float dm => unit.deci;
		public float metres => unit.value;
		public float km => unit.kilo;
		public float Mm => unit.Mega;
		public float Gm => unit.Giga;

		private Metre(Metric unit) { this.unit = unit; }
		public Metre(float value) { unit = new Metric(value, MetricUnit.Metre); }
		public override string ToString() => unit.ToString();

		public static Metre Millis(float m) => new Metre(Metric.Millis(m, MetricUnit.Metre));
		public static Metre Centis(float c) => new Metre(Metric.Millis(c, MetricUnit.Metre));
		public static Metre Decis(float d) => new Metre(Metric.Millis(d, MetricUnit.Metre));
		public static Metre Kilos(float k) => new Metre(Metric.Millis(k, MetricUnit.Metre));
		public static Metre Megas(float M) => new Metre(Metric.Millis(M, MetricUnit.Metre));
		public static Metre Gigas(float G) => new Metre(Metric.Millis(G, MetricUnit.Metre));
	}

	public struct Gram
	{
		private Metric unit;
		public float mg => unit.milli;
		public float cg => unit.centi;
		public float dg => unit.deci;
		public float grams => unit.value;
		public float kg => unit.kilo;
		public float Mg => unit.Mega;
		public float Gg => unit.Giga;

		private Gram(Metric unit) { this.unit = unit; }
		public Gram(float value) { unit = new Metric(value, MetricUnit.Gram); }
		public override string ToString() => unit.ToString();

		public static Gram Millis(float m) => new Gram(Metric.Millis(m, MetricUnit.Gram));
		public static Gram Centis(float c) => new Gram(Metric.Millis(c, MetricUnit.Gram));
		public static Gram Decis(float d) => new Gram(Metric.Millis(d, MetricUnit.Gram));
		public static Gram Kilos(float k) => new Gram(Metric.Millis(k, MetricUnit.Gram));
		public static Gram Megas(float M) => new Gram(Metric.Millis(M, MetricUnit.Gram));
		public static Gram Gigas(float G) => new Gram(Metric.Millis(G, MetricUnit.Gram));
	}

	public struct Kelvin
	{
		private Metric unit;
		public float mg => unit.milli;
		public float cg => unit.centi;
		public float dg => unit.deci;
		public float kelvin => unit.value;
		public float kg => unit.kilo;
		public float Mg => unit.Mega;
		public float Gg => unit.Giga;

		private Kelvin(Metric unit) { this.unit = unit; }
		public Kelvin(float value) { unit = new Metric(value, MetricUnit.Kelvin); }
		public override string ToString() => unit.ToString();

		public static Kelvin Millis(float m) => new Kelvin(Metric.Millis(m, MetricUnit.Kelvin));
		public static Kelvin Centis(float c) => new Kelvin(Metric.Millis(c, MetricUnit.Kelvin));
		public static Kelvin Decis(float d) => new Kelvin(Metric.Millis(d, MetricUnit.Kelvin));
		public static Kelvin Kilos(float k) => new Kelvin(Metric.Millis(k, MetricUnit.Kelvin));
		public static Kelvin Megas(float M) => new Kelvin(Metric.Millis(M, MetricUnit.Kelvin));
		public static Kelvin Gigas(float G) => new Kelvin(Metric.Millis(G, MetricUnit.Kelvin));
	}

	public struct Watt
	{
		private Metric unit;
		public float mg => unit.milli;
		public float cg => unit.centi;
		public float dg => unit.deci;
		public float grams => unit.value;
		public float kg => unit.kilo;
		public float Mg => unit.Mega;
		public float Gg => unit.Giga;

		private Watt(Metric unit) { this.unit = unit; }
		public Watt(float value) { unit = new Metric(value, MetricUnit.Watt); }
		public override string ToString() => unit.ToString();

		public static Watt Millis(float m) => new Watt(Metric.Millis(m, MetricUnit.Watt));
		public static Watt Centis(float c) => new Watt(Metric.Millis(c, MetricUnit.Watt));
		public static Watt Decis(float d) => new Watt(Metric.Millis(d, MetricUnit.Watt));
		public static Watt Kilos(float k) => new Watt(Metric.Millis(k, MetricUnit.Watt));
		public static Watt Megas(float M) => new Watt(Metric.Millis(M, MetricUnit.Watt));
		public static Watt Gigas(float G) => new Watt(Metric.Millis(G, MetricUnit.Watt));
	}

	public struct Hertz
	{
		private Metric unit;
		public float mg => unit.milli;
		public float cg => unit.centi;
		public float dg => unit.deci;
		public float grams => unit.value;
		public float kg => unit.kilo;
		public float Mg => unit.Mega;
		public float Gg => unit.Giga;

		private Hertz(Metric unit) { this.unit = unit; }
		public Hertz(float value) { unit = new Metric(value, MetricUnit.Hertz); }
		public override string ToString() => unit.ToString();

		public static Hertz Millis(float m) => new Hertz(Metric.Millis(m, MetricUnit.Hertz));
		public static Hertz Centis(float c) => new Hertz(Metric.Millis(c, MetricUnit.Hertz));
		public static Hertz Decis(float d) => new Hertz(Metric.Millis(d, MetricUnit.Hertz));
		public static Hertz Kilos(float k) => new Hertz(Metric.Millis(k, MetricUnit.Hertz));
		public static Hertz Megas(float M) => new Hertz(Metric.Millis(M, MetricUnit.Hertz));
		public static Hertz Gigas(float G) => new Hertz(Metric.Millis(G, MetricUnit.Hertz));
	}
}

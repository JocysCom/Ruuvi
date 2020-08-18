namespace JocysCom.Ruuvi.PRTG.Server
{

	public class PrtgData
	{
		public prtg prtg { get; set; }
	}

	public class prtg
	{
		public result[] result { get; set; }
	}

	public class result
	{
		public result() { }

		public result(string c, float v, string unit = "#", int? dm = null)
		{
			channel = c;
			value = v;
			Unit = unit;
			DecimalMode = dm;
		}

		public string channel { get; set; }
		public int @float { get; set; } = 1;
		public float @value { get; set; }
		public string Unit { get; set; }
		public int? DecimalMode { get; set; }

	}
}

using System;
using System.IO;
using System.Linq;

namespace JocysCom.Ruuvi.PRTG.Server
{
	public class RuuviData
	{
		public short RawSignalStrengthInDBm { get; set; }

		public ushort Manufacturer { get; set; }
		public byte DataFormat { get; set; }
		public decimal Temperature { get; set; }
		public decimal Humidity { get; set; }
		public decimal Pressure { get; set; }

		public decimal AccelerationX { get; set; }
		public decimal AccelerationY { get; set; }
		public decimal AccelerationZ { get; set; }

		public byte MovementCounter { get; set; }
		public ushort MeasurementSequence { get; set; }
		public string MacAddress { get; set; }
		public string MacAddressString { get; set; }

		public decimal Voltage { get; set; }
		public decimal Power { get; set; }
		public decimal Signal { get; set; }

		public RuuviData(byte[] buffer)
		{
			var ms = new MemoryStream(buffer);
			var reader = new BinaryReader(ms);
			Manufacturer = reader.ReadUInt16();
			DataFormat = reader.ReadByte();
			if (DataFormat == 5)
			{
				// https://github.com/ruuvi/ruuvi-sensor-protocols/blob/master/dataformat_05.md
				Temperature = BitConverter.ToInt16(reader.ReadBytes(2).Reverse().ToArray(), 0) * 0.005m;
				Humidity = BitConverter.ToUInt16(reader.ReadBytes(2).Reverse().ToArray(), 0) * 0.0025m;
				Pressure = BitConverter.ToUInt16(reader.ReadBytes(2).Reverse().ToArray(), 0) + 50000m;
				AccelerationX = BitConverter.ToInt16(reader.ReadBytes(2).Reverse().ToArray(), 0) / 1000m;
				AccelerationY = BitConverter.ToInt16(reader.ReadBytes(2).Reverse().ToArray(), 0) / 1000m;
				AccelerationZ = BitConverter.ToInt16(reader.ReadBytes(2).Reverse().ToArray(), 0) / 1000m;
				var powerInfo = BitConverter.ToUInt16(reader.ReadBytes(2).Reverse().ToArray(), 0);
				Voltage = (powerInfo >> 5) / 1000m + 1.6m;
				Power = (powerInfo & 0b11111) * 2m - 40m;
				MovementCounter = reader.ReadByte();
				MeasurementSequence = BitConverter.ToUInt16(reader.ReadBytes(2).Reverse().ToArray(), 0);
				var macBytes = reader.ReadBytes(6).ToArray();
				MacAddressString = string.Join(":", macBytes.Select(x => x.ToString("X2")));

			}
		}

	}

}

using JocysCom.ClassLibrary.Services.SimpleService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Windows.Devices.Bluetooth.Advertisement;

namespace JocysCom.Ruuvi.PRTG.Server
{
	public class RuuviPrtgService : ISimpleService
	{
		public bool IsStopping { get; set; }

		public bool IsPaused { get; set; }

		public void DoAction(string[] args, ref bool skipSleep)
		{
		}

		public void InitEnd()
		{
			Watcher.Stop();
			Watcher.Received -= Watcher_Received;
		}

		public void InitStart()
		{
			var prefix = "Ruuvi_";
			var ruuviKeys = ConfigurationManager.AppSettings.AllKeys.Where(x=>x.StartsWith(prefix)).ToList();
			foreach (var key in ruuviKeys)
			{
				var ruuviMac = key.Substring(prefix.Length);
				var ruuviUrl = ConfigurationManager.AppSettings[key];
				RuuviPrtgMaps.Add(ruuviMac, ruuviUrl);
				Console.WriteLine("RUUVI {0} -> {1}", ruuviMac, ruuviUrl);
			}
			// Filter Ruuvi tags by manufaturer.
			var manufacturerData = new BluetoothLEManufacturerData();
			manufacturerData.CompanyId = RuuviCompanyId;
			Watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);
			Watcher.Received += Watcher_Received;
			Watcher.Start();
		}

		private BluetoothLEAdvertisementWatcher Watcher = new BluetoothLEAdvertisementWatcher();

		private const ushort RuuviCompanyId = 0x0499;
		Dictionary<string, string> RuuviPrtgMaps = new Dictionary<string, string>();

		private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
		{
			if (IsStopping || IsPaused)
				return;
			try
			{
				var hexs = BitConverter.GetBytes(args.BluetoothAddress).Reverse().Select(b => b.ToString("X2")).Skip(2);
				var ruuviMac = string.Join("", hexs);
				Console.WriteLine("Watcher_Received: " + ruuviMac);
				if (!RuuviPrtgMaps.Keys.Contains(ruuviMac))
					return;
				var ruuviUrl = new Uri(RuuviPrtgMaps[ruuviMac]);
				RuuviData data = null;
				Console.WriteLine("Address: {0}, Advertisement Type: {1}", ruuviMac, args.AdvertisementType);
				for (int i = 0; i < args.Advertisement.ManufacturerData.Count; i++)
				{
					var md = args.Advertisement.ManufacturerData[i];
					var bytes = new byte[md.Data.Length];
					using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(md.Data))
						dataReader.ReadBytes(bytes);
					/*
					Console.WriteLine("Manufacturer: 0x{0}, Data[{1}]: {2}",
						md.CompanyId.ToString("X4"),
						md.Data.Length,
						string.Join("", bytes.Select(x => x.ToString("X2")))
					);
					*/
				}
				for (int i = 0; i < args.Advertisement.DataSections.Count; i++)
				{
					var ds = args.Advertisement.DataSections[i];
					var bytes = new byte[ds.Data.Length];
					using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(ds.Data))
					{
						dataReader.ReadBytes(bytes);
						if (bytes.Length > 3)
						{
							data = new RuuviData(bytes);
							Console.WriteLine("  Type: {0}, Data[{1}]: {2}",
								ds.DataType,
								ds.Data.Length,
								string.Join("", bytes.Select(x => x.ToString("X2"))));
						}
					}
				}
				//Console.WriteLine("Local Name: {0}", args.Advertisement.LocalName);
				//var serviceUuids = string.Join(", ", args.Advertisement.ServiceUuids);
				//Console.WriteLine("Service UUIDs: {0}", serviceUuids);
				//Console.WriteLine("Flags: {0}", args.Advertisement.Flags);

				if (data != null)
				{
					data.RawSignalStrengthInDBm = args.RawSignalStrengthInDBm;
					var sb = new StringBuilder();
					sb.AppendFormat("  Manufacturer: 0x{0:X4}\r\n", data.Manufacturer);
					sb.AppendFormat("  DataFormat: {0}\r\n", data.DataFormat);
					sb.AppendFormat("  Temperature: {0} C°\r\n", data.Temperature);
					sb.AppendFormat("  Humidity: {0} %\r\n", data.Humidity);
					sb.AppendFormat("  Pressure: {0} Pa\r\n", data.Pressure);
					sb.AppendFormat("  AccelerationX: {0} m/s²\r\n", data.AccelerationX);
					sb.AppendFormat("  AccelerationY: {0} m/s²\r\n", data.AccelerationY);
					sb.AppendFormat("  AccelerationZ: {0} m/s²\r\n", data.AccelerationZ);
					sb.AppendFormat("  MovementCounter: {0}\r\n", data.MovementCounter);
					sb.AppendFormat("  MeasurementSequence: {0}\r\n", data.MeasurementSequence);
					//sb.AppendFormat("  MacAddress: {0}\r\n", data.MacAddress);
					sb.AppendFormat("  MacAddress: {0}\r\n", data.MacAddressString);
					sb.AppendFormat("  Voltage: {0} V\r\n", data.Voltage);
					sb.AppendFormat("  Power: {0} dBm\r\n", data.Power);
					sb.AppendFormat("  Signal: {0} dBm\r\n", data.RawSignalStrengthInDBm);
					Console.WriteLine(sb.ToString());
				}
				var identificationToken = new Guid("E1A4F3C2-D915-4323-B590-5EF55E9DA021");
				var pd = AppHelper.Convert(data);
				Console.WriteLine("PRTG URL: " + ruuviUrl);
				var status = AppHelper.MakeGetRequest(ruuviUrl, identificationToken, pd);
				Console.WriteLine("PRTG GET Status: " + status);

			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
			}

		}
	}
}

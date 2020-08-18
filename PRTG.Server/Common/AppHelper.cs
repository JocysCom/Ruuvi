using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Policy;
using System.Text;

namespace JocysCom.Ruuvi.PRTG.Server
{
	public class AppHelper
	{

		public static PrtgData Convert(RuuviData data)
		{
			var o = new PrtgData()
			{
				prtg = new prtg()
				{
					result = new result[]
					{
						 new result(nameof(RuuviData.Temperature), (float)data.Temperature, "C°", 2),
						 new result(nameof(RuuviData.Humidity), (float)data.Humidity, "%", 2),
						 new result(nameof(RuuviData.Pressure), (float)data.Pressure, "Pa"),
						 new result(nameof(RuuviData.AccelerationX), (float)data.AccelerationX, "m/s²"),
						 new result(nameof(RuuviData.AccelerationY), (float)data.AccelerationY, "m/s²"),
						 new result(nameof(RuuviData.AccelerationZ), (float)data.AccelerationZ, "m/s²"),
						 new result(nameof(RuuviData.Power), (float)data.Power, "dBm"),
						 new result(nameof(RuuviData.Signal), (float)data.RawSignalStrengthInDBm, "dBm"),
						 new result(nameof(RuuviData.Voltage), (float)data.Voltage, "V"),
					}
				}
			};
			return o;
		}

		public static string MakeGetRequest(Uri prtgUrl, Guid sensorToken, PrtgData data)
		{
			var dataString = SerializeToJson(data);
			var url = prtgUrl.AbsoluteUri + "?content=" + System.Net.WebUtility.UrlEncode(dataString);
			return MakeGetRequest(url.ToString());
		}

		public static string MakeGetRequest(string requestUriString)
		{
			string status = null;
			try
			{
				var request = WebRequest.Create(requestUriString);
				var response = (HttpWebResponse)request.GetResponse();
				var code = (int)response.StatusCode;
				var description = response.StatusDescription;
				response.Close();
				status = string.Format("{0} - {1}", code, description);
				response.Close();
			}
			catch (Exception ex)
			{
				status = ex.Message;
			}
			return status;
		}


		#region JSON

		// Notes: Use [DataMember(EmitDefaultValue = false, IsRequired = false)] attribute
		// if you don't want to serialize default values.

		static object JsonSerializersLock = new object();
		static Dictionary<Type, DataContractJsonSerializer> JsonSerializers;
		public static DataContractJsonSerializer GetJsonSerializer(Type type, DataContractJsonSerializerSettings settings = null)
		{
			lock (JsonSerializersLock)
			{
				if (JsonSerializers == null)
					JsonSerializers = new Dictionary<Type, DataContractJsonSerializer>();
				if (!JsonSerializers.ContainsKey(type))
				{
					// Simple dictionary format looks like this: { "Key1": "Value1", "Key2": "Value2" }
					// DataContractJsonSerializerSettings requires .NET 4.5
					if (settings == null)
					{
						settings = new DataContractJsonSerializerSettings();
						settings.IgnoreExtensionDataObject = true;
						settings.UseSimpleDictionaryFormat = true;
					}
					var serializer = new DataContractJsonSerializer(type, settings);
					JsonSerializers.Add(type, serializer);
				}
			}
			return JsonSerializers[type];
		}

		/// <summary>
		/// Serialize object to JSON string.
		/// </summary>
		/// <param name="o">The object to serialize.</param>
		/// <param name="encoding">JSON string encoding.</param>
		/// <returns>JSON string.</returns>
		public static string SerializeToJson(object o, Encoding encoding = null)
		{
			if (o == null)
				return null;
			var serializer = GetJsonSerializer(o.GetType());
			var ms = new MemoryStream();
			lock (serializer)
			{ serializer.WriteObject(ms, o); }
			if (encoding == null)
				encoding = Encoding.UTF8;
			var json = encoding.GetString(ms.ToArray());
			ms.Close();
			return json;
		}

		#endregion

	}
}

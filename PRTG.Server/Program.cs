using JocysCom.ClassLibrary.Services.SimpleService;
using System;

namespace JocysCom.Ruuvi.PRTG.Server
{
	class Program
	{
		[STAThread()]
		static int Main()
		{	
			var service = new SimpleServiceBase<RuuviPrtgService>();
			service.RunServer();
			// Return 0 if all OK.
			return 0;
		}
	}
}

# From RuuviTag to PRTG Server

Server which gathers data from RuuviTag bluetooth beacon and sends data to PRTG Server. You can use RuuviTag to monitor server room temperature, humidity and pressure. You can put RuuviTag outside or strap it to the room door to detect its movement i.e. someone is entering server room. You can lock RuuviTag inside bicycle outside and setup PRTG server to send SMS (BulkSMS) message as soon as it moves :).

![Ruuvi PRTG View](https://github.com/JocysCom/Ruuvi/blob/master/PRTG.Server/Documents/Images/Ruuvi_PRTG_Sensor.png?raw=true "Ruuvi PRTG View")

# Configuration

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <!-- Map RuuviTag Map with PRTG HTTP Sensor URL -->
    <add key="Ruuvi_CE4FD7708BA5" value="http://localhost:5050/F28C6E2A-F0D8-B58A-7A26-A96CBF5540C7"/>
    <add key="Ruuvi_BAB697EF83C4" value="http://localhost:5050/88CE63F8-A83D-0644-F990-ADCEBCFCA452"/>
    <add key="Ruuvi_DD0B1AFF214D" value="http://localhost:5050/FA0D6ABC-D842-CBF0-3FED-185F7D3E1F7A"/>
  </appSettings>
</configuration>
```

# Service and Console

You can install "Jocys.com Ruuvi PRTG Server" as Windows service:

![Ruuvi PRTG Service](https://github.com/JocysCom/Ruuvi/blob/master/PRTG.Server/Documents/Images/Ruuvi_PRTG_Files.png "Ruuvi PRTG Service")

or run Jocyscom.Ruuvi.PRTG.Server.exe as Console Application:

![Ruuvi PRTG Console](https://github.com/JocysCom/Ruuvi/blob/master/PRTG.Server/Documents/Images/Ruuvi_Server_as_Console.png "Ruuvi PRTG Console")

#include <Time.h>
#include <TimeLib.h>

#include <Adafruit_Sensor.h>

#include <DHT.h>
#include <SPI.h>
#include <WiFi.h>
#include <WiFiUdp.h>

#define DHTPIN 2
#define DHTTYPE DHT22
DHT dht(DHTPIN, DHTTYPE);


int status = WL_IDLE_STATUS;
char ssid[] = "Bbox-E452267C";
char pass[] = "D74D33C5C6717DAFF5D4214AE413C7";

unsigned int localPort = 2390;
WiFiUDP Udp;
char ReplyBuffer[255] = "acknowledged";
char packetBuffer[255]; //buffer to hold incoming packet
String ack = "acknowledged";
String ret = "";

void setup() 
{
  Serial.begin(9600);

  // attempt to connect to Wifi network:
  while (status != WL_CONNECTED) 
  {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    status = WiFi.begin(ssid, pass);

    // wait 10 seconds for connection:
    delay(3000);
  }
  Serial.println("Connected to wifi");
  WifiStatus();

  Serial.println("\nStarting connection to server...");
  Udp.begin(localPort);

  Serial.println("\nStarting Temp Sensor...");
  dht.begin();
  Serial.print("\nTemp Sensor OK");
  
  Serial.println("\n**  END SETUP   **");
  Serial.println("");
}

void loop() 
{
  // if there's data available, read a packet
  int packetSize = Udp.parsePacket();
  if (packetSize) 
  {
    String message = ReadMessage(packetSize);
    //Serial.println("message: " + message);
    
    if(message.indexOf("CMD") > 0)
    {
      message.replace("[CMD]", "");
      message.replace("[/CMD]", "");
      Serial.println("command: " + message);
    }

    if(message == "TEMP")
      ("TEMP_"+ReadDHT22()).toCharArray( ReplyBuffer, 512 );
    else if(message == "WIFI")
      WifiStatus().toCharArray( ReplyBuffer, 512 );
    else if(message == "SETDATE")
      GetDate().toCharArray( ReplyBuffer, 512 );
    else if(message == "DATE")
      ClockDisplay().toCharArray( ReplyBuffer, 512 );
    else if(message == "LIGHT")
      ("LIGHT_"+GetLight()).toCharArray( ReplyBuffer, 512 );
      
    Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
    Udp.write(ReplyBuffer);
    Udp.endPacket();

    Serial.println("");
    ack.toCharArray(ReplyBuffer, 12);
  }
}


int photocellPin = 0;     // the cell and 10K pulldown are connected to a0
int photocellReading;     // the analog reading from the sensor divider
String GetLight()
{
  ret = "";
  photocellReading = analogRead(photocellPin);  
  ret += ("Light Analog reading = ");
  ret += (photocellReading);     // the raw analog reading
  Serial.println(ret);
  return ret;
  // LED gets brighter the darker it is at the sensor
  // that means we have to -invert- the reading from 0-1023 back to 1023-0
  //photocellReading = 1023 - photocellReading;
}

String ClockDisplay()
{
  ret = "";
  ret += (hour());
  ret += printDigits(minute());
  ret += printDigits(second());
  ret += (" ");
  ret += (day());
  ret += (" ");
  ret += (month());
  ret += (" ");
  ret += (year()); 
  
  Serial.println(ret);
  return ret;
}

String printDigits(int digits)
{
  // utility function for digital clock display: prints preceding colon and leading 0
  String dummy = "";
  dummy +=(":");
  if(digits < 10)
    dummy += "0" + digits;
  else
    dummy += digits;
  return dummy;
}

const char timeServer[] = "0.pool.ntp.org"; // time.nist.gov NTP server
const int NTP_PACKET_SIZE = 48; // NTP time stamp is in the first 48 bytes of the message
String GetDate()
{
  sendNTPpacket(timeServer); // send an NTP packet to a time server

  // wait to see if a reply is available
  delay(1000);
  if (Udp.parsePacket()) 
  {
    ret = "";
    // We've received a packet, read the data from it
    Udp.read(packetBuffer, NTP_PACKET_SIZE); // read the packet into the buffer

    // the timestamp starts at byte 40 of the received packet and is four bytes,
    // or two words, long. First, extract the two words:

    unsigned long highWord = word(packetBuffer[40], packetBuffer[41]);
    unsigned long lowWord = word(packetBuffer[42], packetBuffer[43]);
    // combine the four bytes (two words) into a long integer
    // this is NTP time (seconds since Jan 1 1900):
    unsigned long secsSince1900 = highWord << 16 | lowWord;
    ret = ("Seconds since Jan 1 1900 = ");
    ret += (secsSince1900);



    // now convert NTP time into everyday time:
    ret += ("\r\nUnix time = ");
    // Unix time starts on Jan 1 1970. In seconds, that's 2208988800:
    const unsigned long seventyYears = 2208988800UL;
    // subtract seventy years:
    unsigned long epoch = secsSince1900 - seventyYears;
    // print Unix time:
    ret += (epoch);
    
    setTime(epoch);

    // print the hour, minute and second:
    ret += ("\r\nThe UTC time is ");       // UTC is the time at Greenwich Meridian (GMT)
    ret += ((epoch  % 86400L) / 3600); // print the hour (86400 equals secs per day)
    ret += (':');
    if (((epoch % 3600) / 60) < 10) {
      // In the first 10 minutes of each hour, we'll want a leading '0'
      ret += ('0');
    }
    ret += ((epoch  % 3600) / 60); // print the minute (3600 equals secs per minute)
    ret += (':');
    if ((epoch % 60) < 10) {
      // In the first 10 seconds of each minute, we'll want a leading '0'
      ret += ('0');
    }
    ret += (epoch % 60); // print the second
  
    Serial.println(ret);  
    return ret;
  }  
}

void sendNTPpacket(const char * address) {
  // set all bytes in the buffer to 0
  memset(packetBuffer, 0, NTP_PACKET_SIZE);
  // Initialize values needed to form NTP request
  // (see URL above for details on the packets)
  packetBuffer[0] = 0b11100011;   // LI, Version, Mode
  packetBuffer[1] = 0;     // Stratum, or type of clock
  packetBuffer[2] = 6;     // Polling Interval
  packetBuffer[3] = 0xEC;  // Peer Clock Precision
  // 8 bytes of zero for Root Delay & Root Dispersion
  packetBuffer[12]  = 49;
  packetBuffer[13]  = 0x4E;
  packetBuffer[14]  = 49;
  packetBuffer[15]  = 52;

  // all NTP fields have been given values, now
  // you can send a packet requesting a timestamp:
  Udp.beginPacket(address, 123); // NTP requests are to port 123
  Udp.write(packetBuffer, NTP_PACKET_SIZE);
  Udp.endPacket();
}

String ReadMessage(int packetSize)
{
    Serial.print("Received packet of size ");
    Serial.print(packetSize);
    Serial.print(" - From ");
    IPAddress remoteIp = Udp.remoteIP();
    Serial.print(remoteIp);
    Serial.print(", port ");
    Serial.print(Udp.remotePort());

    // read the packet into packetBufffer
    int len = Udp.read(packetBuffer, 255);
    if (len > 0)
      packetBuffer[len] = 0;

    Serial.println(packetBuffer);

    String str(packetBuffer);
    return str;
}

String ReadDHT22() 
{
  ret = ""; 

  // Reading temperature or humidity takes about 250 milliseconds!
  // Sensor readings may also be up to 2 seconds 'old' (its a very slow sensor)
  float h = dht.readHumidity();
  // Read temperature as Celsius
  float t = dht.readTemperature();
  // Read temperature as Fahrenheit
  float f = dht.readTemperature(true);
  
  // Check if any reads failed and exit early (to try again).
  if (isnan(h) || isnan(t) || isnan(f)) 
  {
    Serial.println("Failed to read from DHT sensor!");
    ret = "Failed to read from DHT sensor!";
    return ret;
  }

  // Compute heat index
  // Must send in temp in Fahrenheit!
  float hi = dht.computeHeatIndex(f, h);

  ret = "Humidity: "; 
  ret += h;
  ret += " %\t";
  ret += "Temperature: "; 
  ret += t;
  ret += " *C ";
  ret += f;
  ret += " *F\t";
  ret += "Heat index: ";
  ret += hi;
  ret += "*F";

  Serial.println(ret);
  return ret;
}

String WifiStatus() 
{
  ret = "";
  // print the SSID of the network you're attached to:
  ret = ("SSID: ");
  ret += (WiFi.SSID());

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  ret += ("\r\nIP Address: ");
  ret += (ip);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  ret += ("\r\nsignal strength (RSSI):");
  ret += (rssi);
  ret += (" dBm");

  Serial.println(ret);
  return ret;
}

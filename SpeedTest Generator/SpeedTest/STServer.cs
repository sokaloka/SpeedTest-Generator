using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SpeedTest
{

	/*
	 <server url="http://speedtest.ppo.fi/speedtest/upload.php" 
	 * lat="64.0750" 
	 * lon="24.5333"
	 * name="Ylivieska"
	 * country="Finland" 
	 * countrycode="FI" 
	 * sponsor="PPO-Yhtiöt Oy" 
	 * sponsorurl="http://www.ppo.fi" 
	 * id="1726" 
	 * gid="0" 
	 * url2="http://speedtest.ppo.fi/speedtest/upload.php" 
	 * bigsamples="1"/> 
	*/
	public class STServer
	{
		public int Id {get;set;}
		
		public string Name { get; set; }
		public string Country { get; set; }
		public string CountryCode { get; set; }
		public string Sponsor { get; set; }

		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}

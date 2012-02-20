using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SpeedTest
{
	public class SpeedTestServers : List<SpeedTestServer>
	{
		public static SpeedTestServers Parse(XmlReader reader)
		{
			var ret = new SpeedTestServers();
			while (reader.Read())
			{
			 if (reader.NodeType != XmlNodeType.Element	)
				 continue;

				if (reader.Name != "server")
					continue;

				var server = new SpeedTestServer();
				server.Id = Convert.ToInt32(reader.GetAttribute("id"));
				server.Latitude = Convert.ToDouble(reader.GetAttribute("lat"));
				server.Longitude = Convert.ToDouble(reader.GetAttribute("lon"));
				server.Name = reader.GetAttribute("name");
				server.Country = reader.GetAttribute("country");
				server.CountryCode = reader.GetAttribute("countrycode");
				server.Sponsor = reader.GetAttribute("sponsor");

				ret.Add(server);
			}
			return ret;
		}
	}
}

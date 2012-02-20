using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MIL.Html;

namespace SpeedTest
{
	public class STServers : List<STServer>
	{
		public static STServers Parse(string str)
		{
			var ret = new STServers();
			var doc = HtmlDocument.Create(str);
			var servers = doc.Nodes.FindByName("server");

			foreach(HtmlNode node in servers)
			{
				var ele = node as HtmlElement;
				if (ele == null)
					continue;

				var attr = ele.Attributes;

				var server = new STServer();
				server.Id = Convert.ToInt32(attr.GetAttributeValue("id"));
				server.Latitude = Convert.ToDouble(attr.GetAttributeValue("lat"));
				server.Longitude = Convert.ToDouble(attr.GetAttributeValue("lon"));
				server.Name = attr.GetAttributeValue("name");
				server.Country = attr.GetAttributeValue("country");
				server.CountryCode = attr.GetAttributeValue("countrycode");
				server.Sponsor = attr.GetAttributeValue("sponsor");

				ret.Add(server);
			}
			return ret;
		}
	}
}

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedTest;

namespace UnitTests
{
	[TestClass]
	public class SpeedTestTests
	{
		[TestMethod]
		public void TestServers()
		{
			Assert.IsNotNull(SpeedTestServers.Parse(XmlReader.Create(new StringReader(Properties.Resources.Servers))));
		}
	}
}

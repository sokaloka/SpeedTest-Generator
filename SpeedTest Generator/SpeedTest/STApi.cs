using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SpeedTest
{
	public class STApi
	{
		const string ApiUrl = "http://www.speedtest.net/api/api.php";
		const string ServersUrl = "http://speedtest.net/speedtest-servers.php?x={0}";
		const string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:10.0.2) Gecko/20100101 Firefox/10.0.2";
		/// <summary>
		/// ping-upload-download-key
		/// </summary>
		const string HashFormat = "{0}-{1}-{2}-297aae72";

		public STApi()
		{
		}

		/// <summary>
		/// Generates a SpeedTest result based on the given parameters.
		/// </summary>
		/// <param name="server"></param>
		/// <param name="download"></param>
		/// <param name="upload"></param>
		/// <param name="ping"></param>
		/// <returns></returns>
		public static int Generate(STServer server, int download, int upload, int ping)
		{
			var http = (HttpWebRequest)WebRequest.Create(ApiUrl);
			http.Method = WebRequestMethods.Http.Post;
			http.ContentType = "application/x-www-form-urlencoded";
			http.UserAgent = UserAgent;
			http.ServicePoint.Expect100Continue = false;
			http.Referer = "http://c.speedtest.net/flash/speedtest.swf?v=297608";

			var sid = server.Id.ToString(CultureInfo.InvariantCulture);
			var sdown =  download.ToString(CultureInfo.InvariantCulture);
			var sup = upload.ToString(CultureInfo.InvariantCulture);
			var sping = ping.ToString(CultureInfo.InvariantCulture);

			var dict = new Dictionary<string, string> 
			{
				{"promo", ""},
				{"startmode", "serverclick"},
				{"accuracy", "1"},
				{"serverid", sid},
				{"recommendedserverid", sid},
				{"hash", Md5(string.Format(HashFormat, sping, sup, sdown))},
				{"download", sdown},
				{"ping", sping},
				{"upload", sup},
			};
			var postdata = string.Join("&", dict.Select(kv => string.Format("{0}={1}", kv.Key, HttpUtil.UrlEncode(kv.Value))));

			using (var req = http.GetRequestStream())
			{
				req.WriteBytes(Encoding.UTF8.GetBytes(postdata));
			}

			using (var resp = (HttpWebResponse)http.GetResponse())
			{
				using (var sr = new StreamReader(resp.GetResponseStream()))
				{
					var parms = HttpUtil.ParseQueryString(sr.ReadToEnd());
					return Convert.ToInt32(parms["resultid"]);
				}
			}
		}

		public static STServers GetServers()
		{
			var unix = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
			var http = (HttpWebRequest)WebRequest.Create(string.Format(ServersUrl, unix));
			http.UserAgent = UserAgent;

			using (var resp = (HttpWebResponse)http.GetResponse())
			{
				using (var sr = new StreamReader(resp.GetResponseStream()))
				{
					return STServers.Parse(sr.ReadToEnd());
				}
			}
		}

		static string Md5(string str)
		{
			using (var md5 = MD5.Create())
			{
				return string.Join("", md5.ComputeHash(Encoding.UTF8.GetBytes(str)).Select(b => b.ToString("x2")));
			}
		}
	}
}

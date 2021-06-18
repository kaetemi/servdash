using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ServDash
{
	class IniReader
	{

		public static Dictionary<string, Dictionary<string, string>> Read(string filename)
		{
			Dictionary<string, Dictionary<string, string>> res = new Dictionary<string, Dictionary<string, string>>();
			FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader sr = new StreamReader(fs);
			string line;
			Dictionary<string, string> section = new Dictionary<string, string>();
			res[""] = section;
			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith(";"))
					continue;
				else if (line.StartsWith("["))
				{
					section = new Dictionary<string, string>();
					res[line.Substring(1, line.IndexOf("]") - 1)] = section;
				}
				else
				{
					string[] s = line.Split(new char[] { '=' }, 2);
					if (s.Length > 1)
						section[s[0]] = s[1];
					else if (s.Length > 0)
						section[s[0]] = "";
				}
			}
			sr.Close();
			fs.Close();
			sr.Dispose();
			fs.Dispose();
			return res;
		}
	}
}

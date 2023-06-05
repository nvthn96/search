using Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.Utility
{
	public class IOUtility
	{
		public static string GetFolder(string path)
		{
			return AppDomain.CurrentDomain.BaseDirectory + path;
		}

		public static IEnumerable<DataFile> GetFiles(string subFolder, string namePattern)
		{
			var folder = Directory.GetCurrentDirectory();
			var dataFolder = Path.Combine(folder, subFolder);

			var files = Directory.GetFiles(dataFolder, namePattern, SearchOption.TopDirectoryOnly);

			var result = files.Select(file => new DataFile()
			{
				Name = Path.GetFileName(file),
				Content = File.ReadAllText(file),
				Hash = SecurityUtility.GetMD5(file)
			}).ToArray();

			return result;
		}
	}
}

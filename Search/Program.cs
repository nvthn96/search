using Search.ExLucene;
using Search.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
	class Program
	{
		static void Main(string[] args)
		{
			AppUtility.Initial();

			var files = IOUtility.GetFiles(Config.Constant.Folder.Data, Config.Constant.File.FileSearchExtension);
			var searcher = new LucenceSearch();
			searcher.BuildIndices(files);

			var running = true;
			while (running)
			{
				Console.Write("Input keyword: ");
				var keyword = Console.ReadLine();
				if (keyword.Length == 0) running = false;
				else
				{
					var items = searcher.Search(keyword);
					foreach (var item in items)
					{
						Console.WriteLine(item);
					}
				}
			}
		}
	}
}

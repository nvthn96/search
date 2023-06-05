using System.IO;

namespace Search.Utility
{
	public class AppUtility
	{
		public static void Initial()
		{
			Directory.CreateDirectory(Config.Constant.Folder.Data);
			Directory.CreateDirectory(Config.Constant.Folder.Token);

			SQLiteUtility.Initial();
		}
	}
}

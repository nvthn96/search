using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.Utility
{
	public class SQLiteUtility
	{
		private static readonly string dbConnection = "URI=file:" + IOUtility.GetFolder(Config.Constant.SQLite.FileDB);

		private static T RunCommand<T>(Func<SQLiteCommand, T> func)
		{
			using (var con = new SQLiteConnection(dbConnection))
			{
				con.Open();
				using (var cmd = new SQLiteCommand(con))
				{
					return func.Invoke(cmd);
				}
			}
		}

		public static void Initial()
		{
			RunCommand(cmd =>
			{
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS FileExists (md5 blob primary key, filename text, timestamp datetime);";
				return cmd.ExecuteNonQuery();
			});
		}

		public static bool IsExists(string md5)
		{
			return RunCommand(cmd =>
			{
				cmd.CommandText = "SELECT 1 FROM FileExists WHERE md5 = $md5;";
				cmd.Parameters.AddWithValue("$md5", md5);
				var result = cmd.ExecuteScalar();
				return (result != null) && ((long)result == 1L);
			});
		}

		public static bool Add(string md5, string filename, DateTime timestamp)
		{
			return RunCommand(cmd =>
			{
				cmd.CommandText = "INSERT INTO FileExists(md5, filename, timestamp) values ($md5, $filename, $timestamp);";
				cmd.Parameters.AddWithValue("$md5", md5);
				cmd.Parameters.AddWithValue("$filename", filename);
				cmd.Parameters.AddWithValue("$timestamp", timestamp);
				var result = cmd.ExecuteScalar();
				return (result != null) && ((int)result == 1);
			});
		}
	}
}

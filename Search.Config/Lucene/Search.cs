using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.Config.Lucene
{
	public class Search
	{
		public static bool AllowLeadingWildcard { get; set; } = false;
		public static int TopDocumentSize { get; set; } = 100;
	}
}

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Search.Model;
using Search.Utility;
using System;
using System.Collections.Generic;

namespace Search.ExLucene
{
	public class LucenceSearch
	{
		private readonly Lucene.Net.Util.Version Version = Lucene.Net.Util.Version.LUCENE_30;

		public void BuildIndices(IEnumerable<DataFile> Data)
		{
			var analyzer = new StandardAnalyzer(Version);
			var directory = FSDirectory.Open(IOUtility.GetFolder(Config.Constant.Folder.Token));
			using (IndexWriter indexWriter = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				foreach (var item in Data)
				{
					var exists = SQLiteUtility.IsExists(item.Hash);
					if (exists) continue;

					SQLiteUtility.Add(item.Hash, item.Name, DateTime.Now);

					Document document = new Document();
					Field fieldName = new Field(nameof(DataFile.Name), item.Name, Field.Store.YES, Field.Index.ANALYZED);
					Field fieldContent = new Field(nameof(DataFile.Content), item.Content, Field.Store.NO, Field.Index.ANALYZED);
					Field fieldHash = new Field(nameof(DataFile.Hash), item.Hash, Field.Store.YES, Field.Index.ANALYZED);

					document.Add(fieldName);
					document.Add(fieldContent);
					document.Add(fieldHash);
					indexWriter.AddDocument(document);
				}
				indexWriter.Optimize();
				indexWriter.Commit();
			}
		}

		public IEnumerable<string> Search(string input)
		{
			var directory = FSDirectory.Open(IOUtility.GetFolder(Config.Constant.Folder.Token));
			IndexReader indexReader = IndexReader.Open(directory, true);
			Searcher searcher = new IndexSearcher(indexReader);
			Analyzer analyzer = new StandardAnalyzer(Version);

			TopScoreDocCollector collector = TopScoreDocCollector.Create(Config.Lucene.Search.TopDocumentSize, true);

			MultiFieldQueryParser parser = new MultiFieldQueryParser(Version, new[] { nameof(DataFile.Content) }, analyzer)
			{
				AllowLeadingWildcard = Config.Lucene.Search.AllowLeadingWildcard,
			};

			if (Config.Lucene.Search.AllowLeadingWildcard)
			{
				input = "*" + input + "*";
			}

			searcher.Search(parser.Parse(input), collector);

			ScoreDoc[] hits = collector.TopDocs().ScoreDocs;
			List<string> result = new List<string>();
			foreach (var item in hits)
			{
				Document doc = searcher.Doc(item.Doc);
				result.Add(doc.Get(nameof(DataFile.Name)));
			}

			return result;
		}
	}
}

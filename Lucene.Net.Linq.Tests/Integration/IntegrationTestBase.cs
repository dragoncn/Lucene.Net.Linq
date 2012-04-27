﻿using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Linq.Mapping;
using Lucene.Net.Store;
using NUnit.Framework;
using Version = Lucene.Net.Util.Version;

namespace Lucene.Net.Linq.Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        protected LuceneDataProvider provider;
        protected Directory directory;
        protected IndexWriter writer;
        protected static readonly Version version = Version.LUCENE_29;

        [SetUp]
        public void SetUp()
        {
            directory = new RAMDirectory();
            writer = new IndexWriter(directory, GetAnalyzer(version), IndexWriter.MaxFieldLength.UNLIMITED);
            
            provider = new LuceneDataProvider(directory, writer.GetAnalyzer(), version, writer);
        }

        protected virtual Analyzer GetAnalyzer(Version version)
        {
            return new PorterStemAnalyzer(version);
        }

        public class SampleDocument
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public int Scalar { get; set; }

            [NumericField]
            public long Long { get; set; }

            public int? NullableScalar { get; set; }
            public bool Flag { get; set; }

            [Field(Converter = typeof(VersionConverter))]
            public System.Version Version { get; set; }

            [Field("backing_field")]
            public string Alias { get; set; }
        }

        protected void AddDocument(SampleDocument document)
        {
            using (var session = provider.OpenSession<SampleDocument>())
            {
                session.Add(document);
                session.Commit();
            }
        }
    }
}
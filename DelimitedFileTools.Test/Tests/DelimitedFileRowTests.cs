using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DelimitedFileTools.Models;

namespace DelimitedFileTools.Test
{
    [TestClass]
    public class DelimitedFileRowTests
    {
        private readonly byte[] m_line = Encoding.UTF8.GetBytes("111,222,\"text\",\"more text\",333,\"last text\"");

        [TestMethod]
        public void ProperlyHandlesTextQualifiers()
        {
            using (var stream = new MemoryStream(m_line))
            using (var reader = new StreamReader(stream))
            {
                var row = new DelimitedFileRow(reader, '\n', '\r', '"', ',');

                Assert.IsTrue(row.Columns.Count == 6);
            }
        }
    }
}

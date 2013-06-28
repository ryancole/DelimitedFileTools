using System;
using System.IO;
using DelimitedFileTools.Models;

namespace DelimitedFileToolsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DelimitedFile file = new DelimitedFile(args[0]);

            while (file.ReadRow())
            {
                if (file.CurrentRowNumber == 1)
                {
                    continue;
                }

                File.WriteAllText(string.Format(@"c:\users\ryan\desktop\foo_{0}.txt", file.CurrentRowNumber), file.GetColumn("text"));
            }
        }
    }
}

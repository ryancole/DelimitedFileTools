using System;
using System.IO;
using DelimitedFileTools.Models;

namespace DelimitedFileToolsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcepath = args[0];

            // there are a few static utility functions
            Console.WriteLine("The given DAT file has {0} rows.", DelimitedFile.GetRowCount(sourcepath));
            Console.WriteLine("The first row: {0}", DelimitedFile.GetFirstRow(sourcepath));

            // initialize the reader, given the file to read from and that this file a header row
            DelimitedFile file = new DelimitedFile(sourcepath, true);

            // set delimiter values (these are the defaults)
            file.NewlineCharacter = 10;
            file.TextQualifierCharacter = 254;
            file.CarriageReturnCharacter = 13;
            file.ColumnDelimiterCharacter = 20;

            // read in and handle an available line from the file
            while (file.ReadRow())
            {
                if (file.CurrentRowNumber == 1)
                {
                    Console.WriteLine("Column headers: {0}", string.Join(", ", file.CurrentRow.Columns));
                    continue;
                }
                else
                {
                    if (file.SetColumnValue("text", "example"))
                    {
                        Console.WriteLine(file.GetColumnValue("text"));
                    }
                }
            }
        }
    }
}

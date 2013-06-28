DelimitedFileTools
==================

```csharp
using System;
using System.IO;
using DelimitedFileTools.Models;

namespace DelimitedFileToolsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize the reader, given the file to read from and that this file a header row
            DelimitedFile file = new DelimitedFile(args[0], true);

            // set delimiter values (these are the defaults)
            file.NewlineCharacter = 10;
            file.TextQualifierCharacter = 254;
            file.CarriageReturnCharacter = 13;
            file.ColumnDelimiterCharacter = 20;

            // read in and handle an available line from the file
            while (file.ReadRow())
            {
                // don't print out the header row
                if (file.CurrentRowNumber == 1)
                {
                    continue;
                }

                Console.WriteLine(file.CurrentRow);
            }
        }
    }
}
```

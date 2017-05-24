using System.IO;
using System.Collections.Generic;

namespace DelimitedFileTools.Models
{
    public class DelimitedFile
    {
        private int m_currentRowNumber;
        private int m_newlineCharacter;
        private int m_textQualifierCharacter;
        private int m_carriageReturnCharacter;
        private int m_columnDelimiterCharacter;
        private string m_path;
        private bool m_hasHeaders;
        private bool m_countOnly;
        private StreamReader m_stream;
        private DelimitedFileRow m_headerRow;
        private DelimitedFileRow m_currentRow;

        public DelimitedFile(string p_path, bool p_hasHeaders = true, bool p_countOnly = false)
        {
            // save file path
            m_path = p_path;

            // keep track of which row the reader is on
            m_currentRowNumber = 0;

            // default special characters
            m_newlineCharacter = 10;
            m_textQualifierCharacter = 254;
            m_carriageReturnCharacter = 13;
            m_columnDelimiterCharacter = 20;

            // whether or not this delimited file has a header row
            m_hasHeaders = p_hasHeaders;

            // whether or not this instance is only to retrieve a row count
            m_countOnly = p_countOnly;

            // initialize the read file stream
            m_stream = new StreamReader(p_path, true);
        }

        #region Methods

        ~DelimitedFile()
        {
            m_stream.Close();
        }

        public bool ReadRow()
        {
            if (!m_stream.EndOfStream)
            {
                // read in the row data
                m_currentRow = new DelimitedFileRow(m_stream, m_newlineCharacter, m_carriageReturnCharacter, m_textQualifierCharacter, m_columnDelimiterCharacter, m_countOnly);

                // increase the row number
                m_currentRow.RowNumber = ++m_currentRowNumber;

                // if it's a header row save it
                if (m_hasHeaders == true && m_currentRowNumber == 1)
                {
                    m_headerRow = m_currentRow;
                }

                return true;
            }

            return false;
        }

        public IEnumerable<DelimitedFileRow> ReadAllRows()
        {
            while (ReadRow())
            {
                yield return CurrentRow;
            }
        }

        public int GetColumnIndex(string p_name)
        {
            int columnIndex = -1;

            for (int x = 0; x < ColumnNames.Count; x++)
            {
                if (ColumnNames[x].ToLower().Trim() == p_name.ToLower().Trim())
                {
                    columnIndex = x;
                    break;
                }
            }

            return columnIndex;
        }

        public string GetColumnValue(string p_name)
        {
            int columnIndex = GetColumnIndex(p_name);

            if (columnIndex >= 0 && columnIndex < ColumnNames.Count)
            {
                return m_currentRow.Columns[columnIndex];
            }

            return "";
        }

        public bool SetColumnValue(string p_name, string p_value)
        {
            int columnIndex = GetColumnIndex(p_name);

            if (columnIndex >= 0 && columnIndex < ColumnNames.Count)
            {
                m_currentRow.Columns[columnIndex] = p_value;
                return true;
            }

            return false;
        }

        #endregion

        #region Static Functions

        public static DelimitedFileRow GetFirstRow(string path, char column = ',', char text = '"')
        {
            DelimitedFile file = new DelimitedFile(path)
            {
                TextQualifierCharacter = text,
                ColumnDelimiterCharacter = column
            };

            if (file.ReadRow())
            {
                return file.CurrentRow;
            }

            return null;
        }

        public static IEnumerable<DelimitedFileRow> GetAllRows(string path, char column = ',', char text = '"')
        {
            var file = new DelimitedFile(path)
            {
                TextQualifierCharacter = text,
                ColumnDelimiterCharacter = column
            };

            while (file.ReadRow())
            {
                yield return file.CurrentRow;
            }
        }

        public static int GetRowCount(string path, bool headers = true, char column = ',', char text = '"')
        {
            int rowCount = 0;
            var file = new DelimitedFile(path, headers, true)
            {
                TextQualifierCharacter = text,
                ColumnDelimiterCharacter = column
            };

            while (file.ReadRow())
            {
                rowCount++;

                if (headers == true && file.CurrentRowNumber == 1)
                {
                    rowCount--;
                }
            }

            return rowCount;
        }

        #endregion

        #region Properties

        public int ColumnDelimiterCharacter
        {
            get
            {
                return m_columnDelimiterCharacter;
            }

            set
            {
                m_columnDelimiterCharacter = value;
            }
        }

        public int CarriageReturnCharacter
        {
            get
            {
                return m_carriageReturnCharacter;
            }

            set
            {
                m_carriageReturnCharacter = value;
            }
        }

        public int TextQualifierCharacter
        {
            get
            {
                return m_textQualifierCharacter;
            }

            set
            {
                m_textQualifierCharacter = value;
            }
        }

        public int NewlineCharacter
        {
            get
            {
                return m_newlineCharacter;
            }

            set
            {
                m_newlineCharacter = value;
            }
        }

        public string Path
        {
            get
            {
                return m_path;
            }
        }

        public int CurrentRowNumber
        {
            get
            {
                return m_currentRowNumber;
            }
        }

        public DelimitedFileRow CurrentRow
        {
            get
            {
                return m_currentRow;
            }
        }

        public List<string> ColumnNames
        {
            get
            {
                if (m_headerRow != null)
                {
                    return m_headerRow.Columns;
                }

                return new List<string>();
            }
        }

        #endregion
    }
}
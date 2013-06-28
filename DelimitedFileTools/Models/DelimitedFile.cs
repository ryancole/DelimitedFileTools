﻿using System;
using System.IO;
using System.Text;
using System.Linq;
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
        private StreamReader m_stream;
        private DelimitedFileRow m_headerRow;
        private DelimitedFileRow m_currentRow;

        #region Methods

        ~DelimitedFile()
        {
            m_stream.Close();
        }

        public DelimitedFile(string p_path, bool p_hasHeaders = true)
        {
            m_path = p_path;
            m_currentRowNumber = 0;
            m_newlineCharacter = 10;
            m_textQualifierCharacter = 254;
            m_carriageReturnCharacter = 13;
            m_columnDelimiterCharacter = 20;
            m_hasHeaders = p_hasHeaders;
            m_stream = new StreamReader(p_path, true);
        }

        public bool ReadRow()
        {
            if (!m_stream.EndOfStream)
            {
                // read in the row data
                m_currentRow = new DelimitedFileRow(m_stream, m_newlineCharacter, m_carriageReturnCharacter, m_textQualifierCharacter, m_columnDelimiterCharacter);

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

        public string GetColumn(string p_name)
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

            if (columnIndex >= 0 && columnIndex < ColumnNames.Count)
            {
                return m_currentRow.Columns[columnIndex];
            }

            return "";
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
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace DelimitedFileTools.Models
{
    public class DelimitedFileRow
    {
        private int m_newline;
        private int m_carriage;
        private int m_textQualifier;
        private int m_columnDelimiter;
        private int m_rowNumber;
        private List<string> m_columns;

        #region Methods

        public DelimitedFileRow(StreamReader p_reader, int p_newline, int p_carriage, int p_textqualifier, int p_columndelimiter)
        {
            int currentCharacter;
            bool isInsideTextQualifiers = false;

            m_newline = p_newline;
            m_carriage = p_carriage;
            m_textQualifier = p_textqualifier;
            m_columnDelimiter = p_columndelimiter;

            // initialize the column containers
            m_columns = new List<string>();

            // initialize the container for the column payload
            string columnPayload = "";

            while ((currentCharacter = p_reader.Read()) != -1)
            {
                if (currentCharacter == p_textqualifier)
                {
                    isInsideTextQualifiers = !isInsideTextQualifiers;
                    continue;
                }
                else if (currentCharacter == p_columndelimiter && isInsideTextQualifiers == false)
                {
                    // add the column payload to the collection
                    m_columns.Add(columnPayload);

                    // reset the column payload
                    columnPayload = "";

                    // move to the next byte
                    continue;
                }
                else if ((currentCharacter == p_carriage || currentCharacter == p_newline) && isInsideTextQualifiers == false)
                {
                    if (currentCharacter == p_newline)
                    {
                        // add the column payload to the collection
                        m_columns.Add(columnPayload);

                        // we are done with this row
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    columnPayload += Convert.ToChar(currentCharacter);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("<DelimitedFileRow number={0} columns={1}>", m_rowNumber, m_columns.Count);
        }

        #endregion

        #region Properties

        public int RowNumber
        {
            get
            {
                return m_rowNumber;
            }

            set
            {
                m_rowNumber = value;
            }
        }

        public List<string> Columns
        {
            get
            {
                return m_columns;
            }
        }

        public List<string> RawColumns
        {
            get
            {
                char textQualifier = Convert.ToChar(m_textQualifier);
                return new List<string>(m_columns.Select(x => string.Format("{0}{1}{0}", textQualifier, x)));
            }
        }

        public string RawRow
        {
            get
            {
                char columnDelimiter = Convert.ToChar(m_columnDelimiter);
                return string.Join(Convert.ToString(columnDelimiter), RawColumns);
            }
        }

        #endregion
    }
}
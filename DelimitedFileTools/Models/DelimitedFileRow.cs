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

        public DelimitedFileRow(StreamReader p_reader, int p_newline, int p_carriage, int p_textqualifier, int p_columndelimiter, bool p_countOnly = false)
        {
            int nextCharacter = -1;
            int currentCharacter = -1;
            int previousCharacter = -1;

            bool isInsideTextQualifiers = false;

            m_newline = p_newline;
            m_carriage = p_carriage;
            m_textQualifier = p_textqualifier;
            m_columnDelimiter = p_columndelimiter;

            // initialize the column containers
            m_columns = new List<string>();

            // initialize the container for the column payload
            string columnPayload = "";

            do
            {
                // adjust the characters so we can see previous and next character
                previousCharacter = currentCharacter;
                currentCharacter = nextCharacter;
                nextCharacter = p_reader.Read();

                // continue hoping that we get data
                if (currentCharacter == -1)
                {
                    continue;
                }

                if (currentCharacter == p_textqualifier)
                {
                    // we're currently looking at a text qualifier value. if we are not currently inside of a text qualifier value, we
                    // may need to toggle to a text quailifier state. text qualifiers should be the start of the column value. we can
                    // verify start of column value by look at previous character, and seeing if its a new row, or a column delimiter.
                    if (isInsideTextQualifiers == false && (previousCharacter == -1 || previousCharacter == m_columnDelimiter))
                    {
                        isInsideTextQualifiers = true;
                        continue;
                    }

                    // we're currently looking at a text qualifier, while being inside a text qualified column, and the next character
                    // appears to be the end of the file. so, lets add this column value.
                    else if (isInsideTextQualifiers == true && nextCharacter == -1)
                    {
                        m_columns.Add(columnPayload);
                        break;
                    }

                    // we're currently looking at a text qualifier value. if we are already inside of a text qualifier column, we may need
                    // to exit the text qualifier state. column values may also contain text qualifier characters inside of the actual
                    // column value, so we need to confirm that we're able to exit the text qualifier column by inspecting the next value.
                    else if (isInsideTextQualifiers == true && (nextCharacter == m_columnDelimiter || nextCharacter == m_carriage || nextCharacter == m_newline))
                    {
                        isInsideTextQualifiers = false;
                        continue;
                    }

                    // if we're looking at a text qualifier value, but we're also inside of a text qualifier column, and it does not appear
                    // that the column is ready to close, we may be looking at an actual content of the column itself, so lets add the char
                    // to the payload.
                    else if (isInsideTextQualifiers == true && p_countOnly == false)
                    {
                        columnPayload += Convert.ToChar(currentCharacter);
                    }
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
                    if (currentCharacter == p_newline || (currentCharacter == p_carriage && nextCharacter == p_newline))
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
                    if (p_countOnly == false)
                    {
                        columnPayload += Convert.ToChar(currentCharacter);
                    }
                }
            }
            while (nextCharacter != -1);
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
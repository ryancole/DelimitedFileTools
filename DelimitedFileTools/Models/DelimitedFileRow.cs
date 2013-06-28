using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace DelimitedFileTools.Models
{
    public class DelimitedFileRow
    {
        private int m_rowNumber;
        private List<string> m_columns;

        #region Methods

        public DelimitedFileRow(StreamReader p_reader, int p_newline, int p_carriage, int p_textqualifier, int p_columndelimiter)
        {
            int currentCharacter;
            bool isInsideTextQualifiers = false;

            // initialize the column containers
            m_columns = new List<string>();

            // initialize the container for the column payload
            string columnPayload = "";

            while ((currentCharacter = p_reader.Read()) != null)
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

        #endregion
    }
}
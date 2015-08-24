// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller,  
 

using Edu.Wisc.Forest.Flel.Util;
using Landis.Extension.BaseHarvest;
using System.Text;

using BaseHarvest = Landis.Extension.BaseHarvest;

namespace Landis.Extension.BiomassHarvest
{
    /// <summary>
    /// Static class for partial thinning.
    /// </summary>
    public static class PartialThinning
    {
        public static InputValueException MakeInputValueException(string value,
                                                                  string message)
        {
            return new InputValueException(value,
                                           string.Format("\"{0}\" is not a valid percentage for partial thinning", value),
                                           new MultiLineText(message));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a percentage for partial thinning of a cohort age or age
        /// range.
        /// </summary>
        /// <remarks>
        /// The percentage is bracketed by parentheses.
        /// </remarks>
        public static InputValue<Percentage> ReadPercentage(StringReader reader,
                                                            out int      index)
        {
            TextReader.SkipWhitespace(reader);
            index = reader.Index;

            //  Read left parenthesis
            int nextChar = reader.Peek();
            if (nextChar == -1)
                throw new InputValueException();  // Missing value
            if (nextChar != '(')
                throw MakeInputValueException(TextReader.ReadWord(reader),
                                              "Value does not start with \"(\"");
            StringBuilder valueAsStr = new StringBuilder();
            valueAsStr.Append((char) (reader.Read()));

            //  Read whitespace between '(' and percentage
            valueAsStr.Append(ReadWhitespace(reader));

            //  Read percentage
            string word = ReadWord(reader, ')');
            if (word == "")
                throw MakeInputValueException(valueAsStr.ToString(),
                                              "No percentage after \"(\"");
            valueAsStr.Append(word);
            Percentage percentage;
            try {
                percentage = Percentage.Parse(word);
            }
            catch (System.FormatException exc) {
                throw MakeInputValueException(valueAsStr.ToString(),
                                              exc.Message);
            }
            if (percentage < 0.0 || percentage > 1.0)
                throw MakeInputValueException(valueAsStr.ToString(),
                                              string.Format("{0} is not between 0% and 100%", word));

            //  Read whitespace and ')'
            valueAsStr.Append(ReadWhitespace(reader));
            char? ch = TextReader.ReadChar(reader);
            if (! ch.HasValue)
                throw MakeInputValueException(valueAsStr.ToString(),
                                              "Missing \")\"");
            valueAsStr.Append(ch.Value);
            if (ch != ')')
                throw MakeInputValueException(valueAsStr.ToString(),
                                              string.Format("Value ends with \"{0}\" instead of \")\"", ch));

            return new InputValue<Percentage>(percentage, valueAsStr.ToString());
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads whitespace from a string reader.
        /// </summary>
        public static string ReadWhitespace(StringReader reader)
        {
            StringBuilder whitespace = new StringBuilder();
            int i = reader.Peek();
            while (i != -1 && char.IsWhiteSpace((char) i)) {
                whitespace.Append((char) reader.Read());
                i = reader.Peek();
            }
            return whitespace.ToString();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a word from a string reader.
        /// </summary>
        /// <remarks>
        /// The word is terminated by whitespace, the end of input, or a
        /// particular delimiter character.
        /// </remarks>
        public static string ReadWord(StringReader reader,
                                      char         delimiter)
        {
            StringBuilder word = new StringBuilder();
            int i = reader.Peek();
            while (i != -1 && ! char.IsWhiteSpace((char) i) && i != delimiter) {
                word.Append((char) reader.Read());
                i = reader.Peek();
            }
            return word.ToString();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Handler for events when an age or range and its associated
        /// percentage have been read.
        /// </summary>
        public delegate void ReadEventHandler(AgeRange   ageRange,
                                              Percentage percentage);

        //---------------------------------------------------------------------

        /// <summary>
        /// An event when a percentage for partial thinning is found after a
        /// a cohort age or a range of ages.
        /// </summary>
        public static event ReadEventHandler ReadAgeOrRangeEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a cohort age or a range of ages (format: age-age) followed
        /// by an optional percentage for partial thinning.
        /// </summary>
        /// <remarks>
        /// The optional percentage is bracketed by parenthesis.
        /// </remarks>
        public static InputValue<AgeRange> ReadAgeOrRange(StringReader reader,
                                                          out int      index)
        {
            TextReader.SkipWhitespace(reader);
            index = reader.Index;

            string word = ReadWord(reader, '(');
            if (word == "")
                throw new InputValueException();  // Missing value

            AgeRange ageRange = BaseHarvest.InputParametersParser.ParseAgeOrRange(word);

            //  Does a percentage follow?
            TextReader.SkipWhitespace(reader);
            if (reader.Peek() == '(') {
                int ignore;
                InputValue<Percentage> percentage = ReadPercentage(reader, out ignore);
                if (ReadAgeOrRangeEvent != null)
                    ReadAgeOrRangeEvent(ageRange, percentage.Actual);
            }
            else {
                if (ReadAgeOrRangeEvent != null)
                    ReadAgeOrRangeEvent(ageRange, null);
            }

            return new InputValue<AgeRange>(ageRange, word);
        }
    }
}

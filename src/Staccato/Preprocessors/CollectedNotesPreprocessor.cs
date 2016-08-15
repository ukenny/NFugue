﻿using Staccato.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Staccato.Preprocessors
{
    public class CollectedNotesPreprocessor : IPreprocessor
    {
        private static readonly Regex parenPattern = new Regex("\\([^\\)]*\\)\\S", RegexOptions.Compiled);

        public string Preprocess(string s, StaccatoParserContext context)
        {
            var buddy = new StringBuilder();
            int posStart = 0;

            foreach (Match match in parenPattern.Matches(s))
            {
                // First, add the text that occurs before the parenthesis group starts. That text is
                // meant to be added to the result without any modifications.
                int posStartOfGroup = match.Index;
                string sub = s.Substring(posStart, posStartOfGroup - posStart);
                buddy.Append(sub);
                posStart = s.FindNextOrEnd(' ', match.Index + match.Length);

                // Now, get the notes that are collected between parentheses. The "replicand" is the
                // thing that immediately follows the closing parenthesis and is the thing that should
                // be applied to each note within the parentheses.
                int posCloseParen = s.IndexOf(')', posStartOfGroup);
                string replicand = s.Substring(posCloseParen + 1,
                    s.FindNextOrEnd(new List<char> { ' ', '+' }, posCloseParen + 1) - posCloseParen - 1);
                string parenContents = s.Substring(posStartOfGroup + 1, posCloseParen - posStartOfGroup - 1);

                // Split the items in parentheses
                int subindex = 0;
                while (subindex < parenContents.Length)
                {
                    var posSomething = parenContents.FindNextOrEnd(new List<char> { ' ', '+' }, subindex);
                    buddy.Append(parenContents.Substring(subindex, posSomething - subindex));
                    buddy.Append(replicand);
                    if (posSomething != parenContents.Length)
                    {
                        buddy.Append(parenContents.Substring(posSomething, 1));
                    }
                    subindex = posSomething + 1;
                }
            }
            buddy.Append(s.Substring(posStart, s.Length - posStart));
            return buddy.ToString();

        }
    }
}
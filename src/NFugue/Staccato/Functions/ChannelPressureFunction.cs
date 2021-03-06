﻿using System.Collections.Generic;

namespace NFugue.Staccato.Functions
{
    public class ChannelPressureFunction : ISubparserFunction
    {
        public IEnumerable<string> GetNames()
        {
            return new[] { "CHANNELPRESSURE", "CP" };
        }

        public void Apply(string parameters, StaccatoParserContext context)
        {
            var splittedParams = parameters.Split(',');
            if (splittedParams.Length == 1)
            {
                context.Parser.OnChannelPressureParsed(int.Parse(splittedParams[0].Trim()));
            }
        }
    }
}
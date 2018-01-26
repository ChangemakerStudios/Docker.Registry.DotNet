using System;
using System.Collections.Generic;
using System.Linq;

namespace Docker.Registry.DotNet.Authentication
{
    internal static class AuthenticateParser
    {
        public static IDictionary<string, string> Parse(string value)
        {
            //https://stackoverflow.com/questions/45516717/extracting-and-parsing-the-www-authenticate-header-from-httpresponsemessage-in/45516809#45516809
            string[] commaSplit = value.Split(',');

            return commaSplit
                .ToDictionary(GetKey, GetValue);
        }

        public static ParsedAuthentication ParseTyped(string value)
        {
            var parsed = Parse(value);

            return new ParsedAuthentication(
                parsed.GetValueOrDefault("realm"),
                parsed.GetValueOrDefault("service"),
                parsed.GetValueOrDefault("scope"));
        }

        private static string GetKey(string pair)
        {
            int equalPos = pair.IndexOf("=");

            if (equalPos < 1)
                throw new FormatException("No '=' found.");

            return pair.Substring(0, equalPos);
        }

        private static string GetValue(string pair)
        {
            int equalPos = pair.IndexOf("=");

            if (equalPos < 1)
                throw new FormatException("No '=' found.");

            string value = pair.Substring(equalPos + 1).Trim();

            //Trim quotes
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
            }

            return value;
        }
    }
}
using System.Collections.Generic;

namespace IndexServer.Tokens
{
    public interface ITokenizer
    {
        /// <summary>
        /// Tokenizes the given string and returns a sequence of (token, offset) pairs.
        /// </summary>
        IEnumerable<(string, int)> Tokenize(string value);
    }
}

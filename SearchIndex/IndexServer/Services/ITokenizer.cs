using System.Collections.Generic;

namespace IndexServer.Services
{
    public interface ITokenizer
    {
        /// <summary>
        /// Tokenizes the given string to return a sequence of the pairs of token and its offset to the original string.
        /// </summary>
        IEnumerable<(string, int)> Tokenize(string value);
    }
}

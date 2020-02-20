//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.BizQA.NLU.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    public class GeneralSpeller : ISpeller, IDisposable
    {
        private const double DefaultCorrectionFactor = 2.5;
        private const uint DefaultMergeFactor = 3;
        private const uint DefaultBaseUnigramFrequency = 10;
        private const uint DefaultBaseBigramFrequency = 10;
        private const uint MinWordLenToRunOfficeSpeller = 3;

        private static readonly List<char> s_separator = new List<char> { ',', '\'', '"', '?', ')', '.', ';', '(', '=', '|', '[', ']', '_', '#', '@' };
        private static readonly List<char> s_hyphen = new List<char> { '-', '.', '/' };
        private static readonly Regex s_lastWordRegex = new Regex(@"(\w+)\W*$");
        private static readonly Regex s_firstWordRegex = new Regex(@"^\W*(\w+)");

        private readonly IDictionary<string, uint> _unigram;
        private readonly IDictionary<string, uint> _bigram;
        private readonly bool _backoffToCommonNgram;
        private readonly IDictionary<string, uint> _commonUnigram;
        private readonly IDictionary<string, uint> _commonBigram;
        private readonly HashSet<string> _blacklists;
        private readonly IDictionary<string, IList<string>> _whitelists;
        private readonly ITokenizer _tokenizer;

        private readonly double _correctionFactor = DefaultCorrectionFactor;
        private readonly double _mergeFactor = DefaultMergeFactor;
        private readonly double _singleTokenMultiplier = 2.0;

        // use edit distance to filter candidates from Office Speller
        // for longer text, we allow more edits, for text with length > 8, we don't limit
        private readonly int[] _maxEditDistanceByLength = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2 };
        private readonly int _maxCandidatesFromOfficeSpeller = 3;

        public GeneralSpeller(QueryRewriteSpellerOptions spellerOptions, ITokenizer tokenizer)
        {
            _unigram = CommonFileLoader.LoadFileAsDic<uint>(spellerOptions.UnigramFilePath);
            _bigram = CommonFileLoader.LoadFileAsDic<uint>(spellerOptions.BigramFilePath);
            _backoffToCommonNgram = spellerOptions.BackoffToCommonNgram;
            _commonUnigram = _backoffToCommonNgram ? CommonFileLoader.LoadFileAsDic<uint>(spellerOptions.CommonUnigramFilePath) : new Dictionary<string, uint>();
            _commonBigram = _backoffToCommonNgram ? CommonFileLoader.LoadFileAsDic<uint>(spellerOptions.CommonBigramFilePath) : new Dictionary<string, uint>();
            if (File.Exists(spellerOptions.BlacklistFilePath))
            {
                _blacklists = CommonFileLoader.LoadFileAsHashSet(spellerOptions.BlacklistFilePath);
            }
            else
            {
                _blacklists = new HashSet<string>();
            }

            if (File.Exists(spellerOptions.WhitelistFilePath))
            {
                _whitelists = CommonFileLoader.LoadFileAsDicList(spellerOptions.WhitelistFilePath, true);
            }
            else
            {
                _whitelists = new Dictionary<string, IList<string>>();
            }

            _tokenizer = tokenizer;
        }

        public string SpellCorrect(string text)
        {
            var result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = Concat(text);
                var flaggedTokens = OfficeSpellerWrapper.Correct(text);
                var start = 0;
                foreach (var token in flaggedTokens)
                {
                    var tokenOffset = (int)token.Offset;
                    //// tokenOffset smaller than start means we have already processed the token with previuos tokens together
                    //// in this case, we can simply ingore the token correction. otherwise, it would get failed with query below:
                    //// "After the Installation of an .Net-Winword-Addin with VSTO created in VB.Net with VS 2008"
                    if (tokenOffset < start)
                    {
                        continue;
                    }

                    var tokenString = token.Token;
                    var tokenLen = tokenString.Length;
                    var candidates = new List<string>();
                    if (_whitelists.ContainsKey(tokenString))
                    {
                        candidates.AddRange(_whitelists[tokenString]);
                    }

                    candidates.AddRange(token.Suggestions);
                    var candidate = tokenString;
                    if (!DoNotCorrect(tokenString, false) && !IsHyphen(text, tokenOffset - 1))
                    {
                        var pre = GetPrevWord(text, tokenOffset - 1);
                        var post = GetNextWord(text, tokenOffset + tokenLen);
                        candidate = GetBestCandidate(pre, post, candidates, candidate);
                    }

                    result.Append(text.Substring(start, tokenOffset - start));
                    start += tokenOffset - start;
                    result.Append(candidate);
                    start += tokenLen;
                }

                result.Append(text.Substring(start));
            }

            return result.ToString();
        }

        public List<QueryRewriteToken> SpellCorrect(IReadOnlyList<QueryRewriteToken> inputTokens)
        {
            var mergedTokens = MergeTokens(inputTokens);
            var correctedTokens = CandidateSelectCorrect(mergedTokens);

            return correctedTokens;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        private static string GetPrevWord(string text, int end)
        {
            if (string.IsNullOrWhiteSpace(text) || end < 0)
            {
                return string.Empty;
            }

            var subStr = text.Substring(0, end + 1);
            var lastWordMatch = s_lastWordRegex.Match(subStr);
            if (lastWordMatch.Success)
            {
                return lastWordMatch.Groups[1].Value;
            }

            return string.Empty;
        }

        private static string GetNextWord(string text, int start)
        {
            if (string.IsNullOrWhiteSpace(text) || start >= text.Length)
            {
                return string.Empty;
            }

            var subStr = text.Substring(start);
            var firstWordMatch = s_firstWordRegex.Match(subStr);
            if (firstWordMatch.Success)
            {
                return firstWordMatch.Groups[1].Value;
            }

            return string.Empty;
        }

        private static bool IsHyphen(string sentence, int index)
        {
            return index >= 0 && s_hyphen.Contains(sentence[index]);
        }

        private static bool IsCapitalWord(string word, bool initCap)
        {
            return initCap ? (word.Length > 0 && !char.IsLower(word[0]))
                : word.All(ch => !char.IsLower(ch));
        }

        private static bool HasSeparator(string token)
        {
            return token.Any(ch => s_separator.Contains(ch));
        }

        private static IEnumerable<string> SentenceSegmentation(string message, bool includeSegmentation)
        {
            var sentences = new List<string>();

            var startIndex = 0;
            for (var i = 0; i < message.Length; i++)
            {
                var ch = message[i];
                if (ch == '\r' || ch == '\n' || ch == ',' || ch == '!' || ch == ':' || ch == '?'
                    || (ch == '.' && (i == message.Length - 1 || char.IsWhiteSpace(message[i + 1]))))
                {
                    var sentence = includeSegmentation ? message.Substring(startIndex, i - startIndex + 1) : message.Substring(startIndex, i - startIndex);
                    if (sentence.Length > 0)
                    {
                        sentences.Add(sentence);
                    }

                    startIndex = i + 1;
                }
            }

            if (startIndex < message.Length)
            {
                sentences.Add(message.Substring(startIndex, message.Length - startIndex));
            }

            return sentences;
        }

        private uint GetHighestBigramScore(string pre, string post, string candidate, IDictionary<string, uint> bigram)
        {
            uint bigramScorePreCur = string.IsNullOrWhiteSpace(pre) ? 0 : bigram.GetValueOrDefault($"{pre} {candidate}", 0U);
            uint bigramScoreCurPost = string.IsNullOrWhiteSpace(post) ? 0 : bigram.GetValueOrDefault($"{candidate} {post}", 0U);

            return Math.Max(bigramScorePreCur, bigramScoreCurPost);
        }

        private string GetBestCandidate(string pre, string post, IEnumerable<string> candidates, string token)
        {
            var bestCandidate = token;
            bool isSingleToken = (string.IsNullOrWhiteSpace(pre) || pre.Any(c => char.IsDigit(c))
                    || (!_unigram.ContainsKey(pre) && (!_backoffToCommonNgram || !_commonUnigram.ContainsKey(pre))))
                && (string.IsNullOrWhiteSpace(post) || post.Any(c => char.IsDigit(c))
                    || (!_unigram.ContainsKey(post) && (!_backoffToCommonNgram || !_commonUnigram.ContainsKey(post))));
            if (isSingleToken)
            {
                // if it's single token, compare unigram only, but correct only when the token doens't exist in unigram
                if (!_unigram.ContainsKey(token))
                {
                    bestCandidate = CorrectWithUnigram(candidates, token, _unigram, _bigram, _correctionFactor * _singleTokenMultiplier);
                    if (_backoffToCommonNgram && string.Equals(bestCandidate, token, StringComparison.OrdinalIgnoreCase))
                    {
                        bestCandidate = CorrectWithUnigram(candidates, token, _commonUnigram, _commonBigram, _correctionFactor * _singleTokenMultiplier);
                    }
                }
            }
            else
            {
                // if it's not single token, compare both unigram and bigram
                bestCandidate = CorrectWithUniBigram(pre, post, candidates, token, _unigram, _bigram, _correctionFactor);
                if (_backoffToCommonNgram && string.Equals(bestCandidate, token, StringComparison.OrdinalIgnoreCase))
                {
                    var unigramScore = _unigram.GetValueOrDefault(token, 0U);
                    var bigramScore = GetHighestBigramScore(pre, post, token, _bigram);
                    if (unigramScore == 0 && bigramScore == 0)
                    {
                        // try to compare with common ngram if domain ngram for token is not found and the switch is on
                        bestCandidate = CorrectWithUniBigram(pre, post, candidates, token, _commonUnigram, _commonBigram, _correctionFactor);
                    }
                }
            }

            return bestCandidate.ToLowerInvariant();
        }

        private string CorrectWithUnigram(
            IEnumerable<string> candidates,
            string token,
            IDictionary<string, uint> unigram,
            IDictionary<string, uint> bigram,
            double baseCorrectionFactor)
        {
            var bestCandidate = token;
            var bestUnigramScore = Math.Max(unigram.GetValueOrDefault(token, 0U), DefaultBaseUnigramFrequency);
            var correctionFactor = baseCorrectionFactor;

            foreach (var candidate in candidates)
            {
                uint unigramScore = 0;

                // sometimes candidate is a bigram
                if (bigram != null && (candidate.Contains(" ") || candidate.Contains("'")))
                {
                    unigramScore = bigram.GetValueOrDefault(candidate, 0U);
                }
                else
                {
                    unigramScore = unigram.GetValueOrDefault(candidate, 0U);
                }

                if (unigramScore > bestUnigramScore * correctionFactor)
                {
                    bestCandidate = candidate;
                    break;
                }
                else
                {
                    // penalty for candidates at lower position
                    correctionFactor += baseCorrectionFactor;
                }
            }

            return bestCandidate;
        }

        private string CorrectWithUniBigram(
            string pre,
            string post,
            IEnumerable<string> candidates,
            string token,
            IDictionary<string, uint> unigram,
            IDictionary<string, uint> bigram,
            double baseCorrectionFactor)
        {
            var bestCandidate = token;
            var bestUnigramScore = Math.Max(unigram.GetValueOrDefault(token, 0U), DefaultBaseUnigramFrequency);
            var bestBigramScore = Math.Max(GetHighestBigramScore(pre, post, token, bigram), DefaultBaseBigramFrequency);
            var correctionFactor = baseCorrectionFactor;

            foreach (var candidate in candidates)
            {
                uint unigramScore = 0;
                uint bigramScore = 0;

                // sometimes candidate is a bigram
                if (candidate.Contains(" ") || candidate.Contains("'"))
                {
                    unigramScore = bigram.GetValueOrDefault(candidate, 0U);
                    bigramScore = unigramScore;
                }
                else
                {
                    unigramScore = unigram.GetValueOrDefault(candidate, 0U);
                    bigramScore = GetHighestBigramScore(pre, post, candidate, bigram);
                }

                if (unigramScore > bestUnigramScore * correctionFactor && bigramScore > bestBigramScore * correctionFactor)
                {
                    bestUnigramScore = unigramScore;
                    bestBigramScore = bigramScore;
                    bestCandidate = candidate;
                    correctionFactor = baseCorrectionFactor;
                }
                else
                {
                    // penalty for candidates at lower position
                    correctionFactor += baseCorrectionFactor;
                }
            }

            return bestCandidate;
        }

        private string Concat(string sentence)
        {
            foreach (var sen in SentenceSegmentation(sentence, false))
            {
                var words = sen.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 1; i < words.Length; i++)
                {
                    if (words[i - 1].All(c => char.IsDigit(c)) || words[i].All(c => char.IsDigit(c)))
                    {
                        continue;
                    }

                    var scorePrevWord = _unigram.GetValueOrDefault(words[i - 1], 0U);
                    var scoreCurWord = _unigram.GetValueOrDefault(words[i], 0U);
                    var wordUni = $"{words[i - 1]}{words[i]}";
                    var scoreUni = _unigram.ContainsKey(wordUni) ? _unigram[wordUni] : 0;

                    // don't merge if either tokens are of high-frequency, e.g. "show me an ..." to "show mean ..."
                    if (scorePrevWord >= scoreUni * _mergeFactor || scoreCurWord >= scoreUni * _mergeFactor)
                    {
                        continue;
                    }

                    var wordBi = $"{words[i - 1]} {words[i]}";
                    var scoreBi = _bigram.ContainsKey(wordBi) ? _bigram[wordBi] : 0;
                    var scoreNextUni = 0U;
                    if (i < words.Length - 1
                        && !string.IsNullOrWhiteSpace(words[i + 1])
                        && !words[i + 1].All(c => char.IsDigit(c)))
                    {
                        var uniTokenNext = $"{words[i]}{words[i + 1]}";
                        scoreNextUni = _unigram.GetValueOrDefault(uniTokenNext, 0U);
                    }

                    if (scoreUni > scoreBi * _mergeFactor && scoreUni > scoreNextUni)
                    {
                        var regex = new Regex($"{words[i - 1]}\\s+{words[i]}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        sentence = regex.Replace(sentence, wordUni);
                    }
                }
            }

            return sentence;
        }

        private List<QueryRewriteToken> MergeTokens(IReadOnlyList<QueryRewriteToken> inputTokens)
        {
            var mergedTokens = new List<QueryRewriteToken>();
            QueryRewriteToken prevToken = null;
            for (int i = 0; i < inputTokens.Count; i++)
            {
                var curToken = inputTokens[i];

                // do not merge all digit tokens, for example "9 11" to "911"
                if (DoNotCorrect(curToken, false)
                    || ((prevToken != null) && DoNotCorrect(prevToken, false))
                    || curToken.Text.All(c => char.IsDigit(c)))
                {
                    if (prevToken != null)
                    {
                        mergedTokens.Add(prevToken);
                        prevToken = null;
                    }

                    mergedTokens.Add(curToken);
                }
                else if (prevToken == null)
                {
                    prevToken = curToken;
                }
                else
                {
                    var scorePrevToken = _unigram.GetValueOrDefault(prevToken.Text, 0U);
                    var scoreCurToken = _unigram.GetValueOrDefault(curToken.Text, 0U);
                    var uniToken = $"{prevToken.Text}{curToken.Text}";
                    var scoreUni = _unigram.GetValueOrDefault(uniToken, 0U);

                    // don't merge if either token is of high-frequency, e.g. "show me an ..." to "show mean ..."
                    if (scorePrevToken >= scoreUni * _mergeFactor || scoreCurToken >= scoreUni * _mergeFactor)
                    {
                        mergedTokens.Add(prevToken);
                        prevToken = curToken;
                        continue;
                    }

                    var biToken = $"{prevToken.Text} {curToken.Text}";
                    var scoreBi = _bigram.GetValueOrDefault(biToken, 0U);
                    var scoreNextUni = 0U;
                    if (i < inputTokens.Count - 1
                        && !string.IsNullOrWhiteSpace(inputTokens[i + 1].Text)
                        && !inputTokens[i + 1].Text.All(c => char.IsDigit(c)))
                    {
                        var uniTokenNext = $"{curToken.Text}{inputTokens[i + 1].Text}";
                        scoreNextUni = _unigram.GetValueOrDefault(uniTokenNext, 0U);
                    }

                    if (scoreUni > scoreBi * _mergeFactor && scoreUni > scoreNextUni)
                    {
                        var mergedStartIndex = Math.Min(prevToken.Start, curToken.Start);
                        var mergedLength = Math.Max(prevToken.End, curToken.End) - mergedStartIndex;
                        mergedTokens.Add(new QueryRewriteToken(uniToken, mergedStartIndex, mergedLength));
                        prevToken = null;
                    }
                    else
                    {
                        mergedTokens.Add(prevToken);
                        prevToken = curToken;
                    }
                }
            }

            if (prevToken != null)
            {
                mergedTokens.Add(prevToken);
            }

            return mergedTokens;
        }

        // here we only split into two words
        private List<QueryRewriteToken> SplitTokens(IReadOnlyList<QueryRewriteToken> inputTokens)
        {
            var splitTokens = new List<QueryRewriteToken>();
            for (int i = 0; i < inputTokens.Count; i++)
            {
                var curToken = inputTokens[i];
                if (DoNotCorrect(curToken, true)
                    || _unigram.ContainsKey(curToken.Text))
                {
                    splitTokens.Add(curToken);
                    continue;
                }

                var splitCandidates = new List<Tuple<string, string, uint, uint>>();
                GetSplitCandidates(curToken.Text, _unigram, _bigram, splitCandidates, 0);
                if (!splitCandidates.Any() && _backoffToCommonNgram && !_commonUnigram.ContainsKey(curToken.Text))
                {
                    GetSplitCandidates(curToken.Text, _commonUnigram, _commonBigram, splitCandidates, 0);
                }

                if (splitCandidates.Any())
                {
                    var bestCandIdx = 0;
                    var maxUniScore = splitCandidates[bestCandIdx].Item3;
                    var maxBiScore = splitCandidates[bestCandIdx].Item4;
                    for (int j = 1; j < splitCandidates.Count; j++)
                    {
                        // compare bigram score first, if equal, compare unigram score
                        if (splitCandidates[j].Item4 > maxBiScore
                            || (splitCandidates[j].Item4 == maxBiScore && splitCandidates[j].Item3 > maxUniScore))
                        {
                            maxBiScore = splitCandidates[j].Item4;
                            maxUniScore = splitCandidates[j].Item3;
                            bestCandIdx = j;
                        }
                    }

                    var left = splitCandidates[bestCandIdx].Item1;
                    var right = splitCandidates[bestCandIdx].Item2;
                    if (curToken.Text.Length == curToken.Length)
                    {
                        splitTokens.Add(new QueryRewriteToken(left, curToken.Start, left.Length));
                        splitTokens.Add(new QueryRewriteToken(right, curToken.Start + left.Length, right.Length));
                    }
                    else
                    {
                        splitTokens.Add(new QueryRewriteToken(left, curToken.Start, curToken.Length));
                        splitTokens.Add(new QueryRewriteToken(right, curToken.Start, curToken.Length));
                    }
                }
                else
                {
                    splitTokens.Add(curToken);
                }
            }

            return splitTokens;
        }

        private void GetSplitCandidates(
            string tokenText,
            IDictionary<string, uint> unigram,
            IDictionary<string, uint> bigram,
            IList<Tuple<string, string, uint, uint>> splitCandidates,
            uint bigramScoreThreshold)
        {
            for (int j = 1; j < tokenText.Length - 1; j++)
            {
                var left = tokenText.Substring(0, j);
                var leftUniScore = unigram.GetValueOrDefault(left, 0U);
                if (leftUniScore > 0)
                {
                    var right = tokenText.Substring(j);
                    var rightUniScore = unigram.GetValueOrDefault(right, 0U);
                    if (rightUniScore > 0)
                    {
                        // it may help to look at previous and next tokens, here we use simple way
                        var biScore = bigram.GetValueOrDefault($"{left} {right}", 0U);
                        if (biScore > bigramScoreThreshold)
                        {
                            splitCandidates.Add(new Tuple<string, string, uint, uint>(left, right, leftUniScore + rightUniScore, biScore));
                        }
                    }
                }
            }
        }

        private List<QueryRewriteToken> CandidateSelectCorrect(IReadOnlyList<QueryRewriteToken> inputTokens)
        {
            var correctedTokens = new List<QueryRewriteToken>();
            for (int i = 0; i < inputTokens.Count; i++)
            {
                if (DoNotCorrect(inputTokens[i], false))
                {
                    correctedTokens.Add(inputTokens[i]);
                    continue;
                }

                var tokenText = inputTokens[i].Text;
                var flaggedTokens = tokenText.Length < MinWordLenToRunOfficeSpeller ? new List<FlaggedToken>() : OfficeSpellerWrapper.Correct(tokenText);
                flaggedTokens = FilterFlaggedTokens(flaggedTokens);
                var whitelistCandidates = _whitelists.GetValueOrDefault(tokenText, new List<string>());
                if (!whitelistCandidates.Any() && !flaggedTokens.Any())
                {
                    // no candiate
                    correctedTokens.Add(inputTokens[i]);
                }
                else if (whitelistCandidates.Any() || flaggedTokens.Any(t => t.Offset == 0 && t.Token.Length == tokenText.Length))
                {
                    // candidate(s) for whole token text, most corrections should happen here
                    var candidates = new List<string>(whitelistCandidates);
                    foreach (var flaggedToken in flaggedTokens.Where(t => t.Offset == 0 && t.Token.Length == tokenText.Length))
                    {
                        candidates.AddRange(flaggedToken.Suggestions);
                    }

                    var prev = i > 0 ? inputTokens[i - 1].Text : string.Empty;
                    var post = i < inputTokens.Count - 1 ? inputTokens[i + 1].Text : string.Empty;
                    var bestCandidate = GetBestCandidate(prev, post, candidates, tokenText);
                    if (string.Equals(bestCandidate, tokenText, StringComparison.OrdinalIgnoreCase))
                    {
                        correctedTokens.Add(inputTokens[i]);
                    }
                    else
                    {
                        var candidateTokens = _tokenizer.Tokenize(bestCandidate);
                        foreach (var candidateToken in candidateTokens)
                        {
                            correctedTokens.Add(new QueryRewriteToken(candidateToken, inputTokens[i].Start, inputTokens[i].Length));
                        }
                    }
                }
                else
                {
                    // candidates for part of token text
                    var result = new StringBuilder();
                    int start = 0;
                    foreach (var flaggedToken in flaggedTokens)
                    {
                        int flaggedOffset = (int)flaggedToken.Offset;
                        //// tokenOffset smaller than start means we have already processed the token with previuos tokens together
                        //// in this case, we can simply ingore the token correction. otherwise, it would get failed with query below:
                        //// "After the Installation of an .Net-Winword-Addin with VSTO created in VB.Net with VS 2008"
                        if (flaggedOffset < start)
                        {
                            continue;
                        }

                        var flaggedText = flaggedToken.Token;
                        if (DoNotCorrect(flaggedText, false))
                        {
                            continue;
                        }

                        var candidates = new List<string>();
                        if (_whitelists.ContainsKey(flaggedText))
                        {
                            candidates.AddRange(_whitelists[flaggedText]);
                        }

                        candidates.AddRange(flaggedToken.Suggestions);
                        var prev = string.Empty;
                        if (flaggedToken.Offset > 0)
                        {
                            prev = GetPrevWord(tokenText, flaggedOffset - 1);
                        }
                        else if (i > 0)
                        {
                            prev = inputTokens[i - 1].Text;
                        }

                        string post = string.Empty;
                        if (flaggedText.Length < tokenText.Length)
                        {
                            post = GetNextWord(tokenText, flaggedOffset + flaggedText.Length);
                        }
                        else if (i < inputTokens.Count - 1)
                        {
                            post = inputTokens[i + 1].Text;
                        }

                        var bestCandidate = GetBestCandidate(prev, post, candidates, flaggedText);
                        result.Append(tokenText.Substring(start, flaggedOffset - start));
                        start += flaggedOffset - start;
                        result.Append(bestCandidate);
                        start += flaggedText.Length;
                    }

                    result.Append(tokenText.Substring(start));
                    correctedTokens.Add(new QueryRewriteToken(result.ToString(), inputTokens[i].Start, inputTokens[i].Length));
                }
            }

            return correctedTokens;
        }

        private bool DoNotCorrect(string text, bool initCap)
        {
            return _blacklists.Contains(text) || IsCapitalWord(text, initCap) || HasSeparator(text);
        }

        private bool DoNotCorrect(QueryRewriteToken token, bool initCap)
        {
            return token.Tags.HasFlag(QueryRewriteTags.NoAlter) || DoNotCorrect(token.Text, initCap);
        }

        private List<FlaggedToken> FilterFlaggedTokens(IList<FlaggedToken> inFlaggedTokens)
        {
            var outFlaggedTokens = new List<FlaggedToken>();
            foreach (var flaggedToken in inFlaggedTokens)
            {
                var filteredSuggestions = new List<string>();
                foreach (var suggestion in flaggedToken.Suggestions.Take(_maxCandidatesFromOfficeSpeller))
                {
                    // for very long text, we don't limit edit distance as the calculation is expensive
                    if (suggestion.Length >= _maxEditDistanceByLength.Length)
                    {
                        filteredSuggestions.Add(suggestion);
                    }
                    else
                    {
                        var maxEditDistance = _maxEditDistanceByLength[suggestion.Length];
                        if (Math.Abs(suggestion.Length - flaggedToken.Token.Length) <= maxEditDistance
                            && TextUtil.EditDistanceWithSwap(suggestion, flaggedToken.Token) <= maxEditDistance)
                        {
                            filteredSuggestions.Add(suggestion);
                        }
                    }
                }

                if (filteredSuggestions.Count > 0)
                {
                    outFlaggedTokens.Add(new FlaggedToken()
                    {
                        Offset = flaggedToken.Offset,
                        Token = flaggedToken.Token,
                        Suggestions = filteredSuggestions,
                    });
                }
            }

            return outFlaggedTokens;
        }
    }
}

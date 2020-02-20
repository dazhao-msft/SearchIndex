//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    public class TorontoSpellerEngine : IDisposable
    {
        private readonly SpellingWrapper _spellingWrapperObj;

        private readonly IntPtr _spellerId;

        private readonly ProofLexOut[] _lexicons;

        public TorontoSpellerEngine(string resourcePath, string engineDllName, string lexiconFilePath, int languageId)
        {
            _spellingWrapperObj = new SpellingWrapper(
                Path.Combine(resourcePath, engineDllName),
                Path.Combine(resourcePath, lexiconFilePath),
                (ushort)languageId);
            ProofInfo info = _spellingWrapperObj.GetVersion();
            ProofParams proofingParams = new ProofParams { VersionAPI = info.VersionAPI };
            _spellerId = _spellingWrapperObj.Init(ref proofingParams);

            ProofLexIn lexIn = new ProofLexIn
            {
                ExpectedLanguageID = (ushort)languageId,
                LexiconPath = _spellingWrapperObj.LexiconPath,
                LexiconType = ProofLexType.Main,
            };

            // First lexicon file for main, the second one is for CSS main.
            _lexicons = new ProofLexOut[1];
            _lexicons[0] = default(ProofLexOut);

            _spellingWrapperObj.OpenLexicon(_spellerId, ref lexIn, ref _lexicons[0]);

            SpellingWrapper.OptionBit options = 0;
            options = _spellingWrapperObj.GetOptions(_spellerId, SpellingWrapper.OptionSelect.Bits, options);
            options = options | SpellingWrapper.OptionBit.FindRepeatWord | SpellingWrapper.OptionBit.IgnoreAllCaps |
                      SpellingWrapper.OptionBit.IgnoreMixedDigits | SpellingWrapper.OptionBit.IgnoreRomanNumerals |
                      SpellingWrapper.OptionBit.IgnoreSingleLetter;

            _spellingWrapperObj.SetOptions(_spellerId, SpellingWrapper.OptionSelect.Bits, options);
        }

        public IList<FlaggedToken> Check(string input)
        {
            var flaggedTokens = new List<FlaggedToken>();

            SpellerInputBuffer inputBuffer = new SpellerInputBuffer(_lexicons);
            SpellerReturnBuffer returnBuffer = new SpellerReturnBuffer();

            try
            {
                inputBuffer.Input = input;
                inputBuffer.State = SpellerStates.None;
                uint lastProcessingRange = 0;

                while (inputBuffer.ProcessingLength > 0)
                {
                    _spellingWrapperObj.Check(_spellerId, SpellingWrapper.CheckCommand.VerifyBuffer, ref inputBuffer, ref returnBuffer);

                    uint nextProcessingRange = returnBuffer.ProcessedLength;

                    SpellerReturnBuffer.SpellerStatus checkResult = returnBuffer.Status;
                    switch (checkResult)
                    {
                        case SpellerReturnBuffer.SpellerStatus.NoErrors:
                            // no more errors, leave now
                            break;
                        case SpellerReturnBuffer.SpellerStatus.RepeatWord:
                            {
                                flaggedTokens.Add(new FlaggedToken
                                {
                                    Offset = returnBuffer.ErrorOffset,
                                    Token = input.Substring((int)returnBuffer.ErrorOffset, (int)returnBuffer.ErrorLength),
                                });
                                nextProcessingRange = returnBuffer.ErrorOffset;
                                break;
                            }

                        case SpellerReturnBuffer.SpellerStatus.ReturningChangeAlways:
                        case SpellerReturnBuffer.SpellerStatus.ReturningAutoReplace:
                            {
                                string word = inputBuffer.Input.Substring((int)returnBuffer.ErrorOffset, (int)returnBuffer.ErrorLength);

                                var flaggedToken = new FlaggedToken
                                {
                                    Offset = returnBuffer.ErrorOffset,
                                    Token = word,
                                };
                                flaggedTokens.Add(flaggedToken);
                                AddSuggestions(flaggedToken, returnBuffer);
                                break;
                            }

                        case SpellerReturnBuffer.SpellerStatus.UnknownInputWord:
                        case SpellerReturnBuffer.SpellerStatus.ErrorAccent:
                        case SpellerReturnBuffer.SpellerStatus.ErrorCapitalization:
                        case SpellerReturnBuffer.SpellerStatus.ReturningChangeOnce:
                        case SpellerReturnBuffer.SpellerStatus.ContextError:
                            {
                                string word = inputBuffer.Input.Substring((int)returnBuffer.ErrorOffset, (int)returnBuffer.ErrorLength);

                                var flaggedToken = new FlaggedToken
                                {
                                    Offset = returnBuffer.ErrorOffset,
                                    Token = word,
                                };

                                flaggedTokens.Add(flaggedToken);
                                inputBuffer.SetProcessingRange(returnBuffer.ErrorOffset, returnBuffer.ErrorLength);
                                _spellingWrapperObj.Check(_spellerId, SpellingWrapper.CheckCommand.Suggest, ref inputBuffer, ref returnBuffer);
                                AddSuggestions(flaggedToken, returnBuffer);
                                nextProcessingRange = returnBuffer.ErrorOffset + returnBuffer.ErrorLength;
                                break;
                            }

                        default:
                            break;
                    }

                    if (checkResult == SpellerReturnBuffer.SpellerStatus.NoErrors)
                    {
                        break;
                    }

                    if (nextProcessingRange <= lastProcessingRange)
                    {
                        break;
                    }

                    lastProcessingRange = nextProcessingRange;
                    inputBuffer.SetProcessingRange(nextProcessingRange);
                }
            }
            finally
            {
                inputBuffer.Dispose();
                returnBuffer.Dispose();
            }

            return flaggedTokens;
        }

        public void Dispose()
        {
            _spellingWrapperObj.CloseLexicon(_spellerId, _lexicons[0].LexiconID);
            _spellingWrapperObj.Terminate(_spellerId);
        }

        private static void AddSuggestions(FlaggedToken token, SpellerReturnBuffer buffer)
        {
            int suggestionCount = (int)Math.Min(SpellerReturnBuffer.MaxSuggestionCount, buffer.SuggestionCount);

            int added = 0;
            foreach (string suggestion in buffer.Suggestions)
            {
                if (added >= suggestionCount)
                {
                    break;
                }

                if (!string.IsNullOrEmpty(suggestion))
                {
                    token.Suggestions.Add(suggestion);
                    added++;
                }
            }
        }
    }
}

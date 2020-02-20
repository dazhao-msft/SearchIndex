//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    public class TorontoSpeller : IDisposable
    {
        private const string ENUS = "en-us";
        private readonly TorontoSpellerEngine _enusOrthoSpellerEngine;
        private readonly TorontoSpellerEngine _enusCssSpellerEngine;

        public TorontoSpeller()
        {
            var officeSpellerDir = ResolveOfficeSpellerFolderPath();
            var culture = new CultureInfo(ENUS);
            _enusOrthoSpellerEngine = new TorontoSpellerEngine(officeSpellerDir, @"msspell7.dll", "MSSP7EN.LEX", culture.LCID);
            _enusCssSpellerEngine = new TorontoSpellerEngine(officeSpellerDir, @"mscss7en.dll", @"MSSP7EN.LEX", culture.LCID);
        }

        public IList<FlaggedToken> Check(string input, bool enableContextualOfficeSpeller = true, string market = ENUS)
        {
            if (string.Equals(market, ENUS, StringComparison.Ordinal))
            {
                var flaggedTokens = _enusOrthoSpellerEngine.Check(input);

                if (enableContextualOfficeSpeller)
                {
                    var cssFlaggedTokens = _enusCssSpellerEngine.Check(input);

                    if (cssFlaggedTokens != null && cssFlaggedTokens.Any())
                    {
                        HashSet<uint> orthoFlaggedTokenOffsets = new HashSet<uint>();
                        foreach (var ft in flaggedTokens)
                        {
                            orthoFlaggedTokenOffsets.Add(ft.Offset);
                        }

                        // Add contextual office speller flagged token, if ortho speller already flagged a token, ignore the contextual speller's result.
                        foreach (var cssFt in cssFlaggedTokens)
                        {
                            if (!orthoFlaggedTokenOffsets.Contains(cssFt.Offset))
                            {
                                flaggedTokens.Add(cssFt);
                            }
                        }
                    }
                }

                return flaggedTokens;
            }

            throw new NotSupportedException("market " + market + " is not supported in office speller mode");
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _enusOrthoSpellerEngine.Dispose();
                _enusCssSpellerEngine.Dispose();
            }
        }

        private static string ResolveOfficeSpellerFolderPath()
        {
            const string OfficeSpellerFolderPath = ".\\lib\\OfficeSpeller";

            // in case of the path is relative to EXE, such as console application.
            var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, OfficeSpellerFolderPath);
            if (!Directory.Exists(localPath))
            {
                // in case of the path is relative to BIN folder for iis web and service scenario.
                localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", OfficeSpellerFolderPath);
            }

            return localPath;
        }
    }
}

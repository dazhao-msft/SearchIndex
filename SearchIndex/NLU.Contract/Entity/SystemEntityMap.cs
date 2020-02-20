//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.BizQA.NLU.Contract
{
    /// <summary>
    /// System defined well known entities.
    /// </summary>
    public static class SystemEntityMap
    {
        /// <summary>
        /// Other
        /// </summary>
        public const string OtherEntityId = "7dacd929-569e-407c-99c1-90d4fa9b8c78";
        public const string OtherEntityName = "##OTHERS";

        /// <summary>
        /// Don't Know
        /// </summary>
        public const string DontKnowEntityId = "26f7f1d9-7f84-45e2-9902-9e276f6696f9";
        public const string DontKnowEntityName = "##DONTKNOW";

        /// <summary>
        /// Yes
        /// </summary>
        public const string YesEntityId = "c1f73fc8-d2c1-4ce2-8dc6-c81a68c47f0b";
        public const string YesEntityName = "##YES";

        /// <summary>
        /// No
        /// </summary>
        public const string NoEntityId = "a46cc262-018b-468a-b872-31ab38c1aace";
        public const string NoEntityName = "##NO";

        /// <summary>
        /// Appreciation
        /// </summary>
        public const string AppreciationEntityId = "72b2d351-4b3b-48b2-8ee3-a62ef8572d9e";
        public const string AppreciationEntityName = "##APPRECIATION";

        /// <summary>
        /// ChitChat
        /// </summary>
        public const string ChitChatEntityId = "a8337600-aaad-4dc1-87f6-ba1b7eb5c47d";
        public const string ChitChatEntityName = "##CHITCHAT";

        /// <summary>
        /// Closing
        /// </summary>
        public const string ClosingEntityId = "8949e792-d691-45ca-8322-a488413b8a88";
        public const string ClosingEntityName = "##CLOSING";

        /// <summary>
        /// Greetings
        /// </summary>
        public const string GreetingsEntityId = "862f18cb-a5f4-4c20-a89e-b4896690e4e2";
        public const string GreetingsEntityName = "##GREETINGS";

        /// <summary>
        /// Doesn't work
        /// </summary>
        public const string DoesNotWorkEntityId = "755773f9-aa8a-4bf3-8f8f-7c3d37c47909";
        public const string DoesNotWorkEntityName = "##DOESNOTWORK";

        /// <summary>
        /// User Repeat
        /// </summary>
        public const string UserRepeatEntityId = "23fb544b-30fa-4283-916b-99a19ed9fda0";
        public const string UserRepeatEntityName = "##USERREPEAT";

        /// <summary>
        /// Wait
        /// </summary>
        public const string WaitEntityId = "158baf1d-cc27-4b2e-88e7-c138d95abd3a";
        public const string WaitEntityName = "##WAIT";

        /// <summary>
        /// None Of Above
        /// </summary>
        public const string NoneOfAboveEntityId = "45571966-176a-4719-9eee-040cd012a508";
        public const string NoneOfAboveEntityName = "##NONEOFABOVE";

        /// <summary>
        /// All Of Above
        /// </summary>
        public const string AllOfAboveEntityId = "5f8f0b79-5c9d-488e-bc81-ec8eab270d1c";
        public const string AllOfAboveEntityName = "##ALLOFABOVE";

        /// <summary>
        /// Out Of Domain
        /// </summary>
        public const string OutOfDomainEntityId = "8f8202d8-093e-4c85-9bd3-9488186dbd76";
        public const string OutOfDomainEntityName = "##OUTDOMAIN";

        /// <summary>
        /// Restricted Words
        /// </summary>
        public const string RestrictedWordsEntityId = "122d233e-c500-4d0d-af7c-c50477a27484";
        public const string RestrictedWordsEntityName = "##RESTRICTEDWORDS";

        /// <summary>
        /// Talk To Agent
        /// </summary>
        public const string TalkToAgentEntityId = "9f5d89ff-d0c8-49c2-9687-2afe4df84a4f";
        public const string TalkToAgentEntityName = "##TALKTOAGENT";

        /// <summary>
        /// Negative
        /// </summary>
        public const string NegativeEntityId = "5c23b50b-5295-40c5-b304-3d0b70d2f6e3";
        public const string NegativeEntityName = "##NEGATIVE";

        public static readonly IDictionary<string, string> EntityMap = new Dictionary<string, string>()
        {
            { OtherEntityId, OtherEntityName },
            { DontKnowEntityId, DontKnowEntityName },
            { YesEntityId, YesEntityName },
            { NoEntityId, NoEntityName },
            { AppreciationEntityId, AppreciationEntityName },
            { ChitChatEntityId, ChitChatEntityName },
            { ClosingEntityId, ClosingEntityName },
            { GreetingsEntityId, GreetingsEntityName },
            { DoesNotWorkEntityId, DoesNotWorkEntityName },
            { UserRepeatEntityId, UserRepeatEntityName },
            { WaitEntityId, WaitEntityName },
            { NoneOfAboveEntityId, NoneOfAboveEntityName },
            { AllOfAboveEntityId, AllOfAboveEntityName },
            { OutOfDomainEntityId, OutOfDomainEntityName },
            { RestrictedWordsEntityId, RestrictedWordsEntityName },
            { TalkToAgentEntityId, TalkToAgentEntityName },
            { NegativeEntityId, NegativeEntityName },
        };
    }
}

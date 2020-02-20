//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    /// <summary>
    /// Excpetion cause from unmanaged Proofing libraries.
    /// </summary>
    public class ProofingException : Exception
    {
        public ProofingException()
            : base()
        {
        }

        public ProofingException(string message)
            : base(message)
        {
        }

        public ProofingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private ProofingException(string message, MajorErrorCode major, MinorErrorCode minor, Exception innerException)
            : base(message, innerException)
        {
            MajorError = major;
            MinorError = minor;
        }

        public enum MajorErrorCode
        {
            NoErrors = 0,
            OutOfMemory,      /* memory error */
            ModuleError,      /* Something wrong with parameters, or state of spell module. */
            IOErrorMainLex,   /* Read,write,or share error with Main Dictionary. */
            IOErrorUserLex,   /* Read,write,or share error with User Dictionary. */
            NotSupported,     /* No support for requested operation */
            BufferTooSmall,   /* Insufficient room for return info */
            NotFound,         /* Hyphenator and Thesaurus only */
            ModuleNotLoaded,  /* underlying module not loaded (Glue Dll's) */
        }

        public enum MinorErrorCode
        {
            NoErrors = 0,
            ModuleAlreadyBusy = 128,  /* For non-reentrant code */
            InvalidID,                /* Not yet inited or already terminated.*/
            InvalidWsc,               /* Illegal values in WSC struct (speller only) */
            InvalidMainLex,           /* Mdr not registered with session */
            InvalidUserLex,           /* Udr not registered with session */
            InvalidCmd,               /* CheckCommand unknown */
            InvalidFormat,            /* Specified dictionary not correct format */
            OperNotMatchedUserLex,    /* Illegal operation for user dictionary type. */
            FileRead,                 /* Generic read error */
            FileWrite,                /* Generic write error */
            FileCreate,               /* Generic create error */
            FileShare,                /* Generic share error */
            ModuleNotTerminated,      /* Module not able to be terminated completely.*/
            UserLexFull,              /* Could not update Udr without exceeding limit.*/
            InvalidEntry,             /* invalid chars in string(s) */
            EntryTooLong,             /* Entry too long, or invalid chars in string(s) */
            MainLexCountExceeded,     /* Too many Mdr references */
            UserLexCountExceeded,     /* Too many udr references */
            FileOpenError,            /* Generic Open error */
            FileTooLargeError,        /* Generic file too large error */
            UserLexReadOnly,          /* Attempt to add to or write RO udr */
            ProtectModeOnly,          /* (obsolete) */
            InvalidLanguage,          /* requested language not available */
        }

        public MajorErrorCode MajorError { get; private set; }

        public MinorErrorCode MinorError { get; private set; }

        /// <summary>
        /// Notice: Any new property should be added to SerializationInfo, otherwise, json serialization will ignore the property since exception class implemented ISerializable.
        /// https://stackoverflow.com/questions/37170011/why-are-driveinfos-properties-missing-when-serializing-to-json-string-using-jso
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("MajorError", MajorError);
            info.AddValue("MinorError", MinorError);
        }

        public static ProofingException Create(uint error)
        {
            return Create(error, null);
        }

        public static ProofingException Create(uint error, Exception innerException)
        {
            MajorErrorCode majorError = (MajorErrorCode)(error & 0x0000ffff);
            MinorErrorCode minorError = (MinorErrorCode)((error & 0xffff0000) >> 16);
            return new ProofingException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Proofing exception from spelling: {0} {1}",
                    majorError,
                    minorError),
                majorError,
                minorError,
                innerException);
        }
    }
}

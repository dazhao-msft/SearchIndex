//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.BizQA.NLU.Common
{
    public class MurmurHash
    {
        private int _bits;
        private uint _mask;
        private uint _seed;

        // bits <= 31
        public MurmurHash(int bits, uint seed = 0)
        {
            _bits = Math.Min(31, bits);
            _mask = (uint)((1L << bits) - 1);
            _seed = seed;
        }

        public static int GetHashDim(int bits)
        {
            return (int)(1L << bits);
        }

        public int ComputeHash(string s)
        {
            byte[] bs = Encoding.UTF8.GetBytes(s);
            return ComputeHash(bs);
        }

        public int ComputeHash(byte[] data)
        {
            return ComputeHash(data, data.Length);
        }

        public int ComputeHash(byte[] data, int dataLen)
        {
            const uint C1 = 0xcc9e2d51;
            const uint C2 = 0x1b873593;

            int length = dataLen;
            int newLen = length;
            int curIndex = 0;

            uint h = _seed;

            uint[] hackArray = new BytetoUInt32Converter { Bytes = data }.UInts;
            unchecked
            {
                while (newLen >= 4)
                {
                    uint k1 = hackArray[curIndex++];

                    k1 *= C1;
                    k1 = (k1 << 15) | (k1 >> (32 - 15));
                    k1 *= C2;

                    h ^= k1;
                    h = (h << 13) | (h >> (32 - 13));
                    h = (h * 5) + 0xe6546b64;
                    newLen -= 4;
                }

                curIndex = length - 1;

                uint k2 = 0;
                switch (newLen)
                {
                    case 3:
                        k2 ^= (uint)data[curIndex--] << 16;
                        goto case 2;

                    case 2:
                        k2 ^= (uint)data[curIndex--] << 8;
                        goto case 1;

                    case 1:
                        k2 ^= data[curIndex];
                        k2 *= C1;
                        k2 = (k2 << 15) | (k2 >> (32 - 15));
                        k2 *= C2;
                        h ^= k2;
                        break;

                    default:
                        break;
                }

                h ^= (uint)length;

                h ^= h >> 16;
                h *= 0x85ebca6b;
                h ^= h >> 13;
                h *= 0xc2b2ae35;
                h ^= h >> 16;
            }

            return (int)(h & _mask);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct BytetoUInt32Converter
        {
            [FieldOffset(0)]
            public byte[] Bytes;

            [FieldOffset(0)]
            public uint[] UInts;
        }
    }
}

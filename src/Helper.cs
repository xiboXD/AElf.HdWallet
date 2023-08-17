using System;
using NBitcoin;

namespace BIP39Wallet
{
    internal static class Helper
    {
        public static Wordlist GetWordlistByLanguage(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return Wordlist.English;
                case Language.French:
                    return Wordlist.French;
                case Language.Spanish:
                    return Wordlist.Spanish;
                case Language.ChineseSimplified:
                    return Wordlist.ChineseSimplified;
                case Language.Czech:
                    return Wordlist.Czech;
                case Language.Japanese:
                    return Wordlist.Japanese;
                case Language.ChineseTraditional:
                    return Wordlist.ChineseTraditional;
                default:
                    throw new ArgumentException("Unsupported language", nameof(language));
            }
        }
        public static byte[] StringToByteArray(string hexString)
        {
            var length = hexString.Length;
            var byteArray = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
            {
                byteArray[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return byteArray;
        }
    }
}
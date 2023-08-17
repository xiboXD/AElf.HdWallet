using System;
using NBitcoin;

namespace BIP39Wallet;

public class AElfWalletFactory : WalletFactory
{
    public AElfWalletFactory() : base("m/44'/1616'/0'/0")
    {
    }
}

public class WalletFactory
{
    public readonly string MasterPath;

    public WalletFactory(string masterPath)
    {
        MasterPath = masterPath;
    }


    public HDKey Generate(string passphrase = "", Language language = Language.English,
        WordCount wordCount = WordCount.Twelve)
    {
        var mnemonic = new Mnemonic(GetWordlist(language), wordCount);
        return Create(mnemonic, passphrase);
    }

    public HDKey Create(Mnemonic mnemonic, string passphrase = "")
    {
        var extKey = mnemonic.DeriveExtKey(passphrase);
        return extKey.Derive(KeyPath.Parse(MasterPath)).Wrap();
    }

    private static Wordlist GetWordlist(Language language)
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
            case Language.PortugueseBrazil:
            case Language.Unknown:
            default:
                throw new ArgumentException("Unsupported language", nameof(language));
        }
    }
}
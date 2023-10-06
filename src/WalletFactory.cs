using System;
using NBitcoin;

namespace AElf.HdWallet;

public class AElfWalletFactory : WalletFactory
{
    public AElfWalletFactory() : base("m/44'/1616'/0'/0")
    {
    }
}

public class WalletFactory
{
    private readonly string _masterPath;

    protected WalletFactory(string masterPath)
    {
        _masterPath = masterPath;
    }


    public ExtendedKey Create(string passphrase = "", Language language = Language.English,
        WordCount wordCount = WordCount.Twelve)
    {
        var mnemonic = new Mnemonic(GetWordlist(language), wordCount);
        return FromMnemonic(mnemonic, passphrase);
    }

    public ExtendedKey FromMnemonic(string mnemonic, string passphrase)
    {
        return FromMnemonic(new Mnemonic(mnemonic), passphrase);
    }

    private ExtendedKey FromMnemonic(Mnemonic mnemonic, string passphrase)
    {
        var extKey = mnemonic.DeriveExtKey(passphrase);
        return extKey.Derive(KeyPath.Parse(_masterPath)).Wrap();
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
                return Wordlist.PortugueseBrazil;
            case Language.Unknown:
            default:
                throw new ArgumentException("Unsupported language", nameof(language));
        }
    }
}
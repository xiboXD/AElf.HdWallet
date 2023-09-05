using System;
using System.Text;
using NBitcoin;
using Xunit;
// ReSharper disable StringLiteralTypo

namespace BIP39Wallet.Tests;

public class WalletTests
{
    private const string PubKey = "04c0f6abf0e3122f4a49646d67bacf85c80ad726ca781ccba572033a31162f22e55a4a106760cbf1306f26c25aea1e4bb71ee66cb3c5104245d6040cce64546cc7";
    [Fact]
    public void CreateWallet_test()
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        var accountInfo = new AElfWalletFactory().Create();
        accountInfo.PrivateKey.Dispose();
        Assert.NotNull(accountInfo);
    }
    [Theory]
    [InlineData(Language.ChineseSimplified, false)]
    [InlineData(Language.ChineseTraditional, false)]
    [InlineData(Language.English, false)]
    [InlineData(Language.French, false)]
    [InlineData(Language.Japanese, false)]
    [InlineData(Language.Spanish, false)]
    [InlineData(Language.Czech, false)]
    [InlineData(Language.PortugueseBrazil, false)]
    [InlineData(Language.Unknown, true)]
    public void CreateWallet_test_support_multi_language(Language language, bool isThrowException)
    {
        if (!isThrowException)
        {
            var accountInfo = new AElfWalletFactory().Create(language: language);
            Assert.NotNull(accountInfo);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => new AElfWalletFactory().Create(language: language));            
        }
    }

    [Fact]
    public void GetWalletByMnemonic_ReturnsValidAccountInfo()
    {
        // Arrange
        const string mnemonic = "put draft unhappy diary arctic sponsor alien awesome adjust bubble maid brave";
        var accountInfo = new AElfWalletFactory().FromMnemonic(mnemonic).Derive(0);
        Assert.NotNull(accountInfo);
        Assert.Equal(
            new PublicKey(
                PubKey),
            accountInfo.PrivateKey.PublicKey.Decompress()
        );
        Assert.Equal("2ihA5K7sSsA78gekyhuh7gcnX4JkGVqJmSGnf8Kj1hZefR4sX5", accountInfo.PrivateKey.PublicKey.Decompress().ToAddress());
    }
        
    [Fact]
    public void PublicKey_CompareTo_Test()
    {
        var publicKey1 = new PublicKey(PubKey);
        var publicKey2 = new PublicKey(PubKey);
        Assert.Equal(0, publicKey1.CompareTo(publicKey2));
        Assert.Equal(1, publicKey1.CompareTo(null));
    }

    [Fact]
    public void GetWalletByPrivateKey_ReturnsValidAccountInfo()
    {
        const string privateKey = "f0c3bf2cfc4f50405afb2f1236d653cf0581f4caedf4f1e0b49480c840659ba9";
        var accountInfo = PrivateKey.Parse(privateKey);
        var keyTest =
            new PublicKey(
                PubKey);
        Assert.NotNull(accountInfo);
        Assert.Equal(
            keyTest,
            accountInfo.PublicKey.Decompress()
        );

        var compareResult = accountInfo.PublicKey.CompareTo(keyTest);
        Assert.Equal(0, compareResult);
            
        Assert.Equal(PubKey, accountInfo.PublicKey.ToString());
        Assert.Equal("2ihA5K7sSsA78gekyhuh7gcnX4JkGVqJmSGnf8Kj1hZefR4sX5", accountInfo.PublicKey.Decompress().ToAddress());
    }

    [Fact]
    public void Sign_ReturnsValidSignature()
    {
        const string privateKey = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808";
        const string hash = "68656c6c6f20776f726c643939482801";
        const string signed =
            "59EF1D3B2B853FCA1E33D07765DEBAAF38A81442CFE90822D4334E8FCE9889D80C99A0BE1858C1F26B4D99987EFF6003F33B7C3F32BBDB9CEEC68A1E8A4DB4B000";


        // Arrange
        var result = PrivateKey.Parse(privateKey).Sign(Encoding.UTF8.GetBytes(hash));
        var hex = new StringBuilder(result.Length * 2);
        foreach (var b in result)
        {
            hex.AppendFormat("{0:x2}", b);
        }

        var resultSting = hex.ToString();

        // Assert
        Assert.Equal(signed.ToLower(), resultSting);
    }
}
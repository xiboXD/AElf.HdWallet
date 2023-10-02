using System;
using System.Text;
using NBitcoin;
using NBitcoin.DataEncoders;
using Xunit;
using AElf.Cryptography;

// ReSharper disable StringLiteralTypo

namespace AElf.HdWallet.Tests;

public class WalletTests
{
    private const string PubKey =
        "04c0f6abf0e3122f4a49646d67bacf85c80ad726ca781ccba572033a31162f22e55a4a106760cbf1306f26c25aea1e4bb71ee66cb3c5104245d6040cce64546cc7";

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
    public void CreateWallet_test_support_multi_language(Language language, bool isThrowingException)
    {
        if (!isThrowingException)
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
        const string address = "2ihA5K7sSsA78gekyhuh7gcnX4JkGVqJmSGnf8Kj1hZefR4sX5";
        var accountInfo = new AElfWalletFactory().FromMnemonic(mnemonic, "").Derive(0);
        Assert.NotNull(accountInfo);
        Assert.Equal(
            new PublicKey(
                PubKey),
            accountInfo.PrivateKey.PublicKey.Decompress()
        );
        Assert.Equal(address, accountInfo.PrivateKey.PublicKey.ToAddress());
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
        const string address = "2ihA5K7sSsA78gekyhuh7gcnX4JkGVqJmSGnf8Kj1hZefR4sX5";
        var accountInfo = PrivateKey.Parse(privateKey);
        var keyTest =
            new PublicKey(
                PubKey);
        Assert.NotNull(accountInfo);
        Assert.Equal(
            keyTest,
            accountInfo.PublicKey.Decompress()
        );
        Assert.Equal(PubKey, accountInfo.PublicKey.ToString());
        Assert.Equal(address, accountInfo.PublicKey.ToAddress());
    }

    [Fact]
    public void Sign_ReturnsValidSignature()
    {
        const string privateKey = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808";
        const string hash = "68656c6c6f20776f726c643939482801";
        const string signed =
            "59ef1d3b2b853fca1e33d07765debaaf38a81442cfe90822d4334e8fce9889d80c99a0be1858c1f26b4d99987eff6003f33b7c3f32bbdb9ceec68a1e8a4db4b000";
        // Arrange
        var result = PrivateKey.Parse(privateKey).Sign(Encoding.UTF8.GetBytes(hash));
        var hexResult = new HexEncoder().EncodeData(result);
        // Assert
        Assert.Equal(signed, hexResult);
    }

    [Fact]
    public void CompressedKeyToSign()
    {
        const string mnemonic = "put draft unhappy diary arctic sponsor alien awesome adjust bubble maid brave";
        const string hash = "68656c6c6f20776f726c643939482801";
        var privateKey = new AElfWalletFactory().FromMnemonic(mnemonic, "").Derive(0).PrivateKey;
        var signature = privateKey.Sign(Encoding.UTF8.GetBytes(hash));
        var recovered = CryptoHelper.RecoverPublicKey(signature, Encoding.UTF8.GetBytes(hash), out var publicKey);
        Assert.True(recovered);
        Assert.Equal(privateKey.PublicKey.Decompress().ToString(), publicKey.ToHex());
    }

    [Fact]
    public void DecompressedKeyToSign()
    {
        const string decompressedPrivateKey = "f0c3bf2cfc4f50405afb2f1236d653cf0581f4caedf4f1e0b49480c840659ba9";
        const string hash = "68656c6c6f20776f726c643939482801";
        var privateKey = PrivateKey.Parse(decompressedPrivateKey);
        var signature = privateKey.Sign(Encoding.UTF8.GetBytes(hash));
        var recovered = CryptoHelper.RecoverPublicKey(signature, Encoding.UTF8.GetBytes(hash), out var publicKey);
        Assert.True(recovered);
        Assert.Equal(privateKey.PublicKey.Decompress().ToString(), publicKey.ToHex());
    }

    [Fact]
    public void CompressedKey_ConvertToDecompressed()
    {
        const string mnemonic = "put draft unhappy diary arctic sponsor alien awesome adjust bubble maid brave";
        const string decompressedPrivateKey = "f0c3bf2cfc4f50405afb2f1236d653cf0581f4caedf4f1e0b49480c840659ba9";
        var accountInfo = new AElfWalletFactory().FromMnemonic(mnemonic, "").Derive(0);
        var normalizedKey = accountInfo.PrivateKey.NormalizedBitcoinKey;
        Assert.Equal(decompressedPrivateKey, normalizedKey.ToHex());
    }
}
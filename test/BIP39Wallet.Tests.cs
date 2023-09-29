using System;
using System.Text;
using AElf;
using AElf.Cryptography;
using BIP39Wallet;
using NBitcoin;
using Xunit;

namespace BIP39WalletUtils.Tests
{
    public class WalletTests
    {
        [Fact]
        public void CreateWallet_test()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            var accountInfo = new AElfWalletFactory().Create();
            accountInfo.PrivateKey.Dispose();
            Assert.NotNull(accountInfo);
        }
        [Fact]
        public void CreateWallet_test_support_multi_language()
        {
            var accountInfo1 = new AElfWalletFactory().Create(language: Language.ChineseSimplified);
            Assert.NotNull(accountInfo1);
            
            var accountInfo2 = new AElfWalletFactory().Create(language: Language.ChineseTraditional);
            Assert.NotNull(accountInfo2);
            
            var accountInfo3 = new AElfWalletFactory().Create(language: Language.English);
            Assert.NotNull(accountInfo3);
            
            var accountInfo4 = new AElfWalletFactory().Create(language: Language.French);
            Assert.NotNull(accountInfo4);
            
            var accountInfo5 = new AElfWalletFactory().Create(language: Language.Japanese);
            Assert.NotNull(accountInfo5);
            
            var accountInfo6 = new AElfWalletFactory().Create(language: Language.Spanish);
            Assert.NotNull(accountInfo6);
            
            var accountInfo7 = new AElfWalletFactory().Create(language: Language.Czech);
            Assert.NotNull(accountInfo7);
            
            Assert.Throws<ArgumentException>(() => new AElfWalletFactory().Create(language: Language.PortugueseBrazil));            
            // ReSharper disable once RedundantArgumentDefaultValue
            Assert.Throws<ArgumentException>(() => new AElfWalletFactory().Create(language: Language.Unknown));
        }

        [Fact]
        public void GetWalletByMnemonic_ReturnsValidAccountInfo()
        {
            // Arrange
            var mnemonic = "put draft unhappy diary arctic sponsor alien awesome adjust bubble maid brave";
            var accountInfo = new AElfWalletFactory().FromMnemonic(mnemonic).Derive(0);
            Assert.NotNull(accountInfo);
            Assert.Equal(
                new PublicKey(
                    "04c0f6abf0e3122f4a49646d67bacf85c80ad726ca781ccba572033a31162f22e55a4a106760cbf1306f26c25aea1e4bb71ee66cb3c5104245d6040cce64546cc7"),
                accountInfo.PrivateKey.PublicKey.Decompress()
            );
            Assert.Equal("2ihA5K7sSsA78gekyhuh7gcnX4JkGVqJmSGnf8Kj1hZefR4sX5", accountInfo.PrivateKey.PublicKey.Decompress().ToAddress());
        }

        [Fact]
        public void GetWalletByPrivateKey_ReturnsValidAccountInfo()
        {
            var privateKey = "f0c3bf2cfc4f50405afb2f1236d653cf0581f4caedf4f1e0b49480c840659ba9";
            var accountInfo = PrivateKey.Parse(privateKey);
            var keyTest =
                new PublicKey(
                    "04c0f6abf0e3122f4a49646d67bacf85c80ad726ca781ccba572033a31162f22e55a4a106760cbf1306f26c25aea1e4bb71ee66cb3c5104245d6040cce64546cc7");
            Assert.NotNull(accountInfo);
            Assert.Equal(
                keyTest,
                accountInfo.PublicKey.Decompress()
            );

            var compareResult = accountInfo.PublicKey.CompareTo(keyTest);
            Assert.Equal(0, compareResult);

            var keyHash = accountInfo.PublicKey.GetHashCode();
            Assert.Equal(1373039134, keyHash);
            Assert.Equal("04c0f6abf0e3122f4a49646d67bacf85c80ad726ca781ccba572033a31162f22e55a4a106760cbf1306f26c25aea1e4bb71ee66cb3c5104245d6040cce64546cc7", accountInfo.PublicKey.ToString());
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

        [Fact]
        public void Mnemonic_GenerateCompressedKey()
        {
            const string mnemonic = "put draft unhappy diary arctic sponsor alien awesome adjust bubble maid brave";
            const string hash = "68656c6c6f20776f726c643939482801";
            var privateKey = new AElfWalletFactory().FromMnemonic(mnemonic).Derive(0).PrivateKey;
            var signature =privateKey.Sign(Encoding.UTF8.GetBytes(hash));
            var recovered = CryptoHelper.RecoverPublicKey(signature, Encoding.UTF8.GetBytes(hash), out var publicKey);
            Assert.True(recovered);
            Assert.Equal(privateKey.PublicKey.Decompress().ToString(), publicKey.ToHex());
        }
    }
}
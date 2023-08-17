using System;
using System.Text;
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
            var accountInfo = new AElfWalletFactory().Generate();
            Assert.NotNull(accountInfo);
        }

        [Fact]
        public void GetWalletByMnemonic_ReturnsValidAccountInfo()
        {
            // Arrange
            var mnemonic = "put draft unhappy diary arctic sponsor alien awesome adjust bubble maid brave";
            var accountInfo = new AElfWalletFactory().Create(new Mnemonic(mnemonic)).Derive(0);
            Assert.NotNull(accountInfo);
            Assert.Equal(
                new PublicKey(
                    "04c0f6abf0e3122f4a49646d67bacf85c80ad726ca781ccba572033a31162f22e55a4a106760cbf1306f26c25aea1e4bb71ee66cb3c5104245d6040cce64546cc7"),
                accountInfo.PrivateKey.PublicKey.Decompress()
            );
        }

        [Fact]
        public void GetWalletByPrivateKey_ReturnsValidAccountInfo()
        {
            var privateKey = "f0c3bf2cfc4f50405afb2f1236d653cf0581f4caedf4f1e0b49480c840659ba9";
            var accountInfo = PrivateKey.Parse(privateKey);
            Assert.NotNull(accountInfo);
            Assert.Equal(
                new PublicKey(
                    "04c0f6abf0e3122f4a49646d67bacf85c80ad726ca781ccba572033a31162f22e55a4a106760cbf1306f26c25aea1e4bb71ee66cb3c5104245d6040cce64546cc7"),
                accountInfo.PublicKey.Decompress()
            );
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
}
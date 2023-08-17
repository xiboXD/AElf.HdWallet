using NBitcoin;
using System.Text;
using AElf.Types;
using System.Security.Cryptography;
using System;

namespace BIP39Wallet
{
    public class Wallet
    {
        private static string ConvertMnemonicToSeedHex(Mnemonic mnemonic, string password)
        {
            var mnemonicBytes = Encoding.UTF8.GetBytes(mnemonic.ToString().Normalize(NormalizationForm.FormKD));
            var saltSuffix = string.Empty;
            if (!string.IsNullOrEmpty(password))
            {
                saltSuffix = password;
            }
            var salt = $"mnemonic{saltSuffix}";
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            var rfc2898DerivedBytes = new Rfc2898DeriveBytes(mnemonicBytes, saltBytes, 2048, HashAlgorithmName.SHA512);
            var key = rfc2898DerivedBytes.GetBytes(64);
            var hex = BitConverter
                .ToString(key)
                .Replace("-", "")
                .ToLower();

            return hex;
        }

        public static Account CreateWallet(int strength = 12, Language language = Language.English, string password = "")
        {
            var mnemonic = new Mnemonic(Helper.GetWordlistByLanguage(language), Enum.Parse<WordCount>(strength.ToString()));

            var seed = ConvertMnemonicToSeedHex(mnemonic, password);
            var masterKeyPath = new KeyPath(BIP39WalletConstants.AElfPath);
            var masterWallet = new ExtKey(seed).Derive(masterKeyPath);
            var wallet = masterWallet.Derive(new KeyPath(BIP39WalletConstants.PathSuffix)).Derive(0);
            var privateKey = wallet.PrivateKey;
            var newKey = new Key(privateKey.ToBytes(), -1, false);
            var publicKey = newKey.PubKey;

            // Act
            var address = Address.FromPublicKey(publicKey.ToBytes()).ToString().Trim('\"');
            return new Account(address, privateKey.ToHex(), mnemonic.ToString(), publicKey.ToString());
        }

        public static Account GetWalletByMnemonic(string mnemonic, string password = "")
        {
            var mnemonicValue = new Mnemonic(mnemonic, Wordlist.English);
            var seedHex = ConvertMnemonicToSeedHex(mnemonicValue, password);
            var masterKeyPath = new KeyPath(BIP39WalletConstants.AElfPath);
            var masterWallet = new ExtKey(seedHex).Derive(masterKeyPath);
            var wallet = masterWallet.Derive(new KeyPath(BIP39WalletConstants.PathSuffix)).Derive(0);
            var privateKey = wallet.PrivateKey;
            var newKey = new Key(privateKey.ToBytes(), -1, false);
            var publicKey = newKey.PubKey;

            // Act
            var address = Address.FromPublicKey(publicKey.ToBytes()).ToString().Trim('\"');
            return new Account(address, privateKey.ToHex(), mnemonic, publicKey.ToString());
        }

        public static Account GetWalletByPrivateKey(string privateKey)
        {
            var keyByte = Helper.StringToByteArray(privateKey);
            Array.Resize(ref keyByte, 32);
            var key = new Key(keyByte, -1, false);
            var publicKey = key.PubKey;
            var address =  Address.FromPublicKey(publicKey.ToBytes()).ToString().Trim('\"');
            return new Account(address, privateKey, null!, publicKey.ToHex());
        }
        public static byte[] Sign(byte[] privateKey, byte[] hash)
        {
            var hash32 = new uint256(hash);
            Array.Resize(ref privateKey, 32);
            var key = new Key(privateKey, -1, false);
            var signature = key.SignCompact(hash32, false);
        
            var formattedSignature = new byte[65];
            Array.Copy(signature[1..], 0, formattedSignature, 0, 64);
        
            var recoverId = (byte)(signature[0] - 27);
            formattedSignature[64] = recoverId; //last byte holds the recoverId

            return formattedSignature;
        }
    }
}
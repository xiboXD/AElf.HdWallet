using NBitcoin;
using System.Text;
using AElf.Types;
using System.Security.Cryptography;
using System;


namespace BIP39Wallet;

public class BIP39Wallet
{
    public class Wallet
    {
        public class BlockchainWallet
        {
            public string Address { get; private set; }
            public string PrivateKey { get; private set; }
            public string Mnemonic { get; private set; }
            public string PublicKey { get; private set; }

            public BlockchainWallet(string address, string privateKey, string mnemonic, string publicKey)
            {
                Address = address;
                PrivateKey = privateKey;
                Mnemonic = mnemonic;
                PublicKey = publicKey;
            }
        }
        public static string ConvertMnemonicToSeedHex(Mnemonic mnemonic, string password)
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

        public static BlockchainWallet CreateWallet(int strength, Language language, string password)
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

            var seed = Wallet.ConvertMnemonicToSeedHex(mnemonic, "");
            var masterKeyPath = new KeyPath("m/44'/1616'");
            var masterWallet = new ExtKey(seed).Derive(masterKeyPath);
            var wallet = masterWallet.Derive(new KeyPath("0'/0")).Derive(0);
            Key privateKey = wallet.PrivateKey;
            var newKey = new Key(privateKey.ToBytes(), -1, false);
            PubKey publicKey = newKey.PubKey;

            // Act
            var address = Address.FromPublicKey(publicKey.ToBytes()).ToString().Trim('\"');
            return new BlockchainWallet(address, privateKey.ToHex(), mnemonic.ToString(), publicKey.ToString());
        }

        public static BlockchainWallet GetWalletByMnemonic(string mnemonic, string password = "")
        {
            Mnemonic mnemonicValue = new Mnemonic(mnemonic, Wordlist.English);
            var seedHex = ConvertMnemonicToSeedHex(mnemonicValue, password);
            var masterKeyPath = new KeyPath("m/44'/1616'");
            var masterWallet = new ExtKey(seedHex).Derive(masterKeyPath);
            var wallet = masterWallet.Derive(new KeyPath("0'/0")).Derive(0);
            Key privateKey = wallet.PrivateKey;
            var newKey = new Key(privateKey.ToBytes(), -1, false);
            PubKey publicKey = newKey.PubKey;

            // Act
            var address = Address.FromPublicKey(publicKey.ToBytes()).ToString().Trim('\"');
            return new BlockchainWallet(address, privateKey.ToHex(), mnemonic, publicKey.ToString());
        }
        
        static byte[] StringToByteArray(string hexString)
        {
            int length = hexString.Length;
            byte[] byteArray = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
            {
                byteArray[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return byteArray;
        }

        public static BlockchainWallet GetWalletByPrivateKey(string privateKey)
        {
            var keybyte = StringToByteArray(privateKey);
            Array.Resize(ref keybyte, 32);
            var key = new Key(keybyte, -1, false);
            var publicKey = key.PubKey;
            var address =  Address.FromPublicKey(publicKey.ToBytes()).ToString().Trim('\"');
            return new BlockchainWallet(address, privateKey, null!, publicKey.ToHex());
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
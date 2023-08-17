namespace BIP39Wallet
{

    public class Account
    {
        public string Address { get; private set; }
        public string PrivateKey { get; private set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Mnemonic { get; private set; }
        public string PublicKey { get; private set; }

        public Account(string address, string privateKey, string mnemonic, string publicKey)
        {
            Address = address;
            PrivateKey = privateKey;
            Mnemonic = mnemonic;
            PublicKey = publicKey;
        }
    }
}
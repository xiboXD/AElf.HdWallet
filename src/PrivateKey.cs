using System;
using NBitcoin;
using NBitcoin.DataEncoders;
using BitcoinKey = NBitcoin.Key;

namespace BIP39Wallet
{
    public class PrivateKey : IDisposable
    {
        private BitcoinKey _bitcoinKey;
        public PublicKey PublicKey => _bitcoinKey.PubKey.Wrap();

        public BitcoinKey NormalizedBitcoinKey
        {
            get
            {
                if (_bitcoinKey.IsCompressed)
                {
                    var keyBytes = _bitcoinKey.ToBytes();
                    Array.Resize(ref keyBytes, 32);
                    _bitcoinKey = new BitcoinKey(keyBytes, -1, false);
                }
                return _bitcoinKey;
            }
        }

        public static PrivateKey Parse(string privateKey)
        {
            var keyByte = Encoders.Hex.DecodeData(privateKey);
            Array.Resize(ref keyByte, 32);
            var bitcoinKey = new BitcoinKey(keyByte, -1, false);
            return bitcoinKey.Wrap();
        }

        internal static PrivateKey From(BitcoinKey bitcoinKey)
        {
            return new PrivateKey(bitcoinKey);
        }

        private bool _disposed = false;

        private PrivateKey(BitcoinKey bitcoinKey)
        {
            _bitcoinKey = bitcoinKey;
        }
        
        public string ToHex()
        {
            return _bitcoinKey.ToHex();
        }

        public byte[] Sign(byte[] hash)
        {
            var hash32 = new uint256(hash);
            var signature = NormalizedBitcoinKey.SignCompact(hash32, false);

            var formattedSignature = new byte[65];
            Array.Copy(signature[1..], 0, formattedSignature, 0, 64);

            var recoverId = (byte)(signature[0] - 27);
            formattedSignature[64] = recoverId; //last byte holds the recoverId

            return formattedSignature;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_bitcoinKey is IDisposable keyMaterial)
                    keyMaterial.Dispose();
            }

            _disposed = true;
        }

        ~PrivateKey()
        {
            Dispose(false);
        }
    }
}
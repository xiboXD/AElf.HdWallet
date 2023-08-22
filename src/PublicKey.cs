using System;
using BitcoinPubKey = NBitcoin.PubKey;
using IPubKey = NBitcoin.IPubKey;

namespace BIP39Wallet
{
    public class PublicKey : IComparable<PublicKey>, IEquatable<PublicKey>, IPubKey
    {
        private readonly BitcoinPubKey _bitcoinPubKey;

        internal static PublicKey From(BitcoinPubKey bitcoinPubKey)
        {
            return new PublicKey(bitcoinPubKey);
        }

        private PublicKey(BitcoinPubKey bitcoinPubKey)
        {
            _bitcoinPubKey = bitcoinPubKey;
        }

        public PublicKey(string hex)
            : this(new BitcoinPubKey(hex))
        {
        }

        public int CompareTo(PublicKey? other)
        {
            return other is null ? 1 : _bitcoinPubKey.CompareTo(other._bitcoinPubKey);
        }

        public bool Equals(PublicKey? other)
        {
            return other is not null && _bitcoinPubKey.Equals(other._bitcoinPubKey);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PublicKey);
        }

        public override int GetHashCode()
        {
            return _bitcoinPubKey.GetHashCode();
        }

        public override string ToString()
        {
            return _bitcoinPubKey.ToString();
        }

        public PublicKey Decompress()
        {
            return new PublicKey(_bitcoinPubKey.Decompress());
        }
    }
}
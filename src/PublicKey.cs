using System;
using AElf.Types;
using BitcoinPubKey = NBitcoin.PubKey;
using IPubKey = NBitcoin.IPubKey;

namespace AElf.HdWallet
    // ReSharper disable once ArrangeNamespaceBody
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

        public string ToAddress()
        {
            return Address.FromPublicKey(_bitcoinPubKey.Decompress().ToBytes()).ToString().Trim('\"');
        }

        public PublicKey Decompress()
        {
            return new PublicKey(_bitcoinPubKey.Decompress());
        }
    }
}
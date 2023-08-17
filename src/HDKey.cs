using NBitcoin;

namespace BIP39Wallet;

public class HDKey
{
    private readonly ExtKey _extKey;

    private HDKey(ExtKey extKey)
    {
        _extKey = extKey;
    }

    internal static HDKey From(ExtKey extKey)
    {
        return new HDKey(extKey);
    }

    public PrivateKey PrivateKey => _extKey.PrivateKey.Wrap();

    public HDKey Derive(uint index)
    {
        return new HDKey(_extKey.Derive(index));
    }
}
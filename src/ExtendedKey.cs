using NBitcoin;

namespace AElf.HdWallet;

public class ExtendedKey
{
    private readonly ExtKey _extKey;

    private ExtendedKey(ExtKey extKey)
    {
        _extKey = extKey;
    }

    internal static ExtendedKey From(ExtKey extKey)
    {
        return new ExtendedKey(extKey);
    }

    public PrivateKey PrivateKey => _extKey.PrivateKey.Wrap();

    public ExtendedKey Derive(uint index)
    {
        return new ExtendedKey(_extKey.Derive(index));
    }
}
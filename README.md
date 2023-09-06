# BIP39 wallet for AElf unity SDK
## Introduction
This is a BIP39 wallet project for AElf unity SDK. It can generate mnemonic words and private and public key, and can also import mnemonic words to get private key. It can also generate address from public key.
It can be used to not only create wallet but also can be used to restore wallet from mnemonic words or private key.
## Usage
### Create wallet
```c#
var accountInfo = new AElfWalletFactory().Create();
var privateKey = accountInfo.PrivateKey.Dispose();
var publicKey = accountInfo.PrivateKey.PublicKey.Decompress();
var address = accountInfo.PublicKey.Decompress().ToAddress();
```
### Restore wallet from mnemonic words
```c#
const string mnemonic = "put draft unhappy diary arctic sponsor alien awesome adjust bubble maid brave";
var accountInfo = new AElfWalletFactory().FromMnemonic(mnemonic, "").Derive(0);
var privateKey = accountInfo.PrivateKey.Dispose();
var publicKey = accountInfo.PrivateKey.PublicKey.Decompress();
var address = accountInfo.PublicKey.Decompress().ToAddress();
```
### Restore wallet from private key
```c#
const string privateKey = "f0c3bf2cfc4f50405afb2f1236d653cf0581f4caedf4f1e0b49480c840659ba9";
var accountInfo = PrivateKey.Parse(privateKey);
var publicKey = accountInfo.PrivateKey.PublicKey.Decompress();
var address = accountInfo.PublicKey.Decompress().ToAddress();
```
## Test
You can easily run unit tests on BIP39 wallet. Navigate to the test folder and run:
```bash
cd test
dotnet test
```

## Contributing
We welcome contributions to the AElf BIP39 wallet project. If you would like to contribute, please fork the repository and submit a pull request with your changes. Before submitting a pull request, please ensure that your code is well-tested and adheres to the AElf coding standards.
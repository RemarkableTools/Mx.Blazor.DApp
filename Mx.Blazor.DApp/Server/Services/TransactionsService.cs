using Mx.Blazor.DApp.Shared;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Data.Accounts;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.TransactionsManager;
using Mx.NET.SDK.Wallet;
using Mx.NET.SDK.Wallet.Wallet;

namespace Mx.Blazor.DApp.Services;

public interface ITransactionsService
{
    string GenerateWallet(uint shardId);

    Task ScheduleTransactions(TransactionsRequest request);
}

public class TransactionsService(ILogger<TransactionsService> logger) : ITransactionsService
{
    private const int RoundDuration = 6000;
    private GatewayProvider Provider { get; } = new(new GatewayNetworkConfiguration(Network.DevNet));

    public string GenerateWallet(uint shardId)
    {
        var mnemonic = Mnemonic.GenerateInShard(shardId);
        var walletSecretKey = WalletSecretKey.FromMnemonic(mnemonic.SeedPhrase);
        var pem = PemFile.BuildPemFile(walletSecretKey);
        var address = walletSecretKey.GeneratePublicKey().ToAddress();

        File.WriteAllText($"Wallets/{address.Bech32}.pem", pem);
        logger.LogInformation($"---- GenerateWallet | Wallet address: {address.Bech32}");

        return address.Bech32;
    }

    public async Task ScheduleTransactions(TransactionsRequest request)
    {
        logger.LogInformation(
            $"---- ScheduleTransactions | Executor address: {request.ExecutorAddress} | Contract address: {request.ContractAddress}  | Num of transactions: {request.NrOfTransactions} | Scheduled time: {request.ScheduledTime}"
        );

        await Task.Run(
            async () =>
            {
                // wait for scheduled time
                var delay = request.ScheduledTime - DateTime.UtcNow;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay);
                }

                // create txs signer
                var signer = WalletSigner.FromPemFile($"Wallets/{request.ExecutorAddress}.pem");
                var networkConfig = await NetworkConfig.GetFromNetwork(Provider);

                // execute transactions
                var txsLimit = 100;
                var txsExecuted = 0;
                var account = new Account(request.ExecutorAddress);

                do
                {
                    if (request.NrOfTransactions < txsLimit)
                    {
                        txsLimit = request.NrOfTransactions;
                    }

                    for (var i = 0; i < txsLimit; i++)
                    {
                        var tx = SmartContractTransactionRequest.Call(
                            networkConfig,
                            account,
                            Address.FromBech32(request.ContractAddress),
                            new GasLimit(100000000),
                            "smartExecute"
                        );
                        var signedTx = tx.ApplySignature(signer.SignTransaction(tx.SerializeForSigning()));
                        await Provider.SendTransaction(signedTx);
                        account.IncrementNonce();
                        txsExecuted++;
                    }

                    await Task.Delay(RoundDuration);

                    if (txsExecuted == request.NrOfTransactions)
                        break;

                    txsLimit = 100; // get from pool
                    if (txsLimit + txsExecuted > request.NrOfTransactions)
                    {
                        txsLimit = request.NrOfTransactions - txsExecuted;
                    }
                } while (txsExecuted < request.NrOfTransactions);

                // finish execution transaction
                var finishTx = SmartContractTransactionRequest.Call(
                    networkConfig,
                    account,
                    Address.FromBech32(request.ContractAddress),
                    new GasLimit(10000000),
                    "finishExecution",
                    Address.FromBech32(request.InitiatorAddress),
                    Address.FromBech32(request.ExecutorAddress)
                );
                var signedFinishTx = finishTx.ApplySignature(signer.SignTransaction(finishTx.SerializeForSigning()));
                await Provider.SendTransaction(signedFinishTx);
                account.IncrementNonce();

                // wait one round and sync account
                await Task.Delay(RoundDuration);
                await account.Sync(Provider);


                // return remaining amount to initial wallet
                var egldFee = ESDTAmount.EGLD("0.00005");
                var remainingEgldAmount = ESDTAmount.From($"{account.Balance.Value - egldFee.Value}");

                var returnTx = EGLDTransactionRequest.EGLDTransfer(
                    networkConfig,
                    account,
                    Address.FromBech32(request.InitiatorAddress),
                    remainingEgldAmount
                );
                var signedReturnTx = returnTx.ApplySignature(signer.SignTransaction(returnTx.SerializeForSigning()));
                await Provider.SendTransaction(signedReturnTx);
            }
        );
    }
}

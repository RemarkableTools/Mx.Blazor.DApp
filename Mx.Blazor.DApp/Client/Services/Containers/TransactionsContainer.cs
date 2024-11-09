using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using Mx.Blazor.DApp.Client.Models;
using Blazored.SessionStorage;
using Mx.Blazor.DApp.Client.Application.Constants;

namespace Mx.Blazor.DApp.Client.Services.Containers
{
    public class TransactionsContainer
    {
        public readonly List<TransactionModel> TxList;
        public Action? NewTxProcessing;
        public Action? TxExecuted;
        public Action<string[]>? HashesExecuted;

        private readonly ISyncSessionStorageService _sessionStorage;
        public TransactionsContainer(ISyncSessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
            TxList = [];

            var storedTxs = _sessionStorage.GetItem<List<StorageTx>>(BrowserSessionStorage.TxList);
            if (storedTxs == null)
                return;

            storedTxs = storedTxs.Where(t => t.ToDate() > DateTime.UtcNow).ToList();

            if (storedTxs.Count > 0)
            {
                DisplayTransactionsFromStorage(storedTxs);
                _sessionStorage.SetItem(BrowserSessionStorage.TxList, storedTxs);
            }
            else
            {
                _sessionStorage.RemoveItem(BrowserSessionStorage.TxList);
            }
        }

        private void DisplayTransactionsFromStorage(List<StorageTx> list)
        {
            TxList.AddRange(list.Select(stx => new TransactionModel(stx.Title, stx.Txs)));

            NewTxProcessing?.Invoke();
        }

        public void NewTransactions(string title, params string[] hashes)
        {
            var transactionModel = new TransactionModel(title,
                                                        hashes.Select(hash => new TransactionData(hash)).ToList());
            TxList.Add(transactionModel);

            var storedTxs = _sessionStorage.GetItem<List<StorageTx>>(BrowserSessionStorage.TxList) ?? [];
            storedTxs.Add(new StorageTx(title, transactionModel.Transactions, (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds + TxRemovalTime));
            _sessionStorage.SetItem(BrowserSessionStorage.TxList, storedTxs);

            NewTxProcessing?.Invoke();
        }

        public void UpdateTransaction(TransactionModel transactionModel)
        {
            var storedTxs = _sessionStorage.GetItem<List<StorageTx>>(BrowserSessionStorage.TxList);
            var tx = storedTxs.Find(t => t.Txs[0].Hash == transactionModel.Transactions[0].Hash);
            if (tx is null) return; //theoretically should never happen (only to get rid of warning)
            tx.Txs = transactionModel.Transactions;
            _sessionStorage.SetItem(BrowserSessionStorage.TxList, storedTxs);
        }

        public void RemoveTransaction(TransactionModel transactionModel)
        {
            TxList.Remove(transactionModel);

            var storedTxs = _sessionStorage.GetItem<List<StorageTx>>(BrowserSessionStorage.TxList);
            storedTxs.RemoveAll(t => t.Txs[0].Hash == transactionModel.Transactions[0].Hash);
            if (storedTxs.Count > 0)
                _sessionStorage.SetItem(BrowserSessionStorage.TxList, storedTxs);
            else
                _sessionStorage.RemoveItem(BrowserSessionStorage.TxList);
        }

        public void TransactionsExecuted(string[] hashes)
        {
            TxExecuted?.Invoke();
            HashesExecuted?.Invoke(hashes);
        }
    }
}

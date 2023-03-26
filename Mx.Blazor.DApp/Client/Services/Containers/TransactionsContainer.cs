using static Mx.Blazor.DApp.Client.Application.Constants.DappConstants;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserStorage;
using Mx.Blazor.DApp.Client.Models;
using Blazored.SessionStorage;

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
            TxList = new();

            var storedTxs = _sessionStorage.GetItem<List<StorageTx>>(TX_LIST);
            if (storedTxs != null)
            {
                storedTxs = storedTxs.Where(t => t.ToDate() > DateTime.UtcNow).ToList();

                if (storedTxs.Count > 0)
                {
                    DisplayTransactionsFromStorage(storedTxs);
                    _sessionStorage.SetItem(TX_LIST, storedTxs);
                }
                else
                {
                    _sessionStorage.RemoveItem(TX_LIST);
                }
            }
        }

        public void DisplayTransactionsFromStorage(List<StorageTx> list)
        {
            TxList.AddRange(list.Select(stx => new TransactionModel(stx.Title, stx.Txs)));

            NewTxProcessing?.Invoke();
        }

        public void NewTransaction(string title, string hash)
        {
            NewTransaction(title, new string[] { hash });
        }

        public void NewTransaction(string title, string[] hashes)
        {
            var transactionModel = new TransactionModel(title,
                                                        hashes.Select(hash => new TransactionData(hash)).ToList());
            TxList.Add(transactionModel);

            var storedTxs = _sessionStorage.GetItem<List<StorageTx>>(TX_LIST) ?? new List<StorageTx>();
            storedTxs.Add(new StorageTx(title, transactionModel.Transactions, (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds + TX_REMOVAL_TIME));
            _sessionStorage.SetItem(TX_LIST, storedTxs);

            NewTxProcessing?.Invoke();
        }

        public void UpdateTransaction(TransactionModel transactionModel)
        {
            var storedTxs = _sessionStorage.GetItem<List<StorageTx>>(TX_LIST);
            var tx = storedTxs.Find(t => t.Txs[0].Hash == transactionModel.Transactions[0].Hash);
            if (tx is null) return; //theoretically should never happen (only to get rid of warning)
            tx.Txs = transactionModel.Transactions;
            _sessionStorage.SetItem(TX_LIST, storedTxs);
        }

        public void RemoveTransaction(TransactionModel transactionModel)
        {
            TxList.Remove(transactionModel);

            var storedTxs = _sessionStorage.GetItem<List<StorageTx>>(TX_LIST);
            storedTxs.RemoveAll(t => t.Txs[0].Hash == transactionModel.Transactions[0].Hash);
            if (storedTxs.Count > 0)
                _sessionStorage.SetItem(TX_LIST, storedTxs);
            else
                _sessionStorage.RemoveItem(TX_LIST);
        }

        public void TransactionsExecuted(string[] hashes)
        {
            TxExecuted?.Invoke();
            HashesExecuted?.Invoke(hashes);
        }
    }
}

import { HWProvider } from "@multiversx/sdk-hw-provider";
import { Address, SignableMessage, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import {
    showConnectionError,
    hideConnectionError,
    loginApproved,
    loginNotApproved,
    signingMessageModal,
    signingMessageModalClose,
    signingModal,
    signingModalClose,
    cancelTxToast
} from "./common";

class HardwareWallet {
    async init() {
        this.provider = new HWProvider();
        var initialized = await this.provider.init();
        if (!initialized)
            showConnectionError();
        else
            hideConnectionError();

        return initialized;
    }

    async prepLedger() {
        $("#LedgerConnectModal").modal("show");
        return await this.init();
    }

    async getLedgerAddresses(pageNumber, pageSize) {
        try {
            return await this.provider.getAccounts(pageNumber, pageSize);
        }
        catch {
            return null;
        }
    }

    async login(authToken) {
        const payloadToSign = Buffer.from(`${authToken}{}`);
        const addressIndex = document.getElementById("LedgerAddressIndex").value;

        try {
            const { address, signature } = await this.provider.tokenLogin({ addressIndex: addressIndex, token: payloadToSign });

            loginApproved();
            sessionStorage.setItem("wallettype", "3");
            $("#LedgerConnectModal").modal("hide");
            $("#WalletConnectionsModal").modal("hide");
            return JSON.stringify({
                address: address || "",
                signature: signature.hex()
            });
        }
        catch {
            loginNotApproved();
            $("#LedgerConnectModal").modal("hide");
            return JSON.stringify({
                address: "",
                signature: null
            });
        }
    }

    async getAddress() {
        return await this.provider.getAddress();
    }

    async isConnected() {
        return await this.provider.isConnected();
    }

    async logout() {
        await this.provider.logout();
    }

    transactionCanceled() {
        cancelTxToast();
    }

    async signMessage(message) {
        signingMessageModal("Ledger");

        const signableMessage = new SignableMessage({
            message: Buffer.from(message)
        });

        try {
            await this.provider.signMessage(signableMessage);
            return JSON.stringify(signableMessage.toJSON(), null, 4);
        }
        catch (err) {
            return "canceled";
        }
        finally {
            signingMessageModalClose();
        }
    }

    async signTransaction(transactionRequest) {
        signingModal("Ledger");

        try {
            const transaction = new Transaction({
                nonce: transactionRequest.nonce,
                value: transactionRequest.value,
                receiver: new Address(transactionRequest.receiver),
                sender: new Address(transactionRequest.sender),
                gasPrice: transactionRequest.gasPrice,
                gasLimit: transactionRequest.gasLimit,
                data: new TransactionPayload(transactionRequest.data),
                chainID: transactionRequest.chainID,
                version: transactionRequest.transactionVersion
            });

            await this.provider.signTransaction(transaction);
            return JSON.stringify(transaction.toSendable(), null, 4);
        }
        catch (err) {
            return "canceled";
        }
        finally {
            signingModalClose();
        }
    }

    async signTransactions(transactionsRequest) {
        signingModal("Ledger");

        try {
            const transactions = transactionsRequest.map(transactionRequest =>
                new Transaction({
                    nonce: transactionRequest.nonce,
                    value: transactionRequest.value,
                    receiver: new Address(transactionRequest.receiver),
                    sender: new Address(transactionRequest.sender),
                    gasPrice: transactionRequest.gasPrice,
                    gasLimit: transactionRequest.gasLimit,
                    data: new TransactionPayload(transactionRequest.data),
                    chainID: transactionRequest.chainID,
                    version: transactionRequest.transactionVersion
                })
            );
            await this.provider.signTransactions(transactions);
            return JSON.stringify(transactions.map(transaction => transaction.toSendable()), null, 4);
        }
        catch (err) {
            return "canceled";
        }
        finally {
            signingModalClose();
        }
    }
}

export const Obj = new HardwareWallet();

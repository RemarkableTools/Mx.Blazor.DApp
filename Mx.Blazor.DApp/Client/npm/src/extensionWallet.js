import { ExtensionProvider } from "@multiversx/sdk-extension-provider";
import { Address, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import {
    showConnectionError,
    hideConnectionError,
    loginApproved,
    loginNotApproved,
    signingModal,
    signingModalClose,
    cancelTxToast
} from "./common";

class ExtensionWallet {
    async init(address) {
        this.provider = ExtensionProvider.getInstance();
        var initialized = await this.provider.init();
        if (address)
            this.provider = this.provider.setAddress(address);

        if (!initialized)
            showConnectionError();
        else
            hideConnectionError();

        return initialized;
    }

    async login(authToken) {
        await this.provider.login({ token: authToken });

        if (this.provider.account.signature) {
            $("#WalletConnectionsModal").modal("hide");
            loginApproved();
            sessionStorage.setItem("wallettype", "1");
        }
        else {
            loginNotApproved();
        }

        return JSON.stringify({
            address: this.provider.account.address,
            signature: this.provider.account.signature
        });
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

    async signTransaction(transactionRequest) {
        signingModal("MultiversX DeFi Wallet");

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

        try {
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
        signingModal("MultiversX DeFi Wallet");

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

        try {
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

export const Obj = new ExtensionWallet();

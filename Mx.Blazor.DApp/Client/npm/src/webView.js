import { Address, SignableMessage, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import { WebviewProvider } from "@multiversx/sdk-webview-provider/out/WebviewProvider";
import {
    showConnectionError,
    hideConnectionError,
    cancelTxToast
} from "./common";

class WebView {
    async init() {
        this.provider = WebviewProvider.getInstance();
        var initialized = await this.provider.init();
        console.log("init called");

        if (!initialized)
            showConnectionError();
        else
            hideConnectionError();

        return initialized;
    }

    async login() {
        return await this.provider.login();
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
    }

    async signTransaction(transactionRequest) {
        const transaction = new Transaction({
            nonce: transactionRequest.nonce,
            value: transactionRequest.value,
            receiver: new Address(transactionRequest.receiver),
            sender: new Address(transactionRequest.sender),
            gasPrice: transactionRequest.gasPrice,
            gasLimit: transactionRequest.gasLimit,
            data: new TransactionPayload(transactionRequest.data),
            chainID: transactionRequest.chainID,
            version: transactionRequest.transactionVersion,
            options: transactionRequest.options,
            guardian: new Address(transactionRequest.guardian)
        });

        try {
            const signedTransaction = await this.signTransactions([transaction]);
            return JSON.stringify(JSON.parse(signedTransaction)[0], null, 4);
        }
        catch (err) {
            return "canceled";
        }
    }

    async signTransactions(transactionsRequest) {
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
                version: transactionRequest.transactionVersion,
                options: transactionRequest.options,
                guardian: new Address(transactionRequest.guardian)
            })
        );

        try {
            const signedTransactions = await this.provider.signTransactions(transactions);
            return JSON.stringify(signedTransactions.map(transaction => transaction.toSendable()), null, 4);
        }
        catch (err) {
            return "canceled";
        }
    }

    cancelAction() {
        this.provider.cancelAction();
    }
}

export const Obj = new WebView();
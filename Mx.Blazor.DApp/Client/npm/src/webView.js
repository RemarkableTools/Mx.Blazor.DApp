import {WebviewProvider} from "@multiversx/sdk-webview-provider/out";
import {Address, Message, Transaction, TransactionPayload} from "@multiversx/sdk-core/out";
import {
    signingMessageModalClose,
    signingModalClose,
} from "./common";

class WebView {
    async init() {
        this.provider = WebviewProvider.getInstance();
        return await this.provider.init();
    }

    async login() {
        return await this.provider.login();
    }

    async logout() {
        await this.provider.logout();
    }

    async signMessage(message) {
        try {
            let signedMessage = await this.provider.signMessage(
                new Message({
                    data: Buffer.from(message)
                })
            );

            return signedMessage.signature.toString("hex")
        } catch (err) {
            return null;
        } finally {
            signingMessageModalClose();
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
        } catch (err) {
            return null;
        } finally {
            signingModalClose();
        }
    }
}

export const Obj = new WebView();
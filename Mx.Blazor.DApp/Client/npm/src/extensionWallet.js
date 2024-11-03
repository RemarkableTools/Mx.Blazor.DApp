import { ExtensionProvider } from "@multiversx/sdk-extension-provider";
import { Address, Message, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import {
    showConnectionError,
    hideConnectionError,
    signingMessageModal,
    signingMessageModalClose,
    signingModal,
    signingModalClose,
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
        let account = await this.provider.login({ token: authToken });

        if (this.isConnected()) {
            $("#WalletConnectionsModal").modal("hide");
            localStorage.setItem("walletType", "1");
        }

        return account;
    }

    isConnected() {
        return this.provider.isConnected() && this.provider.account.signature;
    }

    async logout() {
        await this.provider.logout();
    }

    async signMessage(message) {
        signingMessageModal("MultiversX DeFi Wallet");

        try {
            let signedMessage = await this.provider.signMessage(
                new Message({
                    data: Buffer.from(message)
                })
            );

            return signedMessage.signature.toString("hex")
        }
        catch (err) {
            return null;
        }
        finally {
            signingMessageModalClose();
        }
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
            version: transactionRequest.transactionVersion,
            options: transactionRequest.options,
            guardian: new Address(transactionRequest.guardian)
        });

        try {
            const signedTransaction = await this.provider.signTransaction(transaction);
            return JSON.stringify(signedTransaction.toSendable(), null, 4);
        }
        catch (err) {
            return null;
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
            return null;
        }
        finally {
            signingModalClose();
        }
    }

    cancelAction() {
        this.provider.cancelAction();
    }
}

export const Obj = new ExtensionWallet();

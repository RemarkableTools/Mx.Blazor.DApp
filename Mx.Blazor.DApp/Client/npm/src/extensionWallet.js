import { ExtensionProvider } from "@multiversx/sdk-extension-provider";
import { Address, SignableMessage, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import {
    showConnectionError,
    hideConnectionError,
    signingMessageModal,
    signingMessageModalClose,
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
            localStorage.setItem("wallettype", "1");
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

    async signMessage(message) {
        signingMessageModal("MultiversX DeFi Wallet");

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
        finally {
            signingModalClose();
        }
    }

    cancelAction() {
        this.provider.cancelAction();
    }
}

export const Obj = new ExtensionWallet();

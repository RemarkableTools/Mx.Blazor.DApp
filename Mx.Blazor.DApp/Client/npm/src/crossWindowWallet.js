﻿import {CrossWindowProvider} from "@multiversx/sdk-web-wallet-cross-window-provider/out";
import {Address, Message, Transaction, TransactionPayload} from "@multiversx/sdk-core/out";
import {
    showConnectionError,
    hideConnectionError,
    signingMessageModal,
    signingMessageModalClose,
    signingModal,
    signingModalClose,
} from "./common";

class CrossWindowWallet {
    async init(walletURL, address) {
        this.provider = CrossWindowProvider.getInstance();
        const initialized = await this.provider.init();
        this.provider.setWalletUrl(walletURL);
        if (address)
            this.provider = this.provider.setAddress(address);

        if (!initialized)
            showConnectionError();
        else
            hideConnectionError();

        return initialized;
    }

    async login(authToken) {
        let account = await this.provider.login({token: authToken});

        if (this.isConnected()) {
            $("#WalletConnectionsModal").modal("hide");
            localStorage.setItem("walletType", "4");
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
        signingMessageModal("MultiversX Web Wallet");

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
        signingModal("MultiversX Web Wallet");

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

    cancelAction() {
        this.provider.cancelAction();
    }
}

export const Obj = new CrossWindowWallet();

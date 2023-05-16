import qs from "qs";
import { WalletProvider } from "@multiversx/sdk-web-wallet-provider";
import { Address, SignableMessage, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import {
    cancelTxToast
} from "./common";

class WebWallet {
    init(walletURL) {
        this.provider = new WalletProvider(walletURL);
        return true;
    }

    async login(authToken) {
        sessionStorage.setItem("authToken", authToken);
        localStorage.setItem("wallettype", "4");
        localStorage.setItem("webwalletstate", "2");
        const callbackUrl = getCurrentLocation();
        await this.provider.login({ callbackUrl: callbackUrl, token: authToken });
    }

    getAddress() {
        return localStorage.getItem("address") || "";
    }

    isConnected() {
        return !!getAddress();
    }

    async logout() {
        await this.provider.logout();
    }

    transactionCanceled() {
        cancelTxToast();
    }

    async signMessage(message) {
        sessionStorage.setItem("sigmessage", message);
        localStorage.setItem("webwalletstate", "3");

        const signableMessage = new SignableMessage({
            message: Buffer.from(message)
        });

        const callbackUrl = getCurrentLocation();
        await this.provider.signMessage(signableMessage, { callbackUrl: callbackUrl });
        return "";
    }

    async signTransaction(transactionRequest) {
        localStorage.setItem("webwalletstate", "4");

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

        const callbackUrl = getCurrentLocation();
        await this.provider.signTransaction(transaction, { callbackUrl: callbackUrl });
        return "";
    }

    async signTransactions(transactionsRequest) {
        localStorage.setItem("webwalletstate", "4");

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

        const callbackUrl = getCurrentLocation();
        await this.provider.signTransactions(transactions, { callbackUrl: callbackUrl });
        return "";
    }
}

export const Obj = new WebWallet();

function getUrlParams() {
    const queryString = window.location.search.slice(1);
    const params = qs.parse(queryString);
    return params;
}

function getCurrentLocation() {
    return window.location.href.split("?")[0];
}

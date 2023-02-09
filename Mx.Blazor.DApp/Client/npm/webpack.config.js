const path = require("path");
const webpack = require("webpack");

module.exports = [
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "extensionWallet.js",
            library: "ExtensionWallet"
        },
        name: "ExtensionWallet",
        entry: "./src/extensionWallet.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ['buffer', 'Buffer'],
            })
        ],
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "walletConnect.js",
            library: "WalletConnect"
        },
        name: "WalletConnect",
        entry: "./src/walletConnect.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ['buffer', 'Buffer'],
            })
        ],
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "walletConnectV2.js",
            library: "WalletConnectV2"
        },
        name: "WalletConnectV2",
        entry: "./src/walletConnectV2.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "webWallet.js",
            library: "WebWallet"
        },
        name: "WebWallet",
        entry: "./src/webWallet.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ['buffer', 'Buffer'],
            })
        ],
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "hardwareWallet.js",
            library: "HardwareWallet"
        },
        name: "HardwareWallet",
        entry: "./src/hardwareWallet.js"
    }
];
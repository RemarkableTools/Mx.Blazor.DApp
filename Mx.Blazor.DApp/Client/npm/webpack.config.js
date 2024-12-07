const path = require("path");
const webpack = require("webpack");

module.exports = [
    {
        resolve: {
            fallback: {
                crypto: false,
                stream: false,
                fs: false,
                path: false,
            },
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader",
                    },
                },
            ],
        },
        output: {
            path: path.resolve(__dirname, "../wwwroot/js/npm"),
            filename: "extensionWallet.js",
            library: "ExtensionWallet",
        },
        name: "ExtensionWallet",
        entry: "./src/extensionWallet.js",
    },
    {
        resolve: {
            fallback: {
                crypto: false,
                stream: false,
                fs: false,
                path: false,
            },
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader",
                    },
                },
            ],
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ["buffer", "Buffer"],
            }),
        ],
        output: {
            path: path.resolve(__dirname, "../wwwroot/js/npm"),
            filename: "xPortalWallet.js",
            library: "XPortalWallet",
        },
        name: "XPortalWallet",
        entry: "./src/xPortalWallet.js",
    },
    {
        resolve: {
            fallback: {
                crypto: false,
                stream: false,
                fs: false,
                path: false,
            },
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader",
                    },
                },
            ],
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ["buffer", "Buffer"],
            }),
        ],
        output: {
            path: path.resolve(__dirname, "../wwwroot/js/npm"),
            filename: "hardwareWallet.js",
            library: "HardwareWallet",
        },
        name: "HardwareWallet",
        entry: "./src/hardwareWallet.js",
    },
    {
        resolve: {
            fallback: {
                crypto: false,
                stream: false,
                fs: false,
                path: false,
            },
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader",
                    },
                },
            ],
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ["buffer", "Buffer"],
            }),
        ],
        output: {
            path: path.resolve(__dirname, "../wwwroot/js/npm"),
            filename: "webView.js",
            library: "WebView",
        },
        name: "WebView",
        entry: "./src/webView.js",
    },
    {
        resolve: {
            fallback: {
                crypto: false,
                stream: false,
                fs: false,
                path: false,
            },
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader",
                    },
                },
            ],
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ["buffer", "Buffer"],
            }),
        ],
        output: {
            path: path.resolve(__dirname, "../wwwroot/js/npm"),
            filename: "crossWindowWallet.js",
            library: "CrossWindowWallet",
        },
        name: "CrossWindowWallet",
        entry: "./src/crossWindowWallet.js",
    },
    {
        resolve: {
            fallback: {
                crypto: false,
                stream: false,
                fs: false,
                path: false,
            },
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader",
                    },
                },
            ],
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ["buffer", "Buffer"],
            }),
        ],
        output: {
            path: path.resolve(__dirname, "../wwwroot/js/npm"),
            filename: "metaMaskWallet.js",
            library: "MetaMaskWallet",
        },
        name: "MetaMaskWallet",
        entry: "./src/metaMaskWallet.js",
    },
];

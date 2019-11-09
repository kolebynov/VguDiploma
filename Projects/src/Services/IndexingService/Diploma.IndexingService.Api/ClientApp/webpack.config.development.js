module.exports = function () {
	const HtmlWebpackPlugin = require('html-webpack-plugin');
	const MiniCssExtractPlugin = require('mini-css-extract-plugin');
	const TsconfigPathsPlugin = require('tsconfig-paths-webpack-plugin');

	const webpack = require('webpack');

	const inputScriptsFile = './src/index.tsx';
	const indexTemplate = './src/index.html';

	const outputChunksFormat = '[chunkhash].js';
	const outputStylesFormat = './styles.css';

	return {
		entry: {
			scripts: inputScriptsFile,
		},

		output: {
			chunkFilename: outputChunksFormat
		},

		resolve: {
			plugins: [new TsconfigPathsPlugin()],
			extensions: ['.ts', '.tsx', '.js', '.jsx']
		},

		optimization: {
			splitChunks: {
				cacheGroups: {
					vendor: {
						test: /[\\/]node_modules[\\/]/,
						name: 'vendor',
						chunks: 'all'
					}
				}
			}
		},

		devServer: {
			hot: true,
			host: '127.0.0.1',
			port: 7778,
			compress: true,
			publicPath: '/',
			clientLogLevel: 'error',

			historyApiFallback: {
				rewrites: [
					{ from: /^\/$/, to: '/index.html' }
				]
			},

			proxy: {
				'/api/*': {
					target: 'http://127.0.0.1:5000'
				}
			}
		},

		plugins: [
			new HtmlWebpackPlugin({
				template: indexTemplate
			}),
			new MiniCssExtractPlugin(outputStylesFormat),
			new webpack.HotModuleReplacementPlugin()
		],

		module: {
			rules: [
				{
					test: /\.(html)$/,
					use: [{
						loader: 'html-loader',
						options: {
							interpolate: true
						}
					}],
				},
				{
					test: /.(ts|tsx)$/,
					exclude: /node_modules/,
					use: [{
						loader: 'babel-loader',
						options: {
							presets: ['@babel/env', '@babel/react'],
							plugins: ['@babel/proposal-class-properties', '@babel/syntax-dynamic-import']
						}
					}]
				},
				{
					test: /\.tsx?$/,
					loader: "awesome-typescript-loader"
				},
				{
					test: /\.(scss)$/,
					use: [
						MiniCssExtractPlugin.loader,
						'css-loader',
						'sass-loader'
					]
				}
			]
		}
	}
}
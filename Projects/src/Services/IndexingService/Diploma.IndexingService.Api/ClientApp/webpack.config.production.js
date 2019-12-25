module.exports = function (props) {
	const HtmlWebpackPlugin = require('html-webpack-plugin');
	const { CleanWebpackPlugin } = require('clean-webpack-plugin');
	const MiniCssExtractPlugin = require('mini-css-extract-plugin');
	const TerserPlugin = require('terser-webpack-plugin');
	const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
	const TsconfigPathsPlugin = require('tsconfig-paths-webpack-plugin');

	const path = require('path');
	const dist = path.resolve(__dirname, props.clientAppPath || 'wwwroot');

	const inputScriptsFile = './src/index.tsx';
	const indexTemplate = './src/index.html';

	const outputChunksFormat = '[name][chunkhash].js';
	const outputScriptsFormat = '[chunkhash].js';
	const outputStylesFormat = '[chunkhash].css';

	return {
		entry: {
			scripts: inputScriptsFile
		},

		resolve: {
			plugins: [new TsconfigPathsPlugin()],
			extensions: ['.ts', '.tsx', '.js', '.jsx']
		},

		output: {
			chunkFilename: outputChunksFormat,
			filename: outputScriptsFormat,
			path: dist
		},

		optimization: {
			splitChunks: {
				cacheGroups: {
					vendor: {
						test: /[\\/]node_modules[\\/]/,
						name: './',
						chunks: 'all'
					}
				}
			}
		},

		plugins: [
			new CleanWebpackPlugin(),
			new TerserPlugin({
				parallel: true,
				terserOptions: {
					compress: {
						drop_console: true,
						module: true,
						passes: 3,
						toplevel: true
					},
					mangle: {
						module: true,
						toplevel: true
					},
					module: true,
					toplevel: true
				}
			}),
			new HtmlWebpackPlugin({
				template: indexTemplate,
				minify: {
					html5: true,
					collapseWhitespace: true,
					removeComments: true,
					removeAttributeQuotes: true,
					removeScriptTypeAttributes: true,
					removeRedundantAttributes: true,
					removeStyleLinkTypeAttributes: true
				}
			}),
			new MiniCssExtractPlugin({ filename: outputStylesFormat }),
			new OptimizeCssAssetsPlugin({})
		],

		module: {
			rules: [
				{
					test: /\.(html)$/,
					use: [{
						loader: 'html-loader',
						options: {
							interpolate: true,
							minimize: true,
							removeComments: true,
							collapseWhitespace: true,
							removeAttributeQuotes: true
						}
					}],
				},
				{
					test: /\.(scss)$/,
					use: [
						MiniCssExtractPlugin.loader,
						'css-loader',
						'sass-loader'
					]
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
				}
			]
		}
	}
}
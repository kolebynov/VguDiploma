module.exports = function (_env, props) {
	return require(`./webpack.config.${props.mode}.js`)(_env, props)
}
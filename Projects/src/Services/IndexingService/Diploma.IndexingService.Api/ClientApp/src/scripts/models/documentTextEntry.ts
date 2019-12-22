enum TextType {
    Text,
	HighlightedText
}

interface DocumentTextEntry {
    textType: TextType;
    text: string;
}

export { TextType, DocumentTextEntry };
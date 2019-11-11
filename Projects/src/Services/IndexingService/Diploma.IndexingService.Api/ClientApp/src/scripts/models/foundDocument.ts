interface FoundDocument {
    id: string;
    fileName: string;
    matches: Record<string, string[]>;
}

export { FoundDocument };
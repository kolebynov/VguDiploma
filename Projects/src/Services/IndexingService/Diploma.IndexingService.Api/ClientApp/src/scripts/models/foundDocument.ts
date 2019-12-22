import { DocumentTextEntry } from "./documentTextEntry";

interface FoundDocument {
    id: string;
    fileName: string;
    matches: Record<string, DocumentTextEntry[][]>;
}

export { FoundDocument };
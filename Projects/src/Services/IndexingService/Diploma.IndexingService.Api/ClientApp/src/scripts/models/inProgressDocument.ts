import { GetDocument } from "./document";

enum InProcessDocumentState {
    InQueue,
    Processing,
    Done,
    Error
};

interface InProgressDocument {
    document: GetDocument;
    state: InProcessDocumentState;
    errorInfo?: string;
}

export { InProgressDocument, InProcessDocumentState };
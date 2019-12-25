import { GetDocument } from "./document";

enum InProcessDocumentState {
    WaitingToUpload,
    Uploading,
    Uploaded,
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
import { GetDocument } from "./document";

enum InProcessDocumentState {
    ReadyToUpload,
    WaitingToUpload,
    Uploading,
    Uploaded,
    InQueue,
    Processing,
    Done
};

interface InProgressDocument {
    document: GetDocument;
    state: InProcessDocumentState;
}

export { InProgressDocument, InProcessDocumentState };
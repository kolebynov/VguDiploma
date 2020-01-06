interface GetDocument {
    id: string;
    fileName: string;
    modificationDate: string;
    size: number;
}

interface AddDocument {
    id: string;
    fileName: string;
    modificationDate: string;
    contentToken: string;
}

interface AddDocuments {
    documents: AddDocument[];
    folderId: string;
}

export { GetDocument, AddDocument, AddDocuments };
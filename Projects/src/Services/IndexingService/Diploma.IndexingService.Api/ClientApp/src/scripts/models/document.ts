interface GetDocument {
    id: string;
    fileName: string;
    modificationDate: string;
}

interface AddDocument {
    id: string;
    fileName: string;
    modificationDate: string;
    contentToken: string;
}

export { GetDocument, AddDocument };
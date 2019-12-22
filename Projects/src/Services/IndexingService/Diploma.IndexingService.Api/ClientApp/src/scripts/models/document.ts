interface GetDocument {
    id: string;
    fileName: string;
    modificationDate: string;
}

interface AddDocument {
    id: string;
    fileName: string;
    modificationDate: Date;
    contentToken: string;
}

export { GetDocument, AddDocument };
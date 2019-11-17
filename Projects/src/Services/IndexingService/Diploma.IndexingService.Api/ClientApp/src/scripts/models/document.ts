interface GetDocument {
    id: string;
    fileName: string;
    modificationDate: Date;
}

interface AddDocument {
    id: string;
    fileName: string;
    modificationDate: Date;
    contentToken: string;
}

export { GetDocument, AddDocument };
import { AddDocumentResult, ApiResult, AddDocument, GetDocument, FoundDocument, InProcessDocumentState } from "@app/models";
import axios from "axios";
import { inProgressDocumentService } from "./inProgressDocumentService";

export interface AddDocumentModel {
    document: GetDocument;
    file: File;
}

class DocumentService {
    public async addDocuments(documents: AddDocumentModel[], callback: (res: AddDocumentResult) => void = null)
        : Promise<AddDocumentResult[]> {
        documents.forEach(({ document }) =>
            inProgressDocumentService.updateState(document, InProcessDocumentState.WaitingToUpload));

        const results: AddDocumentResult[] = [];
        for (const document of documents) {
            const result = await this.addDocument(document);
            if (callback) {
                callback(result);
            }
            results.push(result);
        }

        return results;
    }

    private async addDocument({ file, document }: AddDocumentModel): Promise<AddDocumentResult> {
        const formData = new FormData();
        formData.append("files", file);

        inProgressDocumentService.updateState(document, InProcessDocumentState.Uploading);

        const { data: { data: [contentToken] } } = await axios
            .post<ApiResult<string[]>>("/api/documents/upload", formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

        inProgressDocumentService.updateState(document, InProcessDocumentState.Uploaded);

        const addDocument: AddDocument = {
            id: document.id,
            fileName: file.name,
            modificationDate: document.modificationDate,
            contentToken: contentToken
        };
        const { data: { data: [result] } } = await axios.post<ApiResult<AddDocumentResult[]>>(`/api/documents`, [addDocument]);
        inProgressDocumentService.updateState(document, result.state);
        return result;
    }

    public getDocuments(): Promise<GetDocument[]> {
        return axios
            .get<ApiResult<Array<GetDocument>>>("/api/documents")
            .then(response => response.data.data);
    }

    public getContentUri(docId: string) {
        return `/api/documents/${docId}/content`;
    }

    public search(searchString: string): Promise<FoundDocument[]> {
        return axios
            .get<ApiResult<FoundDocument[]>>(`/api/search?searchString=${searchString}`)
            .then(response => response.data.data);
    }
}

const documentService = new DocumentService();
export { documentService };
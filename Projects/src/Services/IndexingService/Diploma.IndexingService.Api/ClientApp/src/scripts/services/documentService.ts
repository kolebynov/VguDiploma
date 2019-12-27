import { AddDocumentResult, ApiResult, AddDocument, GetDocument, FoundDocument, InProcessDocumentState, AddDocuments } from "@app/models";
import axios from "axios";
import { inProgressDocumentService } from "./inProgressDocumentService";

export interface AddDocumentModel {
    document: GetDocument;
    file: File;
}

class DocumentService {
    public async addDocuments(documents: AddDocumentModel[], folderId: string,
        callback: (res: AddDocumentResult) => void = null)
        : Promise<AddDocumentResult[]> {
        documents.forEach(({ document }) =>
            inProgressDocumentService.updateState(document, InProcessDocumentState.WaitingToUpload));

        const results: AddDocumentResult[] = [];
        for (const document of documents) {
            const result = await this.addDocument(document, folderId);
            if (callback) {
                callback(result);
            }
            results.push(result);
        }

        return results;
    }

    public getContentUri(docId: string) {
        return `/api/documents/${docId}/content`;
    }

    public search(searchString: string): Promise<FoundDocument[]> {
        return axios
            .get<ApiResult<FoundDocument[]>>(`/api/search?searchString=${searchString}`)
            .then(response => response.data.data);
    }

    private async addDocument({ file, document }: AddDocumentModel, folderId: string): Promise<AddDocumentResult> {
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

        const addDocuments: AddDocuments = {
            folderId,
            documents: [{
                id: document.id,
                fileName: file.name,
                modificationDate: document.modificationDate,
                contentToken: contentToken
            }]
        };
        const { data: { data: [result] } } = await axios.post<ApiResult<AddDocumentResult[]>>(`/api/documents`, addDocuments);
        inProgressDocumentService.updateState(document, result.state);
        return result;
    }
}

const documentService = new DocumentService();
export { documentService };
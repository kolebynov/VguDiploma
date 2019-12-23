import { AddDocumentResult, ApiResult, AddDocument, GetDocument, FoundDocument, InProcessDocumentState } from "@app/models";
import axios from "axios";
import { inProgressDocumentService } from "./inProgressDocumentService";

class DocumentService {
    public addDocument(document: GetDocument, file: File): Promise<AddDocumentResult> {
        const formData = new FormData();
        formData.append("files", file);

        inProgressDocumentService.updateState(document, InProcessDocumentState.Uploading);

        return axios
            .post<ApiResult<string[]>>("/api/documents/upload", formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
            .then(({ data: { data: [contentToken] } }) => {
                inProgressDocumentService.updateState(document, InProcessDocumentState.Uploaded);

                const addDocument: AddDocument = {
                    id: document.id,
                    fileName: file.name,
                    modificationDate: new Date(file.lastModified),
                    contentToken: contentToken
                };
                return axios.post<ApiResult<AddDocumentResult[]>>(`/api/documents`, [addDocument]);
            })
            .then(response => {
                const result = response.data.data[0];
                inProgressDocumentService.updateState(document, result.state);
                return result;
            });
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
import { AddDocumentResult, ApiResult, AddDocument, GetDocument, FoundDocument } from "@app/models";
import axios from "axios";

class DocumentService {
    public addDocument(file: File): Promise<AddDocumentResult> {
        const formData = new FormData();
        formData.append("files", file);
        return axios
            .post<ApiResult<string[]>>("/api/documents/upload", formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
            .then(({ data: { data: [contentToken] } }) => {
                const addDocument: AddDocument = {
                    id: `${file.name}_${file.size}`,
                    fileName: file.name,
                    modificationDate: new Date(file.lastModified),
                    contentToken: contentToken
                };
                return axios.post<ApiResult<AddDocumentResult[]>>(`/api/documents`, [addDocument]);
            })
            .then(response => response.data.data[0]);
    }

    public getDocuments(): Promise<GetDocument[]> {
        return axios
            .get<ApiResult<Array<GetDocument>>>("/api/documents")
            .then(response => response.data.data);
    }

    public search(searchString: string): Promise<FoundDocument[]> {
        return axios
            .get<ApiResult<FoundDocument[]>>(`/api/search?searchString=${searchString}`)
            .then(response => response.data.data);
    }
}

const documentService = new DocumentService();
export { documentService };
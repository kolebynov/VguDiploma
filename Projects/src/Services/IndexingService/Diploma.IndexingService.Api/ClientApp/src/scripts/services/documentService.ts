import { AddDocumentResult, ApiResult, AddDocument, GetDocument, FoundDocument, InProcessDocumentState, AddDocuments } from "@app/models";
import axios from "axios";
import { inProgressDocumentService } from "./inProgressDocumentService";
import { BehaviorSubject, Subscribable, PartialObserver, Unsubscribable } from "rxjs";
import { GetFolder } from "@app/models/folder";

export interface AddDocumentModel {
    document: GetDocument;
    file: File;
}

export enum UploadingState {
    WaitingToUpload,
    Uploading,
    Error
}

export interface UploadingDocument {
    state: UploadingState;
    document: GetDocument;
    folder: GetFolder;
}

class DocumentService implements Subscribable<UploadingDocument[]> {
    private readonly uploadingDocuments = new BehaviorSubject<UploadingDocument[]>([]);

    public async addDocuments(documents: AddDocumentModel[], folder: GetFolder): Promise<AddDocumentResult[]> {
        this.uploadingDocuments.next(this.uploadingDocuments.value.concat(
            documents.map(({ document }) => ({
                state: UploadingState.WaitingToUpload,
                document,
                folder
            }))
        ));

        const results: AddDocumentResult[] = [];
        for (const document of documents) {
            const index = this.uploadingDocuments.value
                .findIndex(x => x.document.id === document.document.id && x.folder.id === folder.id);
            this.uploadingDocuments.value[index] = {
                state: UploadingState.Uploading,
                ...this.uploadingDocuments.value[index]
            }
            const result = await this.addDocument(document, folder.id);
            results.push(result);

            this.uploadingDocuments.next(this.uploadingDocuments.value
                .filter(x => !(x.document.id === document.document.id && x.folder.id === folder.id)));
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

    subscribe(observer?: PartialObserver<UploadingDocument[]>): Unsubscribable;
    subscribe(next: null, error: null, complete: () => void): Unsubscribable;
    subscribe(next: null, error: (error: any) => void, complete?: () => void): Unsubscribable;
    subscribe(next: (value: UploadingDocument[]) => void, error: null, complete: () => void): Unsubscribable;
    subscribe(next?: (value: UploadingDocument[]) => void, error?: (error: any) => void, complete?: () => void): Unsubscribable;
    subscribe(next?: any, error?: any, complete?: any) {
        return this.uploadingDocuments.subscribe(next, error, complete);
    }

    private async addDocument({ file, document }: AddDocumentModel, folderId: string): Promise<AddDocumentResult> {
        const formData = new FormData();
        formData.append("files", file);

        const { data: { data: [contentToken] } } = await axios
            .post<ApiResult<string[]>>("/api/documents/upload", formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

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
        return result;
    }
}

const documentService = new DocumentService();
export { documentService };
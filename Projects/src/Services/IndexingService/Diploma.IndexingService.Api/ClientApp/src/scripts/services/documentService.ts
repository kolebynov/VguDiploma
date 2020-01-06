import { AddDocumentResult, ApiResult, AddDocument, GetDocument, FoundDocument, InProcessDocumentState, AddDocuments, PaginationApiResult } from "@app/models";
import { BehaviorSubject, Subscribable, PartialObserver, Unsubscribable } from "rxjs";
import { GetFolder } from "@app/models/folder";
import { apiRequestExecutor } from "./apiRequestExecutor";

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
    error?: string;
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
            this.updateState(document.document.id, folder.id, UploadingState.Uploading);

            try {
            const result = await this.addDocument(document, folder.id);
            results.push(result);
            this.uploadingDocuments.next(this.uploadingDocuments.value
                .filter(x => !(x.document.id === document.document.id && x.folder.id === folder.id)));
            }
            catch (e) {
                this.updateState(document.document.id, folder.id, UploadingState.Error, e.message);
            }
        }

        return results;
    }

    public getContentUri(docId: string) {
        return `/api/documents/${docId}/content`;
    }

    public async search(searchString: string, limit = 10, skip = 0)
        : Promise<{ documents: FoundDocument[], totalCount: number }> {
        const { data, pagination } = await apiRequestExecutor
            .get<PaginationApiResult<FoundDocument[]>>(`/api/search?searchString=${searchString}&limit=${limit}&skip=${skip}`);

        return {
            documents: data,
            totalCount: pagination.totalCount
        }
    }

    public removeFailedUploadings() {
        this.uploadingDocuments.next(this.uploadingDocuments.value
            .filter(x => x.state !== UploadingState.Error));
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

        const { data: [contentToken] } = await apiRequestExecutor
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
        const { data: [result] } = await apiRequestExecutor.post<ApiResult<AddDocumentResult[]>>(`/api/documents`, addDocuments);
        return result;
    }

    private updateState(docId: string, folderId: string, newState: UploadingState, error?: string) {
        const index = this.uploadingDocuments.value
            .findIndex(x => x.document.id === docId && x.folder.id === folderId);
        this.uploadingDocuments.value[index] = {
            ...this.uploadingDocuments.value[index],
            state: newState,
            error
        }
        
        this.uploadingDocuments.next([...this.uploadingDocuments.value]);
    }
}

const documentService = new DocumentService();
export { documentService };
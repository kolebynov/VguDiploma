import { BehaviorSubject, Subscribable, PartialObserver, Unsubscribable } from "rxjs";
import { InProgressDocument, GetDocument, InProcessDocumentState, ApiResult } from "@app/models";
import moment, { Moment } from "moment";
import { apiRequestExecutor } from "./apiRequestExecutor";

class InProgressDocumentService implements Subscribable<InProgressDocument[]> {
    private readonly inProgressDocuments = new BehaviorSubject<InProgressDocument[]>([]);
    private readonly lastUpdateStatesTime: Record<string, Moment> = {};
    private isLoaded = false;

    public updateState(document: GetDocument, newState: InProcessDocumentState, errorInfo = ""): void {
        const index = this.inProgressDocuments.value.findIndex(x => x.document.id === document.id);
        if (index > -1) {
            this.inProgressDocuments.value[index] = {
                document: document,
                state: newState,
                errorInfo: errorInfo
            };
        }
        else {
            this.inProgressDocuments.value.push({
                document: document,
                state: newState,
                errorInfo: errorInfo
            })
        }

        this.lastUpdateStatesTime[document.id] = moment();

        this.inProgressDocuments.next([...this.inProgressDocuments.value]);
    }

    public async remove(documentIds: string[]): Promise<void> {
        const data = await apiRequestExecutor.delete<ApiResult<any>>("/api/inProgressDocuments", {
            data: documentIds
        });

        if (!data.success) {
            throw new Error(data.errors[0].message);
        }

        this.inProgressDocuments.next(this.inProgressDocuments.value.filter(x => !documentIds.includes(x.document.id)));
    }

    subscribe(observer?: PartialObserver<InProgressDocument[]>): Unsubscribable;
    subscribe(next: null, error: null, complete: () => void): Unsubscribable;
    subscribe(next: null, error: (error: any) => void, complete?: () => void): Unsubscribable;
    subscribe(next: (value: InProgressDocument[]) => void, error: null, complete: () => void): Unsubscribable;
    subscribe(next?: (value: InProgressDocument[]) => void, error?: (error: any) => void, complete?: () => void): Unsubscribable;
    subscribe(next?: any, error?: any, complete?: any) {
        this.load();
        return this.inProgressDocuments.subscribe(next, error, complete);
    }

    private load() {
        if (this.isLoaded) {
            return;
        }

        apiRequestExecutor
            .get<ApiResult<InProgressDocument[]>>("/api/inProgressDocuments?limit=1000")
            .then(({ data }) => {
                this.isLoaded = true;
                this.inProgressDocuments.next(data);
            })
    }
}

const inProgressDocumentService = new InProgressDocumentService();

export { inProgressDocumentService };
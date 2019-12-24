import { BehaviorSubject } from "rxjs";
import { InProgressDocument, GetDocument, InProcessDocumentState } from "@app/models";

class InProgressDocumentService {
    public readonly inProgressDocuments = new BehaviorSubject<InProgressDocument[]>([]);

    constructor() {
        setInterval(this.removeCompleted.bind(this), 10000);
    }

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

        this.inProgressDocuments.next([...this.inProgressDocuments.value]);
    }

    private removeCompleted() {
        this.inProgressDocuments.next(this.inProgressDocuments.value.filter(x => x.state !== InProcessDocumentState.Done));
    }
}

const inProgressDocumentService = new InProgressDocumentService();

export { inProgressDocumentService };
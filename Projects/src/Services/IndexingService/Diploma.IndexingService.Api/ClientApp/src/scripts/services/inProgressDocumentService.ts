import { BehaviorSubject } from "rxjs";
import { InProgressDocument, GetDocument, InProcessDocumentState } from "@app/models";

class InProgressDocumentService {
    public readonly inProgressDocuments = new BehaviorSubject<InProgressDocument[]>([]);

    public updateState(document: GetDocument, newState: InProcessDocumentState): void {
        const index = this.inProgressDocuments.value.findIndex(x => x.document.id === document.id);
        if (index > -1) {
            this.inProgressDocuments.value[index] = {
                document: document,
                state: newState
            };
        }
        else {
            this.inProgressDocuments.value.push({
                document: document,
                state: newState
            })
        }

        this.inProgressDocuments.next([...this.inProgressDocuments.value]);
    }
}

const inProgressDocumentService = new InProgressDocumentService();

export { inProgressDocumentService };
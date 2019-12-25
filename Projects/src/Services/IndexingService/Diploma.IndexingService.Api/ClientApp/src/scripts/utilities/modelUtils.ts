import { InProgressDocument, InProcessDocumentState } from "@app/models";

export function getIdForDocument({name, size}: File) {
    return `${name}_${size}`
}

export function getModificationDateForDocument({lastModified}: File) {
    return new Date(lastModified).toISOString();
}

export function isInCompletedState({state}: InProgressDocument) {
    return state === InProcessDocumentState.Done || state === InProcessDocumentState.Error;
}
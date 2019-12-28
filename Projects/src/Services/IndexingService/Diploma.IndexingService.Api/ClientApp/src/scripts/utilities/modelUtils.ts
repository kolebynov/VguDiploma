import { InProgressDocument, InProcessDocumentState } from "@app/models";
import { GetFolder } from "@app/models/folder";

export function getIdForDocument({name, size}: File, { id }: GetFolder) {
    return `${id}_${name}_${size}`
}

export function getModificationDateForDocument({lastModified}: File) {
    return new Date(lastModified).toISOString();
}

export function isInCompletedState({state}: InProgressDocument) {
    return state === InProcessDocumentState.Done || state === InProcessDocumentState.Error;
}
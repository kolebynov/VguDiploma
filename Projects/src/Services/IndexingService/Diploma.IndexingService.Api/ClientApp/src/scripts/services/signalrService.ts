import * as signalR from "@microsoft/signalr";
import { InProgressDocument } from "@app/models";
import { inProgressDocumentService } from "./inProgressDocumentService";
import { accessTokenStorage } from "@app/utilities/accessTokenStorage";

export const signalrService = {
    start: () => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(process.env.SIGNALR_URL, { accessTokenFactory: () => accessTokenStorage.get() })
            .withAutomaticReconnect()
            .build();

        connection.on("inProgressDocumentStateChanged", (inProgressDocument: InProgressDocument) => {
            inProgressDocumentService.updateState(inProgressDocument.document, inProgressDocument.state, inProgressDocument.errorInfo);
        });

        connection.start().catch(error => console.error(error));
    }
};
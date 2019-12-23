import React, { FunctionComponent, memo, ChangeEvent } from "react";
import { Input } from "@material-ui/core";
import { InProgressDocument, InProcessDocumentState } from "@app/models";

interface UploadDocumentItemProps {
    inProgressDocument: InProgressDocument;
};

const UploadDocumentItem: FunctionComponent<UploadDocumentItemProps> = memo(({ inProgressDocument }) => {
    let textState = "";
    switch (inProgressDocument.state) {
        case InProcessDocumentState.ReadyToUpload:
            textState = "Ready to upload";
            break;
        case InProcessDocumentState.WaitingToUpload:
            textState = "Waiting to upload";
            break;
        case InProcessDocumentState.Uploading:
            textState = "Uploading...";
            break;
        case InProcessDocumentState.Uploaded:
            textState = "Uploaded";
            break;
        case InProcessDocumentState.InQueue:
            textState = "In queue";
            break;
    }

    return (
        <div style={{marginBottom: "10px"}}>
            <div>{inProgressDocument.document.fileName} - {textState}</div>
        </div>
    );
});

export { UploadDocumentItem };
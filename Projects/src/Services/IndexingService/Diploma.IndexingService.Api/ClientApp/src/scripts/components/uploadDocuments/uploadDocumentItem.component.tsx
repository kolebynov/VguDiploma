import React, { FunctionComponent, memo, ChangeEvent } from "react";
import { Input } from "@material-ui/core";
import { InProgressDocument, InProcessDocumentState } from "@app/models";

interface UploadDocumentItemProps {
    inProgressDocument: InProgressDocument;
};

const UploadDocumentItem: FunctionComponent<UploadDocumentItemProps> = memo(({ inProgressDocument }) => {
    const textState = InProcessDocumentState[inProgressDocument.state];

    return (
        <div style={{ marginBottom: "10px" }}>
            <div>{inProgressDocument.document.fileName} - {textState}: {inProgressDocument.errorInfo}</div>
        </div>
    );
});

export { UploadDocumentItem };
import React, { FunctionComponent, memo, ChangeEvent } from "react";
import { Input } from "@material-ui/core";

interface UploadDocumentItemProps {
    fileName: string;
    state: UploadItemState;
};

enum UploadItemState {
    Idle,
    Uploading,
    Uploaded,
    Error
}

const UploadDocumentItem: FunctionComponent<UploadDocumentItemProps> = memo(props => {
    return (
        <div>
            <span>{props.fileName}</span>
            <span>{props.state}</span>
        </div>
    );
});

export { UploadDocumentItem, UploadItemState };
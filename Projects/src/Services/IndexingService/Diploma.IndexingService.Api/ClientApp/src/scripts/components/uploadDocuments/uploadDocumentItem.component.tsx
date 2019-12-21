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
    let textState = "";
    switch (props.state) {
        case UploadItemState.Idle:
            textState = "Ready to upload";
            break;
        case UploadItemState.Uploading:
            textState = "Uploading...";
            break;
        case UploadItemState.Uploaded:
            textState = "Uploaded";
            break;
    }

    return (
        <div style={{marginBottom: "10px"}}>
            <div>{props.fileName} - {textState}</div>
        </div>
    );
});

export { UploadDocumentItem, UploadItemState };
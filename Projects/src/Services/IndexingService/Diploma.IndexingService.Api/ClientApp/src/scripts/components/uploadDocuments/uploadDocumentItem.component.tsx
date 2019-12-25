import React, { FunctionComponent, memo } from "react";

interface UploadDocumentItemProps {
    file: File;
};

const UploadDocumentItem: FunctionComponent<UploadDocumentItemProps> = memo(({ file }) => {
    return (
        <div style={{ marginBottom: "10px" }}>
            <div>{file.name}</div>
        </div>
    );
});

export { UploadDocumentItem };
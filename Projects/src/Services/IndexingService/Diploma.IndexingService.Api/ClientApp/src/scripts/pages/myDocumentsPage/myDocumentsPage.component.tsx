import React, { FunctionComponent, memo } from "react";
import { MyDocuments, UploadDocuments } from "@app/components";

const MyDocumentsPage: FunctionComponent = memo(() => {
    return (
        <div>
            <MyDocuments />
            <UploadDocuments />
        </div>
    );
});

export { MyDocumentsPage };
import React, { FunctionComponent, memo } from "react";
import { MyDocuments, UploadDocuments } from "@app/components";
import { constants } from "@app/utilities";
import { useParams } from "react-router";

const MyDocumentsPage: FunctionComponent = memo(() => {
    const { folderId } = useParams();

    return (
        <div>
            <MyDocuments folderId={folderId || constants.RootFolderId} />
            <UploadDocuments />
        </div>
    );
});

export { MyDocumentsPage };
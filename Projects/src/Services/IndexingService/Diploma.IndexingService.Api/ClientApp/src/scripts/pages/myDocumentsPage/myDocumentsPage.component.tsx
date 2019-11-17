import React, { FunctionComponent, memo } from "react";
import { MyDocuments } from "@app/components";

const MyDocumentsPage: FunctionComponent = memo(() => {
    return (
        <div>
            <MyDocuments />
        </div>
    );
});

export { MyDocumentsPage };
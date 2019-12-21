import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { GetDocument } from "@app/models";
import { documentService } from "@app/services";
import { DocumentList, DocumentInfo } from "..";
import { Drawer } from "@material-ui/core";

const MyDocuments: FunctionComponent = memo(() => {
    var [documents, setDocuments] = useState(new Array<GetDocument>());
    var [selectedDocument, setSelectedDocument] = useState<GetDocument>(null);

    useEffect(() => {
        documentService.getDocuments()
            .then(newDocuments => setDocuments(newDocuments));
    }, []);

    return (
        <div>
            <DocumentList documents={documents} onDocumentSelect={docId => setSelectedDocument(documents.find(doc => doc.id === docId))}/>
            <Drawer anchor="right" open={selectedDocument !== null} onClose={() => setSelectedDocument(null)}>
                {selectedDocument != null ? <DocumentInfo document={selectedDocument} /> : null}
            </Drawer>
        </div>
    );
});

export { MyDocuments };
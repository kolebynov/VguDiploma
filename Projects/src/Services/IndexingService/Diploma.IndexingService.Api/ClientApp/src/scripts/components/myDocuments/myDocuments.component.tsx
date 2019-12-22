import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { GetDocument } from "@app/models";
import { documentService } from "@app/services";
import { DocumentList, DocumentInfo } from "..";
import { Drawer, CircularProgress, Container } from "@material-ui/core";
import { Loader } from "../loader/loader.component";

const MyDocuments: FunctionComponent = memo(() => {
    var [documents, setDocuments] = useState(new Array<GetDocument>());
    var [selectedDocument, setSelectedDocument] = useState<GetDocument>(null);
    var [isLoading, setLoading] = useState(false);

    useEffect(() => {
        setLoading(true);
        documentService.getDocuments()
            .then(newDocuments => {
                setDocuments(newDocuments);
                setLoading(false);
            });
    }, []);

    return (
        <div>
            {isLoading
                ? <Loader />
                : <DocumentList documents={documents}
                    onDocumentSelect={docId => setSelectedDocument(documents.find(doc => doc.id === docId))} />}
            <Drawer anchor="right" open={selectedDocument !== null} onClose={() => setSelectedDocument(null)}>
                {selectedDocument != null ? <DocumentInfo document={selectedDocument} /> : null}
            </Drawer>
        </div>
    );
});

export { MyDocuments };
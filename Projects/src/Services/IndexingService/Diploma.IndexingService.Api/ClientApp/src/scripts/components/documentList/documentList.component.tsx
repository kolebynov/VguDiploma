import React, { FunctionComponent, memo, useState } from "react";
import { GetDocument } from "@app/models";
import { List, ListItem, ListItemText, ListItemIcon } from "@material-ui/core";
import DescriptionIcon from '@material-ui/icons/Description';

interface DocumentListProps {
    documents: Array<GetDocument>;
    onDocumentSelect?: (docId: string) => void;
} 

const DocumentList: FunctionComponent<DocumentListProps> = memo((props) => {
    var [selectedDocument, setSelectedDocument] = useState("");

    const onDocumentSelect = (docId: string) => {
        setSelectedDocument(docId);
        if (props.onDocumentSelect) {
            props.onDocumentSelect(docId);
        }
    };

    return (
        <List>
            {props.documents.map(doc => (
                <ListItem key={doc.id} divider={true} button={true} selected={doc.id === selectedDocument} onClick={() => onDocumentSelect(doc.id)}>
                    <ListItemIcon>
                        <DescriptionIcon />
                    </ListItemIcon>
                    <ListItemText primary={doc.fileName} secondary={doc.modificationDate} />
                </ListItem>
            ))}
        </List>
    );
});

export { DocumentList };
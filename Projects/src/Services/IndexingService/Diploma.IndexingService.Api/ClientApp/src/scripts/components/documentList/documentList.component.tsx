import React, { FunctionComponent, memo, useState } from "react";
import { GetDocument } from "@app/models";
import { List, ListItem, ListItemText, ListItemIcon } from "@material-ui/core";
import DescriptionIcon from '@material-ui/icons/Description';
import FolderIcon from '@material-ui/icons/Folder';
import { toUiDateTime } from "@app/utilities/dateTimeUtils";
import { GetFolderItem, GetFolder } from "@app/models/folder";

interface DocumentListProps {
    items: Array<GetFolderItem>;
    onItemSelect?: (item: GetFolderItem) => void;
    onFolderEnter: (folder: GetFolder) => void;
    canBackward: boolean;
    onBackward: () => void;
}

const Folder: FunctionComponent<{folder: GetFolder}> = memo(({ folder }) => (
    <>
        <ListItemIcon>
            <FolderIcon />
        </ListItemIcon>
        <ListItemText primary={folder.name} />
    </>
));

const Document: FunctionComponent<{document: GetDocument}> = memo(({ document }) => (
    <>
        <ListItemIcon>
            <DescriptionIcon />
        </ListItemIcon>
        <ListItemText primary={document.fileName} secondary={toUiDateTime(document.modificationDate)} />
    </>
));

const DocumentList: FunctionComponent<DocumentListProps> = memo((props) => {
    var [selectedItem, setSelectedItem] = useState({} as GetFolderItem);

    const onDocumentSelect = (item: GetFolderItem) => {
        setSelectedItem(item);
        if (props.onItemSelect) {
            props.onItemSelect(item);
        }
    };

    return (
        <List style={{ paddingTop: "0px" }}>
            {props.canBackward
                ? 
                    <ListItem divider={true} button={true} onDoubleClick={props.onBackward}>
                        <ListItemText primary="..." />
                    </ListItem>
                : null}
            {props.items.map(item => (
                <ListItem
                    key={item.document ? item.document.id : item.folder.id}
                    divider={true} button={true}
                    selected={item === selectedItem} onClick={() => onDocumentSelect(item)}
                    onDoubleClick={() => item.folder ? props.onFolderEnter(item.folder) : null}
                >
                    {item.document
                        ? <Document document={item.document}/>
                        : <Folder folder={item.folder} />}
                </ListItem>
            ))}
        </List>
    );
});

export { DocumentList };
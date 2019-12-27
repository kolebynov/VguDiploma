import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { folderService } from "@app/services";
import { DocumentList, DocumentInfo } from "..";
import { Drawer } from "@material-ui/core";
import { Loader } from "../loader/loader.component";
import { GetFolderItem, GetFolder } from "@app/models/folder";
import { FolderItemsToolbar } from "./folderItemToolbar.component";
import { useHistory } from "react-router";
import { constants } from "@app/utilities";

interface FolderItemsProps {
    folderId: string;
}

const MyDocuments: FunctionComponent<FolderItemsProps> = memo(({ folderId }) => {
    var [folderItems, setFolderItems] = useState(new Array<GetFolderItem>());
    var [selectedItem, setSelectedItem] = useState<GetFolderItem>(null);
    var [isLoading, setLoading] = useState(false);
    var [currentFolder, setCurrentFolder] = useState(null as GetFolder);

    useEffect(() => {
        setLoading(true);
        folderService.getItems(folderId)
            .then(newFolderItems => {
                setFolderItems(newFolderItems);
                setLoading(false);
            });
        if (folderId !== constants.RootFolderId) {
            folderService.getFolder(folderId)
                .then(f => setCurrentFolder(f));
        }
    }, [folderId]);

    const onFolderAdd = (newFolder: GetFolder) => {
        let insertIndex = folderItems.findIndex(x => x.folder && x.folder.name > newFolder.name);
        if (insertIndex < 0) {
            const foldersCount = folderItems.reduce((sum, item) => item.folder ? sum + 1 : sum, 0);
            insertIndex = foldersCount > 0 ? foldersCount : 0; 
        }

        const newFolderItems = [...folderItems];
        newFolderItems.splice(insertIndex, 0, { folder: newFolder });
        setFolderItems(newFolderItems);
    }

    const history = useHistory();

    return (
        <div>
            <FolderItemsToolbar currentFolderId={folderId} onFolderAdd={onFolderAdd} />
            {isLoading
                ? <Loader />
                : <DocumentList
                    items={folderItems}
                    onItemSelect={item => setSelectedItem(folderItems.find(x => x === item))}
                    onFolderEnter={({ id }) => history.push(`/myDocuments/${id}`)}
                    canBackward={folderId !== constants.RootFolderId}
                    onBackward={() => history.push(`/myDocuments/${currentFolder.parentId}`)}
                />}
            <Drawer anchor="right" open={Boolean(selectedItem && selectedItem.document)} onClose={() => setSelectedItem(null)}>
                {selectedItem && selectedItem.document ? <DocumentInfo document={selectedItem.document} /> : null}
            </Drawer>
        </div>
    );
});

export { MyDocuments };
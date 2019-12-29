import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { folderService } from "@app/services";
import { DocumentList, DocumentInfo } from "..";
import { Drawer, TablePagination, TableCell, makeStyles, Theme, createStyles } from "@material-ui/core";
import { Loader } from "../loader/loader.component";
import { GetFolderItem, GetFolder } from "@app/models/folder";
import { FolderItemsToolbar } from "./folderItemToolbar.component";
import { useHistory } from "react-router";
import { constants } from "@app/utilities";
import { Pagination } from "../pagination/pagination.component";

interface FolderItemsProps {
    folderId: string;
}

const useStyles = makeStyles(theme => createStyles({
    docList: {
        overflowY: "auto",
        maxHeight: theme.spacing(72)
    }
}));

const MyDocuments: FunctionComponent<FolderItemsProps> = memo(({ folderId }) => {
    const [folderItems, setFolderItems] = useState({
        items: new Array<GetFolderItem>(),
        totalCount: 0
    });
    const [selectedItems, setSelectedItems] = useState<GetFolderItem[]>([]);
    const [isLoading, setLoading] = useState(false);
    const [currentFolder, setCurrentFolder] = useState(null as GetFolder);
    const [pagination, setPagination] = useState({
        limit: 10,
        skip: 0
    });

    const reloadItems = () => {
        setLoading(true);
        setSelectedItems([]);
        setFolderItems({
            items: [],
            totalCount: 0
        });

        const promises: Promise<any>[] = [];

        promises.push(folderService.getItems(folderId, pagination.limit, pagination.skip).then(setFolderItems));

        Promise.all(promises).then(() => setLoading(false));
    }

    useEffect(reloadItems, [folderId, pagination.skip, pagination.limit]);

    useEffect(() => {
        if (folderId !== constants.RootFolderId) {
            folderService
                .getFolder(folderId)
                .then(f => setCurrentFolder(f));
        }
        else {
            setCurrentFolder({
                id: constants.RootFolderId,
                name: "root"
            })
        }
    }, [folderId])

    const onFolderAdd = (newFolder: GetFolder) => {
        reloadItems();
    }

    const isFullyLoaded = () => !isLoading && Boolean(currentFolder);

    const history = useHistory();
    const styles = useStyles({});

    return (
        <div>
            <FolderItemsToolbar
                currentFolder={currentFolder}
                onFolderAdd={onFolderAdd}
                disabled={!isFullyLoaded()}
                selectedItems={selectedItems}
                onSelectedItemsRemoved={reloadItems}
            />
            {isLoading
                ? <Loader />
                : <>
                    <div className={styles.docList}>
                        <DocumentList
                            items={folderItems.items}
                            onItemsSelect={setSelectedItems}
                            onFolderEnter={({ id }) => history.push(`/myDocuments/${id}`)}
                            canBackward={folderId !== constants.RootFolderId}
                            onBackward={() => history.push(`/myDocuments/${currentFolder.parentId}`)}
                            selectedItems={selectedItems}
                        />
                    </div>
                    <Pagination
                        totalCount={folderItems.totalCount}
                        onChangePagination={setPagination}
                        {...pagination}
                    />
                </>}
        </div>
    );
});

export { MyDocuments };
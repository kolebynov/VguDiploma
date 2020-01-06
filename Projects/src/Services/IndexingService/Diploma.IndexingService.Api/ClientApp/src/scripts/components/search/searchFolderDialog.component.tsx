import React, { FunctionComponent, memo, useState } from "react";
import { Dialog, makeStyles, createStyles, DialogContent, DialogActions, Button, Typography, List, ListItem } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { GetFolder, GetFolderItem } from "@app/models/folder";
import ArrowRightIcon from '@material-ui/icons/ArrowRight';
import FolderIcon from '@material-ui/icons/Folder';
import { constants } from "@app/utilities";
import { usePromise } from "@app/utilities/reactUtils";
import { CircularBackdrop } from "../circularBackdrop/circularBackdrop.component";
import { folderService } from "@app/services";

export interface SearchFolderDialogProps {
    open: boolean;
    onClose: () => void;
    onFolderSelect: (folder: GetFolder) => void;
};

const commonResources = resources.getResourceSet("common");

const useStyles = makeStyles(theme => createStyles({
    root: {
        zIndex: theme.zIndex.modal + 1
    },
    folderList: {
        paddingTop: 0,
        paddingBottom: 0,
        paddingLeft: theme.spacing(2)
    },
    folderListRoot: {
        marginLeft: -theme.spacing(2),
        minWidth: "300px",
        minHeight: "200px"
    },
    folderListItem: {
        paddingLeft: 0,
        paddingTop: 0
    },
    expandedIcon: {
        transform: "rotateZ(90deg)"
    }
}));

interface FolderNode {
    folder: GetFolder;
    children?: FolderNode[];
    isExpanded: boolean;
}

interface FolderListProps {
    folders: FolderNode[];
    selectedFolder: GetFolder;
    onSelectFolder: (folder: GetFolder) => void;
    onExpand: (folderNode: FolderNode) => void;
}

const FolderList: FunctionComponent<FolderListProps> = memo(({ folders, selectedFolder, onSelectFolder, onExpand }) => {
    const styles = useStyles({});

    return (
        <List className={styles.folderList}>
            {folders.map(x => (
                <div key={x.folder.id}>
                    <ListItem
                        className={styles.folderListItem}
                        button={true}
                        selected={(selectedFolder || {}).id === x.folder.id}
                        onClick={() => onSelectFolder(x.folder)}
                    >
                        <ArrowRightIcon
                            className={x.isExpanded ? styles.expandedIcon : null}
                            onClick={() => onExpand(x)}
                        />
                        <FolderIcon />
                        <Typography component="span">{x.folder.name}</Typography>
                    </ListItem>
                    {x.isExpanded
                        ? <FolderList
                            folders={x.children || []}
                            selectedFolder={selectedFolder}
                            onSelectFolder={onSelectFolder}
                            onExpand={onExpand}
                        />
                        : null}
                </div>
            ))}
        </List>
    );
});

export const SearchFolderDialog: FunctionComponent<SearchFolderDialogProps> = memo(({ open, onClose, onFolderSelect }) => {
    const styles = useStyles({});
    const rootFolder: GetFolder = {
        id: constants.RootFolderId,
        name: commonResources.getLocalizableValue("rootFolder_name")
    };
    const [folders, setFolders] = useState<FolderNode[]>([
        {
            folder: rootFolder,
            isExpanded: false
        }
    ]);
    const [selectedFolder, setSelectedFolder] = useState<GetFolder>(rootFolder);
    const { isPermorming, execute } = usePromise();

    const handleExpand = async (folderNode: FolderNode) => {
        if (!folderNode.isExpanded && !folderNode.children) {
            const subFolders = (await execute(() => folderService.getItems(folderNode.folder.id, 1000, 0, true))).items as GetFolderItem[];
            folderNode.children = subFolders.map(x => ({
                folder: x.folder,
                isExpanded: false,
            }));
        }

        folderNode.isExpanded = !folderNode.isExpanded;
        setFolders([...folders]);
    }

    const handleSave = () => {
        onFolderSelect(selectedFolder);
        onClose();
    };

    return (
        <>
            <Dialog open={open} className={styles.root}>
                <DialogContent>
                    <div className={styles.folderListRoot}>
                        <FolderList
                            folders={folders}
                            selectedFolder={selectedFolder}
                            onSelectFolder={setSelectedFolder}
                            onExpand={handleExpand}
                        />
                    </div>
                </DialogContent>
                <DialogActions>
                    <Button type="submit" color="primary" onClick={handleSave}>
                        {commonResources.getLocalizableValue("ok")}
                    </Button>
                    <Button onClick={onClose} color="primary">
                        {commonResources.getLocalizableValue("cancel")}
                    </Button>
                </DialogActions>
            </Dialog>
            {isPermorming ? <CircularBackdrop open={true} /> : null}
        </>
    );
});
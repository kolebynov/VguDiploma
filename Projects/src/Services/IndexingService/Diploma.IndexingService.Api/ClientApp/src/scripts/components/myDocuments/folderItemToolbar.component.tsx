import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { Paper, Button, makeStyles, Theme, createStyles, Menu, MenuItem, CircularProgress, Typography } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { GetFolder } from "@app/models/folder";
import { CreateFileDialog } from "./createFileDialog";
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { getIdForDocument, getModificationDateForDocument } from "@app/utilities";
import { documentService, UploadingDocument } from "@app/services";
import { format } from "@app/utilities/stringUtils";

const commonResources = resources.getResourceSet("common");
const uploadResources = resources.getResourceSet("uploadDocuments");

export interface FolderItemsToolbarProps {
    onFolderAdd: (addedFolder: GetFolder) => void;
    currentFolder: GetFolder;
}

const useStyles = makeStyles((theme: Theme) => createStyles({
    button: {
        marginLeft: theme.spacing(1)
    },
    uploadInfo: {
        marginLeft: theme.spacing(4),
        display: "inline-block"
    },
    uploadInfoText: {
        paddingLeft: theme.spacing(1)
    }
}));

interface UploadingItemsInfoProps {
    count: number;
}

const UploadingItemsInfo: FunctionComponent<UploadingItemsInfoProps> = memo(({ count }) => {
    const styles = useStyles({});

    return (
        <>
            {count > 0
                ? <span className={styles.uploadInfo}>
                    <CircularProgress size={20} />
                    <Typography
                        component="span"
                        variant="body2"
                        color="textSecondary"
                        className={styles.uploadInfoText}
                    >
                        {format(uploadResources.getLocalizableValue("upload_files_info_format"), count)}
                    </Typography>
                </span>
                : null}
        </>
    );
});

export const FolderItemsToolbar: FunctionComponent<FolderItemsToolbarProps> = memo(({ onFolderAdd, currentFolder }) => {
    const [showCreateFileDialog, setShowCreateFileDialog] = useState(false);
    const styles = useStyles({});
    const [addMenuAnchorEl, setAddMenuAnchorEl] = useState<HTMLElement | null>(null);
    const [uploadingDocuments, setUploadingDocuments] = useState<UploadingDocument[]>([]);

    const fileInput = document.createElement("input");
    fileInput.type = "file";
    fileInput.multiple = true;

    useEffect(() => {
        const sub = documentService.subscribe(setUploadingDocuments);
        return () => sub.unsubscribe();
    }, []);

    const handleAddFolder = () => {
        setAddMenuAnchorEl(null);
        setShowCreateFileDialog(true);
    }


    const handleAddFile = () => {
        setAddMenuAnchorEl(null);
        fileInput.click();
    }

    const onFilesSelected = ({ target }: Event) => {
        const inputTarget = (target as HTMLInputElement);
        const [...files] = inputTarget.files;

        const addFiles = files
            .map(file => ({
                document: {
                    id: getIdForDocument(file, currentFolder),
                    fileName: file.name,
                    modificationDate: getModificationDateForDocument(file)
                },
                file
            }));

        documentService.addDocuments(addFiles, currentFolder);
        fileInput.value = "";
    }

    fileInput.addEventListener("change", onFilesSelected);

    return (
        <Paper>
            <Button
                variant="contained"
                color="primary"
                endIcon={<ExpandMoreIcon />}
                className={styles.button}
                onClick={e => setAddMenuAnchorEl(e.currentTarget)}
            >
                {commonResources.getLocalizableValue("add")}
            </Button>
            <UploadingItemsInfo count={uploadingDocuments.length} />
            <Menu
                id="simple-menu"
                anchorEl={addMenuAnchorEl}
                keepMounted
                open={Boolean(addMenuAnchorEl)}
                onClose={() => setAddMenuAnchorEl(null)}
                getContentAnchorEl={null}
                anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
            >
                <MenuItem onClick={handleAddFolder}>{commonResources.getLocalizableValue("folder")}</MenuItem>
                <MenuItem onClick={handleAddFile}>{commonResources.getLocalizableValue("file")}</MenuItem>
            </Menu>
            {showCreateFileDialog
                ? <CreateFileDialog
                    currentFolderId={currentFolder.id}
                    onClose={() => setShowCreateFileDialog(false)}
                    onFolderAdd={onFolderAdd}
                />
                : null
            }
        </Paper>
    );
});
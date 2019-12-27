import React, { FunctionComponent, memo, useState } from "react";
import { Paper, Button, Dialog, DialogTitle, DialogContent, DialogContentText, TextField, DialogActions, Backdrop } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { GetFolder } from "@app/models/folder";
import { folderService } from "@app/services";

const resourceSet = resources.getResourceSet("folders");
const commonResources = resources.getResourceSet("common");

export interface FolderItemsToolbarProps {
    onFolderAdd: (addedFolder: GetFolder) => void;
    currentFolderId: string;
}

export const FolderItemsToolbar: FunctionComponent<FolderItemsToolbarProps> = memo(({ onFolderAdd, currentFolderId }) => {
    const [showDialog, setShowDialog] = useState(false);
    const [folderName, setFolderName] = useState("");
    const [creationError, setCreationError] = useState({
        isError: false,
        errorText: ""
    });
    const [isCreating, setIsCreating] = useState(false);

    const handleClose = () => {
        setShowDialog(false);
        setFolderName("");
        setCreationError({
            isError: false,
            errorText: ""
        });
        setIsCreating(false);
    };

    const onInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFolderName(e.target.value);
        setCreationError({
            isError: false,
            errorText: ""
        });
    };

    const createFolder = async () => {
        if (folderName.trim().length === 0) {
            setCreationError({
                isError: true,
                errorText: resourceSet.getLocalizableValue("folder_name_empty")
            })
            return;
        }

        setIsCreating(true);

        try {
            const newFolder = await folderService.addFolder({
                name: folderName,
                parentId: currentFolderId
            });
            onFolderAdd(newFolder);
        }
        finally {
            handleClose();
        }
    };

    return (
        <Paper>
            <Backdrop open={isCreating} style={{ zIndex: 9999 }} />
            <Button variant="contained" onClick={() => setShowDialog(true)}>{resourceSet.getLocalizableValue("add_folder")}</Button>
            <Dialog open={showDialog} aria-labelledby="form-dialog-title">
                <DialogContent>
                    <DialogContentText>
                        {resourceSet.getLocalizableValue("create_folder_dialog_text")}
                    </DialogContentText>
                    <TextField
                        autoFocus
                        margin="dense"
                        id="name"
                        label={resourceSet.getLocalizableValue("folder_name")}
                        fullWidth
                        value={folderName}
                        onChange={onInputChange}
                        error={creationError.isError}
                        helperText={creationError.errorText}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={createFolder} color="primary">
                        {commonResources.getLocalizableValue("ok")}
                    </Button>
                    <Button onClick={handleClose} color="primary">
                        {commonResources.getLocalizableValue("cancel")}
                    </Button>
                </DialogActions>
            </Dialog>
        </Paper>
    );
});
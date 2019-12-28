import React, { memo, FunctionComponent, useState, FormEvent } from "react";
import { Dialog, DialogContent, DialogContentText, TextField, DialogActions, Button, Backdrop } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { folderService } from "@app/services";
import { GetFolder } from "@app/models/folder";

const resourceSet = resources.getResourceSet("folders");
const commonResources = resources.getResourceSet("common");

interface CreateFolderDialogProps {
    currentFolderId: string;
    onClose: () => void;
    onFolderAdd: (newFolder: GetFolder) => void;
    open: boolean;
}

export const CreateFolderDialog: FunctionComponent<CreateFolderDialogProps> = memo(({ currentFolderId, onFolderAdd, onClose, open }) => {
    const [folderName, setFolderName] = useState("");
    const [creationError, setCreationError] = useState({
        isError: false,
        errorText: ""
    });
    const [isCreating, setIsCreating] = useState(false);

    const handleClose = () => {
        setFolderName("");
        setIsCreating(false);
        setCreationError({
            isError: false,
            errorText: ""
        });
        onClose();
    };

    const onInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFolderName(e.target.value);
        setCreationError({
            isError: false,
            errorText: ""
        });
    };

    const onSubmit = async (e: FormEvent) => {
        e.preventDefault();

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
        <>
            <Dialog open={open} aria-labelledby="form-dialog-title">
                <form onSubmit={onSubmit} noValidate>
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
                        <Button type="submit" color="primary">
                            {commonResources.getLocalizableValue("ok")}
                        </Button>
                        <Button onClick={handleClose} color="primary">
                            {commonResources.getLocalizableValue("cancel")}
                        </Button>
                    </DialogActions>
                </form>
            </Dialog>
            <Backdrop open={isCreating && open} style={{ zIndex: 9999 }} />
        </>
    );
});
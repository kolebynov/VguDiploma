import React, { memo, FunctionComponent, useState, FormEvent } from "react";
import { Dialog, DialogContent, DialogContentText, TextField, DialogActions, Button, Backdrop, Typography } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { folderService } from "@app/services";
import { GetFolder } from "@app/models/folder";
import { useForm } from "react-hook-form";
import { ValidateTextField } from "../validateTextField/validateTextField.component";

const resourceSet = resources.getResourceSet("folders");
const commonResources = resources.getResourceSet("common");

interface CreateFolderDialogProps {
    currentFolderId: string;
    onClose: () => void;
    onFolderAdd: (newFolder: GetFolder) => void;
    open: boolean;
}

type FormData = {
    folderName: string;
};

export const CreateFolderDialog: FunctionComponent<CreateFolderDialogProps> = memo(({ currentFolderId, onFolderAdd, onClose, open }) => {
    const [isCreating, setIsCreating] = useState(false);
    const { register, errors, handleSubmit } = useForm<FormData>();
    const [creatingError, setCreatingError] = useState("");

    const handleClose = () => {
        setIsCreating(false);
        setCreatingError("");
        onClose();
    };

    const onSubmit = async ({ folderName }: FormData) => {
        setIsCreating(true);

        try {
            const newFolder = await folderService.addFolder({
                name: folderName,
                parentId: currentFolderId
            });
            onFolderAdd(newFolder);
            handleClose();
        }
        catch (e) {
            setCreatingError(e.message);
        }
        finally {
            setIsCreating(false);
        }
    };

    return (
        <>
            <Dialog open={open} aria-labelledby="form-dialog-title">
                <form onSubmit={handleSubmit(onSubmit)}>
                    <DialogContent>
                        <DialogContentText>
                            {resourceSet.getLocalizableValue("create_folder_dialog_text")}
                        </DialogContentText>
                        <ValidateTextField
                            autoFocus
                            margin="dense"
                            name="folderName"
                            fullWidth
                            inputRef={register({ required: true, maxLength: 100 })}
                            errors={errors}
                            resourceSet={resourceSet}
                        />
                        {creatingError
                            ? <div>
                                <Typography color="error">
                                    {creatingError}
                                </Typography>
                            </div>
                            : null}
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
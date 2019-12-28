import React, { FunctionComponent, memo, useState } from "react";
import { Dialog, DialogContent, DialogContentText, DialogActions, Button, Backdrop } from "@material-ui/core";
import { GetFolderItem } from "@app/models/folder";
import { resources } from "@app/utilities/resources";
import { folderService } from "@app/services";

const resourceSet = resources.getResourceSet("folders");
const commonResources = resources.getResourceSet("common");

export interface RemoveItemDialogProps {
    onItemsRemoved: () => void;
    onClose: () => void;
    itemsToRemove: GetFolderItem[];
    open: boolean;
}

export const RemoveItemDialog: FunctionComponent<RemoveItemDialogProps> = memo(
    ({ onItemsRemoved, itemsToRemove, onClose, open }) => {
        const [isRemoving, setIsRemoving] = useState(false);

        const handleAgree = async () => {
            setIsRemoving(true);
            try {
                await folderService.removeItems(itemsToRemove);
                setTimeout(onItemsRemoved, 500);
            }
            finally {
                setIsRemoving(false);
                onClose();
            }
        };

        return (
            <>
                <Dialog open={open} aria-labelledby="form-dialog-title">
                    <DialogContent>
                        <DialogContentText>
                            {resourceSet.getLocalizableValue("remove_items_dialog")}
                        </DialogContentText>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleAgree} color="secondary">
                            {commonResources.getLocalizableValue("ok")}
                        </Button>
                        <Button onClick={onClose} color="primary">
                            {commonResources.getLocalizableValue("cancel")}
                        </Button>
                    </DialogActions>
                </Dialog>
                <Backdrop open={isRemoving && open} />
            </>
        );
    });
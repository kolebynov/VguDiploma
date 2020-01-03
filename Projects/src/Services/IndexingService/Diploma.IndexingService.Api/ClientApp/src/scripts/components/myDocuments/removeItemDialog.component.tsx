import React, { FunctionComponent, memo, useState } from "react";
import { Dialog, DialogContent, DialogContentText, DialogActions, Button, Backdrop } from "@material-ui/core";
import { GetFolderItem } from "@app/models/folder";
import { resources } from "@app/utilities/resources";
import { folderService } from "@app/services";
import { CircularBackdrop } from "../circularBackdrop/circularBackdrop.component";
import { usePromise, promiseTimeout } from "@app/utilities/reactUtils";

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
        const { isPermorming, execute } = usePromise();

        const handleAgree = async () => {
            try {
                await execute(async () => {
                    await folderService.removeItems(itemsToRemove);
                    await promiseTimeout(500);
                    onItemsRemoved();
                });
            }
            finally {
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
                <CircularBackdrop open={isPermorming && open} />
            </>
        );
    });
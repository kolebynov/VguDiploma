import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { Paper, Button, makeStyles, Theme, createStyles, Menu, MenuItem, CircularProgress, Typography, Drawer } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { GetFolder, GetFolderItem } from "@app/models/folder";
import { CreateFolderDialog } from "./createFolderDialog.component";
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { getIdForDocument, getModificationDateForDocument } from "@app/utilities";
import { documentService, UploadingDocument, UploadingState } from "@app/services";
import { format } from "@app/utilities/stringUtils";
import SettingsIcon from '@material-ui/icons/Settings';
import { ButtonProps } from "@material-ui/core/Button";
import { MenuProps } from "@material-ui/core/Menu";
import { RemoveItemDialog } from "./removeItemDialog.component";
import { DocumentInfo } from "..";

const commonResources = resources.getResourceSet("common");
const uploadResources = resources.getResourceSet("uploadDocuments");
const folderResources = resources.getResourceSet("folders");

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
    uploadingDocuments: UploadingDocument[];
}

const UploadingItemsInfo: FunctionComponent<UploadingItemsInfoProps> = memo(({ uploadingDocuments }) => {
    const styles = useStyles({});
    const count = uploadingDocuments
        .reduce((counter, d) => d.state !== UploadingState.Error ? counter + 1 : counter, 0);
    const failedCount = uploadingDocuments
        .reduce((counter, d) => d.state === UploadingState.Error ? counter + 1 : counter, 0);

    return (
        <>
            {count > 0 || failedCount > 0
                ? <span className={styles.uploadInfo}>
                    <CircularProgress size={20} />
                    <Typography
                        component="span"
                        variant="body2"
                        color="textSecondary"
                        className={styles.uploadInfoText}
                    >
                        {failedCount == 0
                            ? format(uploadResources.getLocalizableValue("upload_files_info_format"), count)
                            : format(uploadResources.getLocalizableValue("upload_files_info_with_error_format"), count, failedCount)}
                    </Typography>
                </span>
                : null}
        </>
    );
});

const ToolbarButton: FunctionComponent<ButtonProps> = memo(props => {
    const styles = useStyles({});

    return (
        <Button variant="contained" color="primary" className={styles.button} {...props}>
            {props.children}
        </Button>
    );
});

interface ToolbarMenuProps {
    anchorEl: HTMLElement;
    onClose: () => void;
}

const ToolbarMenu: FunctionComponent<ToolbarMenuProps> = memo(props => (
    <Menu
        {...props}
        keepMounted
        open={Boolean(props.anchorEl)}
        getContentAnchorEl={null}
        onClose={props.onClose}
        anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
    >
        {props.children}
    </Menu>
));

export interface FolderItemsToolbarProps {
    onFolderAdd: (addedFolder: GetFolder) => void;
    onSelectedItemsRemoved: () => void;
    currentFolder: GetFolder;
    disabled?: boolean;
    selectedItems: GetFolderItem[];
}

export const FolderItemsToolbar: FunctionComponent<FolderItemsToolbarProps> = memo(
    ({ onFolderAdd, currentFolder, disabled, onSelectedItemsRemoved, selectedItems }) => {
        const [showCreateFileDialog, setShowCreateFileDialog] = useState(false);
        const [showRemoveItemsDialog, setShowRemoveItemsDialog] = useState(false);
        const [addMenuAnchorEl, setAddMenuAnchorEl] = useState<HTMLElement | null>(null);
        const [uploadingDocuments, setUploadingDocuments] = useState<UploadingDocument[]>([]);
        const [actionMenuAnchorEl, setActionMenuAnchorEl] = useState<HTMLElement | null>(null);
        const [showDocumentInfo, setShowDocumentInfo] = useState(false);

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

        const handleRemove = () => {
            setActionMenuAnchorEl(null);
            setShowRemoveItemsDialog(true);
        };

        const handleShowInfo = () => {
            setActionMenuAnchorEl(null);
            setShowDocumentInfo(true);
        };

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
                <ToolbarButton
                    endIcon={<ExpandMoreIcon />}
                    onClick={e => setAddMenuAnchorEl(e.currentTarget)}
                    disabled={disabled}
                >
                    {commonResources.getLocalizableValue("add")}
                </ToolbarButton>
                <ToolbarButton
                    endIcon={<SettingsIcon />}
                    onClick={e => setActionMenuAnchorEl(e.currentTarget)}
                    disabled={disabled}
                >
                    {commonResources.getLocalizableValue("actions")}
                </ToolbarButton>
                <UploadingItemsInfo uploadingDocuments={uploadingDocuments} />
                <ToolbarMenu anchorEl={addMenuAnchorEl} onClose={() => setAddMenuAnchorEl(null)}>
                    <MenuItem onClick={handleAddFolder}>{commonResources.getLocalizableValue("folder")}</MenuItem>
                    <MenuItem onClick={handleAddFile}>{commonResources.getLocalizableValue("file")}</MenuItem>
                </ToolbarMenu>
                <ToolbarMenu anchorEl={actionMenuAnchorEl} onClose={() => setActionMenuAnchorEl(null)}>
                    <MenuItem
                        disabled={selectedItems.length === 0}
                        onClick={handleRemove}
                    >
                        {commonResources.getLocalizableValue("remove")}
                    </MenuItem>
                    <MenuItem
                        disabled={selectedItems.length !== 1 || !selectedItems[0] || !selectedItems[0].document}
                        onClick={handleShowInfo}
                    >
                        {folderResources.getLocalizableValue("document_info_label")}
                    </MenuItem>
                </ToolbarMenu>
                <CreateFolderDialog
                    currentFolderId={currentFolder && currentFolder.id}
                    onClose={() => setShowCreateFileDialog(false)}
                    onFolderAdd={onFolderAdd}
                    open={showCreateFileDialog}
                />
                <RemoveItemDialog
                    onClose={() => setShowRemoveItemsDialog(false)}
                    open={showRemoveItemsDialog}
                    onItemsRemoved={onSelectedItemsRemoved}
                    itemsToRemove={selectedItems}
                />
                <Drawer
                    anchor="right"
                    open={showDocumentInfo}
                    onClose={() => setShowDocumentInfo(false)}
                >
                    {selectedItems[0] && selectedItems[0].document
                        ? <DocumentInfo document={selectedItems[0].document} />
                        : null}
                </Drawer>
            </Paper>
        );
    });
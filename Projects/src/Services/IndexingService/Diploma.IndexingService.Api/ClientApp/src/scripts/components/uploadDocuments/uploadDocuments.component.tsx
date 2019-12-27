import React, { FunctionComponent, memo, useState, ChangeEvent } from "react";
import { resources } from "@app/utilities/resources";
import { Button, makeStyles, createStyles } from "@material-ui/core";
import { UploadDocumentItem } from "./uploadDocumentItem.component";
import { AddDocumentModel, documentService } from "@app/services";
import { getIdForDocument, getModificationDateForDocument, constants } from "@app/utilities";

const resourceSet = resources.getResourceSet("uploadDocuments");
const useStyles = makeStyles(theme => createStyles({
    selectFiles: {
        ...theme.typography.button
    }
}));

export const UploadDocuments: FunctionComponent = memo(() => {
    const [filesToUpload, setFilesToUpload] = useState(new Array<AddDocumentModel>());
    const classes = useStyles({});

    const uploadFiles = async () => {
        documentService.addDocuments(filesToUpload, constants.RootFolderId, res =>
            setFilesToUpload(prev => prev.filter(x => x.document.id !== res.id)));
    };

    function removeFile(id: string) {
        setFilesToUpload(filesToUpload.filter(x => x.document.id !== id));
    }

    const onFilesSelected = ({ target }: ChangeEvent) => {
        const inputTarget = (target as HTMLInputElement);
        const [...files] = inputTarget.files;

        const addFiles = files
            .filter(file => !filesToUpload.some(x => x.file.name === file.name))
            .map(file => ({
                document: {
                    id: getIdForDocument(file),
                    fileName: file.name,
                    modificationDate: getModificationDateForDocument(file)
                },
                file
            }));
        setFilesToUpload(filesToUpload.concat(addFiles));

        inputTarget.value = null;
    }

    return (
        <div>
            {filesToUpload.map(({ file }) => (
                <UploadDocumentItem key={file.name} file={file} />
            ))}
            <input type="file" className={classes.selectFiles} multiple onChange={onFilesSelected} />
            <Button variant="contained" onClick={uploadFiles}>{resourceSet.getLocalizableValue("upload_button_title")}</Button>
        </div>
    );
});
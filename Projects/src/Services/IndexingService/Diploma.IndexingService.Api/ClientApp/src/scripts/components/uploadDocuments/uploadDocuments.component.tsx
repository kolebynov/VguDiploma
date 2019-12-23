import React, { FunctionComponent, memo, useState, ChangeEvent, useEffect } from "react";
import { resources } from "@app/utilities/resources";
import { Button, Fab, Input, makeStyles, createStyles } from "@material-ui/core";
import { UploadDocumentItem } from "./uploadDocumentItem.component";
import { documentService } from "@app/services";
import { InProgressDocument, InProcessDocumentState } from "@app/models";
import { inProgressDocumentService } from "@app/services/inProgressDocumentService";

const resourceSet = resources.getResourceSet("uploadDocuments");
const useStyles = makeStyles(theme => createStyles({
    selectFiles: {
        ...theme.typography.button
    }
}));

const UploadDocuments: FunctionComponent = memo(() => {
    const [filesToUpload, setFilesToUpload] = useState(new Array<{ id: string; file: File; }>());
    const [inProgressDocuments, setInProgressDocuments] = useState(new Array<InProgressDocument>());
    const classes = useStyles({});

    useEffect(() => {
        const sub = inProgressDocumentService.inProgressDocuments.subscribe(next => {
            setInProgressDocuments(next);
        });

        return () => sub.unsubscribe();
    }, []);

    const uploadFiles = () => {
        filesToUpload.forEach(({ id }) => 
            inProgressDocumentService.updateState(inProgressDocuments.find(x => x.document.id === id).document, InProcessDocumentState.WaitingToUpload));

        uploadFile(0);
    };

    const uploadFile = (index: number): void => {
        const fileToUpload = filesToUpload[index];
        if (!fileToUpload) {
            setFilesToUpload([]);
            return;
        }

        const { file, id } = fileToUpload;

        documentService.addDocument(inProgressDocuments.find(x => x.document.id === id).document, file)
            .then(() => {
                uploadFile(index + 1);
            });
    };

    const onFilesSelected = ({ target }: ChangeEvent) => {
        const inputTarget = (target as HTMLInputElement);
        const [...files] = inputTarget.files;
        const newFilesToUpload = files
            .map(file => ({
                id: `${file.name}_${file.size}`,
                file
            }))
            .filter(file => !inProgressDocuments.some(x => x.document.id === file.id));
        setFilesToUpload(filesToUpload.concat(newFilesToUpload));
        newFilesToUpload.forEach(file => {
            inProgressDocumentService.updateState({
                id: file.id,
                fileName: file.file.name,
                modificationDate: new Date(file.file.lastModified).toISOString()
            }, InProcessDocumentState.ReadyToUpload);
        });

        inputTarget.value = null;
    }

    return (
        <div>
            {inProgressDocuments.map((inProgressDocument, index) => (
                <UploadDocumentItem key={index} inProgressDocument={inProgressDocument} />
            ))}
            <input type="file" className={classes.selectFiles} multiple onChange={onFilesSelected} />
            <Button variant="contained" onClick={uploadFiles}>{resourceSet.getLocalizableValue("upload_button_title")}</Button>
        </div>
    );
});

export { UploadDocuments };
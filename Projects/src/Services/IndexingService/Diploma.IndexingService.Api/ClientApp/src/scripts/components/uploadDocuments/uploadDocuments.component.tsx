import React, { FunctionComponent, memo, useState, ChangeEvent } from "react";
import { resources } from "@app/utilities/resources";
import { Button, Fab, Input, makeStyles, createStyles } from "@material-ui/core";
import { UploadDocumentItem, UploadItemState } from "./uploadDocumentItem.component";
import { documentService } from "@app/services";

const resourceSet = resources.getResourceSet("uploadDocuments");
const useStyles = makeStyles(theme => createStyles({
    selectFiles: {
        ...theme.typography.button
    }
}));

interface SelectedFile {
    file: File;
    state: UploadItemState;
    cancelUploadFunc?: () => void;
}

const UploadDocuments: FunctionComponent = memo(() => {
    const [state, setState] = useState({
        selectedFiles: new Array<SelectedFile>()
    });
    const classes = useStyles({});

    const uploadFile = (index: number): void => {
        const file = state.selectedFiles[index];
        if (!file) {
            setState({
                selectedFiles: state.selectedFiles.filter(x => x.state != UploadItemState.Uploaded)
            })
            return;
        }

        file.state = UploadItemState.Uploading;
        setState({
            selectedFiles: state.selectedFiles
        });

        documentService.addDocument(file.file)
            .then(() => {
                file.state = UploadItemState.Uploaded;
                setState({
                    selectedFiles: state.selectedFiles
                });
                uploadFile(index + 1);
            });
    };

    const onFilesSelected = ({ target }: ChangeEvent) => {
        const inputTarget = (target as HTMLInputElement);
        const [...files] = inputTarget.files;
        setState({
            selectedFiles: state.selectedFiles.concat(files.map(x => ({
                file: x,
                state: UploadItemState.Idle
            })))
        });

        inputTarget.value = null;
    }

    return (
        <div>
            {state.selectedFiles.map((file, index) => (
                <UploadDocumentItem key={index} fileName={file.file.name} state={file.state} />
            ))}
            <input type="file" className={classes.selectFiles} multiple onChange={onFilesSelected} />
            <Button variant="contained" onClick={() => uploadFile(0)}>{resourceSet.getLocalizableValue("upload_button_title")}</Button>
        </div>
    );
});

export { UploadDocuments };
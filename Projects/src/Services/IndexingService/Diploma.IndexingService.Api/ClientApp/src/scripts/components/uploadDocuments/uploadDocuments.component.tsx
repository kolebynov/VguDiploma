import React, { FunctionComponent, memo, useState } from "react";
import { resources } from "@app/utilities/resources";
import { Button, Fab } from "@material-ui/core";
import { AddCircle } from "@material-ui/icons";
import { UploadDocumentItem } from "./uploadDocumentItem.component";
import axios from "axios";
import { ApiResult, InProcessDocumentState, AddDocument } from "@app/models";

const resourceSet = resources.getResourceSet("uploadDocuments");

interface UploadDocumentsResult {
    id: string;
    state: InProcessDocumentState
}

const UploadDocuments: FunctionComponent = memo(() => {
    const [state, setState] = useState({
        files: new Array<File>()
    });

    const setFile = (index: number, file: File) => {
        state.files[index] = file;

        setState({
            files: state.files
        });
    };

    const upload = () => {
        uploadFile(0);
    };

    const uploadFile = (index: number): void => {
        const file = state.files[index];
        if (!file) {
            return;
        }

        const formData = new FormData();
        formData.append("files", file);
        axios
            .post<ApiResult<string[]>>("/api/documents/upload", formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
            .then(({ data: { data: [contentToken] } }) => {
                const addDocument: AddDocument = {
                    id: `${file.name}_${file.size}`,
                    fileName: file.name,
                    modificationDate: new Date(file.lastModified),
                    contentToken: contentToken
                };
                return axios.post<ApiResult<UploadDocumentsResult[]>>(`/api/documents`, [addDocument]);
            })
            .then(() => uploadFile(index + 1));
    };

    const addUploadItem = () => {
        state.files.push(null);

        setState({
            files: state.files
        });
    };

    return (
        <div>
            {state.files.map((file, index) => (
                <UploadDocumentItem key={index} defaultFile={file} onFileChange={file => setFile(index, file)} />
            ))}
            <Fab color="primary" aria-label="add" onClick={addUploadItem}>
                <AddCircle />
            </Fab>
            <Button variant="contained" onClick={upload}>{resourceSet.getLocalizableValue("upload_button_title")}</Button>
        </div>
    );
});

export { UploadDocuments };
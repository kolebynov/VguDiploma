import React, { FunctionComponent, memo, useState } from "react";
import { resources } from "@app/utilities/resources";
import { Button } from "@material-ui/core";
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
        file: null as File
    });

    const onFileChange = (file: File) => {
        setState({
            file: file
        });
    };

    const upload = () => {
        if (!state.file) {
            return;
        }

        const formData = new FormData();
        formData.append("files", state.file);
        axios
            .post<ApiResult<string[]>>("/api/documents/upload", formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
            .then(({ data: { data: [contentToken] } }) => {
                const addDocument: AddDocument = {
                    id: `${state.file.name}_${state.file.size}`,
                    fileName: state.file.name,
                    modificationDate: new Date(state.file.lastModified),
                    contentToken: contentToken
                };
                return axios.post<ApiResult<UploadDocumentsResult[]>>(`/api/documents`, [addDocument]);
            });
    };

    return (
        <div>
            <UploadDocumentItem onFileChange={onFileChange} defaultFile={state.file}/>
            <Button variant="contained" onClick={upload}>{resourceSet.getLocalizableValue("upload_button_title")}</Button>
        </div>
    );
});

export { UploadDocuments };
import React, { FunctionComponent, memo, ChangeEvent } from "react";
import { Input } from "@material-ui/core";

interface UploadDocumentItemProps {
    defaultFile?: File;
    onFileChange: (file: File) => void;
};

const UploadDocumentItem: FunctionComponent<UploadDocumentItemProps> = memo(props => {
    const onFileSelected = ({ target }: ChangeEvent) => {
        const { files: [file] } = target as HTMLInputElement;

        props.onFileChange(file);
    };

    return (
        <div>
            <Input type="file" onChange={onFileSelected} defaultValue={props.defaultFile}></Input>
        </div>
    );
});

export { UploadDocumentItem };
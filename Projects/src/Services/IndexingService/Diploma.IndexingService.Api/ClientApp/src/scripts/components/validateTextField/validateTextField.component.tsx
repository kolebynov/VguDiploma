import React, { FunctionComponent, memo } from "react";
import TextField, { TextFieldProps } from "@material-ui/core/TextField";
import { NestDataObject } from "react-hook-form";
import { ResourceSet } from "@app/utilities/resources";
import { getErrorMessage } from "@app/utilities/formUtils";

export interface ValidateTextFieldProps {
    errors: NestDataObject<Record<string, any>>;
    name: string;
    resourceSet: ResourceSet;
    labelName?: string;
}

export const ValidateTextField: FunctionComponent<TextFieldProps & ValidateTextFieldProps> = memo(
    ({ errors, name, resourceSet, labelName, ...other }) => {
        const errorMessage = getErrorMessage(errors, name, resourceSet);

        return (
            <TextField
                {...other}
                name={name}
                label={resourceSet.getLocalizableValue(labelName || name)}
                error={Boolean(errorMessage)}
                helperText={errorMessage}
            />
        );
    });
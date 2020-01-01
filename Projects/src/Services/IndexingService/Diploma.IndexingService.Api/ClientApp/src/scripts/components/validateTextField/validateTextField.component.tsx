import React, { FunctionComponent, memo } from "react";
import TextField, { TextFieldProps } from "@material-ui/core/TextField";
import { NestDataObject } from "react-hook-form";
import { ResourceSet } from "@app/utilities/resources";
import { getErrorMessage } from "@app/utilities/formUtils";

export interface ValidateTextFieldProps {
    errors: NestDataObject<Record<string, any>>;
    name: string;
    resourceSet: ResourceSet;
}

export const ValidateTextField: FunctionComponent<TextFieldProps & ValidateTextFieldProps> = memo(
    ({ errors, name, resourceSet, ...other }) => {
        const errorMessage = getErrorMessage(errors, name, resourceSet);

        return (
            <TextField
                {...other}
                name={name}
                label={resourceSet.getLocalizableValue(name)}
                error={Boolean(errorMessage)}
                helperText={errorMessage}
            />
        );
    });
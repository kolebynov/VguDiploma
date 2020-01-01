import { NestDataObject, FieldError } from "react-hook-form";
import { ResourceSet } from "./resources";

export function getErrorMessage<T>(errors: NestDataObject<T>, param: keyof T, resourceSet: ResourceSet) {
    const error = errors[param] as FieldError;
    return error ? resourceSet.getLocalizableValue(`${param}_${error.type}`) : null;
}
import React, { FunctionComponent, memo } from "react";
import { GetDocument } from "@app/models";
import { Card, CardContent, Typography, Divider } from "@material-ui/core";
import { resources } from "@app/utilities/resources";

interface DocumentInfoProps {
    document: GetDocument;
}

const resourceSet = resources.getResourceSet("documentInfo");

const DocumentInfo: FunctionComponent<DocumentInfoProps> = memo(({document}) => {
    return (
        <Card>
            <CardContent>
                <Typography color="textSecondary">
                    {resourceSet.getLocalizableValue("file_name")}
                </Typography>
                <Typography color="textPrimary">
                    {document.fileName}
                </Typography>
                <Divider />
                <Typography color="textSecondary">
                    {resourceSet.getLocalizableValue("modification_date")}
                </Typography>
                <Typography color="textPrimary">
                    {document.modificationDate}
                </Typography>
            </CardContent>
        </Card>
    );
});

export { DocumentInfo };
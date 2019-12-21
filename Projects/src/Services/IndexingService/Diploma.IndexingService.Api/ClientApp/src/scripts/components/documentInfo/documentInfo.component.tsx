import React, { FunctionComponent, memo } from "react";
import { GetDocument } from "@app/models";
import { Card, CardContent, Typography, Divider, CardActions, Button } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { documentService } from "@app/services";

interface DocumentInfoProps {
    document: GetDocument;
}

const resourceSet = resources.getResourceSet("documentInfo");

const DocumentInfo: FunctionComponent<DocumentInfoProps> = memo(({ document }) => {
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
            <CardActions>
                <Button href={documentService.getContentUri(document.id)}>{resourceSet.getLocalizableValue("download_file")}</Button>
            </CardActions>
        </Card>
    );
});

export { DocumentInfo };
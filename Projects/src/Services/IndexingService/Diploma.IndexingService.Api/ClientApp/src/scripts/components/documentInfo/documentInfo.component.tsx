import React, { FunctionComponent, memo } from "react";
import { GetDocument } from "@app/models";
import { Card, CardContent, Typography, Divider, CardActions, Button, makeStyles, createStyles } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { documentService } from "@app/services";
import { toUiDateTime } from "@app/utilities/dateTimeUtils";

interface DocumentInfoProps {
    document: GetDocument;
}

const resourceSet = resources.getResourceSet("documentInfo");

const useStyles = makeStyles(theme => createStyles({
    infoRow: {
        marginBottom: theme.spacing(1)
    }
}));

interface InfoRowProps {
    labelResourceName: string;
    value: string;
}

const InfoRow: FunctionComponent<InfoRowProps> = memo(({ labelResourceName, value }) => {
    const styles = useStyles({});

    return (
        <div className={styles.infoRow}>
            <Typography color="textSecondary">
                {resourceSet.getLocalizableValue(labelResourceName)}
            </Typography>
            <Typography color="textPrimary">
                {value}
            </Typography>
            <Divider />
        </div>
    );
});

const DocumentInfo: FunctionComponent<DocumentInfoProps> = memo(({ document }) => {
    return (
        <Card>
            <CardContent>
                <InfoRow labelResourceName="file_name" value={document.fileName} />
                <InfoRow labelResourceName="modification_date" value={toUiDateTime(document.modificationDate)} />
                <InfoRow labelResourceName="size" value={`${(document.size / 1024).toFixed(2)} KB`} />
            </CardContent>
            <CardActions>
                <Button onClick={() => documentService.downloadDocument(document.id)}>
                    {resourceSet.getLocalizableValue("download_file")}
                </Button>
            </CardActions>
        </Card>
    );
});

export { DocumentInfo };
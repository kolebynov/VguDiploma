import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { UploadingDocument, documentService, UploadingState } from "@app/services";
import { Typography, LinearProgress, makeStyles, createStyles, Button } from "@material-ui/core";
import { resources } from "@app/utilities/resources";

const uploadingStateResources = resources.getResourceSet("uploadingState");
const uploadDocumentsResources = resources.getResourceSet("uploadDocuments");

const useStyles = makeStyles(theme => createStyles({
    root: {
        width: theme.spacing(50),
        padding: "12px"
    },
    actions: {
        marginBottom: theme.spacing(1)
    }
}));

const UploadDocument: FunctionComponent<{ document: UploadingDocument }> = memo(({ document }) => (
    <>
        <Typography color="textPrimary">
            {document.document.fileName}
        </Typography>
        <Typography color={document.state !== UploadingState.Error ? "textSecondary" : "error"}>
            {uploadingStateResources.getLocalizableValue(UploadingState[document.state])}
        </Typography>
        {document.state !== UploadingState.Error
            ? <LinearProgress />
            : <>
                <Typography color="error">{document.error}</Typography>
                <LinearProgress variant="determinate" value={100} color="secondary" />
            </>}
    </>
));

export const UploadDocumentList: FunctionComponent = memo(() => {
    const [documents, setDocuments] = useState<Array<UploadingDocument>>([]);
    const styles = useStyles({});

    useEffect(() => {
        var sub = documentService.subscribe(setDocuments);

        return () => sub.unsubscribe();
    }, []);

    return (
        <div className={styles.root}>
            <div className={styles.actions}>
                <Button onClick={() => documentService.removeFailedUploadings()}>
                    {uploadDocumentsResources.getLocalizableValue("remove_failed")}
                </Button>
            </div>
            {documents.map(x => (
                <UploadDocument key={x.document.id} document={x} />
            ))}
        </div>
    );
});
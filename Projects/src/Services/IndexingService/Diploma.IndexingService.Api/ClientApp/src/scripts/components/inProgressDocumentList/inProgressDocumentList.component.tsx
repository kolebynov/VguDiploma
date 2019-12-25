import React, { FunctionComponent, memo, useEffect, useState } from "react";
import { inProgressDocumentService } from "@app/services";
import { InProgressDocument, InProcessDocumentState } from "@app/models";
import { Typography, makeStyles, createStyles, Theme, Grid, Button } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { InProgressDocumentListItem } from "./inProgressDocumentListItem.component";
import { isInCompletedState } from "@app/utilities";

const resourceSet = resources.getResourceSet("inProgressDocumentList");

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        noDocTitle: {
            padding: "20px"
        },
        root: {
            flexGrow: 1,
            width: theme.spacing(50),
            padding: "12px"
        }
    }));

export const InProgressDocumentList: FunctionComponent = memo(() => {
    const [inProgressDocs, setInProgressDocs] = useState(new Array<InProgressDocument>());
    const classes = useStyles({});

    useEffect(() => {
        const sub = inProgressDocumentService.subscribe(setInProgressDocs);
        return () => sub.unsubscribe();
    }, []);

    const removeCompletedDocuments = () => {
        inProgressDocumentService.remove(inProgressDocs.filter(isInCompletedState).map(x => x.document.id));
    };

    const removeDocument = (id: string) => inProgressDocumentService.remove([id]);

    const renderList = () => (
        <Grid container spacing={3}>
            <Grid item xs={12}>
                <Button onClick={removeCompletedDocuments}>Remove All</Button>
            </Grid>
            {inProgressDocs.map(doc => (
                <Grid key={doc.document.id} container item xs={12}>
                    <InProgressDocumentListItem document={doc} canRemove={isInCompletedState(doc)} onRemove={removeDocument} />
                </Grid>
            ))}
        </Grid>
    );

    return (
        <div className={classes.root}>
            {inProgressDocs.length > 0
                ? renderList()
                : <Typography
                    align="center"
                    color="textSecondary"
                    variant="body2"
                    className={classes.noDocTitle}
                >{resourceSet.getLocalizableValue("no_documents")}</Typography>}
        </div>
    );
});
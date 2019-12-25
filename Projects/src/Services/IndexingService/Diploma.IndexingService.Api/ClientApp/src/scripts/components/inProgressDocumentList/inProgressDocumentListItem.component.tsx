import React, { FunctionComponent, memo, ComponentType } from "react";
import { Grid, LinearProgress, Typography, IconButton } from "@material-ui/core";
import { InProgressDocument, InProcessDocumentState } from "@app/models";
import { resources } from "@app/utilities/resources";
import DescriptionIcon from "@material-ui/icons/Description";
import CloseIcon from "@material-ui/icons/Close";

export interface InProgressDocumentListItemProps {
    document: InProgressDocument;
    onRemove: (id: string) => void;
    canRemove: boolean;
}

const inProgressStateResourceSet = resources.getResourceSet("inProgressDocumentState");

export const InProgressDocumentListItem: FunctionComponent<InProgressDocumentListItemProps> = memo(({ document, onRemove, canRemove }) => {
    let progress = null;
    if (document.state === InProcessDocumentState.Done) {
        progress = (<LinearProgress variant="determinate" value={100} />);
    }
    else if (document.state === InProcessDocumentState.Error) {
        progress = (<LinearProgress variant="determinate" value={100} color="secondary" />);
    }
    else {
        progress = (<LinearProgress />);
    }

    return (
        <Grid container spacing={0}>
            <Grid item xs={1} style={{ margin: "auto" }}>
                <DescriptionIcon />
            </Grid>
            <Grid item xs={10}>
                <Typography color="textPrimary">
                    {document.document.fileName}
                </Typography>
                <Typography color={document.state === InProcessDocumentState.Error ? "error" : "textSecondary"}>
                    {inProgressStateResourceSet.getLocalizableValue(InProcessDocumentState[document.state])}
                    {document.state === InProcessDocumentState.Error ? `: ${document.errorInfo}` : null}
                </Typography>
            </Grid>
            <Grid item xs={1}>
                <IconButton disabled={!canRemove} onClick={() => onRemove(document.document.id)} style={{ marginTop: "-12px" }}>
                    <CloseIcon />
                </IconButton>
            </Grid>
            <Grid item xs={12}>
                {progress}
            </Grid>
        </Grid>
    );
});
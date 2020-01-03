import React, { FunctionComponent, memo } from "react";
import { MyDocuments } from "@app/components";
import { constants } from "@app/utilities";
import { useParams } from "react-router";
import { makeStyles, createStyles, Theme } from "@material-ui/core";

const useStyles = makeStyles((theme: Theme) => createStyles({
    root: {
        paddingTop: theme.spacing(1)
    }
}));

const MyDocumentsPage: FunctionComponent = memo(() => {
    const { folderId } = useParams();
    const styles = useStyles({});

    return (
        <div className={styles.root}>
            <MyDocuments folderId={folderId || constants.RootFolderId} />
        </div>
    );
});

export { MyDocumentsPage };
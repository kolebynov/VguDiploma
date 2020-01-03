import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { Typography, Table, TableBody, TableRow, TableCell, Paper, makeStyles, createStyles } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { GetUser } from "@app/models/User";
import { userService } from "@app/services/userService";
import { CircularBackdrop } from "@app/components/circularBackdrop/circularBackdrop.component";
import { usePromise } from "@app/utilities/reactUtils";

const loginResources = resources.getResourceSet("login");

const useStyles = makeStyles(theme => createStyles({
    root: {
        marginTop: theme.spacing(1),
        padding: `${theme.spacing(2)}px ${theme.spacing(3)}px`
    }
}));

export const ProfilePage: FunctionComponent = memo(() => {
    const [user, setUser] = useState<GetUser>(null);
    const styles = useStyles({});
    const { isPermorming, execute } = usePromise();
    useEffect(() => {
        execute(() => userService.getCurrentUser())
            .then(setUser);
    }, []);

    return (
        <Paper elevation={3} className={styles.root}>
            {isPermorming
                ? <CircularBackdrop open={!Boolean(user)} />
                : null}
            <Table>
                <TableBody>
                    <TableRow>
                        <TableCell>{loginResources.getLocalizableValue("userName")}</TableCell>
                        <TableCell>{user && user.userName}</TableCell>
                    </TableRow>
                    <TableRow>
                        <TableCell>{loginResources.getLocalizableValue("email")}</TableCell>
                        <TableCell>{user && user.email}</TableCell>
                    </TableRow>
                </TableBody>
            </Table>
        </Paper>
    );
});
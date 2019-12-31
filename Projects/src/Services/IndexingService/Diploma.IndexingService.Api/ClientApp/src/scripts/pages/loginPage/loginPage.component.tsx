import React, { FunctionComponent, memo, FormEvent, useState } from "react";
import { Paper, TextField, Button, makeStyles, createStyles } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { userService } from "@app/services/userService";
import { useHistory } from "react-router";

const resourceSet = resources.getResourceSet("login");
const useStyles = makeStyles(theme => createStyles({
    formInput: {
        marginBottom: theme.spacing(2)
    },
    root: {
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        height: "100vh"
    },
    paper: {
        padding: theme.spacing(6)
    }
}));

export const LoginPage: FunctionComponent = memo(() => {
    const styles = useStyles({});
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");
    const history = useHistory();

    const onSubmit = async (e: FormEvent) => {
        e.preventDefault();

        await userService.login(userName, password);
        history.push("/");
    };

    return (
        <div className={styles.root}>
            <Paper className={styles.paper} elevation={3}>
                <form onSubmit={onSubmit}>
                    <div className={styles.formInput}>
                        <TextField
                            label={resourceSet.getLocalizableValue("userName")}
                            required
                            variant="outlined"
                            value={userName}
                            onChange={e => setUserName(e.target.value)}
                        />
                    </div>
                    <div className={styles.formInput}>
                        <TextField
                            label={resourceSet.getLocalizableValue("password")}
                            type="password"
                            autoComplete="current-password"
                            required
                            variant="outlined"
                            value={password}
                            onChange={e => setPassword(e.target.value)}
                        />
                    </div>
                    <div>
                        <Button
                            type="submit"
                            color="primary"
                            variant="contained"
                        >
                            {resourceSet.getLocalizableValue("login")}
                        </Button>
                    </div>
                </form>
            </Paper>
        </div>
    );
});
import React, { FunctionComponent, memo, FormEvent, useState } from "react";
import { Paper, makeStyles, createStyles, TextField, Button } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { useHistory } from "react-router";
import { userService } from "@app/services/userService";

const useStyles = makeStyles(theme => createStyles({
    root: {
        display: "flex",
        height: "100vh",
        alignItems: "center",
        justifyContent: "center"
    },
    paper: {
        margin: `${theme.spacing(2)}px ${theme.spacing(3)}px`,
        padding: theme.spacing(6),
        width: "60%"
    },
    formInput: {
        marginBottom: theme.spacing(2)
    },
    actions: {
        display: "flex",
        justifyContent: "center"
    },
    actionButton: {
        marginRight: theme.spacing(3)
    }
}));

const resourceSet = resources.getResourceSet("login");
const commonResources = resources.getResourceSet("common");

export const RegisterPage: FunctionComponent = memo(() => {
    const styles = useStyles({});
    const history = useHistory();
    const [userName, setUserName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        await userService.register({
            email, password, userName
        });
        history.push("/login");
    };

    const handleCancel = () => {
        history.goBack();
    };

    return (
        <div className={styles.root}>
            <Paper elevation={3} className={styles.paper}>
                <form onSubmit={handleSubmit}>
                    <div className={styles.formInput}>
                        <TextField
                            label={resourceSet.getLocalizableValue("userName")}
                            required
                            variant="outlined"
                            value={userName}
                            fullWidth
                            onChange={e => setUserName(e.target.value)}
                        />
                    </div>
                    <div className={styles.formInput}>
                        <TextField
                            label={resourceSet.getLocalizableValue("email")}
                            required
                            variant="outlined"
                            value={email}
                            fullWidth
                            onChange={e => setEmail(e.target.value)}
                        />
                    </div>
                    <div className={styles.formInput}>
                        <TextField
                            label={resourceSet.getLocalizableValue("password")}
                            type="password"
                            required
                            variant="outlined"
                            value={password}
                            fullWidth
                            onChange={e => setPassword(e.target.value)}
                        />
                    </div>
                    <div className={styles.actions}>
                        <Button
                            type="submit"
                            color="primary"
                            variant="contained"
                            className={styles.actionButton}
                        >
                            {commonResources.getLocalizableValue("ok")}
                        </Button>
                        <Button onClick={handleCancel}>
                            {commonResources.getLocalizableValue("cancel")}
                        </Button>
                    </div>
                </form>
            </Paper>
        </div>
    );
});
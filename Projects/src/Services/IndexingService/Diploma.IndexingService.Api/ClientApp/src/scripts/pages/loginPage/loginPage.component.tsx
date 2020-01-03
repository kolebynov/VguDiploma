import React, { FunctionComponent, memo, useState } from "react";
import { Paper, Button, makeStyles, createStyles, Typography } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { userService } from "@app/services/userService";
import { useHistory } from "react-router";
import { createLinkComponent, usePromise } from "@app/utilities/reactUtils";
import { useForm } from 'react-hook-form'
import { ValidateTextField } from "@app/components/validateTextField/validateTextField.component";
import { ApiRequestError } from "@app/services/apiRequestExecutor";
import { CircularBackdrop } from "@app/components/circularBackdrop/circularBackdrop.component";

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
        padding: theme.spacing(6),
        width: "20%",
        minWidth: "200px"
    },
    actions: {
        display: "flex",
        justifyContent: "space-between"
    }
}));

type FormData = {
    userNameOrEmail: string;
    password: string;
}

export const LoginPage: FunctionComponent = memo(() => {
    const styles = useStyles({});
    const history = useHistory();
    const { register, handleSubmit, errors } = useForm<FormData>();
    const [apiError, setApiError] = useState<ApiRequestError>(null);
    const { isPermorming, execute } = usePromise();

    const onSubmit = async ({ userNameOrEmail, password }: FormData) => {
        try {
            await execute(() => userService.login(userNameOrEmail, password));
            history.push("/");
        }
        catch (e) {
            setApiError(e as ApiRequestError);
        }
    };

    return (
        <div className={styles.root}>
            <CircularBackdrop open={isPermorming} />
            <Paper className={styles.paper} elevation={3}>
                <form onSubmit={handleSubmit(onSubmit)}>
                    <div className={styles.formInput}>
                        <ValidateTextField
                            name="userNameOrEmail"
                            variant="outlined"
                            fullWidth
                            inputRef={register({ required: true, maxLength: 50 })}
                            errors={errors}
                            resourceSet={resourceSet}
                        />
                    </div>
                    <div className={styles.formInput}>
                        <ValidateTextField
                            name="password"
                            type="password"
                            autoComplete="current-password"
                            variant="outlined"
                            inputRef={register({ required: true, minLength: 6, maxLength: 32 })}
                            fullWidth
                            errors={errors}
                            resourceSet={resourceSet}
                        />
                    </div>
                    {apiError
                        ? <div>
                            <Typography color="error">
                                {apiError.message}
                            </Typography>
                        </div>
                        : null}
                    <div className={styles.actions}>
                        <Button
                            type="submit"
                            color="primary"
                            variant="contained"
                        >
                            {resourceSet.getLocalizableValue("login")}
                        </Button>
                        <Button component={createLinkComponent("/register")}>
                            {resourceSet.getLocalizableValue("sign_up")}
                        </Button>
                    </div>
                </form>
            </Paper>
        </div>
    );
});
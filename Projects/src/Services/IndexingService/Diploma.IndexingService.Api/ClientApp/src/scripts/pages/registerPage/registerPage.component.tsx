import React, { FunctionComponent, memo, FormEvent, useState } from "react";
import { Paper, makeStyles, createStyles, TextField, Button } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { useHistory } from "react-router";
import { userService } from "@app/services/userService";
import { RegisterData } from "@app/models/User";
import { useForm } from "react-hook-form";
import { constants } from "@app/utilities";
import { ValidateTextField } from "@app/components/validateTextField/validateTextField.component";

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
    const { register, errors, handleSubmit } = useForm<RegisterData>();

    const onSubmit = async (registerData: RegisterData) => {
        await userService.register(registerData);
        history.push("/login");
    };

    const handleCancel = () => {
        history.goBack();
    };

    return (
        <div className={styles.root}>
            <Paper elevation={3} className={styles.paper}>
                <form onSubmit={handleSubmit(onSubmit)} noValidate>
                    <div className={styles.formInput}>
                        <ValidateTextField
                            name="userName"
                            variant="outlined"
                            fullWidth
                            inputRef={register({ required: true, maxLength: 50 })}
                            errors={errors}
                            resourceSet={resourceSet}
                        />
                    </div>
                    <div className={styles.formInput}>
                        <ValidateTextField
                            name="email"
                            type="email"
                            variant="outlined"
                            fullWidth
                            inputRef={register({ required: true, maxLength: 50, pattern: constants.EmailRegex })}
                            errors={errors}
                            resourceSet={resourceSet}
                        />
                    </div>
                    <div className={styles.formInput}>
                        <ValidateTextField
                            name="password"
                            type="password"
                            variant="outlined"
                            fullWidth
                            inputRef={register({ required: true, minLength: 6, maxLength: 32 })}
                            errors={errors}
                            resourceSet={resourceSet}
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
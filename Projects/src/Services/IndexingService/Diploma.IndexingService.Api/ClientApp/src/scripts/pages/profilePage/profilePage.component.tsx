import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { Typography, Table, TableBody, TableRow, TableCell, Paper, makeStyles, createStyles, Button, Dialog, DialogContent, DialogContentText, DialogActions } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { GetUser } from "@app/models/User";
import { userService } from "@app/services/userService";
import { CircularBackdrop } from "@app/components/circularBackdrop/circularBackdrop.component";
import { usePromise } from "@app/utilities/reactUtils";
import { ValidateTextField } from "@app/components/validateTextField/validateTextField.component";
import { useForm } from "react-hook-form";

const loginResources = resources.getResourceSet("login");
const commonResources = resources.getResourceSet("common");

const useStyles = makeStyles(theme => createStyles({
    root: {
        marginTop: theme.spacing(1),
        padding: `${theme.spacing(2)}px ${theme.spacing(3)}px`
    },
    changePassword: {
        marginTop: theme.spacing(1)
    }
}));

interface ChangePasswordFormData {
    oldPassword: string;
    password: string;
}

export const ProfilePage: FunctionComponent = memo(() => {
    const [user, setUser] = useState<GetUser>(null);
    const [showChangePassword, setShowChangePassword] = useState(false);
    const { register, errors, handleSubmit } = useForm<ChangePasswordFormData>();
    const styles = useStyles({});
    const { isPermorming, execute } = usePromise();
    const [changePasswordError, setChangePasswordError] = useState("");

    useEffect(() => {
        execute(() => userService.getCurrentUser())
            .then(setUser);
    }, []);

    const handleChangePasswordClose = () => {
        setShowChangePassword(false);
        setChangePasswordError("");
    }

    const onSubmit = async ({ oldPassword, password }: ChangePasswordFormData) => {
        try {
            await execute(() => userService.changePassword(oldPassword, password));
            handleChangePasswordClose();
        }
        catch (e) {
            setChangePasswordError(e.message);
        }
    };

    const changePasswordDialog = (
        <Dialog open={true} aria-labelledby="form-dialog-title">
            <form onSubmit={handleSubmit(onSubmit)}>
                <DialogContent>
                    <ValidateTextField
                        autoFocus
                        margin="dense"
                        name="oldPassword"
                        type="password"
                        fullWidth
                        inputRef={register({ required: true })}
                        errors={errors}
                        resourceSet={loginResources}
                    />
                    <ValidateTextField
                        autoFocus
                        margin="dense"
                        name="password"
                        labelName="newPassword"
                        type="password"
                        fullWidth
                        inputRef={register({ required: true, minLength: 6, maxLength: 32 })}
                        errors={errors}
                        resourceSet={loginResources}
                    />
                    {changePasswordError
                        ? <div>
                            <Typography color="error">
                                {changePasswordError}
                            </Typography>
                        </div>
                        : null}
                </DialogContent>
                <DialogActions>
                    <Button type="submit" color="primary">
                        {commonResources.getLocalizableValue("ok")}
                    </Button>
                    <Button onClick={handleChangePasswordClose} color="primary">
                        {commonResources.getLocalizableValue("cancel")}
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    );

    return (
        <Paper elevation={3} className={styles.root}>
            {isPermorming
                ? <CircularBackdrop open={true} />
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
            <Button onClick={() => setShowChangePassword(true)} className={styles.changePassword}>
                {loginResources.getLocalizableValue("change_password")}
            </Button>
            {showChangePassword ? changePasswordDialog : null}
        </Paper>
    );
});
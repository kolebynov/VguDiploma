import { FunctionComponent, memo } from "react";
import { makeStyles, createStyles, Backdrop, CircularProgress } from "@material-ui/core";
import React from "react";

const useStyles = makeStyles((theme) =>
    createStyles({
        backdrop: {
            zIndex: 99999999,
            color: '#fff',
        },
    }),
);

export interface CircularBackdropProps {
    open: boolean;
}

export const CircularBackdrop: FunctionComponent<CircularBackdropProps> = memo(({ open }) => {
    const classes = useStyles({});

    return (
        <Backdrop
            className={classes.backdrop}
            open={open}
        >
            <CircularProgress color="inherit" />
        </Backdrop>
    );
});
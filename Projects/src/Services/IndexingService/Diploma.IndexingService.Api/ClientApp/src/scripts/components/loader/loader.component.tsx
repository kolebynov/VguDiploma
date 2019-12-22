import React, { FunctionComponent, memo } from "react";
import { CircularProgress } from "@material-ui/core";

const Loader: FunctionComponent = memo(() => (
    <div style={{ display: "flex", justifyContent: "center", padding: "10px" }}>
        <CircularProgress />
    </div>
));

export { Loader };
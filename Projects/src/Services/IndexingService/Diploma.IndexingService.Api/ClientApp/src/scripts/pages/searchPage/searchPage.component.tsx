import React, { FunctionComponent, memo } from 'react';
import { Search } from "@app/components";
import { Paper } from '@material-ui/core';

const SearchPage: FunctionComponent = memo(() => {
    return (
        <Paper style={{ padding: "0.8rem", marginTop: "0.5rem" }}>
            <Search />
        </Paper>
    );
});

export { SearchPage };
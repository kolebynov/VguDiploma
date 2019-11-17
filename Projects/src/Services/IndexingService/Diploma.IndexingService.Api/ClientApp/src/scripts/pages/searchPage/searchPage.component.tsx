import React, { FunctionComponent, memo } from 'react';
import { Search } from "@app/components";

const SearchPage: FunctionComponent = memo(() => {
    return (
        <>
            <Search />
        </>
    );
});

export { SearchPage };
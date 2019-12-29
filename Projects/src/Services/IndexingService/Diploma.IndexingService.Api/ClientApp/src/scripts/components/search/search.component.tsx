import React, { FunctionComponent, memo, useState, useEffect } from 'react';
import { resources } from "@app/utilities/resources";
import { TextField, Button, Grid } from '@material-ui/core';
import { SearchResultList } from "@app/components";
import { FoundDocument } from '@app/models';
import { documentService } from '@app/services';
import { Loader } from '../loader/loader.component';
import { Pagination } from '../pagination/pagination.component';

const resourceSet = resources.getResourceSet("search");

const Search: FunctionComponent = memo(() => {
    const [searchString, setSearchString] = useState("");
    const [searchResult, setSearchResult] = useState({
        documents: new Array<FoundDocument>(),
        totalCount: 0
    });
    const [isSearching, setSearching] = useState(false);
    const [pagination, setPagination] = useState({
        limit: 10,
        skip: 0
    });

    const onSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        doSearch();
    };

    const doSearch = () => {
        if (searchString) {
            setSearching(true);
            documentService.search(searchString, pagination.limit, pagination.skip)
                .then(searchResult => {
                    setSearchResult(searchResult);
                    setSearching(false);
                });
        }
    };

    useEffect(doSearch, [pagination.limit, pagination.skip]);

    const onSearchTextFieldChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchString(e.target.value);
    };

    return (
        <div>
            <form style={{ marginBottom: "5px" }} noValidate onSubmit={onSubmit}>
                <TextField
                    label={resourceSet.getLocalizableValue("search_title")}
                    value={searchString}
                    onChange={onSearchTextFieldChange}
                />
                <Button variant="contained" type="submit">
                    {resourceSet.getLocalizableValue("search_button_text")}
                </Button>
            </form>
            <div>
                {isSearching
                    ? <Loader />
                    : <>
                        <SearchResultList foundDocuments={searchResult.documents} />
                        <Pagination
                            totalCount={searchResult.totalCount}
                            skip={pagination.skip}
                            limit={pagination.limit}
                            onChangePagination={setPagination}
                        />
                    </>}
            </div>
        </div>
    );
});

export { Search };
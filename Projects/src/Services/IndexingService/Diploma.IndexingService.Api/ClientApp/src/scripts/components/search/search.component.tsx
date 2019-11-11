import React, { FunctionComponent, memo, useState, useEffect } from 'react';
import { resources } from "@app/utilities/resources";
import { TextField, Button } from '@material-ui/core';
import axios from "axios";
import { SearchResultList } from "@app/components";
import { FoundDocument, ApiResult } from '@app/models';

const resourceSet = resources.getResourceSet("search");

const Search: FunctionComponent = memo(() => {
    const [state, setState] = useState({
        searchString: "",
        searchResult: new Array<FoundDocument>()
    });

    const onSearchButtonClick = () => {
        axios.get<ApiResult<FoundDocument[]>>(`/api/search?searchString=${state.searchString}`)
            .then(response => {
                setState({
                    ...state,
                    searchResult: response.data.data 
                });
            });
    };

    const onSearchTextFieldChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setState({
            ...state,
            searchString: e.target.value
        });
    };

    return (
        <div>
            <div>
                <TextField label={resourceSet.getLocalizableValue("search_title")} value={state.searchString} onChange={onSearchTextFieldChange}/>
                <Button variant="contained" onClick={onSearchButtonClick}>{resourceSet.getLocalizableValue("search_button_text")}</Button>
            </div>
            <div>
                <SearchResultList foundDocuments={state.searchResult} />
            </div>
        </div>
    );
});

export { Search };
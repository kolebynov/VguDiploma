import React, { FunctionComponent, memo, useState, useEffect } from 'react';
import { resources } from "@app/utilities/resources";
import { TextField, Button } from '@material-ui/core';
import axios from "axios";

const resourceSet = resources.getResourceSet("search");

const SearchComponent: FunctionComponent = memo(() => {
    const [state, setState] = useState({
        searchString: "",
        searchResult: ""
    });

    const onSearchButtonClick = () => {
        axios(`http://localhost:5000/api/search?searchString=${state.searchString}`)
            .then(response => {
                setState({
                    ...state,
                    searchResult: JSON.stringify(response.data, null, "    ")
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
                <pre>{state.searchResult}</pre>
            </div>
        </div>
    );
});

export default SearchComponent;
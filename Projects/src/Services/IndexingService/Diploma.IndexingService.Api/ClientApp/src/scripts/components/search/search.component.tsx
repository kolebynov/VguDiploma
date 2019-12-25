import React, { FunctionComponent, memo, useState } from 'react';
import { resources } from "@app/utilities/resources";
import { TextField, Button, Grid } from '@material-ui/core';
import { SearchResultList } from "@app/components";
import { FoundDocument } from '@app/models';
import { documentService } from '@app/services';
import { Loader } from '../loader/loader.component';

const resourceSet = resources.getResourceSet("search");

const Search: FunctionComponent = memo(() => {
    const [state, setState] = useState({
        searchString: "",
        searchResult: new Array<FoundDocument>()
    });
    const [isSearching, setSearching] = useState(false);

    const onSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        
        if (state.searchString) {
            setSearching(true);
            documentService.search(state.searchString)
                .then(foundDocuments => {
                    setState({
                        ...state,
                        searchResult: foundDocuments
                    });
                    setSearching(false);
                });
        }
    };

    const onSearchTextFieldChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setState({
            ...state,
            searchString: e.target.value
        });
    };

    return (
        <div>
            <form style={{marginBottom: "5px"}} noValidate onSubmit={onSubmit}>
                <TextField label={resourceSet.getLocalizableValue("search_title")} value={state.searchString} onChange={onSearchTextFieldChange} />
                <Button variant="contained" type="submit">{resourceSet.getLocalizableValue("search_button_text")}</Button>
            </form>
            <div>
                {isSearching ? <Loader /> : <SearchResultList foundDocuments={state.searchResult} />}
            </div>
        </div>
    );
});

export { Search };
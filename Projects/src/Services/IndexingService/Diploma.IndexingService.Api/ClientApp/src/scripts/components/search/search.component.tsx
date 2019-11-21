import React, { FunctionComponent, memo, useState } from 'react';
import { resources } from "@app/utilities/resources";
import { TextField, Button } from '@material-ui/core';
import { SearchResultList } from "@app/components";
import { FoundDocument } from '@app/models';
import { documentService } from '@app/services';

const resourceSet = resources.getResourceSet("search");

const Search: FunctionComponent = memo(() => {
    const [state, setState] = useState({
        searchString: "",
        searchResult: new Array<FoundDocument>()
    });

    const onSearchButtonClick = () => {
        documentService.search(state.searchString)
            .then(foundDocuments => {
                setState({
                    ...state,
                    searchResult: foundDocuments
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
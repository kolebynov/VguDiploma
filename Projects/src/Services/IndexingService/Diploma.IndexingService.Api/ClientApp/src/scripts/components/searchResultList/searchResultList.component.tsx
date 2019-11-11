import React, { FunctionComponent, memo, useState, useEffect } from 'react';
import { FoundDocument } from "@app/models";
import { List, ListItem } from '@material-ui/core';
import { SearchResultListItem } from '../searchResultListItem/searchResultListItem.component';

interface SearchResultListProps {
    foundDocuments: FoundDocument[];
}

const SearchResultList: FunctionComponent<SearchResultListProps> = memo((props) => {
    return (
        <List>
            {props.foundDocuments.map(doc => (
                <SearchResultListItem key={doc.id} document={doc} />
            ))}
        </List>
    );
});

export { SearchResultList, SearchResultListProps };
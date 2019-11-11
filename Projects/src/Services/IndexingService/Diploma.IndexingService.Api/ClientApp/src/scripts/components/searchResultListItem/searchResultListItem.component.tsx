import React, { FunctionComponent, memo, useState, useEffect } from 'react';
import { FoundDocument } from "@app/models";

interface SearchResultListItemProps {
    document: FoundDocument;
}

const SearchResultListItem: FunctionComponent<SearchResultListItemProps> = memo(props => {
    const { document } = props;
    const fileName = document.matches["fileName"] || document.fileName;
    const text = (document.matches["text"] || []).join(".");

    return (
        <>
            <div>{fileName}</div>
            <div>{text}</div>
        </>
    );
});

export { SearchResultListItem, SearchResultListItemProps };
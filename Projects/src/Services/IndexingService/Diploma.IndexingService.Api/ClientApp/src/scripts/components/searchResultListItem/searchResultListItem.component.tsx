import React, { FunctionComponent, memo, useState, useEffect } from 'react';
import { FoundDocument, DocumentTextEntry, TextType } from "@app/models";
import { makeStyles, createStyles, Theme, Typography } from '@material-ui/core';
import { resources } from '@app/utilities/resources';

interface SearchResultListItemProps {
    document: FoundDocument;
}

const useStyles = makeStyles((theme: Theme) => createStyles({
    highlighted: {
        fontWeight: "bold"
    }
}));

var resourceSet = resources.getResourceSet("search");

interface HighlightedTextProps {
    entries: DocumentTextEntry[];
}

const HighlightedText: FunctionComponent<HighlightedTextProps> = memo(({entries}) => {
    const styles = useStyles({});

    return (
        <>
            {entries.map((entry, i) => (
                <span key={i} className={entry.textType === TextType.HighlightedText ? styles.highlighted : null}>
                    {entry.text}
                </span>
            ))}
        </>
    );
});

const SearchResultListItem: FunctionComponent<SearchResultListItemProps> = memo(props => {
    function createHighlightedText(matches: DocumentTextEntry[][]) {
        return matches.map((entries, i) => (
            <span key={i}>
                {i > 0 ? "..." : null}
                <HighlightedText entries={entries}/>
            </span>
        ));
    }

    const { document } = props;
    const fileName = createHighlightedText(document.matches["fileName"] || [[{
        textType: TextType.Text,
        text: document.fileName
    }]]);

    return (
        <div>
            <div>
                <Typography color="textSecondary" display="inline">
                    {resourceSet.getLocalizableValue("file_name")}
                </Typography>
                <Typography color="textPrimary" display="inline">
                    {fileName}
                </Typography>
            </div>
            <div>
                <Typography color="textSecondary" display="inline">
                    {resourceSet.getLocalizableValue("found_text")}
                </Typography>
                <Typography color="textPrimary" display="inline">
                    {document.matches["text"] ? createHighlightedText(document.matches["text"]) : null}
                </Typography>
            </div>
        </div>
    );
});

export { SearchResultListItem, SearchResultListItemProps };
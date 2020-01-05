import React, { FunctionComponent, memo, useState, useEffect } from 'react';
import { resources } from "@app/utilities/resources";
import { TextField, Button, Grid, makeStyles, createStyles, IconButton } from '@material-ui/core';
import { SearchResultList } from "@app/components";
import { FoundDocument } from '@app/models';
import { documentService } from '@app/services';
import { Loader } from '../loader/loader.component';
import { Pagination } from '../pagination/pagination.component';
import SettingsIcon from '@material-ui/icons/Settings';
import { usePromise } from '@app/utilities/reactUtils';
import { SearchSettingsDialog } from './searchSettingsDialog.component';

const resourceSet = resources.getResourceSet("search");

const useStyles = makeStyles(theme => createStyles({
    form: {
        marginBottom: "5px",
        display: "flex",
        alignItems: "flex-end"
    },
    actions: {
        marginLeft: theme.spacing(2)
    },
    actionButton: {
        marginRight: theme.spacing(1)
    }
}));

const Search: FunctionComponent = memo(() => {
    const [searchString, setSearchString] = useState("");
    const [searchResult, setSearchResult] = useState({
        documents: new Array<FoundDocument>(),
        totalCount: 0
    });
    const [pagination, setPagination] = useState({
        limit: 10,
        skip: 0
    });
    const { isPermorming, execute } = usePromise();
    const [showSettings, setShowSettings] = useState(false);
    const styles = useStyles({});

    const onSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        setPagination({ limit: 10, skip: 0 });
        doSearch();
    };

    const doSearch = () => {
        if (searchString) {
            execute(() => documentService.search(searchString, pagination.limit, pagination.skip))
                .then(setSearchResult);
        }
    };

    useEffect(doSearch, [pagination.limit, pagination.skip]);

    const onSearchTextFieldChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchString(e.target.value);
    };

    return (
        <div>
            <form className={styles.form} noValidate onSubmit={onSubmit}>
                <TextField
                    label={resourceSet.getLocalizableValue("search_title")}
                    value={searchString}
                    onChange={onSearchTextFieldChange}
                />
                <div className={styles.actions}>
                    <Button variant="contained" type="submit" className={styles.actionButton}>
                        {resourceSet.getLocalizableValue("search_button_text")}
                    </Button>
                    <Button variant="contained" onClick={() => setShowSettings(true)}>
                        <SettingsIcon />
                    </Button>
                </div>
            </form>
            <div>
                {isPermorming
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
            {showSettings ? <SearchSettingsDialog open={true} onClose={() => setShowSettings(false)} /> : null}
        </div>
    );
});

export { Search };
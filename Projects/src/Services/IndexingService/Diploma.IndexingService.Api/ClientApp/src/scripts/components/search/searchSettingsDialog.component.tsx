import React, { FunctionComponent, memo, useState } from "react";
import { Dialog, DialogActions, Button, DialogContent, Link, Typography, createStyles, makeStyles, Checkbox, FormControl, RadioGroup, FormLabel, FormControlLabel, Radio } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { settingsStorage } from "@app/utilities/settingsStorage";
import { GetFolder } from "@app/models/folder";
import { constants } from "@app/utilities";
import { SearchSettings, SearchType } from "@app/models/searchSettings";
import { SearchFolderDialog } from "./searchFolderDialog.component";

const commonResources = resources.getResourceSet("common");
const settingsResources = resources.getResourceSet("searchSettings");
const searchTypeResource = resources.getResourceSet("searchType");

const useStyles = makeStyles(theme => createStyles({
    settingRow: {
        display: "flex",
        justifyContent: "space-between",
        minWidth: "250px",
        alignItems: "center"
    },
    settingLabel: {
        marginRight: theme.spacing(2)
    },
    searchType: {
        marginTop: theme.spacing(2)
    }
}));

export interface SearchSettingsDialogProps {
    open: boolean;
    onClose: () => void;
}

interface SettingRowProps {
    label: React.ReactNode;
    value: React.ReactNode;
}

const SettingRow: FunctionComponent<SettingRowProps> = memo(({ label, value }) => {
    const styles = useStyles({});

    return (
        <div className={styles.settingRow}>
            <Typography className={styles.settingLabel}>{label}</Typography>
            <Typography>{value}</Typography>
        </div>
    );
});

export const SearchSettingsDialog: FunctionComponent<SearchSettingsDialogProps> = memo(({ open, onClose }) => {
    const [searchSettings, setSearchSettings] = useState(settingsStorage.getSearchSettings());
    const [showSearchFolderDialog, setShowSearchFolderDialog] = useState(false);

    const setSearchFolder = (folder: GetFolder) =>
        setSearchSettings({
            ...searchSettings,
            searchFolder: folder
        });

    const setSearchInSubFolders = (value: boolean) => setSearchSettings({
        ...searchSettings,
        searchInSubFolders: value
    });

    const setSearchType = (value: SearchType) => setSearchSettings({
        ...searchSettings,
        searchType: value
    });

    const handleSave = () => {
        settingsStorage.save("search_settings", searchSettings);
        onClose();
    }

    const styles = useStyles({});

    return (
        <>
            <Dialog open={open} aria-labelledby="form-dialog-title">
                <DialogContent>
                    <SettingRow
                        label={settingsResources.getLocalizableValue("search_folder")}
                        value={
                            <Link onClick={() => setShowSearchFolderDialog(true)} style={{ cursor: "pointer" }}>
                                {searchSettings.searchFolder.name}
                            </Link>}
                    />
                    <SettingRow
                        label={settingsResources.getLocalizableValue("search_in_subFolders")}
                        value={<Checkbox
                            checked={searchSettings.searchInSubFolders}
                            color="primary"
                            onChange={(_, checked) => setSearchInSubFolders(checked)}
                        />}
                    />
                    <FormControl component="fieldset" className={styles.searchType}>
                        <FormLabel component="legend">
                            {settingsResources.getLocalizableValue("search_type")}
                        </FormLabel>
                        <RadioGroup
                            aria-label="gender"
                            name="gender1"
                            value={searchSettings.searchType}
                            onChange={e => setSearchType(parseInt(e.target.value))}
                        >
                            <FormControlLabel
                                value={SearchType.Default}
                                control={<Radio color="primary" />}
                                label={searchTypeResource.getLocalizableValue(SearchType[SearchType.Default])}
                            />
                            <FormControlLabel
                                value={SearchType.Wildcard}
                                control={<Radio color="primary" />}
                                label={searchTypeResource.getLocalizableValue(SearchType[SearchType.Wildcard])}
                            />
                            <FormControlLabel
                                value={SearchType.Regexp}
                                control={<Radio color="primary" />}
                                label={searchTypeResource.getLocalizableValue(SearchType[SearchType.Regexp])}
                            />
                        </RadioGroup>
                    </FormControl>
                </DialogContent>
                <DialogActions>
                    <Button type="submit" color="primary" onClick={handleSave}>
                        {commonResources.getLocalizableValue("ok")}
                    </Button>
                    <Button onClick={onClose} color="primary">
                        {commonResources.getLocalizableValue("cancel")}
                    </Button>
                </DialogActions>
            </Dialog>
            {showSearchFolderDialog
                ? <SearchFolderDialog
                    open={true}
                    onClose={() => setShowSearchFolderDialog(false)}
                    onFolderSelect={setSearchFolder} />
                : null}
        </>
    );
});
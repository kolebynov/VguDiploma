import React, { FunctionComponent, memo, useState } from "react";
import { Dialog, DialogActions, Button, DialogContent, Link, Typography, createStyles, makeStyles, Checkbox } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { settingsStorage } from "@app/utilities/settingsStorage";
import { GetFolder } from "@app/models/folder";
import { constants } from "@app/utilities";
import { SearchSettings } from "@app/models/searchSettings";
import { SearchFolderDialog } from "./searchFolderDialog.component";

const commonResources = resources.getResourceSet("common");
const settingsResources = resources.getResourceSet("searchSettings");

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
    settingValue: {
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
            <Typography className={styles.settingValue}>{value}</Typography>
        </div>
    );
});

export const SearchSettingsDialog: FunctionComponent<SearchSettingsDialogProps> = memo(({ open, onClose }) => {
    const [searchSettings, setSearchSettings] = useState<SearchSettings>(
        settingsStorage.getOrAdd("search_settings", () => ({
            searchFolder: {
                id: constants.RootFolderId,
                name: commonResources.getLocalizableValue("rootFolder_name")
            },
            searchInSubFolders: true
        })));
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

    const handleSave = () => {
        settingsStorage.save("search_settings", searchSettings);
        onClose();
    }

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
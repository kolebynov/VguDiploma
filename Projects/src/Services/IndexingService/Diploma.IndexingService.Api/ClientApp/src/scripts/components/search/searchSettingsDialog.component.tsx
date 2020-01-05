import React, { FunctionComponent, memo, useState } from "react";
import { Dialog, DialogActions, Button, DialogContent, Link, Typography, createStyles, makeStyles } from "@material-ui/core";
import { resources } from "@app/utilities/resources";
import { settingsStorage } from "@app/utilities/settingsStorage";
import { GetFolder } from "@app/models/folder";
import { constants } from "@app/utilities";

const commonResources = resources.getResourceSet("common");
const settingsResources = resources.getResourceSet("searchSettings");

const useStyles = makeStyles(theme => createStyles({
    settingRow: {
        display: "flex",
        justifyContent: "space-between",
        minWidth: "250px"
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
    const [searchFolder, setSearchFolder] = useState<GetFolder>(
        settingsStorage.getOrAdd("search_folder", () => ({
            id: constants.RootFolderId,
            name: "root"
        })));

    return (
        <Dialog open={open} aria-labelledby="form-dialog-title">
            <DialogContent>
                <SettingRow
                    label={settingsResources.getLocalizableValue("search_folder")}
                    value={<Link>{searchFolder.name}</Link>}
                />
            </DialogContent>
            <DialogActions>
                <Button type="submit" color="primary">
                    {commonResources.getLocalizableValue("ok")}
                </Button>
                <Button onClick={onClose} color="primary">
                    {commonResources.getLocalizableValue("cancel")}
                </Button>
            </DialogActions>
        </Dialog>
    );
});
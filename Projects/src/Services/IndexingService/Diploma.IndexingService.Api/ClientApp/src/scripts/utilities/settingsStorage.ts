import { constants } from ".";
import { resources } from "./resources";
import { SearchType, SearchSettings } from "@app/models/searchSettings";

const commonResources = resources.getResourceSet("common");

class SettingsStorage {
    private key = "settings";

    public getOrAdd(name: string, valueProvider: () => any) {
        const settings = this.getSettings();
        if (settings[name]) {
            const value = settings[name];
            settings[name] = { ...valueProvider(), ...value };
            return settings[name];
        }

        settings[name] = valueProvider();
        this.saveSettings(settings);
        return settings[name];
    }

    public save(name: string, value: any) {
        const settings = this.getSettings();
        settings[name] = value;
        this.saveSettings(settings);
    }

    public getSearchSettings(): SearchSettings {
        return this.getOrAdd("search_settings", () => ({
            searchFolder: {
                id: constants.RootFolderId,
                name: commonResources.getLocalizableValue("rootFolder_name")
            },
            searchInSubFolders: true,
            searchType: SearchType.Default
        }));
    }

    private getSettings() {
        return JSON.parse(localStorage.getItem(this.key) || "{}");
    }

    private saveSettings(settings: any) {
        localStorage.setItem(this.key, JSON.stringify(settings));
    }
}

export const settingsStorage = new SettingsStorage();
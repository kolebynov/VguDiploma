class SettingsStorage {
    private key = "settings";

    public getOrAdd(name: string, valueProvider: () => any) {
        const settings = this.getSettings();
        if (settings[name]) {
            return settings[name];
        }

        settings[name] = valueProvider();
        this.saveSettings(settings);
        return settings[name];
    }

    private getSettings() {
        return JSON.parse(localStorage.getItem(this.key) || "{}");
    }

    private saveSettings(settings: any) {
        localStorage.setItem(this.key, JSON.stringify(settings));
    }
}

export const settingsStorage = new SettingsStorage();
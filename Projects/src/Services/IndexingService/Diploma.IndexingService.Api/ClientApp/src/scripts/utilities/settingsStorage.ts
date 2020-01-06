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

    public save(name: string, value: any) {
        const settings = this.getSettings();
        settings[name] = value;
        this.saveSettings(settings);
    }

    private getSettings() {
        return JSON.parse(localStorage.getItem(this.key) || "{}");
    }

    private saveSettings(settings: any) {
        localStorage.setItem(this.key, JSON.stringify(settings));
    }
}

export const settingsStorage = new SettingsStorage();
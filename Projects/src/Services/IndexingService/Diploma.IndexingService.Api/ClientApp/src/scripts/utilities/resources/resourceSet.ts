class ResourceSet {
    private rawSet: Record<string, string>;

    constructor(rawSet: Record<string, string>) {
        this.rawSet = rawSet;
    }

    public getLocalizableValue(key: string): string {
        const value = this.rawSet[key];
        if (value === undefined) {
            throw new Error(`Localizable value for key ${key} doesn't exist`);
        }

        return value;
    }
}

export default ResourceSet;
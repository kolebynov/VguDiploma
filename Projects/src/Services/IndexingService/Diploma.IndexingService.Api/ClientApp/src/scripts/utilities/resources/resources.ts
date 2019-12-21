import { ResourceSet } from "@app/utilities/resources";

class Resources {
    private resourceSets: Record<string, ResourceSet>;

    constructor(resourceSets: Record<string, ResourceSet>) {
        this.resourceSets = resourceSets;
    }

    public getResourceSet(setKey: string): ResourceSet {
        const set = this.resourceSets[setKey];
        if (set === undefined) {
            throw new Error(`Resource set with key ${setKey} not found`);
        }

        return set;
    }
}

export default new Resources({
    search: new ResourceSet({
        "search_title": "Поиск",
        "search_button_text": "Найти"
    }),
    uploadDocuments: new ResourceSet({
        "upload_button_title": "Upload",
        "add_documents_title": "Add documents"
    }),
    documentInfo: new ResourceSet({
        "file_name": "File Name",
        "modification_date": "Modification Date",
        "download_file": "Download"
    }),
    app: new ResourceSet({
        "my_documents_link": "My Documents",
        "search_link": "Search"
    })
});
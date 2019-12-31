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
        "search_title": "Search",
        "search_button_text": "Search",
        "file_name": "File Name: ",
        "found_text": "Text: "
    }),
    uploadDocuments: new ResourceSet({
        "upload_files_info_format": "Uploading {0} files"
    }),
    documentInfo: new ResourceSet({
        "file_name": "File Name",
        "modification_date": "Modification Date",
        "download_file": "Download"
    }),
    app: new ResourceSet({
        "my_documents_link": "My Documents",
        "search_link": "Search"
    }),
    locale: new ResourceSet({
        "locale": "en"
    }),
    inProgressDocumentState: new ResourceSet({
        "InQueue": "In processing queue",
        "Processing": "Processing",
        "Done": "Processing done",
        "Error": "Error"
    }),
    inProgressDocumentList: new ResourceSet({
        "no_documents": "List of in-progress documents is empty"
    }),
    folders: new ResourceSet({
        "add_folder": "Add new folder",
        "folder_name": "Folder name",
        "create_folder_dialog_text": "To create folder enter a name",
        "folder_name_empty": "Folder name is empty",
        "remove_items_dialog": "Do you really want to remove this item?",
        "document_info_label": "Show info"
    }),
    common: new ResourceSet({
        "ok": "OK",
        "cancel": "Cancel",
        "add": "Add",
        "file": "File",
        "folder": "Folder",
        "actions": "Actions",
        "remove": "Remove"
    }),
    login: new ResourceSet({
        "userName": "User name or Email",
        "password": "Password",
        "login": "Login",
        "sign_up": "Sign Up",
        "logout": "Logout"
    })
});
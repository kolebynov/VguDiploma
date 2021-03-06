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
    searchSettings: new ResourceSet({
        "search_folder": "Root search folder:",
        "search_in_subFolders": "Search in sub-folders:",
        "search_type": "Search type"
    }),
    uploadDocuments: new ResourceSet({
        "upload_files_info_format": "Uploading {0} files",
        "upload_files_info_with_error_format": "Uploading {0} files, {1} failed",
        "remove_failed": "Remove failed"
    }),
    documentInfo: new ResourceSet({
        "file_name": "File Name",
        "modification_date": "Modification Date",
        "download_file": "Download",
        "size": "Size"
    }),
    app: new ResourceSet({
        "my_documents_link": "My documents",
        "search_link": "Search",
        "in_progress": "In-progress documents",
        "profile": "My profile"
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
    uploadingState: new ResourceSet({
        "WaitingToUpload": "Waiting to upload",
        "Uploading": "Uploading",
        "Error": "Error"
    }),
    searchType: new ResourceSet({
        "Default": "Default",
        "Wildcard": "Wildcard",
        "Regexp": "Regexp"
    }),
    inProgressDocumentList: new ResourceSet({
        "no_documents": "List of in-progress documents is empty",
        "remove_all": "Remove all"
    }),
    folders: new ResourceSet({
        "add_folder": "Add new folder",
        "folderName": "Folder name",
        "folderName_required": "Folder name can't be empty",
        "folderName_maxLength": "Folder name can't be longer that 100 characters",
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
        "remove": "Remove",
        "rootFolder_name": "root"
    }),
    login: new ResourceSet({
        "userNameOrEmail": "User name or Email",
        "password": "Password",
        "login": "Login",
        "sign_up": "Sign Up",
        "logout": "Logout",
        "userName": "User name",
        "email": "Email",
        "userName_required": "User name can't be empty",
        "userName_maxLength": "User name can't be longer than 50 characters",
        "userNameOrEmail_required": "User name or email can't be empty",
        "userNameOrEmail_maxLength": "User name or email can't be longer than 50 characters",
        "password_required": "Password can't be empty",
        "password_minLength": "Password can't be shorter than 6 characters",
        "password_maxLength": "Password can't be longer than 32 characters",
        "email_required": "Email can't be empty",
        "email_maxLength": "Email can't be longer than 50 characters",
        "email_pattern": "Email is not valid",
        "oldPassword": "Old password",
        "oldPassword_required": "Old password can't be empty",
        "newPassword": "New password",
        "change_password": "Change password"
    })
});
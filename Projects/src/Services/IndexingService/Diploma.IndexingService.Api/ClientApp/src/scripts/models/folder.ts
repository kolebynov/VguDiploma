import { GetDocument } from "./document";

interface GetFolder {
    id: string;
    name: string;
    parentId?: string;
}

interface GetFolderItem {
    folder?: GetFolder;
    document?: GetDocument;
}

interface AddFolder {
    name: string;
    parentId: string;
}

export { GetFolder, GetFolderItem, AddFolder };
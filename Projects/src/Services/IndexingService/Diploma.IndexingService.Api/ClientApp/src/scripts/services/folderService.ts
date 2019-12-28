import { GetFolderItem, AddFolder, GetFolder } from "@app/models/folder";
import { ApiResult } from "@app/models";
import axios from "axios";

class FolderService {
    public async getItems(folderId: string): Promise<GetFolderItem[]> {
        const { data: { data } } = await axios
            .get<ApiResult<GetFolderItem[]>>(`/api/folders/${folderId}/items`);

        return data;
    }

    public async addFolder(folder: AddFolder): Promise<GetFolder> {
        const { data: { data } } = await axios
            .post<ApiResult<GetFolder>>("/api/folders", folder);

        return data;
    }

    public async getFolder(folderId: string): Promise<GetFolder> {
        const { data: { data } } = await axios
            .get<ApiResult<GetFolder>>(`/api/folders/${folderId}`);

        return data;
    }

    public removeItems(itemsToRemove: GetFolderItem[]): Promise<void> {
        return axios.delete("/api/folders/items", {
            data: itemsToRemove
        });
    }
}

export const folderService = new FolderService();
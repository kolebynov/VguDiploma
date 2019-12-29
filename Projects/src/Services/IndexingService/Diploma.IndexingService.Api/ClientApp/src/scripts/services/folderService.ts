import { GetFolderItem, AddFolder, GetFolder } from "@app/models/folder";
import { ApiResult, PaginationApiResult } from "@app/models";
import axios from "axios";

class FolderService {
    public async getItems(folderId: string, limit: number = 10, skip: number = 0)
        : Promise<{items: GetFolderItem[], totalCount: number}> {
        const { data: { data, pagination } } = await axios
            .get<PaginationApiResult<GetFolderItem[]>>(`/api/folders/${folderId}/items?limit=${limit}&skip=${skip}`);

        return {
            items: data,
            totalCount: pagination.totalCount
        };
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
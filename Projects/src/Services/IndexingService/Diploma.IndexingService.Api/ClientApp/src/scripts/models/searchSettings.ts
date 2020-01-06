import { GetFolder } from "./folder";

export enum SearchType {

}

export interface SearchSettings {
    searchFolder: GetFolder;
    searchInSubFolders: boolean;
    searchType: SearchType;
}
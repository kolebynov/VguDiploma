import { GetFolder } from "./folder";

export enum SearchType {
    Default,
    Wildcard,
    Regexp
}

export interface SearchSettings {
    searchFolder: GetFolder;
    searchInSubFolders: boolean;
    searchType: SearchType;
}
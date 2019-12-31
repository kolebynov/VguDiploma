const accessTokenKey = "access_token";

class AccessTokenStorage {
    public save(accessToken: string): void {
        if (!accessToken) {
            throw new Error("accessToken can't be empty");
        }

        localStorage.setItem(accessTokenKey, accessToken);
    }

    public get(): string {
        return localStorage.getItem(accessTokenKey);
    }

    public deleteToken(): void {
        localStorage.removeItem(accessTokenKey);
    }
}

export const accessTokenStorage = new AccessTokenStorage();
import { GetUser } from "@app/models/User";
import { apiRequestExecutor } from "./apiRequestExecutor";
import { ApiResult } from "@app/models";
import { accessTokenStorage } from "@app/utilities/accessTokenStorage";

interface LoginResponse {
    accessToken: string;
    user: GetUser;
}

class UserService {
    private user: GetUser = null;
    private lastFailed = false;

    public async isLogin(): Promise<boolean> {
        if (this.user) {
            return true;
        }

        if (this.lastFailed) {
            return false;
        }

        try {
            this.user = await this.getCurrentUser();
            return true;
        }
        catch (e) {
            return false;
        }
    }

    public async login(userName: string, password: string): Promise<void> {
        if (this.user) {
            return;
        }

        await this.executeUserRequest(async () => {
            const { data: { user, accessToken } } =await apiRequestExecutor.post<ApiResult<LoginResponse>>("/api/users/login", {
                userName, password
            });
            this.user = user;
            accessTokenStorage.save(accessToken)
        });
    }

    public async getCurrentUser(): Promise<GetUser> {
        if (this.user) {
            return this.user;
        }

        return await this.executeUserRequest(async () => {
            this.user = (await apiRequestExecutor.get<ApiResult<GetUser>>("/api/users/currentUser")).data;
            return this.user;
        });
    }

    public logout() {
        accessTokenStorage.deleteToken();
        location.reload();
    }

    private async executeUserRequest<T>(requestFunc: () => Promise<T>): Promise<T> {
        try {
            const response = await requestFunc();
            this.lastFailed = false;
            return response;
        }
        catch (e) {
            this.lastFailed = true;
            throw e;
        }
    }
}

export const userService = new UserService();
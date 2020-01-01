import { GetUser, RegisterData } from "@app/models/User";
import { apiRequestExecutor } from "./apiRequestExecutor";
import { ApiResult } from "@app/models";
import { accessTokenStorage } from "@app/utilities/accessTokenStorage";

interface LoginResponse {
    accessToken: string;
    user: GetUser;
}

class UserService {
    private user: GetUser = null;

    public async isLogin(): Promise<boolean> {
        if (this.user) {
            return true;
        }

        if (!accessTokenStorage.get()) {
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

        const { data: { user, accessToken } } = await apiRequestExecutor.post<ApiResult<LoginResponse>>(
            "/api/users/login", { userName, password });
        this.user = user;
        accessTokenStorage.save(accessToken);
    }

    public async getCurrentUser(): Promise<GetUser> {
        if (this.user) {
            return this.user;
        }

        this.user = (await apiRequestExecutor.get<ApiResult<GetUser>>("/api/users/currentUser")).data;
        return this.user;
    }

    public logout() {
        accessTokenStorage.deleteToken();
        location.reload();
    }

    public register(registerData: RegisterData): Promise<void> {
        return apiRequestExecutor.post("/api/users/register", registerData);
    }
}

export const userService = new UserService();
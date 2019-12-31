import axios, { AxiosRequestConfig } from "axios";
import { accessTokenStorage } from "@app/utilities/accessTokenStorage";

class ApiRequestExecutor {
    public async get<TResponse>(resource: string, config?: AxiosRequestConfig): Promise<TResponse> {
        const { data } = await axios.get<TResponse>(resource, this.addAuthInfo(config));
        return data;
    }

    public async post<TResponse>(resource: string, data?: any, config?: AxiosRequestConfig): Promise<TResponse> {
        const { data: response } = await axios.post<TResponse>(resource, data, this.addAuthInfo(config));
        return response;
    }

    public async delete<TResponse>(resource: string, config?: AxiosRequestConfig): Promise<TResponse> {
        const { data: response } = await axios.delete<TResponse>(resource, this.addAuthInfo(config));
        return response;
    }

    private addAuthInfo(config: AxiosRequestConfig): AxiosRequestConfig {
        return {
            ...config,
            headers: {
                ...(config || {}).headers,
                "Authorization": `Bearer ${accessTokenStorage.get()}`
            }
        }
    }
}

export const apiRequestExecutor = new ApiRequestExecutor();
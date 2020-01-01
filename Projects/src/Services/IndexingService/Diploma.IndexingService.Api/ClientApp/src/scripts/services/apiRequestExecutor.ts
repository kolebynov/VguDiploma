import axios, { AxiosRequestConfig, AxiosResponse, AxiosError } from "axios";
import { accessTokenStorage } from "@app/utilities/accessTokenStorage";
import { ApiResult, ApiError } from "@app/models";
import { apiErrorHandlers } from "./apiErrorHandlers";

export class ApiRequestError extends Error {
    constructor(public statusCode: number, public serverResponse: string, public apiErrors?: ApiError[]) {
        super(ApiRequestError.getErrorMessage(serverResponse, apiErrors));
    }

    private static getErrorMessage(serverResponse: string, apiErrors?: ApiError[]) {
        return apiErrors ? apiErrors.map(x => x.message).join(". ") : serverResponse;
    }
}

export interface ApiErrorHandler {
    handle: (apiError: ApiRequestError) => void;
}

class ApiRequestExecutor {
    public get<TResponse extends ApiResult<any>>(resource: string, config?: AxiosRequestConfig): Promise<TResponse> {
        return this.executeRequest(() => axios.get<TResponse>(resource, this.addAuthInfo(config)));
    }

    public post<TResponse extends ApiResult<any>>(resource: string, data?: any, config?: AxiosRequestConfig): Promise<TResponse> {
        return this.executeRequest(() => axios.post<TResponse>(resource, data, this.addAuthInfo(config)));
    }

    public delete<TResponse extends ApiResult<any>>(resource: string, config?: AxiosRequestConfig): Promise<TResponse> {
        return this.executeRequest(() => axios.delete<TResponse>(resource, this.addAuthInfo(config)));
    }

    private async executeRequest<TResponse extends ApiResult<any>>(request: () => Promise<AxiosResponse<TResponse>>): Promise<TResponse> {
        try {
            const { data } = await request();
            return data;
        }
        catch (e) {
            const axiosError = e as AxiosError<TResponse>;
            const apiRequestError = new ApiRequestError(
                axiosError.response ? axiosError.response.status : 0,
                axiosError.request.responseText || axiosError.message,
                axiosError.response && axiosError.response.data && axiosError.response.data.errors
            );

            apiErrorHandlers.forEach(x => x.handle(apiRequestError));
            throw apiRequestError;
        }
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
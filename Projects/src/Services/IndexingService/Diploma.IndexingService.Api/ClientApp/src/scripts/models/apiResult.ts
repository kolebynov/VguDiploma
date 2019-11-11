interface ApiError {
    message: string;
}

interface ApiResult<TData> {
    success: boolean;
    error?: ApiError;
    data?: TData;
}

export { ApiError, ApiResult };
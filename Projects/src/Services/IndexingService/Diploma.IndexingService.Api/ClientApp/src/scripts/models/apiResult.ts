interface ApiError {
    message: string;
}

interface ApiResult<TData> {
    success: boolean;
    error?: ApiError;
    data?: TData;
}

interface PaginationData {
    totalCount: number;
}

interface PaginationApiResult<TData> extends ApiResult<TData> {
    pagination: PaginationData;
}

export { ApiError, ApiResult, PaginationData, PaginationApiResult };
interface ApiError {
    message: string;
}

interface ApiResult<TData> {
    success: boolean;
    errors?: ApiError[];
    data?: TData;
}

interface PaginationData {
    totalCount: number;
}

interface PaginationApiResult<TData> extends ApiResult<TData> {
    pagination: PaginationData;
}

export { ApiError, ApiResult, PaginationData, PaginationApiResult };
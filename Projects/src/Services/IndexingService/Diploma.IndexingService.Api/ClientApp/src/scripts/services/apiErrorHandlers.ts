import { ApiErrorHandler } from "./apiRequestExecutor";
import { history } from '@app/utilities';

export const apiErrorHandlers: Array<ApiErrorHandler> = [
    {
        handle: error => {
            if (error.statusCode === 401) {
                history.push("/login");
            }
        }
    }
];
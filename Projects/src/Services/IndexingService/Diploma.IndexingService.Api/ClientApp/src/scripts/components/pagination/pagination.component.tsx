import React, { FunctionComponent, memo, useState } from "react";
import { TablePagination } from "@material-ui/core";

export interface PaginationProps {
    totalCount: number;
    limit: number;
    skip: number;
    onChangePagination: (newPagination: { limit: number; skip: number }) => void;
}

export const Pagination: FunctionComponent<PaginationProps> = memo(({ totalCount, limit, skip, onChangePagination }) => {
    const rowsPerPageOptions = [5, 10, 20, 50];
    let paginationOptions = {
        currentPage: Math.trunc(skip / limit),
        rowsPerPage: limit
    };

    const updatePagination = (prop: "currentPage" | "rowsPerPage", value: number) => {
        paginationOptions = {
            ...paginationOptions,
            currentPage: 0,
            [prop]: value
        };

        onChangePagination({
            limit: paginationOptions.rowsPerPage,
            skip: paginationOptions.currentPage * paginationOptions.rowsPerPage,
        });
    }

    return (
        <TablePagination
            count={totalCount}
            rowsPerPage={paginationOptions.rowsPerPage}
            page={paginationOptions.currentPage}
            onChangePage={(_, page) => updatePagination("currentPage", page)}
            onChangeRowsPerPage={(e) => updatePagination("rowsPerPage", parseInt(e.target.value))}
            rowsPerPageOptions={rowsPerPageOptions}
            component="div"
        />
    );
});
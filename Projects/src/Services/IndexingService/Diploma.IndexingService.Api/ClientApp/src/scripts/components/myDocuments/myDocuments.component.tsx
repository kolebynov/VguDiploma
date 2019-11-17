import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { Document, ApiResult } from "@app/models";
import axios from "axios";
import { Table, TableHead, TableRow, TableCell, TableBody } from "@material-ui/core";

const MyDocuments: FunctionComponent = memo(() => {
    var [state, setState] = useState({
        documents: new Array<Document>()
    });

    useEffect(() => {
        axios.get<ApiResult<Array<Document>>>("/api/documents")
            .then(response => setState({
                documents: response.data.data
            }));
    }, state.documents);

    return (
        <div>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>Id</TableCell>
                        <TableCell>File Name</TableCell>
                        <TableCell>Modification Date</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {state.documents.map(document => (
                        <TableRow key={document.id}>
                            <TableCell>{document.id}</TableCell>
                            <TableCell>{document.fileName}</TableCell>
                            <TableCell>{document.modificationDate}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </div>
    );
});

export { MyDocuments };
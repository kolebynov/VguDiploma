import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { GetDocument, ApiResult } from "@app/models";
import axios from "axios";
import { Table, TableHead, TableRow, TableCell, TableBody } from "@material-ui/core";

const MyDocuments: FunctionComponent = memo(() => {
    var [state, setState] = useState({
        documents: new Array<GetDocument>()
    });

    useEffect(() => {
        axios.get<ApiResult<Array<GetDocument>>>("/api/documents")
            .then(response => setState({
                documents: response.data.data
            }));
    }, []);

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
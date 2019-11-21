import React, { FunctionComponent, memo, useState, useEffect } from "react";
import { GetDocument } from "@app/models";
import { Table, TableHead, TableRow, TableCell, TableBody } from "@material-ui/core";
import { documentService } from "@app/services";

const MyDocuments: FunctionComponent = memo(() => {
    var [state, setState] = useState({
        documents: new Array<GetDocument>()
    });

    useEffect(() => {
        documentService.getDocuments()
            .then(documents => setState({
                documents: documents
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
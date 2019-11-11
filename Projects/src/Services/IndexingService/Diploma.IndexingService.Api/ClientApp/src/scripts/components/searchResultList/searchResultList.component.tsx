import React, { FunctionComponent, memo, useState, useEffect } from 'react';
import { FoundDocument } from "@app/models";

interface SearchResultListProps {
    foundDocuments: FoundDocument[];
}

const SearchResultList: FunctionComponent<SearchResultListProps> = memo((props) => {
    return (
        <div>
            {props.foundDocuments.map(doc => (
                <div key={doc.id}>
                    <div>
                        Id: {doc.id}
                    </div>
                    <div>
                        FileName: {doc.fileName}
                    </div>
                    <div>
                        {Object.getOwnPropertyNames(doc.matches).map(key => (
                            <div key={key}>
                                {key}: {doc.matches[key].join("\n")}
                            </div>
                        ))}
                    </div>
                </div>
            ))}
        </div>
    );
});

export { SearchResultList, SearchResultListProps };
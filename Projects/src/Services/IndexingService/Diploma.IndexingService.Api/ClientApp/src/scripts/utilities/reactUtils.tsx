import { LinkProps, Link } from "react-router-dom";
import React, { useState } from "react";

export function createLinkComponent(to: string) {
    return React.forwardRef<HTMLAnchorElement, Omit<LinkProps, 'innerRef' | 'to'>>(
        (props, ref) => <Link innerRef={ref} to={to} {...props} />,
    );
}

export function usePromise() {
    const [isPermorming, setIsPermorming] = useState(false);

    return {
        isPermorming,
        execute: async (func: () => Promise<any>) => {
            try {
                setIsPermorming(true);
                return await func();
            }
            finally {
                setIsPermorming(false);
            }
        }
    }
}

export function promiseTimeout(timeout: number): Promise<void> {
    return new Promise(res => {
        setTimeout(res, timeout);
    });
}
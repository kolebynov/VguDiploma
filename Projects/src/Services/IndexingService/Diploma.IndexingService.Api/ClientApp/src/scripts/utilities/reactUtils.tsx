import { LinkProps, Link } from "react-router-dom";
import React from "react";

export function createLinkComponent(to: string) {
    return React.forwardRef<HTMLAnchorElement, Omit<LinkProps, 'innerRef' | 'to'>>(
        (props, ref) => <Link innerRef={ref} to={to} {...props} />,
    );
}
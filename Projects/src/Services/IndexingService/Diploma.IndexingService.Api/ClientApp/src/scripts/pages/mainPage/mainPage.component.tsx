import React, { FunctionComponent, memo, useState, Suspense, useEffect } from "react";
import { resources } from "@app/utilities/resources";
import { makeStyles, createStyles, AppBar, Toolbar, Button, Drawer, Typography, IconButton, Menu, MenuItem } from "@material-ui/core";
import { Link, LinkProps, Switch, Route } from "react-router-dom";
import { InProgressDocumentList, Loader } from "@app/components";
import { MyDocumentsPage, SearchPage } from "..";
import { GetUser } from "@app/models/User";
import { userService } from "@app/services/userService";
import AccountCircle from '@material-ui/icons/AccountCircle';
import { createLinkComponent } from "@app/utilities/reactUtils";

const useStyles = makeStyles((theme) =>
    createStyles({
        root: {
            flexGrow: 1
        }
    }));

const resourceSet = resources.getResourceSet("app");
const loginResourceSet = resources.getResourceSet("login");

const UserButton: FunctionComponent<{ user: GetUser }> = memo(({ user }) => {
    const menuId = "user-action-menu";
    const [anchorEl, setAnchorEl] = useState<Element>(null);

    const handleMenuClose = () => setAnchorEl(null);

    const handleLogout = () => {
        handleMenuClose();

        userService.logout();
    };

    return (
        <>
            <Typography variant="h6">
                {user.userName}
            </Typography>
            <IconButton
                edge="end"
                aria-controls={menuId}
                aria-haspopup="true"
                onClick={e => setAnchorEl(e.currentTarget)}
                color="inherit"
                
            >
                <AccountCircle />
            </IconButton>
            <Menu
                anchorEl={anchorEl}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
                id={menuId}
                keepMounted
                open={Boolean(anchorEl)}
                onClose={handleMenuClose}
                getContentAnchorEl={null}
            >
                <MenuItem onClick={handleLogout}>{loginResourceSet.getLocalizableValue("logout")}</MenuItem>
            </Menu>
        </>
    );
});

export const MainPage: FunctionComponent = memo(() => {
    const classes = useStyles({});
    const [showInProgress, setShowInProgress] = useState(false);
    const [currentUser, setCurrentUser] = useState<GetUser>(null);

    useEffect(() => {
        userService.getCurrentUser()
            .then(setCurrentUser);
    }, []);

    const searchLink = createLinkComponent("/");
    const myDocumentsLink = createLinkComponent("/myDocuments");
    
    return (
        <>
            <div className={classes.root}>
                <AppBar position="static">
                    <Toolbar>
                        <Button color="inherit" component={searchLink}>
                            {resourceSet.getLocalizableValue("search_link")}
                        </Button>
                        <Button color="inherit" component={myDocumentsLink}>
                            {resourceSet.getLocalizableValue("my_documents_link")}
                        </Button>
                        <Button color="inherit" onClick={() => setShowInProgress(true)}>
                            Show in-progress documents
                        </Button>
                        <div style={{ flexGrow: 1 }}></div>
                        {currentUser
                            ? <UserButton user={currentUser} />
                            : null}
                    </Toolbar>
                </AppBar>
            </div>

            <Drawer open={showInProgress} anchor="right" onClose={() => setShowInProgress(false)}>
                <InProgressDocumentList />
            </Drawer>

            <Suspense fallback={<Loader />}>
                <Switch>
                    <Route path={"/myDocuments/:folderId?"} component={MyDocumentsPage} />
                    <Route path={"/"} component={SearchPage} />
                </Switch>
            </Suspense>
        </>
    );
});
import React, { FunctionComponent, memo, useState, Suspense, useEffect } from "react";
import { resources } from "@app/utilities/resources";
import { makeStyles, createStyles, AppBar, Toolbar, Button, Drawer, Typography, IconButton, Menu, MenuItem, ListItemIcon } from "@material-ui/core";
import { Switch, Route } from "react-router-dom";
import { InProgressDocumentList, Loader } from "@app/components";
import { MyDocumentsPage, SearchPage } from "..";
import { GetUser } from "@app/models/User";
import { userService } from "@app/services/userService";
import AccountCircle from '@material-ui/icons/AccountCircle';
import { createLinkComponent } from "@app/utilities/reactUtils";
import ExitToAppIcon from '@material-ui/icons/ExitToApp';
import CachedIcon from '@material-ui/icons/Cached';
import DescriptionIcon from '@material-ui/icons/Description';
import PersonIcon from '@material-ui/icons/Person';
import { ProfilePage } from "../profilePage/profilePage.component";

const useStyles = makeStyles(() =>
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
    const [showInProgress, setShowInProgress] = useState(false);

    const handleMenuClose = () => setAnchorEl(null);

    const handleWithClose = (handleFunc: () => void) => {
        return () => {
            handleMenuClose();
            handleFunc();
        };
    };

    const myDocumentsLink = createLinkComponent("/myDocuments");
    const profileLink = createLinkComponent("/profile");

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
                <MenuItem onClick={handleMenuClose} component={profileLink}>
                    <ListItemIcon>
                        <PersonIcon />
                    </ListItemIcon>
                    {resourceSet.getLocalizableValue("profile")}
                </MenuItem>
                <MenuItem component={myDocumentsLink} onClick={handleMenuClose}>
                    <ListItemIcon>
                        <DescriptionIcon />
                    </ListItemIcon>
                    {resourceSet.getLocalizableValue("my_documents_link")}
                </MenuItem>
                <MenuItem onClick={handleWithClose(() => setShowInProgress(true))}>
                    <ListItemIcon>
                        <CachedIcon />
                    </ListItemIcon>
                    {resourceSet.getLocalizableValue("in_progress")}
                </MenuItem>
                <MenuItem onClick={handleWithClose(() => userService.logout())}>
                    <ListItemIcon>
                        <ExitToAppIcon />
                    </ListItemIcon>
                    {loginResourceSet.getLocalizableValue("logout")}
                </MenuItem>
            </Menu>

            <Drawer open={showInProgress} anchor="right" onClose={() => setShowInProgress(false)}>
                <InProgressDocumentList />
            </Drawer>
        </>
    );
});

export const MainPage: FunctionComponent = memo(() => {
    const classes = useStyles({});
    const [currentUser, setCurrentUser] = useState<GetUser>(null);

    useEffect(() => {
        userService.getCurrentUser()
            .then(setCurrentUser);
    }, []);

    const searchLink = createLinkComponent("/");

    return (
        <>
            <div className={classes.root}>
                <AppBar position="static">
                    <Toolbar>
                        <Button color="inherit" component={searchLink}>
                            {resourceSet.getLocalizableValue("search_link")}
                        </Button>
                        <div style={{ flexGrow: 1 }}></div>
                        {currentUser
                            ? <UserButton user={currentUser} />
                            : null}
                    </Toolbar>
                </AppBar>
            </div>

            <Suspense fallback={<Loader />}>
                <Switch>
                    <Route path={"/myDocuments/:folderId?"} component={MyDocumentsPage} />
                    <Route path={"/profile"} component={ProfilePage} />
                    <Route path={"/"} component={SearchPage} />
                </Switch>
            </Suspense>
        </>
    );
});
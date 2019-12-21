import React, { FunctionComponent, memo, Suspense } from 'react';
import { Router, Switch, Route, Redirect } from 'react-router';
import { SearchPage, MyDocumentsPage } from '@app/pages';
import { history } from '@app/utilities';
import { Link, LinkProps } from 'react-router-dom';
import { AppBar, Toolbar, Button, makeStyles, createStyles, Theme } from '@material-ui/core';
import { resources } from '@app/utilities/resources';

const useStyles = makeStyles((theme: Theme) => 
  createStyles({
    root: {
      flexGrow: 1
    }
  }));

const resourceSet = resources.getResourceSet("app");

const App: FunctionComponent = memo(() => {
  const classes = useStyles({});

  const searchLink = React.forwardRef<HTMLAnchorElement, Omit<LinkProps, 'innerRef' | 'to'>>(
    (props, ref) => <Link innerRef={ref} to="/" {...props} />,
  );

  const myDocumentsLink = React.forwardRef<HTMLAnchorElement, Omit<LinkProps, 'innerRef' | 'to'>>(
    (props, ref) => <Link innerRef={ref} to="/myDocuments" {...props} />,
  );

  return (<Router history={history}>
    <div className={classes.root}>
      <AppBar position="static">
        <Toolbar>
          <Button color="inherit" component={searchLink}>
            {resourceSet.getLocalizableValue("search_link")}
          </Button>
          <Button color="inherit" component={myDocumentsLink}>
            {resourceSet.getLocalizableValue("my_documents_link")}
          </Button>
        </Toolbar>
      </AppBar>
    </div>

    <Suspense fallback={null}>
      <Switch>
        <Route path={"/myDocuments"} component={MyDocumentsPage} />
        <Route path={"/"} component={SearchPage} />
      </Switch>
    </Suspense>
  </Router>);
});

export { App };
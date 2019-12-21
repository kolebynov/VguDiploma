import React, { FunctionComponent, memo, Suspense } from 'react';
import { Router, Switch, Route, Redirect } from 'react-router';
import { SearchPage, MyDocumentsPage } from '@app/pages';
import { history } from '@app/utilities';
import { Link } from 'react-router-dom';
import { AppBar, Toolbar, Button, makeStyles, createStyles, Theme } from '@material-ui/core';
import { resources } from '@app/utilities/resources';

const useStyles = makeStyles((theme: Theme) => 
  createStyles({
    root: {
      flexGrow: 1
    },
    link: {
      textDecoration: "none",
      color: "inherit"
    }
  }));

const resourceSet = resources.getResourceSet("app");

const App: FunctionComponent = memo(() => {
  const classes = useStyles({});

  return (<Router history={history}>
    <div className={classes.root}>
      <AppBar position="static">
        <Toolbar>
          <Button color="inherit">
            <Link className={classes.link} to="/">{resourceSet.getLocalizableValue("search_link")}</Link>
          </Button>
          <Button color="inherit">
            <Link className={classes.link} to="/myDocuments">{resourceSet.getLocalizableValue("my_documents_link")}</Link>
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
import React, { FunctionComponent, memo, Suspense } from 'react';
import { Router, Switch, Route, Redirect } from 'react-router';
import { SearchPage, MyDocumentsPage } from '@app/pages';
import { history } from '@app/utilities';
import { Link } from 'react-router-dom';

const App: FunctionComponent = memo(() => (
    <Router history={history}>
      <Link to="/myDocuments">My Documents</Link>
      <Suspense fallback={null}>
        <Switch>
          <Route path={"/myDocuments"} component={MyDocumentsPage}/>
          <Route path={"/"} component={SearchPage}/>
        </Switch>
      </Suspense>
    </Router>
));

export { App };
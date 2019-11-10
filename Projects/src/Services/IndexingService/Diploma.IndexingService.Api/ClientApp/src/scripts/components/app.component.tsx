import React, { FunctionComponent, memo, Suspense } from 'react';
import { Router, Switch, Route, Redirect } from 'react-router';
import { SearchComponent } from '@app/components';
import { history } from '@app/utilities';
import { Link } from 'react-router-dom';

const App: FunctionComponent = memo(() => (
    <Router history={history}>
      <SearchComponent />
      <Suspense fallback={null}>
        <Switch>
        </Switch>
      </Suspense>
    </Router>
));

export { App };
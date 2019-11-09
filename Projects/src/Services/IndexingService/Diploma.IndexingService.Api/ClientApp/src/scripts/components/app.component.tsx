import React, { FunctionComponent, memo, Suspense } from 'react';
import { Router, Switch, Route, Redirect } from 'react-router';
import { Tab1, Tab2, Tab3 } from '@app/components';
import { history } from '@app/utilities';
import { Link } from 'react-router-dom';

const Home: FunctionComponent = memo(() =>
  <>
    <Link to={'/tab1'}>
      tab1
    </Link>
    <Link to={'/tab2'}>
      tab2
    </Link>
    <Link to={'/tab3'}>
      tab3
    </Link>
  </>
);

const App: FunctionComponent = memo(() => (
    <Router history={history}>
      <Home />
      <Suspense fallback={null}>
        <Switch>
          <Route path={'/tab1'} component={Tab1} />
          <Route path={'/tab2'} component={Tab2} />
          <Route path={'/tab3'} component={Tab3} />
          <Redirect from={'*'} to={'/tab1'} />
        </Switch>
      </Suspense>
    </Router>
));

export { App };
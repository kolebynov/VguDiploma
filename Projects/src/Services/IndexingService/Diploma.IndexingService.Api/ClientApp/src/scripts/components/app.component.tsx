import React, { FunctionComponent, memo, Suspense } from 'react';
import { Router, Switch, Route, Redirect } from 'react-router';
import { Tab1, Tab2, Tab3 } from '@app/components';
import { history } from '@app/utilities';

interface Account {
  id: number;
  name: string;
  roles: Array<string>;
}

interface AuthorizationProps {
  children: (account: Account, isLoading: boolean) => JSX.Element;
}

const Loader: FunctionComponent = memo(() => {
  return <div></div>;
});

const Authorization: FunctionComponent<AuthorizationProps> = memo(({ children }) => {
  return <>{children(null, true)}</>;
});

const App: FunctionComponent = memo(() => (
  <Authorization>
    {(account: Account, isLoading: boolean) => {
      if (isLoading) return (
        <Loader />
      );

      if (account) return (
        <Router history={history}>
          <Suspense fallback={null}>
            <Switch>
              <Route path={'/tab1'} component={Tab1} />
              <Route path={'/tab2'} component={Tab2} />
              <Route path={'/tab3'} component={Tab3} />
              <Redirect from={'*'} to={'/tab1'} />
            </Switch>
          </Suspense>
        </Router>
      );
    }}
  </Authorization>
));

export { App };
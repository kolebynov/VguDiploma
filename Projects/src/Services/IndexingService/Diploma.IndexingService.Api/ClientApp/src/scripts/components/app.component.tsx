import React, { FunctionComponent, memo, Suspense, useState, useEffect } from 'react';
import { Router, Switch, Route } from 'react-router';
import { history } from '@app/utilities';
import { signalrService } from '@app/services';
import { LoginPage } from '@app/pages/loginPage/loginPage.component';
import { Loader } from './loader/loader.component';
import { MainPage } from '@app/pages/mainPage/mainPage.component';
import { userService } from '@app/services/userService';
import { RegisterPage } from '@app/pages/registerPage/registerPage.component';

signalrService.start();

const loginWrapped = (WrappedComponent: FunctionComponent): FunctionComponent =>
  memo(() => {
    const [isLogin, setIsLogin] = useState(false);

    useEffect(() => {
      userService.isLogin().then(res => {
        setIsLogin(res);
        if (!res) {
          history.push("/login");
        }
      });
    }, [true]);

    return (
      <>
        {isLogin ? <WrappedComponent /> : null}
      </>
    );
  });

const App: FunctionComponent = memo(() => {
  return (
    <Router history={history}>
      <Suspense fallback={<Loader />}>
        <Switch>
          <Route path={"/login"} component={LoginPage} />
          <Route path={"/register"} component={RegisterPage} />
          <Route path={"*"} component={loginWrapped(MainPage)} />
        </Switch>
      </Suspense>
    </Router>
  );
});

export { App };
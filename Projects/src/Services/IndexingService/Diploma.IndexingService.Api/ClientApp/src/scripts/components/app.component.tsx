import React, { FunctionComponent, memo, Suspense, ErrorInfo } from 'react';
import { Router, Switch, Route } from 'react-router';
import { history } from '@app/utilities';
import { signalrService } from '@app/services';
import { LoginPage } from '@app/pages/loginPage/loginPage.component';
import { Loader } from './loader/loader.component';
import { MainPage } from '@app/pages/mainPage/mainPage.component';
import { RegisterPage } from '@app/pages/registerPage/registerPage.component';
import { Snackbar } from '@material-ui/core';

signalrService.start();

class ErrorBoundary extends React.Component<{}, { error: Error }> {
  constructor(props: {}) {
    super(props);

    this.state = {
      error: null
    }
  }

  public componentDidCatch(error: Error, info: ErrorInfo) {
    this.setState({
      error
    });
  }

  public render() {
    if (this.state.error) {
      return (
        <>
          <Snackbar
            anchorOrigin={{
              vertical: 'bottom',
              horizontal: 'left',
            }}
            open={Boolean(this.state.error)}
            autoHideDuration={6000}
            onClose={this.handleClose.bind(this)}
          >
            Error: {this.state.error.message}
          </Snackbar>
          {this.props.children}
        </>
      );
    }

    return this.props.children;
  }

  private handleClose() {
    this.setState({
      error: null
    });
  }
}

const App: FunctionComponent = memo(() => {
  return (
    <ErrorBoundary>
      <Router history={history}>
        <Suspense fallback={<Loader />}>
          <Switch>
            <Route path={"/login"} component={LoginPage} />
            <Route path={"/register"} component={RegisterPage} />
            <Route path={"*"} component={MainPage} />
          </Switch>
        </Suspense>
      </Router>
    </ErrorBoundary>
  );
});

export { App };
import React from 'react';
import { BrowserRouter, Switch, Route, Redirect } from 'react-router-dom';
import MyTemplate from './components/Template/MyTemplate';
import Login from './layouts/Login';
import LoginFbSuccess from './layouts/LoginFbSuccess';
import Config from './layouts/Config';
import Livestream from './layouts/Livestream';
import BlackList from './layouts/BlackList';
import './App.css';
import './components/Template/MyTemplate.css';
import ManagerUser from './layouts/ManagerUser';

const PrivateRoute = ({ component: Component, ...rest }) => (
  <Route {...rest} render={(props) => {
    const accessToken = localStorage.getItem('access-token');
    // const isReady = localStorage.getItem('isReady');
    // const userName = localStorage.getItem('userName');
    // const pageId = localStorage.getItem('pageId');
    if (!accessToken) {
      return <Redirect to="/login" />
    }
    else {
      return <Component {...props} />
    }
  }} />
)

const App = () => {
  return (
    <React.Fragment>
      <BrowserRouter>
        <Switch>

          <Route path="/login" exact component={Login} />
          <Route path="/loginfb/success" exact component={LoginFbSuccess} />

          <PrivateRoute path="/config" exact component={({ match }) => {
            match.params.key = "1";
            return (
              <MyTemplate path={match}>
                <Config />
              </MyTemplate>
            );
          }} />

          <PrivateRoute path="/livestream" exact component={({ match }) => {
            match.params.key = "2";
            return (
              <MyTemplate path={match}>
                <Livestream />
              </MyTemplate>
            );
          }} />

          <PrivateRoute path="/blacklist" exact component={({ match }) => {
            match.params.key = "4";
            return (
              <MyTemplate path={match}>
                <BlackList />
              </MyTemplate>
            );
          }} />

          <PrivateRoute path="/manageruser" exact component={({ match }) => {
            match.params.key = "5";
            return (
              <MyTemplate path={match}>
                <ManagerUser />
              </MyTemplate>
            );
          }} />

          <Route path="/" component={Login} />
          {/* <Route path="/comment" exact component={LiveComment} />
                <Route path="/manager-account" exact component={ManagerAccount} />
                <Route path="/" component={LiveComment} /> */}
        </Switch>
      </BrowserRouter >
    </React.Fragment>
  );
};

export default App;
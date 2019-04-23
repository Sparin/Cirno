import React, { Component } from 'react';
//import { Route } from 'react-router';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './pages/Home';
import { Blog } from './pages/Blog';
import { Post } from './pages/Post';
import { Dashboard } from './pages/Dashboard';
import Oidc from 'oidc-client';

function getApiBaseUrl() {
    if (process.env.NODE_ENV === 'development')
        return "https://localhost:5003/";
    else
        return "https://localhost/";
}

const oidcConfiguration = {
    authority: "https://localhost:5001",
    client_id: "js",
    redirect_uri: "https://localhost:5005/callback.html",
    response_type: "code",
    scope: "openid profile cirno.blogs",
    post_logout_redirect_uri: "https://localhost:5005/index.html",
}

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path='/dashboard' component={Dashboard} />
                <Route path='/blog/:id' component={Blog} />
                <Route path='/post/:id' component={Post} />
            </Layout>
        );
    }
}


export { getApiBaseUrl, oidcConfiguration };
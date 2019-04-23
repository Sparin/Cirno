import React, { Component } from 'react';
import { getApiBaseUrl, oidcConfiguration } from '../App';
import { Redirect } from 'react-router-dom'
import { Link } from 'react-router-dom'
import Oidc from 'oidc-client';



export class Post extends Component {

    constructor(props) {
        super(props);

        this.state = {
            id: props.match.params.id,
            post: {},
            loading: true
        };
    }

    componentDidMount() {
        this.getPost();
    }

    async sendRequest(method, endpoint, body) {
        try {
            let mgr = new Oidc.UserManager(oidcConfiguration);
            let user = await mgr.getUser();
            console.error(user);
            const response = await fetch(`${getApiBaseUrl()}${endpoint}`, {
                method,
                body: body && JSON.stringify(body),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `${user != null ? user.token_type : ''} ${user != null ? user.access_token : ''}` 
                }
            });
            if (response.status === 401)
                mgr.signinRedirect();
            else if (response.status === 403)
                alert("Forbidden");
            else
                return await response.json();
        } catch (error) {
            console.error(error);
        }
    }

    async getPost() {
        this.setState({ loading: false, post: await this.sendRequest('get', `api/post/${this.state.id}`) });
    }

    async editPost(post) {
        await this.sendRequest('put', `api/post/${post.id}`, post);

        this.props.history.goBack();
        this.getPosts();
    }

    async deletePost(post) {
        if (window.confirm(`Are you sure you want to delete "${post.title}"`)) {
            await this.sendRequest('delete', `api/post/${post.id}`);
            this.getPost();
        }
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <div className="card" style={{ marginBottom: 1 + 'em', padding: '16px' }} >
                <h3 className="card-title">{this.state.post.title}</h3>
                <p dangerouslySetInnerHTML={{ __html: this.state.post.content }} className="card-text"></p>
                <hr style={{ margin: 0, padding: 0 }}></hr>

                <div className="d-flex justify-content-between align-items-center">
                    <div className="btn-group">
                        <button className="btn btn-sm btn-secondary my-2" onClick={() => this.deletePost(this.state.post)}>Edit</button>
                        <button className="btn btn-sm btn-secondary my-2" onClick={() => this.deletePost(this.state.post)}>Delete</button>
                    </div>
                    <small className="text-muted">{this.state.post.updatedAt}</small>
                </div>
            </div>;

        return (
            <div>
                {contents}

            </div>);
    }

}
import React, { Component } from 'react';
import { getApiBaseUrl, oidcConfiguration } from '../App';
import { Redirect } from 'react-router-dom'
import { Link } from 'react-router-dom'
import Oidc from 'oidc-client';
import { Blog } from './Blog';

export class Post extends Component {

    constructor(props) {
        super(props);

        this.state = {
            id: props.match.params.id,
            post: {},
            loading: true,
            isEditing: false,
            isCreating: false
        };


        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.createPost = this.createPost.bind(this);
        this.editPost = this.editPost.bind(this);
        this.deletePost = this.deletePost.bind(this);

        let uriParts = window.location.href.split('/');
        let action = uriParts[uriParts.length - 1];
        if (action == "create")
            this.state.isCreating = true;
        else if (action == "edit")
            this.state.isEditing = true;
    }

    componentDidMount() {
        if (this.state.isCreating)
            this.getBlogs();
        else
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

            else if (response.ok)
                return await response.json();
            else {
                alert(`${response.status} ${response.statusText}`);
                return false;
            }

        } catch (error) {
            console.error(error);
        }
    }

    async getBlogs() {
        this.setState({ loading: false, blogs: await this.sendRequest('get', `api/blog`) });
        this.setState({ post: { blogId: this.state.blogs[0].id } });
    }

    async getPost() {
        this.setState({ loading: false, post: await this.sendRequest('get', `api/post/${this.state.id}`) });
    }

    async createPost(post) {
        return await this.sendRequest('post', `api/post`, post);
    }

    async editPost(post) {
        return await this.sendRequest('put', `api/post/${post.id}`, post);
    }

    async deletePost(post) {
        if (window.confirm(`Are you sure you want to delete "${post.title}"`)) {
            let response = await this.sendRequest('delete', `api/post/${post.id}`);
            if (response != false)                
                this.props.history.push(`/blog/${this.state.post.blogId}`);
        }
    }

    handleChange = (e) => {
        var changes = {
            post: this.state.post
        }

        changes.post[e.target.name] = e.target.value;
        if (e.target.options != undefined) {
            const selectedIndex = e.target.options.selectedIndex;
            changes.post.blogId = e.target.options[selectedIndex].getAttribute('data-key');
        }

        this.setState(changes);
    }

    async handleSubmit(event) {
        if (this.state.isCreating) {
            let response = await this.createPost(this.state.post);
            if (response == false)
                return;
            this.setState({ post: response });
            this.props.history.push(`/post/${this.state.post.id}`);
            this.setState({ isCreating: false });
        }

        if (this.state.isEditing) {
            let response = await this.editPost(this.state.post);
            if (response == false)
                return;
            this.setState({ post: response });
            this.props.history.push(`/post/${this.state.post.id}`);
            this.setState({ isEditing: false });
        }
    }

    renderEditForm(post) {
        return (
            <div>
                {
                    this.state.isCreating &&
                    <div>
                        <label>Select blog</label>
                        <p><select className="form-control" name="blogId" defaultValue={this.state.blogs[0].blogId} onChange={this.handleChange}>
                            {this.state.blogs.map(blog =>
                                <option key={blog.id} name="blogId" data-key={blog.id} value={post.blogId}>{blog.name}</option>
                            )}
                        </select></p>
                    </div>
                }
                <label>Title</label>
                <p><input type="text" className="form-control" name="title" defaultValue={post.title} onChange={this.handleChange} /></p>
                <label>Content</label>
                <p><textarea type="text" className="form-control" name="content" defaultValue={post.content} onChange={this.handleChange} /></p>
                <p><button className="btn btn-primary" onClick={() => this.handleSubmit(post)}>Submit</button></p>
            </div >
        );
    }

    renderPost(post) {
        return <div className="card" style={{ marginBottom: 1 + 'em', padding: '16px' }} >
            <h3 className="card-title">{post.title}</h3>
            <p dangerouslySetInnerHTML={{ __html: post.content }} className="card-text"></p>
            <hr style={{ margin: 0, padding: 0 }}></hr>

            <div className="d-flex justify-content-between align-items-center">
                <div className="btn-group">
                    <button className="btn btn-sm btn-secondary my-2" onClick={() => { this.setState({ isEditing: true }); this.props.history.push(`/post/${this.state.post.id}/edit`) }}>Edit</button>
                    <button className="btn btn-sm btn-secondary my-2" onClick={async () => await this.deletePost(post)}>Delete</button>
                </div>
                <small className="text-muted">{post.updatedAt}</small>
            </div>
        </div>;
    }

    render() {
        if (this.state.loading)
            return (<p><em>Loading...</em></p>);

        let content = null;
        if (this.state.isCreating)
            content = this.renderEditForm({});
        else if (this.state.isEditing)
            content = this.renderEditForm(this.state.post);
        else
            content = this.renderPost(this.state.post);

        return content;
    }
}
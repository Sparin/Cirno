import React, { Component } from 'react';
import { getApiBaseUrl } from '../App';
import { Link } from 'react-router-dom'

export class Blog extends Component {

    constructor(props) {
        super(props);

        this.onDeleteHandle = this.onDeleteHandle.bind(this);
        this.onEditHandle = this.onEditHandle.bind(this);

        this.state = {
            blog: {},
            posts: [],
            pLoading: true,
            bLoading: true
        };

        fetch(getApiBaseUrl() + 'api/blog/' + props.match.params.id)
            .then(response => response.json())
            .then(data => {
                this.setState({ blog: data , bLoading: false });
            });


        fetch(getApiBaseUrl() + 'api/post?blogId=' + props.match.params.id)
            .then(response => response.json())
            .then(data => {
                this.setState({ posts: data , pLoading: false });
            });
    }

    /**
     * Send edit blog request to the API
     * @param {Number} id identifier
     */
    onEditHandle() {
        this.setState({
            currentCount: this.state.currentCount + 1
        });
    }

    /**
     * Send remove blog request to the API
     * @param {Number} id identifier
     */
    onDeleteHandle() {
        this.setState({
            currentCount: this.state.currentCount + 1
        });
    }

    static renderPostList(posts) {
        return (
            <div>
                {posts.map(post =>
                    <div key={post.id} className="card" style={{ marginBottom: 1 + 'em', padding: '16px' }} >
                        <h3 className="card-title">{post.title}</h3>
                        <p dangerouslySetInnerHTML={{ __html: post.content }} className="card-text"></p>
                        <hr style={{ margin: 0, padding: 0 }}></hr>
                        <p style={{ margin: 0, padding:0 }}> Last update at: {post.updatedAt}</p>
                    </div>
                )}
            </div>
        );
    }

    render() {
        let contents = this.state.bLoading && this.state.pLoading
            ? <p><em>Loading...</em></p>
            : Blog.renderPostList(this.state.posts);

        return (
            <div>
                <h1>{this.state.blog.name}</h1>
                <p>Look at this prettiest posts of this blog!</p>
                {contents}
            </div>
        );
    }
}
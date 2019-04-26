import React, { Component } from 'react';
import { getApiBaseUrl } from '../App';
import { Link } from 'react-router-dom'

export class Blog extends Component {

    constructor(props) {
        super(props);

        this.state = {
            blog: {},
            posts: [],
            pLoading: true,
            bLoading: true
        };

        fetch(getApiBaseUrl() + 'api/blog/' + props.match.params.id)
            .then(response => response.json())
            .then(data => {
                this.setState({ blog: data, bLoading: false });
            });


        fetch(getApiBaseUrl() + 'api/post?blogId=' + props.match.params.id)
            .then(response => response.json())
            .then(data => {
                this.setState({ posts: data, pLoading: false });
            });
    }


    static renderPostList(posts) {
        return (
            <div>
                {posts.map(post =>
                    <div key={post.id} className="card" style={{ marginBottom: 1 + 'em', padding: '16px' }} >
                        <h3 className="card-title"><Link to={'/post/' + post.id}>{post.title}</Link></h3>
                        <p dangerouslySetInnerHTML={{ __html: post.content }} className="card-text"></p>
                        <hr style={{ margin: 0, padding: 0 }}></hr>
                        <p style={{ margin: 0, padding: 0 }}> Last update at: {post.updatedAt}</p>
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
import React, { Component } from 'react';
import { getApiBaseUrl } from '../App';

export class PostForm extends Component {
    constructor() {
        super();
        this.state = {
            blogs: [],
            blogId: 0,
            title: '',
            content: '',
            loading: true
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);

        fetch(getApiBaseUrl() + 'api/blog')
            .then(response => response.json())
            .then(data => {
                this.setState({ blogs: data, loading: false });
            });
    }

    handleChange = (e) => {
        this.setState({ [e.target.name]: e.target.value });
    }

    handleSubmit(event) {
        alert('A name was submitted: ' + this.state);
        event.preventDefault();
    }

    render() {

        const { blogId, title, content } = this.state;
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <form onSubmit={this.handleSubmit}>
                <p><input
                    type="text"
                    name="title"
                    value={title}
                    onChange={this.handleChange}
                /></p>
                <p><input
                    type="text"
                    name="content"
                    value={content}
                    onChange={this.handleChange}
                /></p>
                <select value={blogId} onChange={this.handleChange}>
                    {this.state.blogs.map(blog =>
                        <option key={blog.id} value="{blog.id}">{blog.Name}</option>
                    )}
                </select>
                <input type="submit" value="Submit" />
            </form>;
        return (
            <div>
                {contents}
            </div>
        );
    }
}
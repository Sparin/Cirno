import React, { Component } from 'react';
import { getApiBaseUrl } from '../App';
import { Redirect } from 'react-router-dom'
import { Link, Route} from 'react-router-dom'
import { Post } from './Post';

export class Dashboard extends Component {

    constructor(props) {
        super(props);

        this.state = { blogs: [], loading: true };

        fetch(getApiBaseUrl() + 'api/blog')
            .then(response => response.json())
            .then(data => {
                this.setState({ blogs: data, loading: false });
            });
    }

    static renderDashboardTable(blogs) {
        return (
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Posts</th>
                    </tr>
                </thead>
                <tbody>
                    {blogs.map(blog =>
                        <tr key={blog.id}>
                            <td><Link to={'/blog/' + blog.id}>{blog.name}</Link></td>
                            <td>{blog.postCount}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }


    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Dashboard.renderDashboardTable(this.state.blogs);

        return (
            <div>
                <h1>Cirno's dashboard!</h1>
                <p>Look at this prettiest blogs you ever have seen</p>
                <p><Link to="/post/create" className="btn btn-sm btn-primary">Submit post to them!</Link></p>
                <Route path={`/post/create`} component={Post} />
                {contents}
            </div>
        );
    }
}
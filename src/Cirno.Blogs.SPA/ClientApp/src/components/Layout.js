import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import styles from '../styles/site.css';

export class Layout extends Component {
    static displayName = Layout.name;

    render() {
        return (
            <div className="d-flex flex-column h-100">
                <NavMenu />
                <Container>
                    {this.props.children}
                </Container>
                <footer className="footer mt-auto py-3">
                    <div className="container">
                        2019 &copy; Sparin - Cirno's project for achieving a bachelor degree
                </div>
                </footer>
            </div>
            
        );
    }
}

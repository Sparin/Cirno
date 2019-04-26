import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import Oidc from 'oidc-client';
import { oidcConfiguration } from '../App';
import './NavMenu.css';

export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true,
            user: null
        };

        this.mgr = new Oidc.UserManager(oidcConfiguration);

        this.mgr.getUser().then(user => {
            this.setState({ user: user });
        });
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    renderLoginLinks() {
    }

    render() {
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                    <Container>
                        <NavbarBrand tag={Link} to="/">Cirno's Blogs</NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                            <ul className="navbar-nav flex-grow">
                                <NavItem>
                                    <Link tag={Link} className="text-dark nav-link" to="/dashboard">Dashboard</Link>
                                </NavItem>
                                {!this.state.user &&
                                    <NavItem>
                                        <div className="nav-link text-dark" style={{ cursor: "pointer", }} onClick={() => this.mgr.signinRedirect()}>Login</div>
                                    </NavItem>
                                }
                                {this.state.user &&
                                    <NavItem>
                                        <div className="nav-link text-dark" style={{ cursor: "pointer", }} onClick={() => this.mgr.signoutRedirect()}>Logout</div>
                                    </NavItem>
                                }
                            </ul>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }
}

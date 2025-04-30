import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import LoginButton from "./LoginButton";
import {UserInfo} from "./UserInfo";
import {isLoggedIn} from "../constants";
import {UserAvatar} from "./UserAvatar";

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  componentDidMount() {
    const checkAuth = async () => {
      try {
        const loggedIn = await isLoggedIn();
        this.setState({
          isAuthenticated: loggedIn
        });
      } catch (error) {
        console.error("Error checking authentication:", error);
        this.setState({
          isAuthenticated: false
        });      
      }
    };

    checkAuth();
  }
  
  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true,
      isAuthenticated: false
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render() {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
          <NavbarBrand tag={Link} to="/">HvZBot</NavbarBrand>
          <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/counter">Counter</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/decrementer">Decrementer</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/fetch-data">Fetch data</NavLink>
              </NavItem>
            </ul>
          </Collapse>
          {
            this.state.isAuthenticated ? (
                <UserInfo size={32} withName={false} />
            ) : (
                <LoginButton/>
            )
          }
        </Navbar>
      </header>
    );
  }
}

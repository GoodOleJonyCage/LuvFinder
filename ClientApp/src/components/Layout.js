import React, { Component } from 'react';
/*import { Container } from 'reactstrap';*/
import { Header } from './Header';
import { Footer } from './Footer';
/*import { NavMenu } from './NavMenu';*/

export class Layout extends Component {
    static displayName = Layout.name;

    render() {
        return (
            <>
                <Header></Header>
                {/* <NavMenu />*/}
                {/*<Container>*/}
                {/*    {this.props.children}*/}
                {/*</Container>*/}
                {this.props.children}
                <Footer></Footer>
            </>
        );
    }
}

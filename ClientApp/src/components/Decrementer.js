import React, { Component } from 'react';

export class Decrementer extends Component {
  static displayName = Decrementer.name;

  constructor(props) {
    super(props);
    this.state = { currentCount: 100 };
    this.decrementDecrementer = this.decrementDecrementer.bind(this);
  }

  decrementDecrementer() {
    this.setState({
      currentCount: this.state.currentCount - 1
    });
  }

  render() {
    return (
      <div>
        <h1>Decrementer</h1>

        <p>This is an example of a complex, customized React component.</p>

        <p aria-live="polite">Current count: <strong>{this.state.currentCount}</strong></p>

        <button className="btn btn-primary" onClick={this.decrementDecrementer}>Decrement</button>
      </div>
    );
  }
}

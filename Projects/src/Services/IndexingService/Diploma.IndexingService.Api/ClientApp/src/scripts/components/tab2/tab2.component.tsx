import React, { FunctionComponent, memo, useState, useEffect } from 'react';

const Tab2: FunctionComponent = memo(() => {
  const [state, setState] = useState({
    sosat: 1
  });

  useEffect(() => {
    setTimeout(() => setState({
      sosat: 2
    }), 1000);
  });

  return (<div>Tab2: {state.sosat}</div>);
});

export default Tab2;
import React, { FunctionComponent, memo, useState, useEffect } from 'react';

const Tab1: FunctionComponent = memo(() => {
  const [counter, setCounter] = useState(0);

  useEffect(() => {
    setTimeout(() => setCounter(counter + 1), 500);
  });

  return (<>
    <div>Tab1</div>
    <div>{counter}</div>
  </>);
});

export default Tab1;
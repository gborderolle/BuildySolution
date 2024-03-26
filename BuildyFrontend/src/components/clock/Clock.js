import React, { useState, useEffect } from "react";

import classes from "./Clock.module.css";

const Clock = () => {
  const [time, setTime] = useState(new Date());

  useEffect(() => {
    const timerID = setInterval(() => {
      setTime(new Date());
    }, 1000);

    return () => {
      clearInterval(timerID);
    };
  }, []);

  const timeString = `${time.getHours().toString().padStart(2, "0")}:${time
    .getMinutes()
    .toString()
    .padStart(2, "0")}:${time.getSeconds().toString().padStart(2, "0")}`;

  return <div className={classes.Clock}>{timeString}</div>; // Aplique los estilos aquí
};

export default Clock;

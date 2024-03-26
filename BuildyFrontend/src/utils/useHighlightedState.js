import { useState, useEffect } from "react";

const useHighlightedState = (initialValue, dependency) => {
  const [isHighlighted, setIsHighlighted] = useState(initialValue);
  useEffect(() => {
    setIsHighlighted(true);
    const timer = setTimeout(() => {
      setIsHighlighted(false);
    }, 300);

    return () => {
      clearTimeout(timer);
    };
  }, [dependency]);

  return [isHighlighted, setIsHighlighted];
};

export default useHighlightedState;
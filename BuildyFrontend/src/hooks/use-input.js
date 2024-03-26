import { useState } from "react";

const useInput = (validateValue, onChangeCallback, initialValue = "") => {
  const [enteredValue, setEnteredValue] = useState(initialValue);
  const [isTouched, setIsTouched] = useState(false);

  const valueIsValid = validateValue(enteredValue);
  const hasError = !valueIsValid && isTouched;

  const valueChangeHandler = (event) => {
    const newValue = event.target.value;
    setEnteredValue(newValue);

    // Invoca el callback si existe
    if (onChangeCallback) {
      onChangeCallback(newValue);
    }
  };

  const inputBlurHandler = () => {
    setIsTouched(true);
  };

  const reset = (initialValue = "") => {
    setEnteredValue(initialValue);
    setIsTouched(false);
  };

  return {
    value: enteredValue,
    isValid: valueIsValid,
    hasError,
    valueChangeHandler,
    inputBlurHandler,
    reset,
  };
};

export default useInput;

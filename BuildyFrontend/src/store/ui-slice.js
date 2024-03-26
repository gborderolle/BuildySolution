import { createSlice } from "@reduxjs/toolkit";

const uiSlice = createSlice({
  name: "ui",
  initialState: {
    isToastShown: false,
  },
  reducers: {
    showToast(state) {
      state.isToastShown = true;
    },
  },
});

// incluyo acá mismo el Actions (porque son métodos simples)
export const uiActions = uiSlice.actions;

export default uiSlice;

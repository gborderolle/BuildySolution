import { configureStore } from "@reduxjs/toolkit";
import changeState from "./oldStore.js";
import generalDataSlice from "./generalData-slice";
import uiSlice from "./ui-slice";
import authSlice from "./auth-slice";
import thunk from "redux-thunk";

const store = configureStore({
  reducer: {
    oldState: changeState,
    generalData: generalDataSlice.reducer,
    ui: uiSlice.reducer,
    auth: authSlice.reducer,
  },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(thunk),
});

export default store;

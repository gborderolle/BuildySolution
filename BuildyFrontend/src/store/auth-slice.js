import { createSlice } from "@reduxjs/toolkit";

const getInitialState = () => {
  const isLoggedInData = JSON.parse(localStorage.getItem("isLoggedIn"));
  let isLoggedIn = false;

  if (isLoggedInData && new Date().getTime() < isLoggedInData.expiry) {
    isLoggedIn = isLoggedInData.value;
  } else {
    localStorage.removeItem("isLoggedIn");
    localStorage.removeItem("userEmail");
    localStorage.removeItem("username");
    localStorage.removeItem("userRole");
    localStorage.removeItem("userFullname");
    localStorage.removeItem("userId");
    localStorage.removeItem("isMobile");
    localStorage.removeItem("authToken");
  }

  return {
    isLoggedIn,
    userEmail: localStorage.getItem("userEmail") || "",
    username: localStorage.getItem("username") || "",
    userRole: localStorage.getItem("userRole") || "",
    userFullname: localStorage.getItem("userFullname") || "",
    userId: localStorage.getItem("userId") || "",
    isMobile: Boolean(localStorage.getItem("isMobile")) || false,
    authToken: localStorage.getItem("authToken") || "",
  };
};

const authSlice = createSlice({
  name: "auth",
  initialState: getInitialState(),
  reducers: {
    login(state, action) {
      const expiry = new Date().getTime() + 1 * 60 * 60 * 1000; // 1 hora
      state.isLoggedIn = true;
      state.userEmail = action.payload.userEmail;
      state.username = action.payload.username;
      state.userRole = action.payload.userRole;
      state.userFullname = action.payload.userFullname;
      state.userId = action.payload.userId;
      state.isMobile = action.payload.isMobile;
      state.authToken = action.payload.authToken; // Agrega esta línea

      const data = {
        value: true,
        expiry,
      };

      localStorage.setItem("isLoggedIn", JSON.stringify(data));
      localStorage.setItem("userEmail", state.userEmail);
      localStorage.setItem("userRole", state.userRole);
      localStorage.setItem("userFullname", state.userFullname);
      localStorage.setItem("userId", state.userId);
      localStorage.setItem("isMobile", state.isMobile);
      localStorage.setItem("authToken", state.authToken); // Almacenar el token en localStorage
    },

    logout(state) {
      state.isLoggedIn = false;
      state.userEmail = "";
      state.username = "";
      state.userRole = "";
      state.userFullname = "";
      state.userId = "";
      state.authToken = "";

      const data = {
        value: false,
        expiry: new Date().getTime(),
      };

      localStorage.setItem("isLoggedIn", JSON.stringify(data));
      localStorage.removeItem("userEmail");
      localStorage.removeItem("username");
      localStorage.removeItem("userRole");
      localStorage.removeItem("userFullname");
      localStorage.removeItem("userId");
      localStorage.removeItem("isMobile");
      localStorage.removeItem("authToken");
    },

    setIsMobile(state, action) {
      state.isMobile = action.payload;
      localStorage.setItem("isMobile", action.payload);
    },

    initializeAuthState(state) {
      const isLoggedInData = JSON.parse(localStorage.getItem("isLoggedIn"));

      if (isLoggedInData && new Date().getTime() < isLoggedInData.expiry) {
        state.isLoggedIn = isLoggedInData.value;
      } else {
        localStorage.removeItem("isLoggedIn");
        state.isLoggedIn = false;
      }

      state.userEmail = localStorage.getItem("userEmail") || "";
      state.username = localStorage.getItem("username") || "";
      state.userRole = localStorage.getItem("userRole") || "";
      state.userFullname = localStorage.getItem("userFullname") || "";
      state.userId = localStorage.getItem("userId") || "";
      state.isMobile =
        localStorage.getItem("isMobile") == "true" ? true : false;
      state.authToken = localStorage.getItem("authToken") || "";
    },

    resetAuthState(state) {
      localStorage.removeItem("isLoggedIn");
      localStorage.removeItem("userEmail");
      localStorage.removeItem("username");
      localStorage.removeItem("userRole");
      localStorage.removeItem("userFullname");
      localStorage.removeItem("userId");
      localStorage.removeItem("isMobile");
      localStorage.removeItem("authToken");

      state.isLoggedIn = false;
      state.userEmail = null;
      state.username = null;
      state.userRole = null;
      state.userFullname = null;
      state.userId = null;
      state.isMobile = null;
    },
  },
});

export const authActions = authSlice.actions;

export default authSlice;

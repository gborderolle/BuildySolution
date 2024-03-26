import { createSlice } from "@reduxjs/toolkit";

const generalDataSlice = createSlice({
  name: "generalData",
  initialState: {
    estateList: [],
    jobList: [],
    rentList: [],
    reportList: [],
    workerList: [],
    tenantList: [],
    countryList: [],
    ownerList: [],
    provinceList: [],
    cityList: [],
    userList: [],
    userRoleList: [],
    userRole: null,
    logsList: [],
  },
  reducers: {
    setEstateList: (state, action) => {
      state.estateList = action.payload;
    },
    setJobList: (state, action) => {
      state.jobList = action.payload;
    },
    setRentList: (state, action) => {
      state.rentList = action.payload;
    },
    setReportList: (state, action) => {
      state.reportList = action.payload;
    },
    setWorkerList: (state, action) => {
      state.workerList = action.payload;
    },
    setTenantList: (state, action) => {
      state.tenantList = action.payload;
    },
    setCountryList: (state, action) => {
      state.countryList = action.payload;
    },
    setOwnerList: (state, action) => {
      state.ownerList = action.payload;
    },
    setProvinceList: (state, action) => {
      state.provinceList = action.payload;
    },
    setCityList: (state, action) => {
      state.cityList = action.payload;
    },
    setUserList: (state, action) => {
      state.userList = action.payload;
    },
    setUserRoleList: (state, action) => {
      state.userRoleList = action.payload;
    },
    setUserRole: (state, action) => {
      state.userRole = action.payload;
    },
    setLogsList: (state, action) => {
      state.logsList = action.payload;
    },
  },
});

export const generalDataActions = generalDataSlice.actions;

export default generalDataSlice;

import axios from "axios";
import { generalDataActions } from "./generalData-slice";

import {
  urlEstate,
  urlJob,
  urlRent,
  urlReport,
  urlWorker,
  urlTenant,
  urlCity,
  urlProvince,
  urlCountry,
  urlOwner,
  urlAccount,
} from "../endpoints";

const fetchApi = async (url) => {
  try {
    const authToken = localStorage.getItem("authToken");
    const config = {
      headers: {
        Authorization: `Bearer ${authToken}`,
        "x-version": "1",
      },
    };

    const response = await axios.get(url, config);
    return response.data;
  } catch (error) {
    console.error("API error:", error);
    throw error;
  }
};

export const fetchEstateList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlEstate + "/GetEstate");
      if (data.result) {
        dispatch(generalDataActions.setEstateList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchJobList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlJob + "/GetJob");
      if (data.result) {
        dispatch(generalDataActions.setJobList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchRentList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlRent + "/GetRent");
      if (data.result) {
        dispatch(generalDataActions.setRentList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchReportList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlReport + "/GetReport");
      if (data.result) {
        dispatch(generalDataActions.setReportList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchWorkerList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlWorker + "/GetWorker");
      if (data.result) {
        dispatch(generalDataActions.setWorkerList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchTenantList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlTenant + "/GetTenant");
      if (data.result) {
        dispatch(generalDataActions.setTenantList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchCityList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlCity + "/GetCity");
      if (data.result) {
        dispatch(generalDataActions.setCityList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchProvinceList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlProvince + "/GetProvince");
      if (data.result) {
        dispatch(generalDataActions.setProvinceList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchCountryList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlCountry + "/GetCountry");
      if (data.result) {
        dispatch(generalDataActions.setCountryList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchOwnerList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlOwner + "/GetOwner");
      if (data.result) {
        dispatch(generalDataActions.setOwnerList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchUserList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlAccount + "/GetUsers");
      if (data.result) {
        dispatch(generalDataActions.setUserList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchUserRoleList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlAccount + "/GetRoles");
      if (data.result) {
        dispatch(generalDataActions.setUserRoleList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchUserRole = (id) => {
  return async () => {
    try {
      const data = await fetchApi(`${urlAccount}/GetUserRole/${id}`);
      return data.result;
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

export const fetchLogsList = () => {
  return async (dispatch) => {
    try {
      const data = await fetchApi(urlAccount + "/GetLogs");
      if (data.result) {
        dispatch(generalDataActions.setLogsList(data.result));
      }
    } catch (error) {
      console.error("Fetch error:", error);
    }
  };
};

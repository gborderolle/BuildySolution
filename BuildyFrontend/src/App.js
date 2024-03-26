import React, { Suspense } from "react";
import { HashRouter, Route, Routes } from "react-router-dom";

// redux imports
import { useSelector } from "react-redux";

import "./scss/style.scss";

const loading = (
  <div className="pt-3 text-center">
    <div className="sk-spinner sk-spinner-pulse"></div>
  </div>
);

const Login = React.lazy(() => import("./views/pages/login/Login"));
const Register = React.lazy(() => import("./views/pages/register/Register"));
const Page404 = React.lazy(() => import("./views/pages/page404/Page404"));
const Page500 = React.lazy(() => import("./views/pages/page500/Page500"));
const DefaultLayout = React.lazy(() =>
  import("./components/defaultLayout/DefaultLayout")
);

function App() {
  //#region Consts ***********************************

  const isLoggedIn = useSelector((state) => state.auth.isLoggedIn);

  //#endregion Consts ***********************************

  //#region Hooks ***********************************

  //#endregion Hooks ***********************************

  return (
    <HashRouter>
      <Suspense fallback={loading}>
        <Routes>
          {!isLoggedIn && <Route path="*" element={<Login />} />}
          {isLoggedIn && (
            <>
              <Route exact path="/register" element={<Register />} />
              <Route exact path="/404" element={<Page404 />} />
              <Route exact path="/500" element={<Page500 />} />
              <Route path="*" element={<DefaultLayout />} />
            </>
          )}
        </Routes>
      </Suspense>
    </HashRouter>
  );
}

export default App;

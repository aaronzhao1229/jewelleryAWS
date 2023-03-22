
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import { Container } from "@mui/system";
import { useCallback, useEffect, useState } from "react";
import { Outlet, useLocation } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css'; // to supply the css that react-toastify for users
import { fetchCurrentUser } from "../../features/account/accountSlice";
import { fetchBasketAsync } from "../../features/basket/basketSlice";
import HomePage from "../../features/Home/HomePage";

import { useAppDispatch } from "../store/configureStore";
import Header from "./Header";
import LoadingComponent from "./LoadingComponent";


function App() {
    const location = useLocation() // we can get the path of where we're currently browsing to 
    const dispatch = useAppDispatch()
    const [loading, setLoading] = useState(true)

    const initApp = useCallback(async () => {
        try {
            await dispatch(fetchCurrentUser())
            await dispatch(fetchBasketAsync())
        } catch (error) {
            console.log(error)
        }
    }, [dispatch])

    // When we use fetchCurrentUser or fetchBasketAsync, it could cause initApp change and make an infinite loop. So we need to wrap the initApp() function with useCallback(). What it does is it will memorize the initApp function and ensure it does not change on any re-render.
    useEffect(() => {
        initApp().then(() => setLoading(false))
    }, [initApp])

    const [darkMode, setDarkMode] = useState(false)
    const paletteType = darkMode ? 'dark' : 'light'
    const theme = createTheme({
        palette: {
            mode: paletteType,
            background: {
                default: paletteType === 'light' ? '#eaeaea' :'#121212' 
            }
        }
    })

    function handleThemeChange() {
      setDarkMode(!darkMode)
    }


    return (
      <ThemeProvider theme={theme}>
          
          <ToastContainer position="bottom-right" hideProgressBar theme="colored" />
          <CssBaseline />
          <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
          {loading ? <LoadingComponent message='Initialising app...' /> : location.pathname === '/' ? <HomePage /> : 
          <Container sx={{mt: 4}}>
              <Outlet />
          </Container>}
          
          
      </ThemeProvider>
  );
}

export default App;

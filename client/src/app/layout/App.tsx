
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import { Container } from "@mui/system";
import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css'; // to supply the css that react-toastify for users
import agent from "../api/agent";
import { useStoreContext } from "../context/StoreContext";
import { getCookie } from "../util/util";
import Header from "./Header";
import LoadingComponent from "./LoadingComponent";


function App() {
    const {setBasket} = useStoreContext()
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const buyerId = getCookie('buyerId')
        if (buyerId) {
            agent.Basket.get()
            .then(basket => setBasket(basket))
            .catch(error => console.log(error))
            .finally(() => setLoading(false))
        } else {
            setLoading(false)
        }
    }, [setBasket])

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

    if (loading) return <LoadingComponent message='Initialising app...' />

    return (
      <ThemeProvider theme={theme}>
          
          <ToastContainer position="bottom-right" hideProgressBar theme="colored" />
          <CssBaseline />
          <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
          <Container>
              <Outlet />
          </Container>
          
      </ThemeProvider>
  );
}

export default App;


import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import { Container } from "@mui/system";
import { useState } from "react";
import { Outlet } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css'; // to supply the css that react-toastify for users

import Header from "./Header";


function App() {
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
          <Container>
              <Outlet />
          </Container>
          
      </ThemeProvider>
  );
}

export default App;

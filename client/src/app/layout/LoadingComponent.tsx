import { Backdrop, Box, CircularProgress, Typography } from "@mui/material";

interface Props {
    message?: string
}

export default function LoadingComponent({message = 'Loading...'} : Props) {
   // use backdrop because it takes over the full screen of the app and prevents the user from clicking on something while the app is going through loading of something.
    return (
        <Backdrop open={true} invisible={true}>
            <Box display='flex' justifyContent='center'alignItems='center' height='100vh'>
                <CircularProgress size={100} color='secondary' />
                <Typography variant="h4" sx={{justifyContent: 'center', position: 'fixed', top: '60%'
                }}>{message}</Typography>
            </Box>
        </Backdrop>
    )
}
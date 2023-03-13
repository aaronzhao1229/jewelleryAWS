import Avatar from '@mui/material/Avatar';
import TextField from '@mui/material/TextField';

import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { Paper } from '@mui/material';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { FieldValues, useForm } from 'react-hook-form';
import { LoadingButton } from '@mui/lab';
import { useAppDispatch } from '../../app/store/configureStore';
import { signInUser } from './accountSlice';

// react-hook-form is going to track the loading status when we are submitting and we'll make use of this flag down in our component
export default function Login() {
    const navigate = useNavigate()
    const location = useLocation() // as we need to get a hold of our states and see if we actually have a from location to sent them to
    const dispatch = useAppDispatch()
    const {register, handleSubmit, formState: {isSubmitting, errors, isValid}} = useForm({
        mode: 'onTouched' // tell what mode we want it to use to valid inputs. onTouch: as soon as they click into an input and then click out again, it's going to validate the field
    })

    async function submitForm(data: FieldValues) {
        try {
            await dispatch(signInUser(data))
            navigate(location.state?.from || '/catalog')
          
        } catch (error) {
            console.log(error)
        }
        // user try catch to catch the error in case the uncaught error appears in the console

    }

  return (
    
      <Container component={Paper} maxWidth="sm" sx={{display: 'flex', flexDirection: 'column', alignItems: 'center', p: 4}}>
        
          <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
            <LockOutlinedIcon />
          </Avatar>
          <Typography component="h1" variant="h5">
            Sign in
          </Typography>
          <Box component="form" onSubmit={handleSubmit(submitForm)} noValidate sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              fullWidth
              label="Username"
              autoFocus
              {...register('username', {required: 'Username is required'})} // register includes onChange and name
              error={!!errors.username} // !! casts username into a boolean. If it is false, it will give a red color
              helperText={errors?.username?.message as string}
            />
            <TextField
              margin="normal"
              fullWidth
             
              label="Password"
              type="password"
              {...register('password', {required:  'Password is required'})}
              error={!!errors.password} // !! casts username into a boolean. If it is false, it will give a red color
              helperText={errors?.password?.message as string}
            />
            
            <LoadingButton
              loading={isSubmitting}
              disabled={!isValid}
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 3, mb: 2 }}
            >
              Sign In
            </LoadingButton>
            <Grid container>
              
              <Grid item>
                <Link to="/register">
                  {"Don't have an account? Sign Up"}
                </Link>
              </Grid>
            </Grid>
          </Box>
        
        
      </Container>
    
  )
}

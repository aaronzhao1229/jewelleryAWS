import { createAsyncThunk, createSlice, isAnyOf } from "@reduxjs/toolkit";
import { FieldValues } from "react-hook-form";
import { toast } from "react-toastify";
import agent from "../../app/api/agent";
import { User } from "../../app/models/user";
import { router } from "../../app/router/Routes";
import { setBasket } from "../basket/basketSlice";

interface AccountState {
    user: User | null
}

const initialState: AccountState = {
    user: null
}

export const signInUser = createAsyncThunk<User, FieldValues>(
    'account/signInUser',
    async (data, thunkAPI) => {
        try {
            const userDto = await agent.Account.login(data)
            const {basket, ...user} = userDto
            if (basket) thunkAPI.dispatch(setBasket(basket))
            localStorage.setItem('user', JSON.stringify(user)) // store it in the localStorage
            return user
        } catch (error: any) {
            return thunkAPI.rejectWithValue({error: error.data})
        }
    }
)

export const fetchCurrentUser = createAsyncThunk<User>(
  'account/fetchCurrentUser',
  async (data, thunkAPI) => {
      thunkAPI.dispatch(setUser(JSON.parse(localStorage.getItem('user')!)))
      try {
          const userDto = await agent.Account.currentUser()
          const {basket, ...user} = userDto
          if (basket) thunkAPI.dispatch(setBasket(basket))
          localStorage.setItem('user', JSON.stringify(user)) // store it in the localStorage
          return user
      } catch (error: any) {
          return thunkAPI.rejectWithValue({error: error.data})
      }
  },
  {
      condition: () => {
          if (!localStorage.getItem('user')) return false // we're not going to make the network request if we do not have the user key inside local storage
      }
  }
)

export const accountSlice = createSlice({
    name: 'account',
    initialState,
    reducers: {
        signOut: (state) => {
            state.user = null
            localStorage.removeItem('user')
            router.navigate('/')
        }, 
        setUser: (state, action) => {
            let claims = JSON.parse(atob(action.payload.token.split('.')[1])) // get the payload part of the token
            let roles = claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
            state.user = {...action.payload, roles: typeof(roles) === 'string' ? [roles] : roles}
        }
    },
    extraReducers: (builder => {
        builder.addCase(fetchCurrentUser.rejected, (state) => {
            state.user = null
            localStorage.removeItem('user')
            toast.error('Session expired - please login again')
            router.navigate('/')
        })
      // signInUser and fetchCurrentUser both return the user and we want to set  user  inside here. If we get an error, we will use the same case for that as well. We don't have to worry about the pending status because our React hook form, as we we've already seen, is taking care of the loading indicators there. So All we need is two cases and use addMatcher so that we use the same case for both methods. 
        builder.addMatcher(isAnyOf(signInUser.fulfilled, fetchCurrentUser.fulfilled), (state, action) => {
            let claims = JSON.parse(atob(action.payload.token.split('.')[1])) // get the payload part of the token
            let roles = claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
            state.user = {...action.payload, roles: typeof(roles) === 'string' ? [roles] : roles}
        });
        builder.addMatcher(isAnyOf(signInUser.rejected), (state, action) => {
          throw action.payload
        })
    })
})

export const {signOut, setUser} = accountSlice.actions
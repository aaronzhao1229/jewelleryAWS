import { createAsyncThunk, createSlice, isAnyOf } from "@reduxjs/toolkit";
import agent from "../../app/api/agent";
import { Basket } from "../../app/models/basket";
import { getCookie } from "../../app/util/util";

interface BasketState {
    basket: Basket | null
    status: string
}

const initialState: BasketState = {
  basket: null,
  status: 'idle'
}

// the first type in createAsyncThunk function represents what are we returning from this method which is Basket in this case. The next parameter is the argument type, which is productId and quantity
// createAsyncThunk methods create actions on our behalf that we can use to do something inside our store and what we'll need to do is we want to set this to pending as soon as we start to get our baskets item; and then we want to set it back to idle once that is fullfilled or we get an error

export const fetchBasketAsync = createAsyncThunk<Basket> (
    'basket/fetchBasketAsync',
    async (_, thunkAPI) => {
        try {
            return agent.Basket.get()
        } catch (error: any) {
          thunkAPI.rejectWithValue({error: error.data})
        }
    },
    {
        condition: () => {
           if (!getCookie('buyerId')) return false
        }
    }
)

export const addBasketItemAsync = createAsyncThunk<Basket, {productId: number, quantity?: number}>(
    'basket/addBasketItemAsync',  // async creator
    async ({productId, quantity = 1}, thunkAPI) => {
        try {
            return await agent.Basket.addItem(productId, quantity)
        } catch (error: any) {
            thunkAPI.rejectWithValue({error: error.data})
        }
    }
)

export const removeBasketItemAsync = createAsyncThunk<void, {productId: number, quantity: number, name?: string}>(
    'basket/removeBasketItemAsync',
    async ({productId, quantity}, thunkAPI) => {
        try {
            await agent.Basket.removeItem(productId, quantity)
        } catch (error: any) {
          thunkAPI.rejectWithValue({error: error.data})
        }
    }

)

export const basketSlice = createSlice({
    name: 'basket',
    initialState,
    reducers: {
        setBasket: (state, action) => {
            state.basket = action.payload
        },
        clearBasket: (state) => {
            state.basket = null
        }
    },
    extraReducers: (builder => {
        builder.addCase(addBasketItemAsync.pending, (state, action) => {
            state.status = 'pendingAddItem' + action.meta.arg.productId // data structure to get the productId from the action. This is for loading indicator only for the item
        });
      
        builder.addCase(removeBasketItemAsync.pending, (state, action) => {
            state.status = 'pendingRemoveItem' + action.meta.arg.productId + action.meta.arg.name
        })
        builder.addCase(removeBasketItemAsync.fulfilled, (state, action) => {
            const {productId, quantity} = action.meta.arg
            const itemIndex = state.basket?.items.findIndex(i => i.productId === productId)
           
            if (itemIndex === -1 || itemIndex === undefined) return
            
           
            state.basket!.items[itemIndex].quantity -= quantity
           
            

            if (state.basket?.items[itemIndex].quantity === 0) state.basket.items.splice(itemIndex, 1)
            state.status = 'idle'
        })
        builder.addCase(removeBasketItemAsync.rejected, (state, action) => {
            state.status = 'idle'
            console.log(action.payload)
        })
        builder.addMatcher(isAnyOf(addBasketItemAsync.fulfilled, fetchBasketAsync.fulfilled), (state, action) => {
          state.basket = action.payload
          state.status = 'idle'
        })
        builder.addMatcher(isAnyOf(addBasketItemAsync.rejected, fetchBasketAsync.rejected), (state, action) => {
            state.status = 'idle'
            console.log(action.payload)
        })


    })
})

export const {setBasket, clearBasket} = basketSlice.actions
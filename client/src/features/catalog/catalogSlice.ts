import { createAsyncThunk, createEntityAdapter, createSlice } from "@reduxjs/toolkit";
import agent from "../../app/api/agent";
import { MetaData } from "../../app/models/pagination";
import { Product, ProductParams } from "../../app/models/product";
import { RootState } from "../../app/store/configureStore";

interface CatalogState {
    productsLoaded: boolean;
    filtersLoaded: boolean;
    status: string
    categories: string[]
    productParams: ProductParams
    metaData: MetaData | null
}

const productsAdapter = createEntityAdapter<Product>()

function getAxiosParams(productParams: ProductParams) {
    const params = new URLSearchParams()
    params.append('pageNumber', productParams.pageNumber.toString())
    params.append('pageSize', productParams.pageSize.toString())
    params.append('orderBy', productParams.orderBy)
    if (productParams.searchTerm) params.append('searchTerm', productParams.searchTerm)
    if (productParams.categories) params.append('categories', productParams.categories.toString())
    if (productParams.categories?.length === 0) params.delete('categories')
    return params
}

export const fetchProductsAsync = createAsyncThunk<Product[], void, {state: RootState}>(
    'catalog/fetchProductsAsync',
    async (_, thunkAPI) => { //thunkAPI has a method that we can use called getState and because we're storing our product parameters in the state, then we can go and get that state and past it to the list()
        const params = getAxiosParams(thunkAPI.getState().catalog.productParams) //get 
        try {
            const response =  await agent.Catalog.list(params)
            thunkAPI.dispatch(setMetaData(response.metaData))
            return response.items
        } catch (error: any) {
            return thunkAPI.rejectWithValue({error: error.data})
        }
    }
)

export const fetchProductAsync = createAsyncThunk<Product, number>(
  'catalog/fetchProductAsync',
  async (productId, thunkAPI) => { // thunkAPI is for error handling
      try {
          return await agent.Catalog.details(productId)
          
      } catch (error: any) {
          return thunkAPI.rejectWithValue({error: error.data })
      }
  }
)

export const fetchFilters = createAsyncThunk(
    'catalog/fetchFilters',
    async (_, thunkAPI) => {
        try {
            return await agent.Catalog.fetchFilters()
        } catch (error: any) {
          return thunkAPI.rejectWithValue({error: error.data })
        }
    }
)
function initParams() {
    return {
      pageNumber: 1,
      pageSize: 6,
      orderBy:'name'
    }
}


export const catalogSlice = createSlice({
    name: 'catalog',
    initialState: productsAdapter.getInitialState<CatalogState>({
        productsLoaded: false,
        filtersLoaded: false,
        status: 'idle',
        categories: [],
        productParams: initParams(),
        metaData: null,

    }),  // we haven't created initial state ealier because what we get from our products adapter is a method to go ahead and create our initial state.
    // getInitialState() will return ids, entities with the additional properties you defined (productsLoaded and status in this case)
    reducers: {
        setProductParams: (state, action) => {
            state.productsLoaded = false // we want to trigger our useEffect methods in Catalog, because that's effectively listeing for the productsLoaded state. Because our products will not be loaded, it will go and dispatch, and go and get the next batch of products. So we'll effectively using our states to trigger a request to our API to go and gets the next products
            state.productParams = {...state.productParams, ...action.payload, pageNumber: 1}
        },
        setPageNumber: (state, action) => {
          state.productsLoaded = false 
          state.productParams = {...state.productParams, ...action.payload}
        },
        setMetaData: (state, action) => {
            state.metaData = action.payload
        },
        resetProductParams: (state) => {
            state.productParams = initParams()
        }
    },
    extraReducers: (builder => {
        builder.addCase(fetchProductsAsync.pending, (state) => {
            state.status = 'pendingFetchProducts'
        })
        builder.addCase(fetchProductsAsync.fulfilled, (state, action) => {
            productsAdapter.setAll(state, action.payload)
            state.status = 'idle'
            state.productsLoaded = true
        })
        builder.addCase(fetchProductsAsync.rejected, (state, action) => {
            state.status = 'idle'
            console.log(action.payload)
        })
        builder.addCase(fetchProductAsync.pending, (state) => {
            state.status = 'pendingFetchProduct'
        })
        builder.addCase(fetchProductAsync.fulfilled, (state, action) => {
            productsAdapter.upsertOne(state, action.payload)  // upsert a new product into our product entities 
            state.status = 'idle'
        })
        builder.addCase(fetchProductAsync.rejected, (state, action) => {
            console.log(action.payload)
            state.status = 'idle'
        })
        builder.addCase(fetchFilters.pending, (state) => {
            state.status = 'pendingFetchFilters'
        })
        builder.addCase(fetchFilters.fulfilled, (state, action) => {
            state.categories = action.payload.categories
            state.filtersLoaded = true
            state.status = 'idle'
        })
        builder.addCase(fetchFilters.rejected, (state, action) => {
            console.log(action.payload)
            state.status = 'idle'
        })
    })
})

export const productSelectors = productsAdapter.getSelectors((state: RootState) => state.catalog)

export const {setProductParams, resetProductParams, setMetaData, setPageNumber} = catalogSlice.actions
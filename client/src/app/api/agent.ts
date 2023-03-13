import axios, { AxiosError, AxiosResponse } from "axios"
import { toast } from "react-toastify"
import { PaginatedResponse } from "../models/pagination"
import { router } from "../router/Routes"
import { store } from "../store/configureStore"

const sleep = () => new Promise(resolve => setTimeout(resolve, 500))

axios.defaults.baseURL = 'http://localhost:5000/api/'
axios.defaults.withCredentials = true // with both sides of this element in place, then our browser will receive the cookie and it will set the cookie inside our application storage as well

const responseBody = (response: AxiosResponse) => response.data

axios.interceptors.request.use(config => {
    const token = store.getState().account.user?.token // in order to use the user object from getState(), we will need to make sure that if we have a token in local storage, we actually set this inside our state before we're going to be able to use this and set the token accordingly.
    if (token) config.headers.Authorization = `Bearer ${token}`
    return config
})

axios.interceptors.response.use(async response => {
    await sleep();
    const pagination = response.headers['pagination'] // need to be lower case even if there are upper cases in the header
    if (pagination) {
        response.data = new PaginatedResponse(response.data, JSON.parse(pagination))
        return response
    }
    return response
}, (error: AxiosError) => {
    const {data, status} = error.response as AxiosResponse; // add as AxiosResponse to remove the error from the interceptor
    switch (status) {
        case 400: 
        if (data.errors) {
            const modelStateErrors: string[] = []
            for (const key in data.errors) {
                if (data.errors[key]) {  // this if statement purly for TS purposes, just an extra bit of boilerplate so that we don't see a type error on the following line
                    modelStateErrors.push(data.errors[key])
                }
        
            }
            throw modelStateErrors.flat() // flat the array to strings of the problems. Use throw to stop the code running here and the following code would not run
        }
            toast.error(data.title);
            break;
        case 401: 
            toast.error(data.title);
            break;
        case 500: 
            router.navigate('/server-error', {state: {error: data}})
            break;
    }


    return Promise.reject(error.response)  // we are not able to catch and handle the responses in Axios interceptors. That's not what they say it's been designed for. Therefore, we still need to catch the errors inside our components as well. We still need to catch the error at the end of the errors journey which is inside the AboutPage for error tests
})

const requests = {
  // URLSearchParams is not needed to be imported. It allows us to pass the params as query strings to our request. In our Axios get requests, it can take some configuration, and one of the configuration options is to send up a params object and it can be of type URL search parameters
    get: (url: string, params?: URLSearchParams) => axios.get(url, {params}).then(responseBody),
    post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
    put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
    delete: (url: string) => axios.delete(url).then(responseBody),

}

const Catalog = {
    list: (params: URLSearchParams) => requests.get('products', params),
    details: (id: number) => requests.get(`products/${id}`),
    fetchFilters: () => requests.get('products/filters')
}

const TestErrors = {
    get400Error: () => requests.get('buggy/bad-request'),
    get401Error: () => requests.get('buggy/unauthorised'),
    get404Error: () => requests.get('buggy/not-found'),
    get500Error: () => requests.get('buggy/server-error'),
    getValidationError: () => requests.get('buggy/validation-error')
}

const Basket = {
    get: () => requests.get('basket'),
    addItem: (productId: number, quantity = 1) => requests.post(`basket?productId=${productId}&quantity=${quantity}`, {}), // we need an empty object as body even if we use the query string
    removeItem: (productId: number, quantity = 1) => requests.delete(`basket?productId=${productId}&quantity=${quantity}`),
}

const Account = {
    login: (values: any) => requests.post('account/login', values),
    register: (values: any) => requests.post('account/register', values),
    currentUser: () => requests.get('account/currentUser'),
}

const agent = {
    Catalog,
    TestErrors,
    Basket,
    Account
}

export default agent
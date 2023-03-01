import axios, { AxiosError, AxiosResponse } from "axios"
import { toast } from "react-toastify"
import { router } from "../router/Routes"

const sleep = () => new Promise(resolve => setTimeout(resolve, 500))

axios.defaults.baseURL = 'http://localhost:5000/api/'

const responseBody = (response: AxiosResponse) => response.data

axios.interceptors.response.use(async response => {
    await sleep();
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
    get: (url: string) => axios.get(url).then(responseBody),
    post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
    put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
    delete: (url: string) => axios.delete(url).then(responseBody),

}

const Catalog = {
    list: () => requests.get('products'),
    details: (id: number) => requests.get(`products/${id}`)
}

const TestErrors = {
    get400Error: () => requests.get('buggy/bad-request'),
    get401Error: () => requests.get('buggy/unauthorised'),
    get404Error: () => requests.get('buggy/not-found'),
    get500Error: () => requests.get('buggy/server-error'),
    getValidationError: () => requests.get('buggy/validation-error')
}

const agent = {
    Catalog,
    TestErrors
}

export default agent
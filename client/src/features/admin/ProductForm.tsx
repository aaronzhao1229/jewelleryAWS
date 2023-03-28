import { Typography, Grid, Paper, Box, Button } from "@mui/material";
import { useEffect } from "react";
import { FieldValues, useForm } from "react-hook-form";
import AppDropzone from "../../app/components/AppDropzone";
import AppSelectList from "../../app/components/AppSelectList";
import AppTextInput from "../../app/components/AppTextInput";
import useProducts from "../../app/hooks/useProducts";
import { Product } from "../../app/models/product";
import {yupResolver} from '@hookform/resolvers/yup'
import { validationSchema } from "./productValidation";
import agent from "../../app/api/agent";
import { useAppDispatch } from "../../app/store/configureStore";
import { setProduct } from "../catalog/catalogSlice";
import { LoadingButton } from "@mui/lab";

interface Props {
    product?: Product
    cancelEdit: () => void
}


export default function ProductForm({product, cancelEdit} : Props) {
    const { control, reset, handleSubmit, watch, formState: {isDirty, isSubmitting} } = useForm({
        resolver: yupResolver(validationSchema)
    });
    const {categories} = useProducts()
    const watchFile = watch('file', null)
    const dispatch = useAppDispatch()

    // we want to remove the preview when our component dismounts when we no longer are inside our product form because we've even submitted the data or the user's cancelled out of the form or for whatever reason, they're no longer displaying the form with the preview file. 
    useEffect(() => {
        if (product && !watchFile && !isDirty) reset(product) // we only want to reset the product only when we don't have a watchFile and the form is not dirty
        // everything inside the return will happen only when our component is destroyed
        return () => {
            if (watchFile) URL.revokeObjectURL(watchFile.preview) // this will remove the object URL
        }
    }, [product, reset, watchFile, isDirty])

    async function handleSubmitData(data: FieldValues) {
        try {
          let response: Product
          if (product) {
              response = await agent.Admin.updateProduct(data)
          } else {
              response = await agent.Admin.createProduct(data)
          }
          dispatch(setProduct(response))
          cancelEdit() // leave the form
        } catch (error) {
          console.log(error)
        }
    }

    return (
        <Box component={Paper} sx={{p: 4}}>
            <Typography variant="h4" gutterBottom sx={{mb: 4}}>
                Product Details
            </Typography>
            <form onSubmit={handleSubmit(handleSubmitData)}>
                <Grid container spacing={3}>
                    <Grid item xs={12} sm={12}>
                        <AppTextInput control={control} name='name' label='Product name' />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <AppSelectList control={control} items={categories} name='category' label='Category' />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <AppTextInput type='number' control={control} name='price' label='Price' />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <AppTextInput type='number' control={control} name='quantityInStock' label='Quantity in Stock' />
                    </Grid>
                    <Grid item xs={12}>
                        <AppTextInput multiline={true} rows={4} control={control} name='description' label='Description' />
                    </Grid>
                    <Grid item xs={12}>
                        <Box display='flex' justifyContent='space-between' alignItems='center'>
                            <AppDropzone control={control} name='file'  />
                            {watchFile ? (
                                <img src={watchFile.preview} alt="preview" style={{maxHeight: 200}} />
                            ) : (
                                <img src={product?.pictureUrl} alt={product?.name} style={{maxHeight: 200}} />
                            )}
                        </Box>
                        
                    </Grid>
                </Grid>
            <Box display='flex' justifyContent='space-between' sx={{mt: 3}}>
                <Button onClick={cancelEdit} variant='contained' color='inherit'>Cancel</Button>
                <LoadingButton loading={isSubmitting} type='submit' variant='contained' color='success'>Submit</LoadingButton>
            </Box>
            </form>
        </Box>
    )
}
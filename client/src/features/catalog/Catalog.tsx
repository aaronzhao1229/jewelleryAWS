
import { Grid, Paper } from "@mui/material";
import AppPagination from "../../app/components/AppPagination";
import CheckboxButtons from "../../app/components/CheckboxButtons";
import RadioButtonGroup from "../../app/components/RadioButtonGroup";
import useProducts from "../../app/hooks/useProducts";

import LoadingComponent from "../../app/layout/LoadingComponent";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { setPageNumber, setProductParams } from "./catalogSlice";
import ProductList from "./ProductList"
import ProductSearch from "./ProductSearch";

const sortOptions = [
    {value: 'name', label: 'Alphabetical'},
    {value: 'priceDesc', label: 'Price - High to low'},
    {value: 'price', label: 'Price - Low to High'},
]


export default function Catalog() {

  const { products, filtersLoaded, categories, metaData} = useProducts()
  const { productParams } = useAppSelector(state => state.catalog)
  const dispatch = useAppDispatch()

  if (!filtersLoaded) return <LoadingComponent message="Loading products"/>

  return (
    <Grid container columnSpacing={4}>
        <Grid item xs={3}>
            <Paper sx={{mb: 2}}>
                <ProductSearch />
            </Paper>
            <Paper sx={{mb: 2, p: 2}}>
                <RadioButtonGroup
                    selectedValue={productParams.orderBy}
                    options={sortOptions}
                    onChange={(e) => dispatch(setProductParams({orderBy: e.target.value}))}
                />
            </Paper>
            <Paper sx={{mb: 2, p: 2}}>
                <CheckboxButtons
                    items={categories}
                    checked={productParams.categories}
                    onChange={(items: string[]) => dispatch(setProductParams({categories: items}))}
                />
            </Paper>
        </Grid>
        <Grid item xs={9}>
            <ProductList products={products} />
        </Grid>
        <Grid item xs={3} />
        <Grid item xs={9} mb={2} mt={2}>
              {metaData &&
             <AppPagination 
                metaData={metaData}
                onPageChange={(page: number) => dispatch(setPageNumber({pageNumber: page}))}
             />
              }
        </Grid>
                      
        
          
    </Grid>
  )
}
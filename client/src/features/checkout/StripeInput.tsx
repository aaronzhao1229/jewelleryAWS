import { InputBaseComponentProps } from "@mui/material";
import { forwardRef, Ref, useImperativeHandle, useRef } from "react";

interface Props extends InputBaseComponentProps {

}

export const StripeInput = forwardRef(function StripeInput({component: Component, ...props} : Props, ref: Ref<unknown>)
{
    // we're forwarding ref from material UI text input then we're creating a ref inside our stripe input component (elementRef)
    const elementRef = useRef<any>()

    useImperativeHandle(ref, () => ({
        focus: () => elementRef.current.focus
    }))

    return (
        <Component 
            onReady={(element: any) => elementRef.current = element} // this takes place when a component stripe inputs is mounted and it's ready. We then pass it the elements to the current elementRef
            {...props}
        />
    )
})

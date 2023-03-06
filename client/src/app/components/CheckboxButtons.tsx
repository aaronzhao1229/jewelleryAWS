
import { Checkbox, FormControlLabel, FormGroup } from "@mui/material";
import { useState } from "react";

interface Props {
    items: string[]
    checked?: string[]
    onChange: (item: string[]) => void
}

export default function CheckboxButtons({items, checked, onChange}: Props) {
    const [checkedItems, setCheckedItems] = useState(checked || [])

    function handleChecked(value: string) {
        const currentIndex = checkedItems.findIndex(item => item === value)

        let newChecked: string[] = []
        if(currentIndex === - 1) newChecked = [...checkedItems, value] // if unchecked, then check it
        else newChecked = checkedItems.filter(item => item !== value) // uncheck it if it is check
        setCheckedItems(newChecked)
        onChange(newChecked)
    }
    return (
      <FormGroup>
          {items.map(item => (
              <FormControlLabel 
                  control={<Checkbox 
                      checked={checkedItems.indexOf(item)!== -1} 
                      onClick={() => handleChecked(item)}
                  />} 
                      label={item} 
                      key={item} />
          ))}
      </FormGroup>      
    )
}
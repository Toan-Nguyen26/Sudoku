using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;
using System.Linq;


public class NumberManager : Selectable
{   
    [SerializeField] private GameObject num;

    private GameObject emptyGrid;
    // Flag check for whenever the value corresponding is empty of not
    private bool flag = false;

    // The value that a grid correspod to
    public int number_ = 0;

    public bool pencilStatus = false;
    


    public void SetNumbers(int number , int appear){
        // Toss a coin to determine whenever it's head or tail
        // 1 is head and 2 is tail then don't make the number appear
        // If it's a 1 then we set the numbers corresponding to it , 0 set it empty otherwise and flag is false
        if(appear == 1){
            SetGrid(number , Color.black);
        }else{
            number_ = number;
            num.GetComponent<Text>().text = "";
            flag = false;
        }
    }
    
    public void SetGrid(int number , Color32 textColor){
        number_ = number;
        num.GetComponent<Text>().text = number_.ToString();
        num.GetComponent<Text>().color = textColor;
        flag = true;
    }


    // Check whenever the flag is true or not , if it's true then it's it has the value being printed initially , and false if not
    void OnMouseDown()
    {   
        
        bool r = num.GetComponentInParent<GridManager>().ready;
        if(r){
          if(num.transform.parent.gameObject.tag == "Number"){
            // Fill the empty numbers in here , we should also check whenever the pencil is on or not
                num.GetComponentInParent<GridManager>().fillEmptyGrid(num.transform.parent.gameObject , 
                num.GetComponentInParent<GridManager>().emptyTile);
                return;
          }

          num.GetComponentInParent<GridManager>().ready = false;
        }
        

        if(flag){
            // Just highlight it if the flag are true
            num.GetComponentInParent<GridManager>().highlightNumGrid(number_);
        }else{

            // If not then make sure it's ready to be put in the emptyGrid
            num.GetComponentInParent<GridManager>().ready = true;
            num.GetComponentInParent<GridManager>().emptyTile = num.transform.parent.gameObject;
            num.GetComponentInParent<ClickManager>().emptyTile = num.transform.parent.gameObject;
            num.GetComponentInParent<GridManager>().highlightEmptyGrid(num.transform.parent.gameObject);
        }
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{   
    public GameObject emptyTile;

    private GridManager grid;

    public GameObject GlobalCanvas;

    public GameObject PauseImage;

    public GameObject PencilButtonOn;

    public GameObject PencilButtonOff;

    public GameObject PauseButton;

    public GameObject PlayButton;

    public  string key;

    private GameObject oldTile;
    void Start(){
        grid = GetComponent<GridManager>();

    }

    // Update is called once per frame
    public void ButtonTime()
    {   
        // It will do something about reset the button and such
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        
    }

    // Undo the step that we do , bruh
    // TODO : keep track of previous itertion of the grid and so on..
    public void Undo(){
        grid.popUndoStack();
        // If it's bigger than 0 then it's time
        if(grid.undoStack.Count > 0){
            GameObject undoing = grid.undoStack.Peek();

            if(oldTile == null){
                oldTile = undoing;
            }
            // Empty the small one just in case()
            for (int i = 0; i < 9; i++)
            {
                undoing.transform.Find("SmallNum").GetChild(i).gameObject.SetActive(false);
            }

            undoing.transform.Find("Text").GetComponent<Text>().text = grid.textUndoStack.Peek();
            if(grid.isBlue.Peek() == false) undoing.transform.Find("Text").GetComponent<Text>().color = Color.red;
            else undoing.transform.Find("Text").GetComponent<Text>().color = Color.blue;

            // If it's a different tile then empty it , this undo took a while lol
            if(oldTile != undoing){
                oldTile.transform.Find("Text").GetComponent<Text>().text = "";
                oldTile = undoing;
            }

        }else{
            oldTile = null;
        }

    }

    // EraseButton : empty the unfilledTile to empty , straightforward
    public void Erase(){
        if(emptyTile) emptyTile.transform.Find("Text").GetComponent<Text>().text = "";       
    }

    // Give a small grid even smaller number for whatever reason
    // Should have toggle on then make the pencil blurry or smt
    public void PencilOn(){
        PencilButtonOn.SetActive(true);
        PencilButtonOff.SetActive(false);
        // If this is on then starting put small numbers in it
        grid.pencilStatus = true;
    }

    // Give a small grid even smaller number for whatever reason
    // Turn it off and return to it's original state
    public void PencilOff(){
        PencilButtonOn.SetActive(false);
        PencilButtonOff.SetActive(true);
        grid.pencilStatus = false;
    }

    // Show them the answer (Cheating)
    public void Hint(){

        // Empty the small one just in case()
        

        if(emptyTile){
            Tuple<int , int ,int> x;
            grid.d_grid.TryGetValue(emptyTile, out x);
            emptyTile.transform.Find("Text").GetComponent<Text>().text = x.Item3.ToString();
            emptyTile.transform.Find("Text").GetComponent<Text>().color = Color.blue;

                for (int i = 0; i < 9; i++)
                {
                    emptyTile.transform.Find("SmallNum").GetChild(i).gameObject.SetActive(false);
                }
        }
    }

    // Pause the game + time
    public void Pause(){
        PlayButton.SetActive(true);
        PauseImage.SetActive(true);
        GlobalCanvas.SetActive(false);

    }

    // Play the game + resume time
    public void Play(){
        PlayButton.SetActive(false);
        PauseImage.SetActive(false);
        GlobalCanvas.SetActive(true);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Data Manager is where all the number and logic being handled
// This is not fun I think
public class DataManager : MonoBehaviour
{   
    public GameObject DifficultyButton;

    public GameObject DifficultyBoard;

    public GameObject SettingBoard;

    public GameObject S_on;

    public GameObject S_off;

    public GameObject HLD_on;

    public GameObject HLD_off;

    public GameObject HIN_on;

    public GameObject HIN_off;

    private AudioSource audioSource;
    
    private string key = "easy";
    // Start is called before the first frame update
    public int[,] final_data = new int[9 ,9]; 

    // A premaid list for backtracking purposes
    public List<int> arr = new List<int>{ 1, 2, 3, 4, 5 , 6 ,7 , 8, 9};
    
    // A list full of zero to fill in the future
    private  List<int> zero_arr = new List<int>{
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 
                                 0, 0, 0, 0, 0, 0, 0, 0 ,0 ,
                                 0 , 0 ,0 ,0 ,0 ,0 , 0 ,0, 0,
                                 0 ,0 ,0 , 0 , 0, 0, 0, 0 ,0,
                                 0 , 0 , 0 , 0 ,0 ,0, 0 ,0 ,0,
                                 0, 0 ,0 ,0 ,0 ,0 ,0 , 0 ,0 ,
                                 0, 0, 0, 0 , 0, 0, 0, 0 ,0,
                                 0, 0, 0 ,0, 0, 0 ,0 ,0 , 0};


    // A list full of one and zero is to determine whenever an open spot can be put in or nah
    public List<int> app = new List<int>{
                                1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 ,1 ,1 ,0 ,0 , 0 ,0, 0,
                                 0 ,0 ,0 , 0 , 0, 0, 0, 0 ,0,
                                 0 , 0 , 0 , 0 ,0 ,0, 0 ,0 ,0,
                                 0, 0 ,0 ,0 ,0 ,0 ,0 , 0 ,0 ,
                                 0, 0, 0, 0 , 0, 0, 0, 0 ,0,
                                 0, 0, 0 ,0, 0, 0 ,0 ,0 , 0};

    // Easy open spot
    private List<int> easy = new List<int>{
                                1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 ,1 ,1 ,1 ,1 , 0 ,0, 0,
                                 0 ,0 ,0 , 0 , 0, 0, 0, 0 ,0,
                                 0 , 0 , 0 , 0 ,0 ,0, 0 ,0 ,0,
                                 0, 0 ,0 ,0 ,0 ,0 ,0 , 0 ,0 ,
                                 0, 0, 0, 0 , 0, 0, 0, 0 ,0,
                                 0, 0, 0 ,0, 0, 0 ,0 ,0 , 0};

    // Normal open spot
    private List<int> normal = new List<int>{
                                1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 0 ,0 , 0 ,0,0 ,0 , 0 ,0, 0,
                                 0 ,0 ,0 , 0 , 0, 0, 0, 0 ,0,
                                 0 , 0 , 0 , 0 ,0 ,0, 0 ,0 ,0,
                                 0, 0 ,0 ,0 ,0 ,0 ,0 , 0 ,0 ,
                                 0, 0, 0, 0 , 0, 0, 0, 0 ,0,
                                 0, 0, 0 ,0, 0, 0 ,0 ,0 , 0};

    // Hard open spot
    private List<int> hard = new List<int>{
                                1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 , 1, 1, 1, 1, 1, 1, 1,
                                 1 , 1 , 1, 0, 0, 0, 0, 0, 0,
                                 0 ,0 , 0 ,0,0 ,0 , 0 ,0, 0,
                                 0 ,0 ,0 , 0 , 0, 0, 0, 0 ,0,
                                 0 , 0 , 0 , 0 ,0 ,0, 0 ,0 ,0,
                                 0, 0 ,0 ,0 ,0 ,0 ,0 , 0 ,0 ,
                                 0, 0, 0, 0 , 0, 0, 0, 0 ,0,
                                 0, 0, 0 ,0, 0, 0 ,0 ,0 , 0};

    void Awake()
    {      
         // Grab the text object and do such
        GameObject child = DifficultyButton.transform.GetChild(0).gameObject;
        string k = PlayerPrefs.GetString(key, "Easy");
        child.GetComponent<Text>().text = k;
        switch(k){
            case "Normal":
                app = normal;
                break;
            case "Hard":
                app = hard;
                break;
            default:
                app = easy;
                break;

        }
        var rng = new System.Random();
        arr = arr.OrderBy(x => rng.Next()).ToList();
        app = app.OrderBy(x => rng.Next()).ToList();
        arr.AddRange(zero_arr);

    }
    
    public bool SolveSudoku_v2(int[,] numScript_2D , int n){
        int row = -1;
        int col = -1;
        bool isEmpty = true;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (numScript_2D[i, j] == 0)
                {
                    row = i;
                    col = j;
 
                    // We still have some remaining
                    // missing values in Sudoku
                    isEmpty = false;
                    break;
                }
            }

            // If this false then just break for now 
            if (!isEmpty)
            {
                break;
            }

            
        }
 
        // no empty space left
        if (isEmpty)
        {
            final_data = numScript_2D;
            return true;
        }

        // else for each-row backtrack
        for (int num = 1; num <= n; num++)
        {
            if (isSafe(numScript_2D, row, col, num))
            {
                numScript_2D[row, col] = num;
                if (SolveSudoku_v2(numScript_2D, n)) return true;
                else numScript_2D[row, col] = 0;
            }
        }
        return false;
    }

   

    private static bool isSafe(int[,] numScript_2D, int row, int col, int num)
        {
        
            // Check if we find the same num
            // in the similar row or col , we
            // return false
            for (int x = 0; x <= 8; x++)
                if ((numScript_2D[row,x] == num) || (numScript_2D[x,col] == num))
                    return false;
            
            // Check if we find the same num
            // in the particular 3*3
            // matrix, we return false
            int startRow = row - row % 3, startCol = col - col % 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (numScript_2D[i + startRow,j + startCol] == num)
                        return false;
            
            return true;
        }

    // -------------------------------------------------------------
    // This is for the difficulty board man
    // ------------------------------------------------------------
    
    // Cancel it out
    public void Cancel(){
        this.GetComponent<ClickManager>().GlobalCanvas.SetActive(true);
        DifficultyBoard.SetActive(false);
    }

    public void Easy(){
        PlayerPrefs.SetString(key, "Easy");
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void Medium(){
        PlayerPrefs.SetString(key, "Normal");
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void Hard(){
        PlayerPrefs.SetString(key, "Hard");
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    // Turn the difficult board on
    public void Difficulty(){
        this.GetComponent<ClickManager>().GlobalCanvas.SetActive(false);
        DifficultyBoard.SetActive(true);
    }

    // -------------------------------------------------------------
    // This is for the setting board man
    // ------------------------------------------------------------

    // For the settings button duh
    public void Setting(){
        this.GetComponent<ClickManager>().GlobalCanvas.SetActive(false);
        SettingBoard.SetActive(true);
    }

    // Sound effect for this game , idk
    // On by default
    public void SoundOn(){
        S_on.SetActive(true);
        S_off.SetActive(false);
        AudioListener.volume = 0.25f;
    }

    public void SoundOff(){
        S_on.SetActive(false);
        S_off.SetActive(true);
        AudioListener.volume = 0.0f;
    }

    // Highlight grid for this game , idk
    // On by default
    public void HighLightGridOn(){
        HLD_on.SetActive(true);
        HLD_off.SetActive(false);
        this.GetComponent<GridManager>().highLightSameNumberGrid = true;
    }

    public void HighLightGridOff(){
        HLD_on.SetActive(false);
        HLD_off.SetActive(true);
        this.GetComponent<GridManager>().highLightSameNumberGrid = false;
    }
    //


    // Highlight the same number grid for this game , idk
    // On by default
    public void HighLightSameNumberGridOn(){
        HIN_on.SetActive(true);
        HIN_off.SetActive(false);
        this.GetComponent<GridManager>().highLightGrid = true;
    }

    public void HighLightSameNumberGridOff(){
        HIN_on.SetActive(false);
        HIN_off.SetActive(true);
        this.GetComponent<GridManager>().highLightGrid = false;
    }
    //

    // Probably not working , should have remove it
    public void AutoRemoveNotes(){

    }


    // Cancel it out
    public void SettingContinue(){
        this.GetComponent<ClickManager>().GlobalCanvas.SetActive(true);
        SettingBoard.SetActive(false);
    }
}

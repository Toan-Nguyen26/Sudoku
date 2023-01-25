using System;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Main code for this sudoku project , a little bit weird I must say

public class GridManager : MonoBehaviour {
    [SerializeField] public float width, height;

    // Small border tile ish
    [SerializeField] public GameObject tile;
    
    // Big border game object
    [SerializeField] public GameObject bigBorder;

    [SerializeField] public GameObject numTile;

    // ---------------------------------------------------
    // Hold the timer 
    public Text gameTimerText;

    // Check to see if the button on settingMenu for highlightGrid is on or off
    public bool highLightGrid = true;

    // Check to see if the button on settingMenu is on or off
    public bool highLightSameNumberGrid = true;

    // Check to see if the pencil is on or nah
    public bool pencilStatus = false;

    // Text appear for 5 seconds then go away after you win 
    public GameObject winText;
     
    // Where the block should start
    public Vector2 start = new Vector2(0.0f , 0.0f);

    // Where the button of number should start
    public Vector2 buttonNumber = new Vector2(0.0f , 0.0f);

    // Checking whenever the grid is empty and ready to be set for a different number
    public bool ready = false;

    // An empty gameObject to hold onto the empty object that is to change
    public GameObject emptyTile;

    // How much offset do we want
    public float sq_offset = 0.0f;

    // Hold the scale for the code
    public float square_scale = 1.0f;
    
    // A 2d array that hold whenever the value it coresspond to should be hidden or nah 
    public int[,] numScript_2D = new int[9 ,9];

    // position of the numScript_2d_x
    private int numScript_2D_x = 0;

    // === y 
    private int numScript_2D_y = 0;

    // A dict of grid 
    public Dictionary<GameObject, Tuple<int ,int , int >> d_grid = new Dictionary<GameObject, Tuple<int , int , int>>();

    // A stack to contain the GameObject stuff for the undo shit
    public Stack<GameObject> undoStack = new Stack<GameObject>();

    public Stack<string> textUndoStack = new Stack<string>();

    public Stack<bool> isBlue = new Stack<bool>();

    //--------------------------------------------------------------

    // A dict of number grid
    private Dictionary<GameObject, Tuple<int ,int , int >> d_button = new Dictionary<GameObject, Tuple<int , int , int>>();

    // A list contains 9 big border
    public List<GameObject> l_bigBorder = new List<GameObject>();

    // Datamanager data
    private DataManager data;

    // ClickManager clickData
    private ClickManager clickData;
    
    // Color for the clicked one
    private Color32 clickedColor = new Color32(61, 126,  243 , 255);

    // Color for the highlight around it
    private Color32 highlightColor = new Color32(128, 199,  245 , 255);

    private RectTransform square_rect;

    // The script that hold the value of all 81 numbers
    private int[] numScript = new int[81];

    // Hold the timer 
    private float gameTimer = 0f;
    
    
    public void Start() {
        clickData = GetComponent<ClickManager>();
        getSudoku();
        iniGrid();
        posGrid();
        winText.SetActive(false);
    }

    // Get the sudoku from dataManager , randomly
    // Will probably remove it later I think
    private void getSudoku(){

        // get the data from a list of pre generated sudoku
        int pos = 0;
        data = GetComponent<DataManager>();
        numScript = data.arr.ToArray();


        for(int row = 0 ; row < 9 ; row++){
            for(int col = 0; col < 9; col++){
                numScript_2D[row ,  col] = numScript[pos];
                pos++;
            }
        }

        if(data.SolveSudoku_v2(numScript_2D , 9)){
            numScript_2D = data.final_data;
        }else{
            print("No solution , error has occured somewhere");
        }
    }

    private void iniGrid(){
        // Get which numbers to appear and what's not
        List<int> appearance = data.app;
        int pos_appearance = 0;
        

        // Ini the sudoku main table second
        for(int row = 0 ; row <width ; row++ ){



            for(int col = 0 ; col < height ; col++){

                // Ini the gameObject as well as it's scale
                var spawnedTile = Instantiate(tile) as GameObject;
                spawnedTile.name = $"Tile {row} {col}";

                // Make sure the postion scale and such
                spawnedTile.transform.parent = this.transform;
                spawnedTile.transform.localScale = new Vector3(square_scale, square_scale ,square_scale);

                // getting the first rec for future reference
                square_rect = spawnedTile.GetComponent<RectTransform>();
                
                // Ini the number value that it had in the numScript
                int value = numScript_2D[numScript_2D_x , numScript_2D_y];
                spawnedTile.GetComponent<NumberManager>().SetNumbers(value , appearance[pos_appearance]);
                pos_appearance++;

                // If the value is 0 then we pass in a false flag , else it's a true
                d_grid.Add(spawnedTile , Tuple.Create(row, col, value ));

                // Incremeant the position of the 2d list and the filled_board one
                numScript_2D_y++;
                if(numScript_2D_y == 9){
                    numScript_2D_y = 0;
                    numScript_2D_x++;
                }
            }
        }
        

        // Ini the selection number grid later
        for(int i = 0; i < 9 ; i++){
            var spawnedTile = Instantiate(numTile) as GameObject;
            spawnedTile.name = $"Button {i+1}";
            spawnedTile.transform.parent = this.transform;
            spawnedTile.transform.localScale = new Vector3(square_scale, square_scale ,square_scale);
            spawnedTile.GetComponent<NumberManager>().SetGrid(i+1 , clickedColor);
            var numbers = Tuple.Create(0, i + 1, i + 1);
            d_button.Add(spawnedTile , numbers);
            Destroy(spawnedTile.transform.Find("Border"));
        }

        // Ini the big grid first
        // -----------------
        for(int i = 0; i < 9; i++){
            GameObject big = Instantiate(bigBorder) as GameObject;
            big.name = $"Big Boy {i+1}";
            big.transform.parent = this.transform;
            big.transform.localScale = new Vector3(square_scale, square_scale ,square_scale);
            l_bigBorder.Add(big);
        }
    }


    private void posGrid(){

        Vector2 offset = new Vector2();
        offset.x = square_rect.rect.width * square_rect.transform.localScale.x + sq_offset;
        offset.y = square_rect.rect.height * square_rect.transform.localScale.y + sq_offset;
        
        List<Vector3> v = new List<Vector3>();
        int col = 0;
        int row = 0;

        // Iterate through the list using num_border
        int num_border = 0;
        // Set the position for the sudoku grid
        foreach (KeyValuePair<GameObject, Tuple<int ,int , int>> square in d_grid){
            if(col + 1 > height){
                row++; 
                col = 0;
            }

            var x_offset = offset.x * col;
            var y_offset = offset.y * row;

            Vector3 pos = new Vector3(start.x + x_offset , start.y - y_offset);
            square.Key.GetComponent<RectTransform>().anchoredPosition = pos;
            
            // for every col that % 3 will be having the big grid on it that do all the stuff
            if(row % 3 == 1 && col % 3 == 1 ){
                v.Add(pos);
            }

            col++; 
        }
        
        // for every col that % 3 will be having the big grid on it that do all the stuff
        for(int i = 0 ; i < 9 ; i++){
            l_bigBorder[i].GetComponent<RectTransform>().anchoredPosition = v[i];
        }
        // For the button at the bottom
        col = 0;
        row = 0;
        
        // Set the position for the number button grid
        foreach (KeyValuePair<GameObject, Tuple<int ,int , int>> square in d_button){
            var x_offset = offset.x * col;
            var y_offset = offset.y * row;
            Vector3 pos = new Vector3(buttonNumber.x + x_offset , buttonNumber.y - y_offset);
            square.Key.GetComponent<RectTransform>().anchoredPosition = pos;
            square.Key.tag = "Number";

            col++; 
        }
    }

    // This code will highlight the click that the user has click on 
    public void highlightNumGrid(int number_){

        foreach(KeyValuePair<GameObject, Tuple<int ,int , int>> square in d_grid)
        {      
            if((square.Key.transform.Find("Text").GetComponent<Text>().text == number_.ToString()) && highLightGrid){
                square.Key.transform.Find("Image").GetComponent<Image>().color = clickedColor;
            }else{
                square.Key.transform.Find("Image").GetComponent<Image>().color = Color.white; 
            }
            
        }
    }
    
    // This code will highlight all rows and cols of the empty grid that being clicked on 
    public void highlightEmptyGrid(GameObject tile){
        Tuple<int , int ,int> x;
        d_grid.TryGetValue(tile, out x);
        int x_pos = x.Item1;
        int y_pos = x.Item2;
        int startRow = x_pos - x_pos % 3, startCol = y_pos - y_pos % 3;
        foreach(KeyValuePair<GameObject, Tuple<int ,int , int >> square in d_grid)
        {   
            // Turn all the col and rows into green
            bool in_x = (square.Value.Item1 <= (startRow + 2)) && (square.Value.Item1 >= startRow);
            bool in_y = (square.Value.Item2 <= (startCol + 2)) && (square.Value.Item2 >= startCol);
            bool equal_x = square.Value.Item1 == x_pos;
            bool equal_y = square.Value.Item2 == y_pos;
            if((equal_x || equal_y || (in_x && in_y)) && highLightSameNumberGrid){
                square.Key.transform.Find("Image").GetComponent<Image>().color = highlightColor;
            }
            // Neither than it has to be white 
            else{
                square.Key.transform.Find("Image").GetComponent<Image>().color = Color.white;
            }
 
            
        }

        tile.transform.Find("Image").GetComponent<Image>().color = clickedColor;
    }


    // This code will fill up the empty grid with the button that the user click
    public void fillEmptyGrid(GameObject button , GameObject empty){
        // First check whenever the pencil is on or not , if it's on then we fill the small thingy , else we just proceed as usual
        if(pencilStatus){
            fillSmallEmptyGrid(button , empty);
        }else{
            fillDefaultEmptyGrid(button ,empty);
        }
    }

    public void fillDefaultEmptyGrid(GameObject button , GameObject empty){
        // Empty the small one just in case()
        for (int i = 0; i < 9; i++)
        {
            empty.transform.Find("SmallNum").GetChild(i).gameObject.SetActive(false);
        }


        // Second grab the value of the corresponding empty tile
        Tuple<int , int ,int> x;
        d_grid.TryGetValue(empty, out x);

        // Grab the text of the button
        string button_txt = button.transform.Find("Text").GetComponent<Text>().text;

        //Grab the text of the empty tiles
        Text emptyText = empty.transform.Find("Text").GetComponent<Text>();

        // Fill in the value
        empty.transform.Find("Text").GetComponent<Text>().text = button_txt;

        // Have to check whenever the text the user put in is match with or not
        // Color red meaning that it's wrong , blue is otherwise
        string v = x.Item3.ToString();
        if(button_txt.Equals(v)){
            empty.transform.Find("Text").GetComponent<Text>().color = Color.blue;
            isBlue.Push(true);
        }else{
            empty.transform.Find("Text").GetComponent<Text>().color = Color.red;
            isBlue.Push(false);
        }

        // Add to the stack so that we have something to undo later on
        addStack(empty);
        
    }

    public void fillSmallEmptyGrid(GameObject button , GameObject empty){

        // Empty just in case
        empty.transform.Find("Text").GetComponent<Text>().text = "";    

        // Grab the text of the button
        string button_txt = button.transform.Find("Text").GetComponent<Text>().text;

        // Iterate through child objects to find the correct number to assign to
        for (int i = 0; i < 9; i++)
        {
            if(empty.transform.Find("SmallNum").GetChild(i).GetComponent<Text>().text.Equals(button_txt)){
                empty.transform.Find("SmallNum").GetChild(i).gameObject.SetActive(true);
                
                return;
            }
        }
    }



    // constanlty update the game to check whenever the game is win or not
    void Update(){
        // TODO: check whenever all 81 grid has been filled out , if yes then show the winning screen or smt
        int total = 0;
        foreach(KeyValuePair<GameObject, Tuple<int ,int , int>> square in d_grid){
            string grid_txt = square.Key.transform.Find("Text").GetComponent<Text>().text;
            if(grid_txt != ""){
                int grid_int = Convert.ToInt32(grid_txt);
                int value_int = square.Value.Item3;
                // print(grid_txt);
                // print(value_int);
                if(grid_int == value_int){
                    total++;
                }
            }
        }
        
        // If it's 81 then do this irl ( show for 5 seconds then disappear)
        if(total == 81){
            winText.SetActive(true);
            return;
        }

        // Check for the timer and update it every seconds , once it reach 60 seconds then it's a minute ans so on 
        gameTimer += Time.deltaTime;

        int seconds = (int)(gameTimer % 60);
        int minutes = (int)(gameTimer / 60) % 60;
        gameTimerText.text = string.Format("{0:00}:{1:00}",minutes ,seconds );


    }

    // Add the gameObjetc to the stack that we have
    private void addStack(GameObject empty){
        undoStack.Push(empty);
        textUndoStack.Push(empty.transform.Find("Text").GetComponent<Text>().text);
        print(undoStack.Count);
    }

    // Pop it out
    public void popUndoStack(){

        if(undoStack.Count > 0){
            if(undoStack.Count == 1){
                GameObject undoing = undoStack.Peek();
                undoing.transform.Find("Text").GetComponent<Text>().text = "";
            }
            undoStack.Pop();
            textUndoStack.Pop();
            isBlue.Pop();
        } 
    }
}
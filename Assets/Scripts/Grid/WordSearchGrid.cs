using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class WordSearchGrid : MonoBehaviour
{
    public int rows;
    public int columns;
    public int cellSizeX;
    public int cellSizeY;
    public int gridSpacing;
    public Image gridImage;
    public Transform gridBackGroundParent;
    //public TextMeshProUGUI[][] imageChar;
    public List<ValueWord> ValueWords;
    public GridPosition[][] gridPositions;
    public string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static WordSearchGrid instance;
    public TextMeshProUGUI wordsText;

    public List<ValueWord> foundWords;
    public Transform foundWordsParent;
    public GameObject overlayImage;

    public int numberOfWordsInSearch = 15;
    public List<ValueWord> wordsInSearch;
    public float sizeOffset;
    public Vector2 randomPos;

    public class GridPosition
    {
        public int x;
        public int y;
        public bool filled;
        public Image image;
        public bool word = false;
        public string s_word = null;
        public bool targeted = false;
        public TextMeshProUGUI text;
        public bool found = false;
        public ValueWord valueWord;
        public Vector3 position;
        public Color currentColor;

        public void AddColor(Color color)
        {
            GameObject go = Instantiate(WordSearchGrid.instance.overlayImage, position, Quaternion.identity);
            go.transform.SetParent(WordSearchGrid.instance.gridBackGroundParent);
            go.GetComponent<Image>().color = color;
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(WordSearchGrid.instance.cellSizeX * WordSearchGrid.instance.sizeOffset, WordSearchGrid.instance.cellSizeX * WordSearchGrid.instance.sizeOffset);
        }

        public void ResetSelf()
        {
            x = 0;
            y = 0;
            filled = false;
            word = false;
            s_word = null;
        }

        public GridPosition(int x, int y, bool filled, Image image)
        {
            this.x = x;
            this.y = y;
            this.filled = filled;
            this.image = image;
        }

        public void UpdateColor(Color color)
        {
            currentColor = color;
            currentColor.a = 1f;
            image.color = currentColor;
        }

        public void Update()
        {
            //if (word)
            //    image.color = Color.red;
        }
    }

    public enum InputDirection
    {
        Up,
        Down,
        Left,
        Right,
        //UL,
        //UR,
        //DR,
        //DL
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //imageChar = new TextMeshProUGUI[rows][];
        //for (int k = 0; k < rows; k++)
        //{
        //    imageChar[k] = new TextMeshProUGUI[columns];
        //}
        foundWords = new List<ValueWord>();
        gridPositions = new GridPosition[rows][];
        for (int l = 0; l < rows; l++)
        {
            gridPositions[l] = new GridPosition[columns];
        }

        for (int i = 0; i < numberOfWordsInSearch; i++)
        {
            ValueWord randomWord = ValueWords[Random.Range(0, ValueWords.Count)];
            if (!wordsInSearch.Contains(randomWord))
            {
                wordsInSearch.Add(randomWord);
            }
            else
            {
                i--;
            }
        }

        InitializeGrid();
        //PlaceWords();
        if (PlaceWords2())
        {
            Debug.Log("PLACED ALL WORDS");
        }
        else
        {
            Debug.Log("BIG NOPE");
        }
        PlaceLetters();

        //UpdateWordsText();

    }

    static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }

    bool PlaceWords2()
    {
        List<ValueWord> valueWordsPlaced = new List<ValueWord>();
        List<Vector4> valueWordPositionAndDirection = new List<Vector4>();

        while (valueWordsPlaced.Count < numberOfWordsInSearch)
        {
            ValueWord currentWord = ValueWords[Random.Range(0, ValueWords.Count)];
            int switchedInput = 0;
            if ((currentWord.word.Length > rows && currentWord.word.Length > columns) || valueWordsPlaced.Contains(currentWord))
            {
                continue;
            }

            InputDirection inputDir = GetRandomEnum<InputDirection>();

        beforeSwitch:

            if (switchedInput == 4)
            {
                if (valueWordsPlaced.Count == 0)
                    return false;
                Vector2 startPos = new Vector2(valueWordPositionAndDirection[valueWordPositionAndDirection.Count - 1].x, valueWordPositionAndDirection[valueWordPositionAndDirection.Count - 1].y);
                for (int i = 0; i < valueWordsPlaced[valueWordsPlaced.Count - 1].word.Length; i++)
                {
                    gridPositions[(int)startPos.x][(int)startPos.y].filled = false;
                    gridPositions[(int)startPos.x][(int)startPos.y].word = false;
                    gridPositions[(int)startPos.x][(int)startPos.y].s_word = null;
                    startPos.x += valueWordPositionAndDirection[valueWordPositionAndDirection.Count - 1].z;
                    startPos.y += valueWordPositionAndDirection[valueWordPositionAndDirection.Count - 1].w;
                }
                valueWordsPlaced.RemoveAt(valueWordsPlaced.Count - 1);
                valueWordPositionAndDirection.RemoveAt(valueWordPositionAndDirection.Count - 1);
                Debug.Log("Removed Word");
                continue;
            }
            switch (inputDir)
            {
                case InputDirection.Up:
                    {
                        int counter = 0;
                        while (!PlaceWordUpwards(currentWord.word.ToString()))
                        {


                            counter++;
                            if (counter > 500)
                            {
                                inputDir = InputDirection.Down;
                                switchedInput++;
                                goto beforeSwitch;
                            }
                        }

                        //if can place
                        valueWordsPlaced.Add(currentWord);
                        valueWordPositionAndDirection.Add(new Vector4(randomPos.x, randomPos.y, 1, 0));
                    }
                    break;
                case InputDirection.Down:
                    {
                        int counter = 0;
                        while (!PlaceWordDown(currentWord.word.ToString()))
                        {


                            counter++;
                            if (counter > 500)
                            {
                                inputDir = InputDirection.Left;
                                switchedInput++;
                                goto beforeSwitch;
                            }
                        }

                        //if can place
                        valueWordsPlaced.Add(currentWord);
                        valueWordPositionAndDirection.Add(new Vector4(randomPos.x, randomPos.y, -1, 0));
                    }
                    break;
                case InputDirection.Left:
                    {
                        int counter = 0;
                        while (!PlaceWordLeft(currentWord.word.ToString()))
                        {


                            counter++;
                            if (counter > 500)
                            {
                                inputDir = InputDirection.Right;
                                switchedInput++;
                                goto beforeSwitch;
                            }
                        }

                        //if can place
                        valueWordsPlaced.Add(currentWord);
                        valueWordPositionAndDirection.Add(new Vector4(randomPos.x, randomPos.y, 0, -1));

                    }
                    break;
                case InputDirection.Right:
                    {
                        int counter = 0;
                        while (!PlaceWordRight(currentWord.word.ToString()))
                        {


                            counter++;
                            if (counter > 500)
                            {
                                inputDir = InputDirection.Up;
                                switchedInput++;
                                goto beforeSwitch;
                            }
                        }

                        //if can place
                        valueWordsPlaced.Add(currentWord);
                        valueWordPositionAndDirection.Add(new Vector4(randomPos.x, randomPos.y, 0, 1));
                    }
                    break;

            }
        }

        foreach(ValueWord vw in valueWordsPlaced)
        {
            wordsText.text += vw.word + ", ";
        }
        wordsInSearch = valueWordsPlaced;
        return true;
    }

    void PlaceWords()
    {
        ValueWords = SortByLength(ValueWords);
        ValueWords.Reverse();
        int counter = 0;
        foreach (ValueWord vw in wordsInSearch)
        {
            if (vw.word.Length > rows && vw.word.Length > columns)
                continue;
            InputDirection inputDir = GetRandomEnum<InputDirection>();
            bool placed = false;
            int cantPlace = 0;

            while (!placed)
            {
                switch (inputDir)
                {
                    case InputDirection.Up:
                        {
                            bool canPlace = true;
                            while (!PlaceWordUpwards(vw.word.ToString()))
                            {
                                counter++;
                                if (counter > 100)
                                {
                                    Debug.Log("Had to break out");
                                    Vector2 pos = GetRandomBound();
                                    if (!PlaceWordUpwards(vw.word.ToString(), (int)pos.x, (int)pos.y))
                                    {
                                        counter++;
                                        if (counter > 1000)
                                        {
                                            inputDir = GetRandomEnum<InputDirection>();
                                            counter = 0;
                                            canPlace = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        placed = true;
                                    }
                                }
                            }

                            if (!canPlace)
                            {
                                cantPlace++;
                            }
                            else
                            {
                                placed = true;
                                counter = 0;
                            }
                        }
                        break;
                    case InputDirection.Down:
                        {
                            bool canPlace = true;
                            while (!PlaceWordDown(vw.word.ToString()))
                            {
                                counter++;
                                if (counter > 100)
                                {
                                    Debug.Log("Had to break out");
                                    Vector2 pos = GetRandomBound();
                                    if (!PlaceWordDown(vw.word.ToString(), (int)pos.x, (int)pos.y))
                                    {
                                        counter++;
                                        if (counter > 1000)
                                        {
                                            inputDir = GetRandomEnum<InputDirection>();
                                            counter = 0;
                                            canPlace = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        placed = true;
                                    }
                                }
                            }
                            if (!canPlace)
                            {
                                cantPlace++;
                            }
                            else
                            {
                                placed = true;
                                counter = 0;
                            }
                        }
                        break;
                    case InputDirection.Left:
                        {
                            bool canPlace = true;
                            while (!PlaceWordLeft(vw.word.ToString()))
                            {
                                counter++;
                                if (counter > 100)
                                {
                                    Debug.Log("Had to break out");
                                    Vector2 pos = GetRandomBound();
                                    if (!PlaceWordLeft(vw.word.ToString(), (int)pos.x, (int)pos.y))
                                    {
                                        counter++;
                                        if (counter > 1000)
                                        {
                                            inputDir = GetRandomEnum<InputDirection>();
                                            counter = 0;
                                            canPlace = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        placed = true;
                                    }
                                }
                            }
                            if (!canPlace)
                            {
                                cantPlace++;
                            }
                            else
                            {
                                placed = true;
                                counter = 0;
                            }
                        }
                        break;
                    case InputDirection.Right:
                        {
                            bool canPlace = true;
                            while (!PlaceWordRight(vw.word.ToString()))
                            {
                                counter++;
                                if (counter > 100)
                                {
                                    Debug.Log("Had to break out");
                                    Vector2 pos = GetRandomBound();
                                    if (!PlaceWordRight(vw.word.ToString(), (int)pos.x, (int)pos.y))
                                    {
                                        counter++;
                                        if (counter > 1000)
                                        {
                                            inputDir = GetRandomEnum<InputDirection>();
                                            counter = 0;
                                            canPlace = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        placed = true;
                                    }
                                }
                            }
                            if (!canPlace)
                            {
                                cantPlace++;
                            }
                            else
                            {
                                placed = true;
                                counter = 0;
                            }
                        }
                        break;

                }
            }
            if (cantPlace >= 4)
                continue;
            wordsText.text += vw.word + ", ";
        }
    }

    Vector2 GetRandomBound()
    {
        int x = 0;
        int y = 0;
        //top
        x = rows - 1;
        y = Random.Range(0, columns - 1);
        //bottom
        x = 0;
        y = Random.Range(0, columns - 1);
        //left
        x = Random.Range(0, rows - 1);
        y = 0;
        //right
        x = Random.Range(0, rows - 1);
        y = columns - 1;

        return new Vector2(x, y);
    }

    void PlaceLetters()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (gridPositions[i][j].filled == false)
                {
                    gridPositions[i][j].text.text = alpha[Random.Range(0, alpha.Length)].ToString();
                    gridPositions[i][j].filled = true;
                }
            }
        }
    }

    bool PlaceWordUpwards(string word, int xAxis, int yAxis)
    {
        int count = 0;
        int x = xAxis;
        int y = yAxis;

        //check it can fit
        if (x + word.Length - 1 > rows - 1)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (x + i <= rows - 1)
            {
                if (gridPositions[x + i][y].filled)
                {
                    if (gridPositions[x + i][y].text.text == word[i].ToString())
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        for (int m = i; m >= 0; m--)
                        {
                            if (gridPositions[x + m][y].s_word == word)
                            {
                                gridPositions[x + m][y].filled = false;
                                gridPositions[x + m][y].word = false;
                                gridPositions[x + m][y].s_word = null;
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    string currentS = word[i].ToString();
                    gridPositions[x + i][y].text.text = word[i].ToString();
                    gridPositions[x + i][y].filled = true;
                    gridPositions[x + i][y].word = true;
                    gridPositions[x + i][y].s_word = word;
                    count++;
                }
            }
        }

        return (count == word.Length) ? true : false;
    }

    bool PlaceWordUpwards(string word)
    {
        int count = 0;
        int x = Random.Range(0, rows);
        int y = Random.Range(0, columns);
        randomPos = new Vector2(x, y);

        //check it can fit
        if (x + word.Length - 1 > rows - 1)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (x + i <= rows - 1)
            {
                if (gridPositions[x + i][y].filled)
                {
                    if (gridPositions[x + i][y].text.text == word[i].ToString())
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        for (int m = i; m >= 0; m--)
                        {
                            if (gridPositions[x + m][y].s_word == word)
                            {
                                gridPositions[x + m][y].filled = false;
                                gridPositions[x + m][y].word = false;
                                gridPositions[x + m][y].s_word = null;
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    string currentS = word[i].ToString();
                    gridPositions[x + i][y].text.text = word[i].ToString();
                    gridPositions[x + i][y].filled = true;
                    gridPositions[x + i][y].word = true;
                    gridPositions[x + i][y].s_word = word;
                    count++;
                }
            }
        }

        return (count == word.Length) ? true : false;
    }

    bool PlaceWordDown(string word, int xAxis, int yAxis)
    {
        int count = 0;
        int x = xAxis;
        int y = yAxis;

        //check it can fit
        if (x - word.Length - 1 < 0)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (x - i >= 0)
            {
                if (gridPositions[x - i][y].filled)
                {
                    if (gridPositions[x - i][y].text.text == word[i].ToString())
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        for (int m = i; m >= 0; m--)
                        {
                            if (gridPositions[x - m][y].s_word == word)
                            {
                                gridPositions[x - m][y].filled = false;
                                gridPositions[x - m][y].word = false;
                                gridPositions[x - m][y].s_word = null;
                            }
                        }
                        return false;
                    }
                }
                else
                {


                    string currentS = word[i].ToString();
                    gridPositions[x - i][y].text.text = word[i].ToString();
                    gridPositions[x - i][y].filled = true;
                    gridPositions[x - i][y].word = true;
                    gridPositions[x - i][y].s_word = word;
                    count++;
                }
            }
        }

        return (count == word.Length) ? true : false;
    }
    bool PlaceWordDown(string word)
    {
        int count = 0;
        int x = Random.Range(0, rows);
        int y = Random.Range(0, columns);
        randomPos = new Vector2(x, y);

        //check it can fit
        if (x - word.Length - 1 < 0)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (x - i >= 0)
            {
                if (gridPositions[x - i][y].filled)
                {
                    if (gridPositions[x - i][y].text.text == word[i].ToString())
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        for (int m = i; m >= 0; m--)
                        {
                            if (gridPositions[x - m][y].s_word == word)
                            {
                                gridPositions[x - m][y].filled = false;
                                gridPositions[x - m][y].word = false;
                                gridPositions[x - m][y].s_word = null;
                            }
                        }
                        return false;
                    }
                }
                else
                {


                    string currentS = word[i].ToString();
                    gridPositions[x - i][y].text.text = word[i].ToString();
                    gridPositions[x - i][y].filled = true;
                    gridPositions[x - i][y].word = true;
                    gridPositions[x - i][y].s_word = word;
                    count++;
                }
            }
        }

        return (count == word.Length) ? true : false;
    }

    bool PlaceWordRight(string word, int xAxis, int yAxis)
    {
        int count = 0;
        int x = xAxis;
        int y = yAxis;

        //check it can fit
        if (y + word.Length - 1 > columns - 1)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (y + i <= columns - 1)
            {
                if (gridPositions[x][y + i].filled)
                {
                    if (gridPositions[x][y + i].text.text == word[i].ToString())
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        for (int m = i; m >= 0; m--)
                        {
                            if (gridPositions[x][y + m].s_word == word)
                            {
                                gridPositions[x][y + m].filled = false;
                                gridPositions[x][y + m].word = false;
                                gridPositions[x][y + m].s_word = null;
                            }
                        }
                        return false;
                    }
                }
                else
                {

                    string currentS = word[i].ToString();
                    gridPositions[x][y + i].text.text = word[i].ToString();
                    gridPositions[x][y + i].filled = true;
                    gridPositions[x][y + i].word = true;
                    gridPositions[x][y + i].s_word = word;
                    count++;
                }
            }
        }

        return (count == word.Length) ? true : false;
    }
    bool PlaceWordRight(string word)
    {
        int count = 0;
        int x = Random.Range(0, rows);
        int y = Random.Range(0, columns);
        randomPos = new Vector2(x, y);

        //check it can fit
        if (y + word.Length - 1 > columns - 1)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (y + i <= columns - 1)
            {
                if (gridPositions[x][y + i].filled)
                {
                    if (gridPositions[x][y + i].text.text == word[i].ToString())
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        for (int m = i; m >= 0; m--)
                        {
                            if (gridPositions[x][y + m].s_word == word)
                            {
                                gridPositions[x][y + m].filled = false;
                                gridPositions[x][y + m].word = false;
                                gridPositions[x][y + m].s_word = null;
                            }
                        }
                        return false;
                    }
                }
                else
                {

                    string currentS = word[i].ToString();
                    gridPositions[x][y + i].text.text = word[i].ToString();
                    gridPositions[x][y + i].filled = true;
                    gridPositions[x][y + i].word = true;
                    gridPositions[x][y + i].s_word = word;
                    count++;
                }
            }
        }

        return (count == word.Length) ? true : false;
    }
    bool PlaceWordLeft(string word, int xAxis, int yAxis)
    {
        int count = 0;
        int x = xAxis;
        int y = yAxis;

        //check it can fit
        if (y - word.Length - 1 < 0)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (y - i >= 0)
            {
                if (gridPositions[x][y - i].filled)
                {
                    if (gridPositions[x][y - i].text.text == word[i].ToString())
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        for (int m = i; m >= 0; m--)
                        {
                            if (gridPositions[x][y - m].s_word == word)
                            {
                                gridPositions[x][y - m].filled = false;
                                gridPositions[x][y - m].word = false;
                                gridPositions[x][y - m].s_word = null;
                            }
                        }
                        return false;
                    }
                }
                else
                {

                    string currentS = word[i].ToString();
                    gridPositions[x][y - i].text.text = word[i].ToString();
                    gridPositions[x][y - i].filled = true;
                    gridPositions[x][y - i].word = true;
                    gridPositions[x][y - i].s_word = word;
                    count++;
                }
            }
        }

        return (count == word.Length) ? true : false;
    }
    bool PlaceWordLeft(string word)
    {
        int count = 0;
        int x = Random.Range(0, rows);
        int y = Random.Range(0, columns);
        randomPos = new Vector2(x, y);

        //check it can fit
        if (y - word.Length - 1 < 0)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            if (y - i >= 0)
            {
                if (gridPositions[x][y - i].filled)
                {
                    if (gridPositions[x][y - i].text.text == word[i].ToString())
                    {
                        count++;
                        continue;
                    }
                    else
                    {
                        for (int m = i; m >= 0; m--)
                        {
                            if (gridPositions[x][y - m].s_word == word)
                            {
                                gridPositions[x][y - m].filled = false;
                                gridPositions[x][y - m].word = false;
                                gridPositions[x][y - m].s_word = null;
                            }
                        }
                        return false;
                    }
                }
                else
                {

                    string currentS = word[i].ToString();
                    gridPositions[x][y - i].text.text = word[i].ToString();
                    gridPositions[x][y - i].filled = true;
                    gridPositions[x][y - i].word = true;
                    gridPositions[x][y - i].s_word = word;
                    count++;
                }
            }
        }

        return (count == word.Length) ? true : false;
    }

    // [2,1][2,2][2,3]
    // [1,1][1,2][1,3]
    // [0,1][0,2][0,3]
    void InitializeGrid()
    {
        if (columns > 0 && rows > 0)
        {
            int cellsCount = columns * rows;
            Vector3 position = gameObject.GetComponent<RectTransform>().position;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Vector3 drawPos = position + new Vector3(cellSizeX * j, cellSizeX * i, 0);
                    Image go = Instantiate(gridImage, drawPos, Quaternion.identity/*, gridBackGroundParent*/);
                    go.transform.SetParent(gridBackGroundParent, true);
                    //go.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeX, cellSizeY);
                    go.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeX * sizeOffset, cellSizeX * sizeOffset);
                    TextMeshProUGUI tmp = go.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                    gridPositions[i][j] = new GridPosition(i, j, false, go);
                    gridPositions[i][j].text = tmp;
                    gridPositions[i][j].text.text = i.ToString() + ", " + j.ToString();
                    gridPositions[i][j].position = drawPos;
                    gridPositions[i][j].text.fontSize = cellSizeX / 1.5f;
                }
            }
        }
    }
    //45 60
    //37 50

    public int GridDistance(GridPosition leftSide, GridPosition rightSide)
    {
        if (leftSide == null || rightSide == null)
            return 0;
        int y = Mathf.Abs(leftSide.y - rightSide.y);
        int x = Mathf.Abs(leftSide.x - rightSide.x);
        return y + x;
    }

    public Vector2 FindGridPiece(Image img)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (img == gridPositions[i][j].image)
                {
                    return new Vector2(i, j);
                }
            }
        }

        return Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            ResetGrid();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                gridPositions[i][j].Update();
            }
        }

    }


    public void CreateGrid()
    {

    }

    public void ResetGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                gridPositions[i][j].ResetSelf();
                gridPositions[i][j].text.text = "0/0";
            }
        }

        PlaceWords();
        PlaceLetters();
    }

    List<ValueWord> SortByLength(List<ValueWord> valueWords)
    {
        var sortedList = valueWords.OrderBy(go => go.word.Length).ToList();
        return sortedList;
    }

    public ValueWord FindWord(string word)
    {
        foreach (ValueWord vw in ValueWords)
        {
            if (vw.word == word)
            {
                if (!foundWords.Contains(vw))
                    foundWords.Add(vw);
                return vw;
            }
        }
        return null;
    }

    public void UpdateWordsText()
    {
        wordsText.text = "";
        foreach (ValueWord vw in wordsInSearch)
        {
            if (foundWords.Contains(vw))
            {
                wordsText.text += "<s>" + vw.word + "</s>" + ", ";
            }
            else
                wordsText.text += vw.word + ", ";
        }
    }
}

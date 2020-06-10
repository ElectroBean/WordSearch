using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class WorldSpaceInputManager : MonoBehaviour
{
    bool holdingInput = false;
    bool pressed = false;
    public string selected = "";
    public Camera cam;
    public static WorldSpaceInputManager instance;

    public List<WordSearchGrid1.GridPosition> gridpositions;
    public WordSearchGrid1.GridPosition lastSelected;
    public WordSearchGrid1.GridPosition currentSelected;
    public Vector2 direction;

    public Color currentColor;
    public Color[] colors;
    int colorIndex;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        gridpositions = new List<WordSearchGrid1.GridPosition>();
        colorIndex = UnityEngine.Random.Range(0, colors.Length);
        currentColor = colors[colorIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touches.Any(x => x.phase == TouchPhase.Began))
        {
            pressed = true;
            NextColor();

            //raycast
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
            {

                if (hit.collider.gameObject.tag == "GridPiece")
                {
                    Vector2 index = WordSearchGrid1.instance.FindGridPiece(hit.collider.gameObject.GetComponent<SpriteRenderer>());
                    WordSearchGrid1.GridPosition gp = WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y];
                    InfoPanelManager.UpdatePanel(gp.valueWord);
                }
            }

        }

        if (Input.GetMouseButtonUp(0) || Input.touches.Any(x => x.phase == TouchPhase.Ended))
        {
            pressed = false;
            currentSelected = null;
            //check if there's a word

            if (SearchForWord())
            {

            }
            else
            {
                ////reverse string
                //char[] charArray = selected.ToCharArray();
                //System.Array.Reverse(charArray);
                //selected = new string(charArray);
                //SearchForWord();
            }

            selected = "";
            foreach (WordSearchGrid1.GridPosition gp in gridpositions)
            {
                gp.targeted = false;
                gp.UpdateColor(Color.white);
            }

            gridpositions.RemoveRange(0, gridpositions.Count);
        }

        if (pressed)
        {
            if (true)
            {
                holdingInput = true;

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
                {

                    //if current is a grid object
                    if (hit.collider.gameObject.tag == "GridPiece")
                    {
                        //find index of grid position
                        Vector2 index = WordSearchGrid1.instance.FindGridPiece(hit.collider.gameObject.GetComponent<SpriteRenderer>());
                        //set gridposition to current index
                        WordSearchGrid1.GridPosition gp = WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y];

                        //if the current gridposition isnt in current positions tracker
                        if (!gridpositions.Contains(WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y]))
                        {
                            //get distance from current to last
                            if (WordSearchGrid1.instance.GridDistance(currentSelected, gp) <= 1)
                            {
                                //if we have 1 currently selected 
                                if (selected.Length == 1)
                                {
                                    //set direction you can select
                                    direction = new Vector2((gp.x - currentSelected.x), (gp.y - currentSelected.y));
                                }
                                //if more than 1 selected
                                else if (selected.Length > 1)
                                {
                                    //get direction from current to last
                                    Vector2 newDir = new Vector2((gp.x - currentSelected.x), (gp.y - currentSelected.y));
                                    //if our current direction doesnt match stored direction of first 2
                                    if (newDir != direction)
                                    {
                                        //return, as we aren't trying to pick something in the same direction as before
                                        return;
                                    }
                                }

                                //set gridposition as targeted
                                WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y].targeted = true;
                                WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y].UpdateColor(currentColor);
                                //add the gridpositions letter to our selected string
                                selected += WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y].text.text;

                                Debug.Log("Hold mouse");
                                //add the current gridposition to our stored positions list
                                gridpositions.Add(WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y]);
                                //set the current grid position to our currently selected
                                currentSelected = gp;
                            }
                            //if they aren't in range, dont do anything
                            else
                            {
                                Debug.Log("NOT NEXT TO EACH OTHER");
                                Vector2 newDir = new Vector2((gp.x - currentSelected.x), (gp.y - currentSelected.y));
                                // see if this one is in the same direction
                                if (newDir.normalized == direction.normalized)
                                {
                                    //if it is
                                    //loop through grid positions from current selected
                                    for (int i = 1; i < newDir.magnitude + 1; i++)
                                    {
                                        //set each of the grid positions to used and whatever
                                        Vector2 cIndex = new Vector2(currentSelected.x, currentSelected.y);
                                        cIndex = cIndex + direction.normalized * i;

                                        //set gridposition as targeted
                                        WordSearchGrid1.instance.gridPositions[(int)cIndex.x][(int)cIndex.y].targeted = true;
                                        WordSearchGrid1.instance.gridPositions[(int)cIndex.x][(int)cIndex.y].UpdateColor(currentColor);
                                        //add the gridpositions letter to our selected string
                                        selected += WordSearchGrid1.instance.gridPositions[(int)cIndex.x][(int)cIndex.y].text.text;

                                        Debug.Log("Hold mouse");
                                        //add the current gridposition to our stored positions list
                                        gridpositions.Add(WordSearchGrid1.instance.gridPositions[(int)cIndex.x][(int)cIndex.y]);
                                    }

                                    //set the current grid position to our currently selected
                                    currentSelected = gp;
                                }
                            }
                        }
                        //if the grid pos. is currently in our stored positions (it's been selected)
                        else
                        {
                            //if our current equals our last
                            if (currentSelected != gp)
                            {
                                //if further than 1 cell away, dont do anything
                                if (WordSearchGrid1.instance.GridDistance(currentSelected, gp) > 1)
                                {
                                    return;
                                }
                                //remove the current one from our stored positions and set it to untargeted


                                gridpositions[gridpositions.Count - 1].targeted = false;
                                gridpositions[gridpositions.Count - 1].UpdateColor(Color.white);
                                //remove the end letter of our stored string (since we're taking the last grid position off) - doing this because i cant -= a letter from a string
                                selected = selected.Substring(0, selected.Length - 1);
                                gridpositions.RemoveAt(gridpositions.Count - 1);
                                currentSelected = gp;
                            }
                        }
                    }
                }
            }
        }
    }

    void AddLetter(Vector2 index)
    {
        //set gridposition as targeted
        WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y].targeted = true;
        WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y].UpdateColor(currentColor);
        //add the gridpositions letter to our selected string
        selected += WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y].text.text;

        Debug.Log("Hold mouse");
        //add the current gridposition to our stored positions list
        gridpositions.Add(WordSearchGrid1.instance.gridPositions[(int)index.x][(int)index.y]);
    }

    void NextColor()
    {
        colorIndex++;
        if (colorIndex > colors.Length - 1)
            colorIndex = 0;
        currentColor = colors[colorIndex];
    }

    public bool SearchForWord()
    {
        ValueWord vw = WordSearchGrid1.instance.FindWord(selected);
        if (vw != null)
        {
            //found a word
            //set tiles to found
            //set tiles to have valueword
            foreach (WordSearchGrid1.GridPosition gp in gridpositions)
            {
                gp.found = true;
                gp.valueWord = vw;
                gp.AddColor(currentColor);
            }

            WordSearchGrid1.instance.UpdateWordsText();
            return true;
        }
        else return false;
    }
}

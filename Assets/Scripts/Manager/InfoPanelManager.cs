using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class InfoPanelManager : MonoBehaviour
{
    public static InfoPanelManager instance;

    public TextMeshProUGUI valueWord;
    public TextMeshProUGUI wordDescription;
    public Image wordImage;

    ValueWord currentValueWord;

    public string defaultName;
    public string defaultDesc;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        UpdatePanel(null);
        Debug.Log("Width: " + Screen.width + ", Height: " + Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void UpdatePanel(ValueWord vWord)
    {
        if (instance == null)
            return;
        if (vWord == null)
        {
            instance.valueWord.text = instance.defaultName;
            instance.wordDescription.text = instance.defaultDesc;
            return;
        }
        instance.valueWord.text = vWord.word;
        instance.wordDescription.text = vWord.description;
        //instance.wordImage.sprite = vWord.wordArtwork;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SwitchBackgroundSprite : MonoBehaviour
{
    public int pace = 2;
    public List<Sprite> backgroundSprites;
    public Image sourceImage;
    
    public void Start(){
        Switch(0);
    }

    public void PopulateSprites(List<Sprite> sourceSprite){
        backgroundSprites = new List<Sprite>(sourceSprite);
    }

    public void Switch(int index){
        sourceImage.sprite = backgroundSprites[index];
    }

}

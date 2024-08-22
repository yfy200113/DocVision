using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Loadingpics : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] buttons; // Array of buttons
    public string folderPath= "C:/Users/yfy/Desktop/师兄论文资料"; // Path to the folder containing images

    private Texture2D[] textures; // Array to store textures

    void Start()
    {
        LoadTexturesFromFolder();
        SetBackgroundImagesForButtons();
    }

    void LoadTexturesFromFolder()
    {
        Debug.Log("111");
        if (!Directory.Exists(folderPath))
        {
            //Debug.LogError("Folder does not exist: " + folderPath);
            //return;
        }
        Debug.Log("222");
        string[] imagePaths = Directory.GetFiles(@"C:\\Users\\yfy\\Desktop\\师兄论文资料", "*.png");
        Debug.Log("length::"+imagePaths.Length);
        textures = new Texture2D[imagePaths.Length];
        int number = 0;
        for (int i = 0; i < imagePaths.Length; i++)
        {
            number++;
            byte[] fileData = File.ReadAllBytes(imagePaths[i]);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // Autodetects format and loads the image data
            textures[i] = texture;
        }
        Debug.Log(number);
    }

    void SetBackgroundImagesForButtons()
    {
        if (buttons.Length != textures.Length)
        {
            Debug.LogError("Number of buttons does not match number of textures.");
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            SetButtonBackgroundImage(buttons[i], textures[i]);
        }
    }

    void SetButtonBackgroundImage(GameObject button, Texture2D texture)
    {
        if (button == null)
        {
            Debug.LogError("Button reference is null.");
            return;
        }

        Image image = button.GetComponentInChildren<Image>();
        if (image == null)
        {
            Debug.LogError("Image component not found in the button's children.");
            return;
        }

        // Convert the texture to a sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        // Set the sprite as the button's background image
        image.sprite = sprite;

        // Adjust button size based on the image's aspect ratio
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            float aspectRatio = (float)texture.width / texture.height;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y * aspectRatio, rectTransform.sizeDelta.y);
        }
    }
}

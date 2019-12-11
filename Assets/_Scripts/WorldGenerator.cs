using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    Color[,] colorOfPixel;
    public Texture2D outlineImage;

    public GameObject wall;
    public GameObject ground;
    public GameObject exit;
    public GameObject shop;
    
    private int[,] worldMap = new int[,]
    {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 3, 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
        { 1, 0, 1, 1, 1, 1, 1, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 1, 0, 1 },
        { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 0, 1, 1 },
        { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
        { 1, 0, 1, 1, 1, 1, 1, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 1, 0, 1 },
        { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 1, 0, 1 },
        { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 },
        { 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 1, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 1, 1, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 1, 0, 1 },
        { 1, 0, 1, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 2, 1, 1, 1, 1, 1, 1, 1, 1 },
    };

    // Start is called before the first frame update
    void Start()
    {
        GenerateFromArray();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void GenerateFromArray()
    {
        int i, j;
        for (i = 0; i < worldMap.Length / 10; i++)
        {
            for (j = 0; j < 10; j++)
            {
                if (worldMap[i, j] == 1)
                {
                    Instantiate(wall, new Vector3(50.0f - i * 10.0f, 1f, 50 - j * 10), Quaternion.identity, gameObject.transform);
                }
                else if (worldMap[i, j] == 0)
                {
                    Instantiate(ground, new Vector3(50.0f - i * 10.0f, -2f, 50 - j * 10), Quaternion.identity, gameObject.transform);
                }
                else if (worldMap[i, j] == 2)
                {
                    GameObject exitBlock = Instantiate(exit, new Vector3(50.0f - i * 10.0f, 1f, 50 - j * 10), Quaternion.identity, gameObject.transform);
                    exitBlock.GetComponent<Building>().LevelID = 2;
                }
                else if (worldMap[i, j] == 3)
                {
                    Instantiate(ground, new Vector3(50.0f - i * 10.0f, -2f, 50 - j * 10), Quaternion.identity, gameObject.transform);
                    Instantiate(shop, new Vector3(50.0f - i * 10.0f, 1f, 50 - j * 10), Quaternion.identity, gameObject.transform);
                }
            }
        }
        gameObject.transform.position = new Vector3(-27, 0, -112);
        gameObject.transform.RotateAround(gameObject.transform.position,gameObject.transform.up, 90);
    }
    void GenerateFromImage()
    {
        colorOfPixel = new Color[outlineImage.width, outlineImage.height];
        for (int x = 0; x < outlineImage.width; x++)
        {
            for (int y = 0; y < outlineImage.height; y++)
            {

                colorOfPixel[x, y] = outlineImage.GetPixel(x, y);
                if (colorOfPixel[x, y] == Color.black)
                {

                    GameObject t = (GameObject)(Instantiate(wall, new Vector3((outlineImage.width / 2) - x, 1f, (outlineImage.height / 2) - y), Quaternion.identity));
                    print("Wall @ " + x + ", " + y);

                }
                else if (colorOfPixel[x, y] == Color.white)
                {
                    GameObject t = (GameObject)(Instantiate(ground, new Vector3((outlineImage.width / 2) - x, 0f, (outlineImage.height / 2) - y), Quaternion.identity));
                    print("Ground @ " + x + ", " + y);

                }
            }




        }

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour {
    [Header("Field")]
    public Texture2D tex2d;
    public int fieldSize;
    public Color32[,] fieldColors = new Color32[1000,1000];

    [Header("Objects")]
    public Creature[,] Creatures = new Creature[1000,1000];
    [HideInInspector]
    public Creature[] aliveBots = new Creature [1000000];

    public GameObject sphere;

    [Header("Numbers")]
    public float stepDelay;

    [Header("Beginning")]
    public int startBotCount;
    public int botCount;
    // Use this for initialization
    void Start()
    {
        for (int x = 0; x < fieldSize; x++)
        {
            for (int y = 0; y < fieldSize; y++)
            {
                fieldColors[x, y] = Color.white;
            }
        }
        CreateBots();
    }

    void Render()
    {
        tex2d = new Texture2D(fieldSize, fieldSize)
        {
            filterMode = FilterMode.Point
        };
        for (int x = 0; x < fieldSize; x++)
        {
            for (int y = 0; y < fieldSize; y++)
            {
                tex2d.SetPixel(x, y, fieldColors[x,y]);
            }
        }
        tex2d.Apply();

        gameObject.GetComponent<RawImage>().texture = tex2d;
    }

    void CreateBots()
    {
        int curBot = 0;
        for (int i = 0; i < startBotCount; i++)
        {
            int a = Random.Range(0, fieldSize);
            int b = Random.Range(0, fieldSize);
            if (Creatures[a, b] == null)
            {
                Creatures[a, b] = new Creature()
                {
                    cordinates = new Vector2Int(a, b)
                };
                aliveBots[curBot] = Creatures[a, b];
                aliveBots[curBot].id = (byte)curBot;
                curBot += 1;
                botCount += 1;
            }
            else startBotCount += 1;
        }
        BotStart();
    }

    void Step()
    {
        Render();
        print(aliveBots[1]);
        for (int x = 0; x < fieldSize; x++)
        {
            for (int y = 0; y < fieldSize; y++)
            {
                //fieldColors[x, y] = Color.white;
            }
        }



        for (int i = 0; i < botCount; i++)
        {
            if (aliveBots[i] != null)
            aliveBots[i].Step();
        }
    }

    void BotStart()
    {
        //for (int x=0; x<fieldSize; x++)
        {
            //for (int y = 0; y < fieldSize; y++)
            {
                //if (Creatures[x, y] != null)
                {
                    //Creatures[x, y].LateStart();
                }
            }
        }
        for (int i =0; i<botCount; i++)
        {
          aliveBots[i].LateStart();
        }
        InvokeRepeating("Step", 0, stepDelay);
    }


    void Render2()
    {
        tex2d = new Texture2D(fieldSize, fieldSize)
        {
            filterMode = FilterMode.Point
        };
        for (int x = 0; x < fieldSize; x++)
        {
            for (int y = 0; y < fieldSize; y++)
            {
                tex2d.SetPixel(x, y, fieldColors[x, y]);
            }
        }
        tex2d.Apply();

        //gameObject.GetComponent<RawImage>().texture = tex2d;

        sphere.GetComponent<Renderer>().material.mainTexture = tex2d;
    }
}

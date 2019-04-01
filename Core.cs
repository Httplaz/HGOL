using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Core : MonoBehaviour {


    [Header("Input")]
    public bool paused = false;
    public RawImage selectedGenome;
    public Vector2Int pos;
    public int b;

    [Header("Field")]
    public Texture2D tex2d;
    public int fieldSize;
    public Color32[,] fieldColors = new Color32[1024,1024];

    [Header("Objects")]
    public Creature[,] Creatures = new Creature[1024,1024];
    //[HideInInspector]
    public Creature[] aliveBots = new Creature [1000000];

    public GameObject sphere;

    [Header("Numbers")]
    public float stepDelay;
    public byte[] savedGenome = new byte[64];

    [Header("Beginning")]
    public int startBotCount;
    public int botCount;
    public int reserveBotCount;
    // Use this for initialization
    private void Awake()
    {
        PlayerPrefs.SetInt("BotStartCount", startBotCount);
        string toSave = "";
        for (byte i = 0; i < 63; i++)
        {
            toSave += 1 + " ";
        }
        PlayerPrefs.SetString("SavedGenome", toSave);
        PlayerPrefs.Save();
    }


    void Start()
    {
        string[] str = PlayerPrefs.GetString("SavedGenome").Split( );
        for (int i = 0; i < 63; i++)
        {
            int.TryParse(str[i], out b);
            savedGenome[i] = (byte)b;
        }
        startBotCount = PlayerPrefs.GetInt("BotStartCount");
        reserveBotCount = startBotCount;
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
            {
                if (aliveBots[i].alive == true)
                    aliveBots[i].Step();
            }
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
          //aliveBots[Random.Range(0,botCount)].genome =
        }
        InvokeRepeating("Step", 0, stepDelay);
    }

    public void Restart()
    {
        savedGenome = aliveBots[Random.Range(0, botCount)].genome;
        //PlayerPrefs.set
        for (int x = 0; x < fieldSize; x++)
        {
            for (int y = 0; y < fieldSize; y++)
            {
                fieldColors[x, y] = Color.white;
                Creatures[x, y] = null;
                aliveBots[x * y] = null;
                botCount = 0;
            }
        }
        botCount = 0;
        startBotCount = reserveBotCount;
        Start();
    }

    public void Restart2()
    {
        PlayerPrefs.SetInt("BotStartCount", reserveBotCount);
        SceneManager.LoadScene(0);
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if ((int)(Input.mousePosition.x / 17)<=63)
            pos = new Vector2Int((int)(Input.mousePosition.x/17), (int)(Input.mousePosition.y/17));
            //print(Input.mousePosition);
            selectedGenome.color = fieldColors[pos.x, pos.y];
        }
    }

    public void Pause()
    {
        paused = !paused;
        if (paused)
        {
            CancelInvoke("Step");
        }
        else
        {
            InvokeRepeating("Step", 0, stepDelay);
        }
    }

    public void SaveGenome()
    {
        string toSave = "";
        for (byte i=0; i<63; i++)
        {
            toSave += Creatures[pos.x, pos.y].genome[i]+" ";
        }
        PlayerPrefs.SetString("SavedGenome", toSave);
        PlayerPrefs.Save();
        
    }

    public void InsertGenome()
    {
        Creatures[pos.x, pos.y].genome = savedGenome;
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// TODO: get rid of magic numbers


public class HGOLApp : MonoBehaviour
{

    [Header("Input")]
    public bool paused = false;
    public RawImage selectedGenome;
    private Creature selectedCreature;

    [Header("Field")]
    public Texture2D tex2d;
    public int fieldSize;

    [Header("Objects")]
    private Core core;
    public GameObject sphere;

    [Header("Numbers")]
    public float stepDelay;
    public byte[] savedGenome = new byte[64];

    [Header("Beginning")]
    public int startBotCount;
    public int botCount
    {
        get
        {
            return core.BotCount;
        }
    };

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
        string[] str = PlayerPrefs.GetString("SavedGenome").Split();
        int b;
        for (int i = 0; i < 63; i++)
        {
            int.TryParse(str[i], out b);
            savedGenome[i] = (byte)b;
        }
        startBotCount = PlayerPrefs.GetInt("BotStartCount");
        this.core = new Core(Vector2Int(fieldSize, fieldSize), startBotCount);
        InvokeRepeating("Step", 0, stepDelay);
    }

    void Render()
    {
        tex2d = new Texture2D(fieldSize, fieldSize)
        {
            filterMode = FilterMode.Point
        };
        for (int x = 0; x < core.FieldSize.x; x++)
        {
            for (int y = 0; y < core.FieldSize.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                tex2d.SetPixel(x, y, core.GetColor(pos));
            }
        }
        tex2d.Apply();

        gameObject.GetComponent<RawImage>().texture = tex2d;
    }

    void Step()
    {
        Render();
        core.Step();
    }

    public void Restart()
    {
        savedGenome = core.GetRandomCreature().genome;
        Start();
    }

    public void Restart2()
    {
        PlayerPrefs.SetInt("BotStartCount", startBotCount);
        SceneManager.LoadScene(0);
    }

    private Vector2Int ComputeSelectedCreaturePos()
    {
        int x = (int)(Input.mousePosition.x / 17);
        int y = (int)(Input.mousePosition.y / 17);
        return new Vector2Int(x, y);
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2Int newSelectedCreaturePos = ComputeSelectedCreaturePos();
            if (core.PosExists(newSelectedCreaturePos) && core.GetCreature(newSelectedCreaturePos) != null)
            {
                selectedCreature = core.GetCreature(newSelectedCreaturePos);
                selectedGenome.color = core.GetColor(newSelectedCreaturePos);
            }
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
        for (byte i = 0; i < 63; i++)
        {
            toSave += selectedCreature.genome[i] + " ";
        }
        PlayerPrefs.SetString("SavedGenome", toSave);
        PlayerPrefs.Save();
    }

    public void InsertGenome()
    {
        selectedCreature.genome = savedGenome;
    }
}

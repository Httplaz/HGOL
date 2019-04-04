using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// TODO: get rid of magic numbers
// TODO: move field logic out of input/output


public class Core : MonoBehaviour {

    [Header("Input")]
    public bool paused = false;
    public RawImage selectedGenome;
    private Creature selectedCreature;

    [Header("Field")]
    public Texture2D tex2d;
    public int fieldSize;
    private Color32[,] fieldColors = new Color32[1024, 1024];

    [Header("Objects")]
    private Creature[,] Creatures = new Creature[1024, 1024];
    //[HideInInspector]
    private Creature[] aliveBots = new Creature [1000000];

    public GameObject sphere;

    [Header("Numbers")]
    public float stepDelay;
    public byte[] savedGenome = new byte[64];

    [Header("Beginning")]
    private int startBotCount;
    private int botCount;
    private int reserveBotCount;

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
        botCount = 0;
        string[] str = PlayerPrefs.GetString("SavedGenome").Split();
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
                Vector2Int pos = new Vector2Int(x, y);
                SetColor(pos, Color.white);
            }
        }
        CreateBots();
        InvokeRepeating("Step", 0, stepDelay);
    }

    void CreateBots()
    {
        for (int curBotIndex = 0; curBotIndex < startBotCount; curBotIndex++)
        {
            do
            {
                int x = Random.Range(0, fieldSize);
                int y = Random.Range(0, fieldSize);
                Vector2Int pos = new Vector2Int(x, y);
            } while (GetCreature(pos) != null)

            Creature creature = new Creature(this, pos);
        }
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
                Vector2Int pos = new Vector2Int(x, y);
                tex2d.SetPixel(x, y, GetColor(pos));
            }
        }
        tex2d.Apply();

        gameObject.GetComponent<RawImage>().texture = tex2d;
    }

    void Step()
    {
        Render();

        for (int i = 0; i < botCount; i++)
        {
            if (aliveBots[i] != null)
            {
                if (aliveBots[i].alive)
                    aliveBots[i].Step();
            }
        }
    }

    public void Restart()
    {
        savedGenome = aliveBots[Random.Range(0, botCount)].genome;
        //PlayerPrefs.set
        for (int x = 0; x < fieldSize; x++)
        {
            for (int y = 0; y < fieldSize; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                SetCreature(pos, null);
            }
        }
        for (int i = 0; i < botCount; i++)
        {
            aliveBots[i] = null;
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

    private Vector2Int ComputeSelectedCreaturePos() {
        int x = (int)(Input.mousePosition.x / 17);
        int y = (int)(Input.mousePosition.y / 17);
        return new Vector2Int(x, y);
    }

    private bool PosExists(Vector2Int pos) {
        return pos.x <= 63;  // TODO
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2Int newSelectedCreaturePos = ComputeSelectedCreaturePos();
            if (PosExists(newSelectedCreaturePos))
            {
                selectedCreature = GetCreature(newSelectedCreaturePos);
            }
            selectedGenome.color = GetColor(selectedCreature.pos);  // TODO: should it be moved to the "if" above?
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
        Creature selectedCreature = selectedCreature;
        for (byte i = 0; i < 63; i++)
        {
            toSave += selectedCreature.genome[i]+" ";
        }
        PlayerPrefs.SetString("SavedGenome", toSave);
        PlayerPrefs.Save();
    }

    public void InsertGenome()
    {
        Creature selectedCreature = selectedCreature;
        selectedCreature.genome = savedGenome;
    }

    public Creature GetCreature(Vector2Int pos)
    {
        return Creatures[pos.x, pos.y];
    }

    private void SetCreature(Vector2Int pos, Creature creature)
    {
        Creatures[pos.x, pos.y] = creature;
        creature.pos = pos;
        Color color = Color.white;
        if (creature != null) {
            color = creature.myColor;
        }
        SetColor(pos, color);
    }

    public void AddCreature(Vector2Int pos, Creature creature)
    {
        botCount += 1;
        int id = botCount - 1;
        creature.id = id;
        aliveBots[id] = creature;
        SetCreature(pos, creature);
    }

    public void RemoveCreature(Vector2Int pos)
    {
        Creature creature = GetCreature(pos);
        creature.id = -1;
        aliveBots[creature.id] = null;
        SetCreature(pos, null);
    }

    public void MoveCreature(Creature creature, Vector2Int targetPos)
    {
        if (GetCreature(targetPos) != null) {
            throw new System.InvalidOperationException("Cell must be empty to move to");
        }
        Vector2Int oldPos = creature.pos;
        SwapCreatures(oldPos, targetPos);
        SetColor(oldPos, creature.GetPathColor());
    }

    public void SwapCreatures(Vector2Int pos1, Vector2Int pos2)
    {
        Creature creature1 = GetCreature(pos1);
        Creature creature2 = GetCreature(pos2);
        SetCreature(pos2, creature1);
        SetCreature(pos1, creature2);
    }

    public Color GetColor(Vector2Int pos)
    {
        return fieldColors[pos.x, pos.y];
    }

    public void SetColor(Vector2Int pos, Color color)
    {
        fieldColors[pos.x, pos.y] = color;
    }
}

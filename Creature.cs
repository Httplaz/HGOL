using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature: Core
{
    public Core core;
    public Vector2Int cordinates;
    public Color myColor;
    public Color pathColor;

    [Header("Stats")]
    byte energy = 10;
    public byte id;

    [Header("Commands")]
    public byte[] genome = new byte[64];
    public byte genomeEffectiveSize = 10;
    public byte commandsCount = 64;
    public byte commandBorder = 8;
    public byte currentCommand;
    byte nesw;
    byte switcher;

    [Header("Memory")]
    public Color[] remColors = new Color[4];
    public byte[] remBytes = new byte[255];

    [Header("Field")]
    public Vector2Int[] nearCords = new Vector2Int[4];
    // Use this for initialization
    public void LateStart()
    {
        commandsCount = 0;
        core = GameObject.FindGameObjectWithTag("GameController").GetComponent<Core>();
        ChooseColor();
        CreateGenome();
        core.fieldColors[cordinates.x, cordinates.y] = myColor;
        nesw = (byte)Random.Range(0, 4);
    }

    public void Step()
    {
        Debug.Log(energy);
        core.fieldColors[cordinates.x, cordinates.y] = myColor;
        currentCommand += switcher;
        if (currentCommand > genomeEffectiveSize) currentCommand = (byte)(currentCommand - genomeEffectiveSize);
        DoSomething(genome[currentCommand]);
        energy -= 1;
        if (energy <= 0)
            Death();
        else if (energy > 50)
            energy = 50;
    }

    public void FreeStep()
    {


    }

    public void Movement()
    {
        CheckNear2();
        if (core.Creatures[nearCords[nesw].x, nearCords[nesw].y] == null)
        {
            {
                core.fieldColors[cordinates.x, cordinates.y] = Color.white;
                core.Creatures[nearCords[nesw].x, nearCords[nesw].y] = this;
                core.Creatures[cordinates.x, cordinates.y] = null;
                core.fieldColors[cordinates.x, cordinates.y] = pathColor;
                cordinates = nearCords[nesw];
                core.fieldColors[cordinates.x, cordinates.y] = myColor;
                switcher = 1;
            }
        }
        else switcher = 2;
        //else nesw = Random.Range(0, 4);
    }

    public void CheckNear()
    {
        if (cordinates.x < core.fieldSize - 1 & cordinates.x > 0)
        {
            nearCords[0] = new Vector2Int(cordinates.x - 1, cordinates.y);
            nearCords[3] = new Vector2Int(cordinates.x + 1, cordinates.y);
        }
        else if (cordinates.x == 0)
        {
            nearCords[0] = new Vector2Int(fieldSize - 1, cordinates.y);
            nearCords[3] = new Vector2Int(1, cordinates.y);
        }
        else if (cordinates.x == fieldSize - 1)
        {
            nearCords[0] = new Vector2Int(fieldSize - 2, cordinates.y);
            nearCords[3] = new Vector2Int(0, cordinates.y);
        }


        if (cordinates.y < fieldSize - 1 & cordinates.y > 0)
        {
            nearCords[1] = new Vector2Int(cordinates.x, cordinates.y - 1);
            nearCords[2] = new Vector2Int(cordinates.x, cordinates.y + 1);
        }
        else if (cordinates.y == 0)
        {
            nearCords[1] = new Vector2Int(cordinates.x, fieldSize - 1);
            nearCords[2] = new Vector2Int(cordinates.x, 1);
        }
        else if (cordinates.y == fieldSize - 1)
        {
            nearCords[1] = new Vector2Int(cordinates.x, fieldSize - 2);
            nearCords[2] = new Vector2Int(cordinates.x, 0);
        }
    }

    public void CheckNear2()
    {
        nearCords[0] = new Vector2Int((fieldSize + cordinates.x - 1) & (fieldSize - 1), cordinates.y);
        nearCords[3] = new Vector2Int((fieldSize + cordinates.x + 1) & (fieldSize - 1), cordinates.y);
        nearCords[1] = new Vector2Int(cordinates.x, (fieldSize + cordinates.y - 1) & (fieldSize - 1));
        nearCords[2] = new Vector2Int(cordinates.x, (fieldSize + cordinates.y + 1) & (fieldSize - 1));
    }


    void ChooseColor()
    {
        byte c = 255;
        byte r = (byte)(Random.Range(0,17) * 15);
        byte g = (byte)(Random.Range(0, 17) * 15 - r);
        byte b = (byte)(255-g-r);
        myColor = new Color32(r, g, b, 255);
        pathColor = new Color32((byte)(255-r/17), (byte)(255 - g / 17), (byte)(255 - b / 17), 255);
    }

    void CreateGenome()
    {
        for (int i = 0; i < genomeEffectiveSize; i++)
        {
            genome[i] = (byte)Random.Range(0, commandBorder+1);
        }
    }

    void DoSomething(int command)
    {
        //Debug.Log(command);
        switch (command)
        {
            case 0:             //movement
                Movement();
                break;
            case 1:             //rotating left
                nesw = 0;
                switcher = 5;
                break;
            case 2:             //rotating right
                nesw = 3;
                switcher = 4;
                break;
            case 3:             //rotating up
                nesw = 2;
                switcher = 3;
                break;
            case 4:             //rotating down
                nesw = 1;
                switcher = 2;
                break;
            case 5:             //rotating
                nesw = (byte)Random.Range(0, 4);
                switcher = 1;
                break;
            case 6:             //looking forward
                switcher = InFront();
                //Debug.Log(switcher);
                break;
            case 7:             //movement
                CheckEnergy();
                break;
            case 8:             //photosynthesis
                energy+=3;
                break;
            case 9:
                Multiply((byte)(energy/10), 0, 1, 2);
                break;
               
        }
    }

    public byte InFront()
    {
        byte result = 5; //some color
        if (core.Creatures[nearCords[nesw].x, nearCords[nesw].y] == null) //empty cell
        {
            if (core.fieldColors[nearCords[nesw].x, nearCords[nesw].y] == myColor)
                result = 4; //my territory
            else if (core.fieldColors[nearCords[nesw].x, nearCords[nesw].y] == Color.white)
                result = 1; //nothing

            else //remembered color
            {
                for (int i = 0; i < 4; i++)
                    if (core.fieldColors[nearCords[nesw].x, nearCords[nesw].y] == remColors[i])
                        result = (byte)(4 * 10 + i);
            }
        }
        else if (core.Creatures[nearCords[nesw].x, nearCords[nesw].y].myColor != myColor)
            result = 2; //not bro
        else
            result = 3; //bro
        return result;
    }

    public void CheckEnergy()
    {
        switcher = energy;
        remBytes[0] = energy;
    }

    public void Death()
    {
        myColor = Color.gray;
        core.fieldColors[cordinates.x, cordinates.y] = myColor;
        aliveBots[id] = null;
        //core.aliveBots[id] = core.aliveBots[core.botCount - 1];
        //core.botCount -= 1;
        //core.aliveBots[id].Step();
    }

    public void Multiply(byte amount, byte side1, byte side2, byte side3)
    {
        for (int i=0; i<amount; i++)
        {
            energy -= 10;
            if (Creatures[nearCords[i].x, nearCords[i].y] == null && energy > 0)
            {
                botCount += 1;
                aliveBots[botCount] = new Creature
                {
                    cordinates = nearCords[i]
                };
                Creatures[nearCords[i].x, nearCords[i].y] = aliveBots[botCount];
                aliveBots[botCount].LateStart();
                aliveBots[botCount].myColor = myColor;
            }
            else Death();
        }
    }

}




using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Creature
{
    public Core core;
    public Vector2Int cordinates;
    public Color myColor;
    public Color pathColor;

    [Header("Stats")]
    byte energy = 10;
    public byte id;
    public bool alive = true;

    [Header("Commands")]
    public byte[] genome = new byte[64];
    public byte[] switchers = new byte[64];
    public byte genomeEffectiveSize = 63;
    public byte commandsCount = 64;
    public byte commandBorder = 11; //last executable command
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
        ChooseColor();
        core.fieldColors[cordinates.x, cordinates.y] = myColor;
        nesw = (byte)Random.Range(0, 4);
    }

    public void Step()
    {
        //Debug.Log(cordinates);
        //Debug.Log(core.Creatures[cordinates.x, cordinates.y].cordinates);
        currentCommand += switcher;
        if (currentCommand > genomeEffectiveSize) currentCommand = (byte)(currentCommand - genomeEffectiveSize);
        if (genome.Length < currentCommand) Debug.Log(switcher);
        DoSomething(genome[currentCommand]);
        energy -= 1;
        if (energy <= 0)
            Death();
        else if (energy > 50)
            energy = 50;
        SendColor();
        if (core.Creatures[cordinates.x, cordinates.y] == null)
            Debug.Log("error");
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
                switcher = switchers[12];
            }
        }
        else switcher = switchers[13];
    }


    public void CheckNear2()
    {
        nearCords[0] = new Vector2Int((core.fieldSize + cordinates.x - 1) & (core.fieldSize - 1), cordinates.y);
        nearCords[3] = new Vector2Int((core.fieldSize + cordinates.x + 1) & (core.fieldSize - 1), cordinates.y);
        nearCords[1] = new Vector2Int(cordinates.x, (core.fieldSize + cordinates.y - 1) & (core.fieldSize - 1));
        nearCords[2] = new Vector2Int(cordinates.x, (core.fieldSize + cordinates.y + 1) & (core.fieldSize - 1));
    }


    void ChooseColor()
    {
        byte r = (byte)(Random.Range(0,17) * 15);
        byte g = (byte)(Random.Range(0, 17-r/15) * 15);
        byte b = (byte)(255-g-r);
        myColor = new Color32(r, g, b, 255);
        pathColor = new Color32((byte)(255-r/17), (byte)(255 - g / 17), (byte)(255 - b / 17), 255);
    }

    void CreateGenome()
    {
        for (int i = 0; i < genomeEffectiveSize; i++)
        {
            genome[i] = (byte)UnityEngine.Random.Range(0, commandBorder+1);
            switchers[i] = (byte)UnityEngine.Random.Range(0, genomeEffectiveSize);
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
                switcher = switchers[0];
                break;
            case 2:             //rotating right
                nesw = 3;
                switcher = switchers[1];
                break;
            case 3:             //rotating up
                nesw = 2;
                switcher = switchers[2];
                break;
            case 4:             //rotating down
                nesw = 1;
                switcher = switchers[3];
                break;
            case 5:             //rotating
                nesw = (byte)Random.Range(0, 4);
                switcher = switchers[4];
                break;
            case 6:             //looking forward
                switcher = InFront();
                //Debug.Log(switcher);
                break;
            case 7:             //movement
                CheckEnergy();
                break;
            case 8:             //photosynthesis
                energy+=7;
                break;
            case 17:             //multiplying
                Multiply((byte)(energy/10), 0, 1, 2);
                break;
            case 9:             //remember color
                CheckNear2();
                remColors[nesw] = core.fieldColors[nearCords[nesw].x, nearCords[nesw].y];
                break;
            case 11:            //eating
                Eating();
                break;
            case 16:            //eating
                Eating();
                break;


        }
    }

    public byte InFront()
    {
        byte result = switchers[5]; //some color
        if (core.Creatures[nearCords[nesw].x, nearCords[nesw].y] == null) //empty cell
        {
            if (core.fieldColors[nearCords[nesw].x, nearCords[nesw].y] == myColor)
                result = switchers[6]; //my territory
            else if (core.fieldColors[nearCords[nesw].x, nearCords[nesw].y] == Color.white)
                result = switchers[7]; //nothing

            else //remembered color
            {
                for (int i = 0; i < 4; i++)
                    if (core.fieldColors[nearCords[nesw].x, nearCords[nesw].y] == remColors[i])
                        result = (byte)(switchers[8] + i);
            }
        }
        else if (core.Creatures[nearCords[nesw].x, nearCords[nesw].y].myColor != myColor)
            result = switchers[9]; //not bro
        else if (core.Creatures[nearCords[nesw].x, nearCords[nesw].y].myColor == Color.gray)
            result = switchers[10]; //dead body
        else
            result = switchers[11];
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
        alive = false;
    }

    public void Multiply(byte amount, byte side1, byte side2, byte side3)
    {
        CheckNear2();
        if (amount > 4)
            amount = 4;
        for (int i=0; i<amount; i++)
        {
            energy -= 10;
            if (core.Creatures[nearCords[i].x, nearCords[i].y] == null && energy > 0 && alive)
            {
                core.botCount += 1;
                core.aliveBots[core.botCount-1] = new Creature
                {
                    cordinates = nearCords[i]
                };
                core.Creatures[nearCords[i].x, nearCords[i].y] = core.aliveBots[core.botCount-1];
                core.aliveBots[core.botCount-1].LateStart();
                core.aliveBots[core.botCount - 1].genome = genome;
                core.aliveBots[core.botCount-1].myColor = myColor;
                core.aliveBots[core.botCount - 1].energy = 5;
            }
            else Death();
        }
    }

    public void Eating()
    {
        if (core.Creatures[nearCords[nesw].x, nearCords[nesw].y] != null)
            {
            energy += (byte)(core.Creatures[nearCords[nesw].x, nearCords[nesw].y].energy / 2);
            core.aliveBots[core.Creatures[nearCords[nesw].x, nearCords[nesw].y].id] = null;
            core.Creatures[nearCords[nesw].x, nearCords[nesw].y] = null;
            Movement();
            }
    }

    public void SendColor()
    {
        core.fieldColors[cordinates.x, cordinates.y] = myColor;
    }

}




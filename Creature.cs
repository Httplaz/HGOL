
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Creature
{
    public Core core;
    public Vector2Int pos;
    public Color myColor;

    [Header("Stats")]
    byte energy = 10;
    public int id;
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

    public Creature(Core core, Vector2Int pos, byte[] genome, Color color)
    {
        this.core = core;
        core.AddCreature(pos, this);
        commandsCount = 0;

        if (genome == null)
        {
            genome = ChooseGenome();
        }
        this.genome = genome;

        CreateSwitchers();

        if (color == null)
        {
            color = ChooseColor();
        }
        this.myColor = color;

        nesw = (byte)Random.Range(0, 4);
    }

    public Creature(Core core, Vector2Int pos)
    {
        new Creature(core, pos, null, Color.green);
    }

    public void Step()
    {
        //Debug.Log(pos);
        //Debug.Log(core.GetCreature(pos).pos);
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
        if (core.GetCreature(pos) == null)
            Debug.Log("error");
    }

    public void FreeStep()
    {
        // TODO
    }

    public void Movement()
    {
        CheckNear2();
        if (core.GetCreature(nearCords[nesw]) == null)
        {
            core.MoveCreature(this, nearCords[nesw]);
            switcher = switchers[12];
        }
        else switcher = switchers[13];
    }

    public void CheckNear2()
    {
        nearCords[0] = new Vector2Int((core.fieldSize + pos.x - 1) & (core.fieldSize - 1), pos.y);
        nearCords[3] = new Vector2Int((core.fieldSize + pos.x + 1) & (core.fieldSize - 1), pos.y);
        nearCords[1] = new Vector2Int(pos.x, (core.fieldSize + pos.y - 1) & (core.fieldSize - 1));
        nearCords[2] = new Vector2Int(pos.x, (core.fieldSize + pos.y + 1) & (core.fieldSize - 1));
    }

    private Color32 ChooseColor()
    {
        byte r = (byte)(Random.Range(0, 17) * 15);
        byte g = (byte)(Random.Range(0, 17 - r / 15) * 15);
        byte b = (byte)(255 - g - r);
        return new Color32(r, g, b, 255);
    }

    public Color32 GetPathColor()
    {
        byte r = (byte)(255 - myColor.r / 17);
        byte g = (byte)(255 - myColor.g / 17);
        byte b = (byte)(255 - myColor.b / 17);
        return new Color32(r, g, b, 255);
    }

    private byte[] ChooseGenome()
    {
        byte[] result = new byte[64];
        for (int i = 0; i < genomeEffectiveSize; i++)
        {
            result[i] = (byte)UnityEngine.Random.Range(0, commandBorder + 1);
        }
        return result;
    }

    void CreateSwitchers()
    {
        for (int i = 0; i < genomeEffectiveSize; i++)
        {
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
                energy += 7;
                break;
            case 17:             //multiplying
                Multiply((byte)(energy / 10), 0, 1, 2);
                break;
            case 9:             //remember color
                CheckNear2();
                remColors[nesw] = core.GetColor(nearCords[nesw]);
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
        if (core.GetCreature(nearCords[nesw]) == null) //empty cell
        {
            if (core.GetColor(nearCords[nesw]) == myColor)
                result = switchers[6]; //my territory
            else if (core.GetColor(nearCords[nesw]) == Color.white)
                result = switchers[7]; //nothing

            else //remembered color
            {
                for (int i = 0; i < 4; i++)
                    if (core.GetColor(nearCords[nesw]) == remColors[i])
                        result = (byte)(switchers[8] + i);
            }
        }
        else if (core.GetCreature(nearCords[nesw]).myColor != myColor)
            result = switchers[9]; //not bro
        else if (core.GetCreature(nearCords[nesw]).myColor == Color.gray)
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
        for (int i = 0; i < amount; i++)
        {
            energy -= 10;
            if (core.GetCreature(nearCords[i]) == null && energy > 0 && alive)
            {
                Creature child = new Creature(core, nearCords[i], genome, myColor);
                child.energy = 5;
            }
            else Death();
        }
    }

    public void Eating()
    {
        if (core.GetCreature(nearCords[nesw]) != null)
        {
            energy += (byte)(core.GetCreature(nearCords[nesw]).energy / 2);
            core.RemoveCreature(nearCords[nesw]);
            Movement();
        }
    }

    public void SendColor()
    {
        core.SetColor(pos, myColor);
    }
}
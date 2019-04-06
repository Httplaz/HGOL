using System.Collections.Generic;
using UnityEngine;


public class Core
{
    [Header("Field")]
    public Vector2Int FieldSize;
    private Creature[,] CreaturesField;
    private Color32[,] FieldColors;

    [Header("Bots")]
    private LinkedList<Creature> Bots;
    public int BotCount;
    {
        get
        {
            return Bots.Count;
        }
    }

    public Core (Vector2Int fieldSize, int startBotCount)
    {
        FieldSize = fieldSize;
        CreaturesField = new Creature[FieldSize.x, FieldSize.y];
        FieldColors = new Color32[FieldSize.x, FieldSize.y];
        Bots = new LinkedList<Creature>();
        CreatureBots(startBotCount);
    }

    private bool PosExists(Vector2Int pos)
    {
        bool x_exists = 0 <= pos.x && pos.x < FieldSize.x;
        bool y_exists = 0 <= pos.y && pos.y < FieldSize.y;
        return x_exists && y_exists;
    }

    void Step()
    {
        for
        (
            LinkedListNode<Creature> node = Bots.First;
            node != null;
            node = node.Next
        )
        {
            Creature bot = node.value;
            if (bot.alive)
            {
                bot.Step();
            }
        }
    }

    void CreateBots(int botCount)
    {
        for (int curBotIndex = 0; curBotIndex < botCount; curBotIndex++)
        {
            Vector2Int pos;
            do
            {
                int x = Random.Range(0, FieldSize);
                int y = Random.Range(0, FieldSize);
                pos = new Vector2Int(x, y);
            } while (GetCreature(pos) != null);

            Creature creature = new Creature(this, pos);
        }
    }

    public Creature GetCreature(Vector2Int pos)
    {
        return CreaturesField[pos.x, pos.y];
    }

    private void SetCreature(Vector2Int pos, Creature creature)
    {
        CreaturesField[pos.x, pos.y] = creature;
        if (creature != null)
        {
            creature.pos = pos;
        }
        // TODO
        Color color = Color.white;
        if (creature != null)
        {
            color = creature.myColor;
        }
        SetColor(pos, color);
    }

    public void AddCreature(Vector2Int pos, Creature creature)
    {
        Bots.AddLast(creature);
        SetCreature(pos, creature);
    }

    public void RemoveCreature(Vector2Int pos)
    {
        Creature creature = GetCreature(pos);
        SetCreature(pos, null);
        Bots.Remove(creature);  // TODO: O(n) may cause problems
    }

    public void MoveCreature(Creature creature, Vector2Int targetPos)
    {
        if (GetCreature(targetPos) != null)
        {
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
        return FieldColors[pos.x, pos.y];
    }

    public void SetColor(Vector2Int pos, Color color)
    {
        FieldColors[pos.x, pos.y] = color;
    }

    // TODO: move out
    public void Mutate(Creature creature, bool commandsOrSwitchers)
    {
        if (commandsOrSwitchers)
            creature.genome[Random.Range(0, creature.genome.Length)] = (byte)Random.Range(0, creature.commandBorder + 1);
    }

    public Creature GetRandomCreature()
    {
        int BotIndex = Random.Range(0, BotCount);
        LinkedListNode<Creature> node = Bots.First;
        for (int i = 0; i != BotIndex; i++)
        {
            node = node.next;
        }
        return node.value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public Node Parent;
    public float Cost;
    public Dictionary<string, int> State;
    public GAction Action;

    public Node(Node parent, float cost, Dictionary<string, int> allStates, GAction action)
    {
        Parent = parent;
        Cost = cost;
        State = new Dictionary<string, int>(allStates);
        Action = action;
    }

    public Node(Node parent, float cost, Dictionary<string, int> allStates, Dictionary<string, int> beliefStates, GAction action)
    {
        Parent = parent;
        Cost = cost;
        State = new Dictionary<string, int>(allStates);

        foreach (KeyValuePair<string, int> belief in beliefStates)
        {
            if (!State.ContainsKey(belief.Key))
                State.Add(belief.Key, belief.Value);
        }

        Action = action;
    }
}

public class GPlanner
{
    public Queue<GAction> Plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates beliefStates)
    {
        List<GAction> usableActions = new List<GAction>();

        foreach (GAction action in actions)
        {
            if (action.IsAchievable())
                usableActions.Add(action);
        }

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, GWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success) { return null; }

        Node cheapest = null;

        //Calculates cheapest action
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.Cost < cheapest.Cost)
                    cheapest = leaf;
            }
        }

        List<GAction> result = new List<GAction>();
        Node node = cheapest;

        while(node != null)
        {
            if(node.Action != null)
            {
                result.Insert(0, node.Action);
            }

            node = node.Parent;
        }

        Queue<GAction> queue = new Queue<GAction>();

        foreach (GAction action in result)
        {
            queue.Enqueue(action);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false;

        foreach (GAction action in usableActions)
        {
            if (action.IsAchievableGiven(parent.State))
            {
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.State);
                foreach (KeyValuePair<string, int> effect in action.Effects)
                {
                    if (!currentState.ContainsKey(effect.Key))
                    {
                        currentState.Add(effect.Key, effect.Value);
                    }
                }

                Node node = new Node(parent, parent.Cost + action.Cost, currentState, action);

                if(GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    List<GAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                        foundPath = true;
                }
            }
        }

        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach (KeyValuePair<string, int> g in goal)
        {
            if (!state.ContainsKey(g.Key))
                return false;
        }

        return true;
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction actionToRemove)
    {
        List<GAction> subset = new List<GAction>();

        foreach (GAction action in actions)
        {
            if (!action.Equals(actionToRemove))
                subset.Add(action);
        }

        return subset;
    }
}

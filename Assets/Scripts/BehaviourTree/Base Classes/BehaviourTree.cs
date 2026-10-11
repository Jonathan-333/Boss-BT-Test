using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTree : MonoBehaviour
{
    protected BehaviourComposite root;
    [HideInInspector] public BehaviourNode activeNode;

    private readonly List<BehaviourInterupt> interupts = new List<BehaviourInterupt>();

    abstract public void InitializeTree();

    private void Update()
    {
        // for, not foreach: an interupt can restart the tree while the list is being read
        for (int i = 0; i < interupts.Count; i++)
        {
            interupts[i].Tick(Time.unscaledDeltaTime);
        }

        if (activeNode == null) return;

        activeNode.Tick(GameManager.ScaledDeltaTime);
    }

    public void RunTree()
    {
        root.ExecuteAction(null);
    }

    private void Start()
    {
        InitializeTree();
        RunTree();
    }

    private void OnDestroy()
    {
        foreach (BehaviourInterupt interupt in interupts)
        {
            interupt.Stop();
        }
        interupts.Clear();
    }

    public void RegisterInterupt(BehaviourInterupt interupt)
    {
        interupts.Add(interupt);
    }

    public void Interupt()
    {
        if (activeNode != null)
        {
            activeNode.Interupt();
        }

        RunTree();
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;

namespace LleuadNetworkSim.Utils.GodotUtils;

public class InstancePool<T>(PackedScene _Instantiable) where T : Node
{
    readonly private Queue<T> Instances = [];
    readonly private PackedScene Instantiable = _Instantiable;
    private Task GenerationTask;
    public bool IsFinished { get; private set; } = false;

    public int PoolSize => Instances.Count;

    public async Task Initialise(int _Count = 15) {
        GenerationTask = Generate(_Count);
        await GenerationTask;
    }

    private async Task Generate(int _Count) {

        IsFinished = false;
        
        if (Instantiable.Instantiate() is not T Instance) //todo custom exception
        { throw new Exception("Instance generated null!"); }
        
        Instances.Enqueue(Instance);
        
        for (int i = 0; i < _Count--; i++)
        { Instances.Enqueue((Instantiable.Instantiate() as T)!); }

        IsFinished = true;
    }

    public T Get() {
        if (!IsFinished)
        { GenerationTask.Wait(); }

        if (Instances.Count <= 2)
        { Generate(5).Wait(); }

        return Instances.Dequeue();
    }

    public void Return(T _Node) {
        Instances.Enqueue(_Node);
    }
}

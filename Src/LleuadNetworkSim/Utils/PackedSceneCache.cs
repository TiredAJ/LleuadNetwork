using CSharpFunctionalExtensions;

using Godot;
// ReSharper disable MemberCanBePrivate.Global

namespace LleuadNetworkSim.Utils;

public partial class PackedSceneCache<T> : Resource where T : Node
{
    required public PackedScene Scene { get; init; }
    
    private Maybe<T> Instance = Maybe<T>.None;

    public T GetInstance() {
        if (Instance.HasNoValue)
        { Instance = Scene.Instantiate<T>(); }

        return Instance.Value;
    }
}

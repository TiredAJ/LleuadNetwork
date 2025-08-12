using CSharpFunctionalExtensions;

using Godot;

// ReSharper disable MemberCanBePrivate.Global

namespace LleuadNetworkSim.Utils.GodotUtils;

//TODO: TEST - maybe this
public partial class PackedSceneSingleton<T> : Resource where T : Node
{
    public bool HasInstance => Instance.HasValue;
    
    required public PackedScene Scene {
        get;
        set {
            Instance = Maybe<T>.None;
            field = value;
        }
    }

    private Maybe<T> Instance = Maybe<T>.None;

    public T GetInstance() {
        if (Instance.HasNoValue)
        { Instance = Scene.Instantiate<T>(); }

        return Instance.Value;
    }
}

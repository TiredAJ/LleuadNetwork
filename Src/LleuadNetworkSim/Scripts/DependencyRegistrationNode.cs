using Godot;
using Godot.DependencyInjection;

using LleuadNetworkSim.Models.Repo;

using Microsoft.Extensions.DependencyInjection;

namespace LleuadNetworkSim.Scripts;

public partial class DependencyRegistrationNode : Node, IServicesConfigurator
{
    public void ConfigureServices(IServiceCollection _Services) {

        _Services.AddGodotServices();
        _Services.AddSingleton<IDBWrapper, Repo>();
    }
}

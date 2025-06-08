using Godot;

using Godot.DependencyInjection;

using LleuadNetworkSim.Models.Repo;

using Microsoft.Extensions.DependencyInjection;

public partial class DependencyRegistrationNode : Node, IServicesConfigurator
{
    public void ConfigureServices(IServiceCollection services) {

        services.AddGodotServices();
        services.AddSingleton<IDBWrapper, DBWrapper>();
    }
}

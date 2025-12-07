using Bladix.Primitives.Core.Primitive;
using Bladix.Primitives.Core.Rect;
using Microsoft.Extensions.DependencyInjection;

namespace Bladix.Primitives;

public static class BladixExtensions
{
    public static IServiceCollection AddBladixPrimitives(this IServiceCollection services)
    {
        services.AddScoped<BladixRect>();
        services.AddSingleton<BladixDom>();
        return services;

    }
}

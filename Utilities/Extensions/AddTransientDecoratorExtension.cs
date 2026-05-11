using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeautifulClient.Utilities.Extensions;

public static class AddTransientDecoratorExtension
{
    public static IServiceProvider AddPageDecorator<TViewModel, TPage, TLayout>(
        this HostApplicationBuilder builder, 
        object? key)
    
        where TPage : class, IView<TViewModel> 
        where TViewModel : class 
        where TLayout : IView<TViewModel>
    {
        builder.Services.AddTransient<IView<TViewModel>, TPage>();
        builder.Services.Decorate<IView<TViewModel>, TLayout>();
        
        return builder.Services.BuildServiceProvider();
    }
}
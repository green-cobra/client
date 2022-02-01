using System.CommandLine.Invocation;
using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Commands.Proxy.Services;
using GreenCobra.Client.Helpers;
using GreenCobra.Client.Infrastructure.ServerCommunication;
using GreenCobra.Client.Logging.Adapters;

namespace GreenCobra.Client.Commands.Proxy.Handlers;

public class ProxyCommandHandler : IProxyCommandHandler
{
    private readonly ICommandBinder<ProxyCommandParams> _paramsBinder;
    private readonly ILoggerAdapter<ProxyCommandHandler> _logger;
    private readonly IProxyService _proxyService;
    private readonly IServerCommunicationService _serverCommunicationService;

    public ProxyCommandHandler(
        ICommandBinder<ProxyCommandParams> paramsBinder,
        ILoggerAdapter<ProxyCommandHandler> logger,
        IProxyService proxyService, 
        IServerCommunicationService serverCommunicationService)
    {
        Guard.AgainstNull(paramsBinder);
        Guard.AgainstNull(logger);
        Guard.AgainstNull(proxyService);
        Guard.AgainstNull(serverCommunicationService);

        _paramsBinder = paramsBinder;
        _logger = logger;
        _proxyService = proxyService;
        _serverCommunicationService = serverCommunicationService;
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var commandParams = _paramsBinder.BindParametersFromContext(context.BindingContext);
        var cancellationToken = context.GetCancellationToken();

        var proxyConfiguration = await _serverCommunicationService.GetServerProxyConfigurationAsync(commandParams, cancellationToken);

        // wait until app will not be closed
        await _proxyService.StartProxyAsync(proxyConfiguration, cancellationToken);

        return context.ExitCode;
    }
}


﻿using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.Net;
using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Commands.Proxy.Handlers;

namespace GreenCobra.Client.Commands.Proxy;

public class ProxyCommand : Command
{
    private new const string Name = "proxy";
    private new const string Description = "Proxy requests from public server to your locally running application";

    private const string StubDescription = "COMING SOON!";

    #region Options

    private static readonly Option<string> LocalHostOption = new(
        aliases: new[] {"--local-host", "-l"},
        description: StubDescription,
        getDefaultValue: () => "127.0.0.1");

    protected static readonly Option<int> LocalPortOption = new(
        aliases: new[] {"--local-port", "-p"},
        description: StubDescription,
        getDefaultValue: () => 80);

    // todo: future improvement
    //private const string LocalHostDefault = "127.0.0.1";
    //private readonly Option<string> _localServerUrlOption = new(
    //    aliases: new[] { "--local-url", "-l" },
    //    description: StubDescription,
    //    getDefaultValue: () => new Uri($"{_localPortOption.}"));

    private static readonly Option<Uri> RemoteUrlOption = new(
        aliases: new[] {"--remote-url", "-s"},
        description: StubDescription,
        getDefaultValue: () => new Uri("https://localtunnel.me/"));

    private const string RemoteDomainDefault = "?new";
    private static readonly Option<string> RemoteDomainOption = new(
        aliases: new[] {"--remote-domain-request", "-d"},
        description: StubDescription,
        getDefaultValue: () => RemoteDomainDefault); // todo: maybe we will use enum here (/green-cobra-733)

    #endregion

    public ProxyCommand()
        :base(Name, Description)
    {
        AddOptions();

        var proxyParamsBinder = new ProxyParamsBinder();

        this.SetHandler(
            async (ProxyCommandParams proxyParams,
                InvocationContext ctx,
                CancellationToken cancellationToken) =>
            {
                var handler = new ProxyCommandHandler(proxyParams, cancellationToken);

                await handler.InvokeAsync(ctx);
            },
            proxyParamsBinder);
    }

    // Adds options for this command
    private void AddOptions()
    {
        AddOption(LocalHostOption);
        AddOption(LocalPortOption);
        AddOption(RemoteUrlOption);
        AddOption(RemoteDomainOption);
    }

    private class ProxyParamsBinder : BinderBase<ProxyCommandParams>
    {
        protected override ProxyCommandParams GetBoundValue(BindingContext bindingContext)
        {
            T? GetOptionValue<T>(Option<T> option) => bindingContext.ParseResult.GetValueForOption(option);

            var localServerPort = GetOptionValue(LocalPortOption);
            var localServeHost = GetOptionValue(LocalHostOption);

            return new ProxyCommandParams(
                IPEndPoint.Parse($"{localServeHost}:{localServerPort}"),
                GetOptionValue(RemoteUrlOption) 
                    ?? throw new ArgumentException("Invalid remote server url was provided"), // todo: maybe this param will be not configurable
                GetOptionValue(RemoteDomainOption) ?? RemoteDomainDefault
            );
        }
    }
}
namespace GreenCobra.Client.Proxy.V2;

public record ProxyResult(byte[]? ServerMessageHeading, byte[]? ClientMessageHeading);
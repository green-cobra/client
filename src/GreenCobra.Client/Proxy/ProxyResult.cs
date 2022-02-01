namespace GreenCobra.Client.Proxy;

public record ProxyResult(byte[]? ServerMessageHeading, byte[]? ClientMessageHeading);
namespace Lunar.Server.Scripting.Api
{
    public abstract class CommandScript
    {
        public abstract void Register(ICommandRegistrar registrar);
    }
}

namespace TurtleWorld.Structure
{
    public interface IConfigBuilder
    {
        void Build(string rawLine);
        IConfiguration Get();
    }
}

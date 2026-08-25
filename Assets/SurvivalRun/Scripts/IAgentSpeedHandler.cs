namespace ZombieElimination
{
    public interface IAgentSpeedHandler
    {
        float CurrentSpeed { get; }

        float TargetSpeed { get; }

        void SetTargetSpeed(float speed);
    }
}

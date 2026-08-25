namespace ZombieElimination
{
    public interface IZombieState
    {
        void EnterState(ZombieAgent agent);
        void UpdateState(ZombieAgent agent);
        void ExitState(ZombieAgent agent);
    }

}

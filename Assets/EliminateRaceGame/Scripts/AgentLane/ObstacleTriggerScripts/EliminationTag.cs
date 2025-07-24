namespace EliminateRaceGame
{
    [System.Serializable]
    public enum EliminationTag
    {
        None, // Default, can be eliminated by any obstacle
        Jump, // Eliminated by JumpTrigger
        LargeBoulder, // Eliminated by LargeBoulderTrigger
        SidewaysBoulder, // Eliminated by SidewaysBoulderTrigger
        YetiSwipe, // Eliminated by YetiSwipeTrigger
        FallingTree, // Eliminated by FallingTreeTrigger
        BreakingPlank, // Eliminated by BreakingPlankTrigger
        MoveToSafe
    }
}

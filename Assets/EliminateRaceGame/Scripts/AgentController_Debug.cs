using TMPro;

namespace EliminateRaceGame
{
    public partial class AgentController_RVO
    {
        public TextMeshProUGUI debugText;

        public void UpdateDebugText()
        {
            if (debugText == null)
            {
                debugText = GetComponentInChildren<TextMeshProUGUI>();
                return;
            }
            debugText.text = $"{currentSpeed}={targetSpeed}|{forceSwitchLaneIndex}|{eliminationTag.ToString()}";
        }
    }
}

using MiningGame.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiningGame.UI
{
    public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public BuildMode buildMode;   // przeci¹gnij obiekt z BuildMode
        public int buildTypeIndex;    // 0=Ladder, 1=Torch, 2=Rope, 3=Cart, 4=Rails
        public string displayName;    // np. "Torch"

        public void OnPointerEnter(PointerEventData e)
        {
            int cost = buildMode.GetCostByIndex(buildTypeIndex);
            var stats = buildMode.GetComponentInChildren<Stats>();

            string msg = $"{displayName}\nKoszt: {cost} $";
            if (stats && stats.CurrentMoney < cost)
                msg += "\n<color=red>Za ma³o pieniêdzy</color>";

            ToolTipController.I.Show(msg, e.position);
        }

        public void OnPointerExit(PointerEventData e) => ToolTipController.I.Hide();

        public void OnPointerClick(PointerEventData e)
        {
            buildMode.SetBuildIndex(buildTypeIndex);
            ToolTipController.I.Hide();
        }
    }
}

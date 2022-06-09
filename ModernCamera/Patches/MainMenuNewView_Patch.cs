using HarmonyLib;
using ProjectM.UI;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal class MainMenuNewView_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuNewView), nameof(MainMenuNewView.Start))]
    private static void Start(MainMenuNewView __instance)
    {
        var content = __instance.transform.Find("Content");

        Transform textTMP = content.GetComponentsInChildren<Transform>().First(x => x.name == "Text (TMP)");
        var font = textTMP.GetComponent<TextMeshProUGUI>().font;

        var modNotice = new GameObject("ModNotice");
        modNotice.transform.parent = content;
        modNotice.transform.SetSiblingIndex(1);
        modNotice.AddComponent<CanvasRenderer>();
        var modNoticeRectTransform = modNotice.AddComponent<RectTransform>();
        modNoticeRectTransform.pivot = new Vector2(.5f, 0);
        modNoticeRectTransform.anchorMin = new Vector2(.5f, 0);
        modNoticeRectTransform.anchorMax = new Vector2(.5f, 0);
        modNoticeRectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        modNoticeRectTransform.localPosition = new Vector3(0, 0, 0);
        var modNoticeImage = modNotice.AddComponent<Image>();
        modNoticeImage.sprite = null;
        modNoticeImage.color = new Color(0.6132f, 0, 0.1001f, 0.4667f);
        var modNoticeHorizontalLayoutGroup = modNotice.AddComponent<HorizontalLayoutGroup>();
        modNoticeHorizontalLayoutGroup.spacing = 8f;
        var modNoticeContentSizeFitter = modNotice.AddComponent<ContentSizeFitter>();
        modNoticeContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        modNoticeContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var modNoticeText = new GameObject("ModNoticeText");
        modNoticeText.transform.parent = modNotice.transform;
        modNoticeText.AddComponent<CanvasRenderer>();
        var modNoticeTextRectTransform = modNoticeText.AddComponent<RectTransform>();
        modNoticeTextRectTransform.localScale = new Vector3(1, 1, 1);
        var modNoticeTextTextMeshProUGUI = modNoticeText.AddComponent<TextMeshProUGUI>();
        modNoticeTextTextMeshProUGUI.fontSize = 38;
        modNoticeTextTextMeshProUGUI.fontStyle = FontStyles.SmallCaps;
        modNoticeTextTextMeshProUGUI.alignment = TextAlignmentOptions.Center;
        modNoticeTextTextMeshProUGUI.SetText(@"
<b>Mods Are Active!</b>
<size=24>
<b>Official</b> servers are <b>DISABLED</b>
<b>Private</b>/<b>Hosted</b>/<b>Local</b> are <b>ENABLED</b>
Discord http://dev.il.gy
</size>
        ");
        modNoticeTextTextMeshProUGUI.font = font;
        var modNoticeTextContentSizeFitter = modNoticeText.AddComponent<ContentSizeFitter>();
        modNoticeTextContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
}

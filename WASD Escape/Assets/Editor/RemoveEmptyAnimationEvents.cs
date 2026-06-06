using UnityEditor;
using UnityEngine;

public class RemoveEmptyAnimationEvents
{
    [MenuItem("Tools/Animation/Remove Empty Animation Events")]
    private static void RemoveEmptyEvents()
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip");

        int fixedClipCount = 0;
        int removedEventCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

            if (clip == null)
            {
                continue;
            }

            AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);

            if (events == null || events.Length == 0)
            {
                continue;
            }

            var newEvents = new System.Collections.Generic.List<AnimationEvent>();

            foreach (AnimationEvent animEvent in events)
            {
                if (string.IsNullOrEmpty(animEvent.functionName))
                {
                    removedEventCount++;
                    Debug.Log("빈 Animation Event 삭제: " + clip.name + " / " + path);
                }
                else
                {
                    newEvents.Add(animEvent);
                }
            }

            if (newEvents.Count != events.Length)
            {
                AnimationUtility.SetAnimationEvents(clip, newEvents.ToArray());
                fixedClipCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("완료: " + fixedClipCount + "개 애니메이션 클립에서 빈 이벤트 " + removedEventCount + "개 삭제");
    }
}
using System;
using System.Collections.Generic;
using UnityEditor;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    ///     A helper class to copy and paste feedback properties
    /// </summary>
    internal static class MMF_PlayerCopy
    {
        private static List<SerializedProperty> Properties = new();

        public static readonly List<MMF_Feedback> CopiedFeedbacks = new();

        public static List<MMF_Player> ShouldKeepChanges = new();

        private static string[] IgnoreList =
        {
            "m_ObjectHideFlags",
            "m_CorrespondingSourceObject",
            "m_PrefabInstance",
            "m_PrefabAsset",
            "m_GameObject",
            "m_Enabled",
            "m_EditorHideFlags",
            "m_Script",
            "m_Name",
            "m_EditorClassIdentifier"
        };
        // Single Copy --------------------------------------------------------------------

        public static Type Type { get; private set; }

        public static bool HasCopy()
        {
            return CopiedFeedbacks != null && CopiedFeedbacks.Count == 1;
        }

        public static bool HasMultipleCopies()
        {
            return CopiedFeedbacks != null && CopiedFeedbacks.Count > 1;
        }

        public static void Copy(MMF_Feedback feedback)
        {
            var feedbackType = feedback.GetType();
            var newFeedback = (MMF_Feedback)Activator.CreateInstance(feedbackType);
            EditorUtility.CopySerializedManagedFieldsOnly(feedback, newFeedback);
            CopiedFeedbacks.Clear();
            CopiedFeedbacks.Add(newFeedback);
        }

        public static void CopyAll(MMF_Player sourceFeedbacks)
        {
            CopiedFeedbacks.Clear();
            foreach (var feedback in sourceFeedbacks.FeedbacksList)
            {
                var feedbackType = feedback.GetType();
                var newFeedback = (MMF_Feedback)Activator.CreateInstance(feedbackType);
                EditorUtility.CopySerializedManagedFieldsOnly(feedback, newFeedback);
                CopiedFeedbacks.Add(newFeedback);
            }
        }

        // Multiple Copy ----------------------------------------------------------


        public static void PasteAll(MMF_PlayerEditor targetEditor)
        {
            foreach (var feedback in CopiedFeedbacks) targetEditor.TargetMmfPlayer.AddFeedback(feedback);
            CopiedFeedbacks.Clear();
        }
    }
}
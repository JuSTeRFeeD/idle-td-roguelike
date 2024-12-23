using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Prefabs.Lobby.Skills_Meta
{
    [CreateAssetMenu(fileName = "NewSkillTree", menuName = "Skill Tree")]
    public class SkillTree : ScriptableObject
    {
        public List<SkillNode> skills = new();

        [System.Serializable]
        public class IconByType
        {
            public SkillType type;
            [PreviewField()]
            public Sprite icon;
        }
        public List<IconByType> icons = new();

        [Button]
        private void OpenEditor()
        {
            SkillTreeEditorWindow.OpenWindow(this);
        }
    }
}
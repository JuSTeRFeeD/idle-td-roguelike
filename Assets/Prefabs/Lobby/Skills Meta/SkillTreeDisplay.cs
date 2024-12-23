using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Prefabs.Lobby.Skills_Meta
{
    public class SkillTreeDisplay : MonoBehaviour
    {
        [SerializeField] private SkillTree skillTree;
        [SerializeField] private RectTransform content;
        [Space]
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private float nodeSize = 64;
        [SerializeField] private Vector2 decreaseSize = new(1000, 1000);
        
        [Title("Skill Point")] 
        [SerializeField] private Sprite isOpennedBg;
        [SerializeField] private Sprite isClosedBg;

        private void Start()
        {
            if (skillTree != null)
            {
                content.sizeDelta = new Vector2(
                    SkillTreeConstants.GridSize * nodeSize,
                    SkillTreeConstants.GridSize * nodeSize) - decreaseSize;
                DisplayTree();
            }
        }

        private void DisplayTree()
        {
            var contentCenter = new Vector2(content.rect.width / 2f, content.rect.height / 2f);
            foreach (var node in skillTree.skills)
            {
                var pos = new Vector2(SkillTreeConstants.GridSize / 2f, SkillTreeConstants.GridSize / 2f);
                pos.x = node.position.x - pos.x;
                pos.y -= node.position.y;
                pos *= nodeSize;
                CreateNode(node, contentCenter + pos);
            }
        }

        private void CreateNode(SkillNode node, Vector2 position)
        {
            var nodeObject = Instantiate(nodePrefab, content);
            var rect = nodeObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(nodeSize, nodeSize);

            var skillPoint = nodeObject.GetComponent<SkillPoint>();
            var icon = skillTree.icons.FirstOrDefault(i => i.type == node.skillType);
            skillPoint.icon.sprite = icon?.icon;
            skillPoint.icon.enabled = icon?.icon;
            skillPoint.background.sprite = node.skillType == SkillType.Root ? isOpennedBg : isClosedBg;
            skillPoint.valueText.enabled = node.skillType != SkillType.Root && 
                                           node.skillType != SkillType.None &&
                                           node.skillType != SkillType.AddUnitsCount;
            if (node.skillType is SkillType.AllTowersHealth or SkillType.AllTowersHealth)
            {
                skillPoint.valueText.SetText($"{Mathf.CeilToInt(node.value)}");
            }
            else
            {
                skillPoint.valueText.SetText($"{node.value * 100:##.##}%");
            }

        }
    }
}
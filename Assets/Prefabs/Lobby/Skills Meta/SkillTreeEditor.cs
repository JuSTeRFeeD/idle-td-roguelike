using System.Linq;
using Prefabs.Lobby.Skills_Meta;
using UnityEditor;
using UnityEngine;

public class SkillTreeEditorWindow : EditorWindow
{
    private static SkillTree _skillTree; // Ссылка на объект дерева
    private Vector2 _scrollPosition; // Позиция скролла
    private const int CellSize = 64; // Размер одной ячейки

    private SkillNode _selectedSkill = null; // Выбранный узел для редактирования

    private GUIStyle _buttonStyle () => new GUIStyle(GUI.skin.button)
    {
        fontSize = 12,
        alignment = TextAnchor.MiddleCenter,
        normal = { textColor = Color.white },
        padding = new RectOffset(2, 2, 5, 5)
    };
    
    [MenuItem("Tools/Skill Tree Editor")]
    public static void OpenWindow(SkillTree tree)
    {
        _skillTree = tree;
        GetWindow<SkillTreeEditorWindow>("Skill Tree Editor");
    }
    
    private void OnEnable()
    {
        this.Focus();
        
        if (_skillTree == null)
        {
            _skillTree = FindObjectOfType<SkillTree>();
            if (_skillTree == null)
            {
                Debug.LogWarning("No Skill Tree found in the scene. Please create one.");
            }
        }
        
        if (_skillTree != null)
        {
            EnsureRootSkillExists();
        }
        
        // Center of window to root
        _scrollPosition = new Vector2(
            SkillTreeConstants.GridSize * CellSize / 2.5f, 
            SkillTreeConstants.GridSize * CellSize / 2.5f
            );
    }

    private void OnGUI()
    {
        if (_skillTree == null)
        {
            EditorGUILayout.HelpBox("Select a Skill Tree to edit.", MessageType.Warning);
            return;
        }

        DrawGrid();
        DrawNodes();
        
        DrawToolbar();
        if (!IsMouseOverSaveButton())
        {
            ProcessMouseEvents();
        }
        ProcessMouseEvents();

        if (_selectedSkill != null)
        {
            DrawSkillEditor();
        }
    }

    private bool IsMouseOverSaveButton()
    {
        Rect toolBarRect = new Rect(position.width - 80, 20, 70, 30);
        return toolBarRect.Contains(Event.current.mousePosition);
    }

    private void DrawToolbar()
    {
        // GUILayout.BeginHorizontal(EditorStyles.toolbar);
        // Размер и положение кнопки
        var buttonRect = new Rect(position.width - 80, 20, 70, 30);

        // Фон кнопки
        EditorGUI.DrawRect(buttonRect, new Color(0.1f, 0.5f, 0.1f, 1.0f)); // Зелёный фон без прозрачности
        
        // Отрисовка кнопки
        if (GUI.Button(buttonRect, "Save", _buttonStyle()))
        {
            Save();
        }
        // GUILayout.EndHorizontal();
        
        // Stats
        // var panelRect = GUILayoutUtility.GetRect(300, 150);

        // Отрисовка фона
        var totalsRect = new Rect(new Vector2(2, position.height - 110), new Vector2(300, 90));
        EditorGUI.DrawRect(totalsRect, new Color(0f, 0f, 0f, 0.5f));
        GUILayout.BeginArea(totalsRect);
        var totalCritChance = _skillTree.skills.Sum(i => i.skillType == SkillType.CriticalChance ? i.value : 0);
        var totalCritDamage = _skillTree.skills.Sum(i => i.skillType == SkillType.CriticalDamage ? i.value : 0);
        var totalAllTowersDamage = _skillTree.skills.Sum(i => i.skillType == SkillType.AllTowersDamage ? i.value : 0);
        var totalAllTowersHealth = _skillTree.skills.Sum(i => i.skillType == SkillType.AllTowersHealth ? i.value : 0);
        var totalAdditionalUnits = _skillTree.skills.Sum(i => i.skillType == SkillType.AddUnitsCount ? i.value : 0);
        GUILayout.Label($"Total Crit. Chance: {totalCritChance}", EditorStyles.boldLabel);
        GUILayout.Label($"Total Crit. Damage: {totalCritDamage}", EditorStyles.boldLabel);
        GUILayout.Label($"Total All Towers Damage: {totalAllTowersDamage}", EditorStyles.boldLabel);
        GUILayout.Label($"Total All Towers Health: {totalAllTowersHealth}", EditorStyles.boldLabel);
        GUILayout.Label($"Total Additional Units: {totalAdditionalUnits}", EditorStyles.boldLabel);
        GUILayout.EndArea();
    }

    private void DrawSkillEditor()
    {
        // Размеры области панели
        var panelRect = GUILayoutUtility.GetRect(position.width - 16, 150);

        // Отрисовка фона
        EditorGUI.DrawRect(panelRect, new Color(0.2f, 0.2f, 0.2f, 0.99f));

        // GUILayout.Space(-150); // Сбрасываем отступы для элементов интерфейса

        GUILayout.BeginArea(new Rect(10, panelRect.y + 16, position.width - 20, position.height - 10));
        GUILayout.Label("Edit Skill", EditorStyles.boldLabel);

        if (_selectedSkill != null)
        {
            _selectedSkill.skillType= (SkillType)EditorGUILayout.EnumPopup("Type", _selectedSkill.skillType);
            _selectedSkill.value = EditorGUILayout.FloatField("Typed Value", _selectedSkill.value);

            // _selectedSkill.description = EditorGUILayout.TextField("Description", _selectedSkill.description);
            // _selectedSkill.icon = (Sprite)EditorGUILayout.ObjectField("Icon", _selectedSkill.icon, typeof(Sprite), false);
            GUILayout.Space(10);
            if (GUILayout.Button("Deselect", _buttonStyle()))
            {
                _selectedSkill = null; // Снимаем выбор узла
            }
        }
        GUILayout.EndArea();
    }


    private static void Save()
    {
        EditorUtility.SetDirty(_skillTree);
        AssetDatabase.SaveAssets();
    }

    private void DrawGrid()
    {
        var scrollRect = new Rect(0, 0, position.width, position.height);
        var contentRect = new Rect(0, 0, SkillTreeConstants.GridSize * CellSize, SkillTreeConstants.GridSize * CellSize);

        _scrollPosition = GUI.BeginScrollView(scrollRect, _scrollPosition, contentRect);
        Handles.color = Color.gray;

        // Рисуем линии сетки
        for (var x = 0; x <= SkillTreeConstants.GridSize; x++)
        {
            Handles.DrawLine(
                new Vector2(x * CellSize, 0), 
                new Vector2(x * CellSize, SkillTreeConstants.GridSize * CellSize)
                );
        }

        for (var y = 0; y <= SkillTreeConstants.GridSize; y++)
        {
            Handles.DrawLine(
                new Vector2(0, y * CellSize), 
                new Vector2(SkillTreeConstants.GridSize * CellSize, y * CellSize)
                );
        }
        GUI.EndScrollView();
    }
    
    private void EnsureRootSkillExists()
    {
        var center = new Vector2Int(SkillTreeConstants.GridSize / 2, SkillTreeConstants.GridSize / 2);

        if (!_skillTree.skills.Exists(skill => skill.position == center))
        {
            var rootSkill = new SkillNode
            {
                skillType = SkillType.CriticalChance,
                position = center
            };
            _skillTree.skills.Add(rootSkill);
        }
    }

    private void DrawNodes()
    {
        foreach (var skill in _skillTree.skills)
        {
            // Учитываем scrollPosition при отрисовке
            var position = GridToWorld(skill.position) - _scrollPosition;
            var rect = new Rect(position.x, position.y, CellSize, CellSize);

            // Отображаем иконку, если она есть
            var icon = _skillTree.icons.FirstOrDefault(i => i.type == skill.skillType) ?? null;
            if (icon != null)
            {
                GUI.DrawTexture(new Rect(
                    rect.x + 5, 
                    rect.y + 5, 
                    rect.width - 10, 
                    rect.height - 10
                ), icon.icon.texture);
            }
            
            // Отображение узла
            if (skill.skillType == SkillType.Root) GUI.color = Color.yellow;
            else GUI.color = Color.green;
            GUI.Box(rect, $"{skill.skillType.ToString()} ({skill.value})");
            GUI.color = Color.white;
            
            // Обработка клика по узлу
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 1) // Правая кнопка мыши
                {
                    ShowContextMenu(skill);
                    Event.current.Use();
                }
            }
        }
    }

    private void ProcessMouseEvents()
    {
        if (_selectedSkill != null)
        {
            return;
        }

        // Перемещение окна (Средняя кнопка мыши)
        if (Event.current.type == EventType.MouseDrag && Event.current.button == 2) 
        {
            _scrollPosition -= Event.current.delta; 
            Repaint();
        }
        
        // Создать (ЛКМ)
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            var gridPosition = WorldToGrid(Event.current.mousePosition + _scrollPosition);

            if (_skillTree.skills.Exists(skill => skill.position == gridPosition))
                return; // Узел уже есть в этой позиции

            var newSkill = new SkillNode
            {
                position = gridPosition
            };
            _skillTree.skills.Add(newSkill);
            Repaint();
        }
    }


    private void ShowContextMenu(SkillNode skill)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Edit"), false, () => EditSkill(skill));
        menu.AddItem(new GUIContent("Delete"), false, () => DeleteSkill(skill));
        menu.ShowAsContext();
    }

    private void EditSkill(SkillNode skill)
    {
        _selectedSkill = skill; // Устанавливаем выбранный узел
    }

    private void DeleteSkill(SkillNode skill)
    {
        _skillTree.skills.Remove(skill);
        Repaint();
    }

    private Vector2 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector2(gridPosition.x * CellSize, gridPosition.y * CellSize);
    }

    private Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x / CellSize), Mathf.FloorToInt(worldPosition.y / CellSize));
    }
}

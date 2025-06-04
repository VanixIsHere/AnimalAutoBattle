using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameCreditsEditorWindow : EditorWindow {
    private CreditsDatabase database;
    private Vector2 scrollPos;
    private Dictionary<CreditCategory, bool> foldouts = new Dictionary<CreditCategory, bool>();

    [MenuItem("Window/Game Credits Manager")]
    public static void ShowWindow() {
        GetWindow<GameCreditsEditorWindow>("Game Credits");
    }

    private void OnGUI() {
        EditorGUILayout.Space();
        database = (CreditsDatabase)EditorGUILayout.ObjectField("Credits Database", database, typeof(CreditsDatabase), false);
        if (database == null) {
            EditorGUILayout.HelpBox("Assign a CreditsDatabase asset to manage.", MessageType.Info);
            return;
        }

        if (foldouts.Count == 0) {
            // Initialize foldouts for all categories
            foreach (CreditCategory cat in System.Enum.GetValues(typeof(CreditCategory))) {
                foldouts[cat] = false;
            }
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (CreditCategory category in System.Enum.GetValues(typeof(CreditCategory))) {
            if (!foldouts.ContainsKey(category))
                foldouts[category] = false;

            foldouts[category] = EditorGUILayout.Foldout(foldouts[category], category.ToString(), true);

            if (foldouts[category]) {
                EditorGUI.indentLevel++;

                var entries = database.entries.Where(e => e.category == category).ToList();
                for (int i = 0; i < entries.Count; i++) {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();

                    entries[i].name = EditorGUILayout.TextField("Name", entries[i].name);
                    entries[i].altname = EditorGUILayout.TextField("Alt Name", entries[i].altname);
                    entries[i].type = (CreditType)EditorGUILayout.EnumPopup("Type", entries[i].type);
                    entries[i].roleOrUsage = EditorGUILayout.TextField("Role/Usage", entries[i].roleOrUsage);
                    entries[i].notes = EditorGUILayout.TextArea(entries[i].notes, GUILayout.MinHeight(40));
                    entries[i].imageOrLogo = (Texture2D)EditorGUILayout.ObjectField("Logo", entries[i].imageOrLogo, typeof(Texture2D), false);

                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                        database.entries.Remove(entries[i]);
                        break;
                    }

                    EditorGUILayout.EndVertical();
                }

                // 👇 This will always show even if no entries exist
                if (GUILayout.Button($"Add New Entry to {category}")) {
                    database.entries.Add(new CreditEntry() { category = category });
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.EndScrollView();

        if (GUI.changed) {
            EditorUtility.SetDirty(database);
        }
    }
}

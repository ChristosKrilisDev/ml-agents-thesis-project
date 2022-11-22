using UnityEditor;
using UnityEngine;


namespace ML_Agents.PF.Scripts.RL.Editor
{
    public class TrainingStateWindow : EditorWindow
    {
        /// <summary>
        /// condition list
        ///
        /// </summary>
        private int _prefixLabelWidth = 120;
        private int _inputFieldWidth = 160;

        private static bool _trainingFoldoutValue = true;
        private static bool _nodesFoldoutValue = false;
        // private static bool _graphFoldoutValue = false;

        private static bool _liveUpdate;
        private static string _trainignWindowLiveUpdate = "TrainingWindow_LiveUpdate";
        private Vector2 _scrollPosition;
        private GUIStyle _moraleStyle;
        private int _tempInt;

        private PathFindAgent _agent;
        private PathFindAgent.AgentExpose _agentExpose;

        private static bool LiveUpdate
        {
            get => _liveUpdate;
            set
            {
                EditorPrefs.SetBool("CharacterExposeWindow_LiveUpdate", value);
                _liveUpdate = value;
            }
        }

        [MenuItem("RL/Training Info Window", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<TrainingStateWindow>("Current Training Info");
            _liveUpdate = EditorPrefs.GetBool(_trainignWindowLiveUpdate);
        }

        protected void OnEnable()
        {
            EditorApplication.playModeStateChanged += PauseStateChanged;
            ClearCharacter();
            _liveUpdate = EditorPrefs.GetBool(_trainignWindowLiveUpdate);
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PauseStateChanged;
            ClearCharacter();
            _liveUpdate = EditorPrefs.GetBool(_trainignWindowLiveUpdate);
        }

        private void PauseStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    _liveUpdate = EditorPrefs.GetBool(_trainignWindowLiveUpdate);

                    break;

                case PlayModeStateChange.EnteredEditMode:
                    ClearCharacter();

                    break;
            }
        }

        private void ClearCharacter()
        {
            //ItemsFoldoutValues.Clear();
            //CardsFoldoutValues.Clear();
            //_exposedCharacter = null;
            //_aiSelectedMoves.Clear();
            Repaint();
        }

        private void TryGetCharacter()
        {
            _agent = FindObjectOfType<PathFindAgent>();
            _agentExpose = new PathFindAgent.AgentExpose(_agent);
            Repaint();
        }

        private void Update()
        {
            if (!EditorApplication.isPlaying) return;

            if (_agent == null)
            {
                TryGetCharacter();
            }
            else
            {
                if (LiveUpdate) Repaint();
            }
        }

        protected void OnGUI()
        {
            _inputFieldWidth = (int)position.width / 3;
            _prefixLabelWidth = _inputFieldWidth - 40;

            if(!Application.isPlaying) return;

            if (_agent == null)
            {
                EditorGUILayout.LabelField("NO Agent", EditorStyles.boldLabel);

                if (GUILayout.Button("Get Agent"))
                {
                    TryGetCharacter();

                    return;
                }

                return;
            }

            using var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
            _scrollPosition = scrollViewScope.scrollPosition;

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = _prefixLabelWidth;

            EditorGUILayout.Space();
            Title();
            EditorGUILayout.Space();
            TrainingInfo();
            EditorGUILayout.Space();
            NodesInfo();
            EditorGUILayout.Space();
            // Morality();
            EditorGUILayout.Space();

            //Passage();
            EditorGUILayout.Space();
            CreateIndentedLabel("ConditionsData : ", _agentExpose.ConditionsData.ToString());
            CreateIndentedLabel("RewardDataStruct : ", _agentExpose.RewardDataStruct.ToString());
            CreateIndentedLabel("RewardConditions : ", _agentExpose.RewardConditions.ToString());
            CreateIndentedLabel("RewardDataStructFromT : ", _agentExpose.RewardDataStructFromT.ToString());

            //Deck();
            EditorGUILayout.Space();
            //Inventory();
            EditorGUILayout.Space();
            //AIMoves();
            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = labelWidth;
        }
        private void Title()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{_agentExpose.AgentName}", EditorStyles.boldLabel);
            LiveUpdate = GUILayout.Toggle(LiveUpdate, "Live Update");
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.ObjectField(_agent, typeof(GameObject), false);
            EditorGUILayout.ObjectField(_agentExpose.Area, typeof(GameObject), false);
            EditorGUILayout.ObjectField(_agentExpose.Graph, typeof(GameObject), false);
        }

        private void TrainingInfo()
        {
            BeginVerticalBoxArea();

            _trainingFoldoutValue = EditorGUILayout.Foldout(_trainingFoldoutValue, "-Training-");

            if (_trainingFoldoutValue)
            {
                var index = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                CreateIndentedLabel("Phase : ", _agentExpose.PhaseType.ToString());
                CreateIndentedLabel("Training : ", _agentExpose.TrainingType.ToString());
                CreateIndentedLabel("State Machine Type : ", _agentExpose.TrainingStateMachine.GetType().ToString());
                EditorGUILayout.Space();
                EditorGUI.indentLevel = index;
            }
            EndVerticalBoxArea();
        }

        private void NodesInfo()
        {
            BeginVerticalBoxArea();

            _nodesFoldoutValue = EditorGUILayout.Foldout(_nodesFoldoutValue, "Nodes Info");

            if (_nodesFoldoutValue)
            {
                var index = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                CreateIndentedLabel("Target Index : ", _agentExpose.TargetIndex.ToString());
                EditorGUILayout.ObjectField(_agentExpose.CurrentTarget, typeof(GameObject), false);

                foreach (var node in _agentExpose.NodesToFind)
                {
                    EditorGUILayout.ObjectField(node, typeof(GameObject), false);
                }


                EditorGUI.indentLevel = index;
            }
            EndVerticalBoxArea();
        }


        // private void Morality()
        // {
        //     BeginVerticalBoxArea();
        //
        //     _graphFoldoutValue = EditorGUILayout.Foldout(_graphFoldoutValue, "Graph Info");
        //
        //     if (_graphFoldoutValue)
        //     {
        //         var index = EditorGUI.indentLevel;
        //         EditorGUI.indentLevel++;
        //
        //         EditorGUILayout.BeginHorizontal(_moraleStyle);
        //         CreateMoraleColumn(_agentExpose.Graph.Nodes, typeof(Sin), "Sins");
        //         CreateMoraleColumn(_exposedCharacter.Virtues, typeof(Virtue), "Virtues");
        //         EditorGUILayout.EndHorizontal();
        //
        //         EditorGUI.indentLevel = index;
        //     }
        //     EndVerticalBoxArea();
        // }


        // private void CreateMoraleColumn(int[] moraleValues, Type moraleType, string moraleName)
        // {
        //     EditorGUILayout.BeginVertical();
        //     EditorGUILayout.LabelField(moraleName, EditorStyles.boldLabel);
        //
        //     for (var index = 0; index < moraleValues.Length; index++)
        //     {
        //         if (CreateIntField(Enum.GetName(moraleType, index), moraleValues[index], out _tempInt))
        //         {
        //             if (moraleType == typeof(Sin)) _exposedCharacter.SetSin((Sin) index, _tempInt);
        //             else if (moraleType == typeof(Virtue)) _exposedCharacter.SetVirtue((Virtue) index, _tempInt);
        //         }
        //     }
        //     EditorGUILayout.EndVertical();
        // }


        private void CreateIndentedLabel(string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(label, value, EditorStyles.boldLabel, GUILayout.Width(_inputFieldWidth + 48));
            EditorGUILayout.EndHorizontal();
        }

        private bool CreateIntField(string label, int value, out int newValue)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            newValue = EditorGUILayout.IntField(label, value, GUILayout.Width(_inputFieldWidth));
            bool hasChanged = EditorGUI.EndChangeCheck();

            GUILayout.Space(4);

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                newValue -= 1;
                hasChanged = true;
            }

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                newValue += 1;
                hasChanged = true;
            }

            EditorGUILayout.EndHorizontal();

            return hasChanged;
        }


        private void BeginVerticalBoxArea()
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical("box");
        }

        private void EndVerticalBoxArea()
        {
            EditorGUILayout.Space();
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DisplayIcon(Sprite sprite, float multiplier = 1, float xDistance = 0)
        {
            var myRectPos = GUILayoutUtility.GetLastRect();
            myRectPos.x += 25 + myRectPos.width + xDistance;
            myRectPos.width = multiplier * 50;
            myRectPos.height = myRectPos.width * ((float)sprite.texture.height / sprite.texture.width);
            UnityEngine.GUI.DrawTexture(myRectPos, sprite.texture, ScaleMode.StretchToFill);
            EditorGUILayout.Space(myRectPos.height);
        }

        // private void CreateMoraleColumn(int[] moraleValues, Type moraleType, string moraleName)
        // {
        //     EditorGUILayout.BeginVertical();
        //     EditorGUILayout.LabelField(moraleName, EditorStyles.boldLabel);
        //
        //     for (var index = 0; index < moraleValues.Length; index++)
        //     {
        //         if (CreateIntField(Enum.GetName(moraleType, index), moraleValues[index], out _tempInt))
        //         {
        //             if (moraleType == typeof(Sin)) _exposedCharacter.SetSin((Sin) index, _tempInt);
        //             else if (moraleType == typeof(Virtue)) _exposedCharacter.SetVirtue((Virtue) index, _tempInt);
        //         }
        //     }
        //     EditorGUILayout.EndVertical();
        // }

    }
}

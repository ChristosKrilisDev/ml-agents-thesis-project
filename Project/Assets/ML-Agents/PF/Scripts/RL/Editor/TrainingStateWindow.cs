using ML_Agents.PF.Scripts.StateMachine;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ML_Agents.PF.Scripts.RL.Editor
{
    public class TrainingStateWindow : EditorWindow
    {

        [MenuItem("RL/Training Info Window", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<TrainingStateWindow>("Current Training Info");
            _liveUpdate = EditorPrefs.GetBool(TRAINING_WINDOW_LIVE_UPDATE);
        }

    #region Vars

        private PathFindAgent _agent;
        private PathFindAgent.AgentExpose _agentExpose;

        private static bool _trainingFoldoutValue = true;
        private static bool _nodesFoldoutValue = true;
        // private static bool _graphFoldoutValue = true;
        private static bool _conditionsFoldoutValue = true;
        private static bool _rewardFoldoutValue = true;

        private const string TRAINING_WINDOW_LIVE_UPDATE = "TrainingWindow_LiveUpdate_Key";
        private static bool _liveUpdate = true;

        private Vector2 _scrollPosition;
        private GUIStyle _moraleStyle;
        private int _tempInt;
        private int _prefixLabelWidth = 120;
        private int _inputFieldWidth = 160;

    #endregion

    #region Control

        private static bool LiveUpdate
        {
            get => _liveUpdate;
            set
            {
                EditorPrefs.SetBool(TRAINING_WINDOW_LIVE_UPDATE, value);
                _liveUpdate = value;
            }
        }
        protected void OnEnable()
        {
            EditorApplication.playModeStateChanged += PauseStateChanged;
            ClearAgent();
            _liveUpdate = EditorPrefs.GetBool(TRAINING_WINDOW_LIVE_UPDATE);
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PauseStateChanged;
            ClearAgent();
            _liveUpdate = EditorPrefs.GetBool(TRAINING_WINDOW_LIVE_UPDATE);
        }

        private void PauseStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    _liveUpdate = EditorPrefs.GetBool(TRAINING_WINDOW_LIVE_UPDATE);

                    break;

                case PlayModeStateChange.EnteredEditMode:
                    ClearAgent();

                    break;
            }
        }

        private void ClearAgent()
        {
            Repaint();
        }

        private void TryGetAgent()
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
                TryGetAgent();
            }
            else
            {
                if (LiveUpdate) Repaint();
            }
        }

    #endregion

    #region GUI Forms

        protected void OnGUI()
        {
            _inputFieldWidth = (int)position.width / 3;
            _prefixLabelWidth = _inputFieldWidth - 40;

            if (!Application.isPlaying) return;

            if (_agent == null)
            {
                EditorGUILayout.LabelField("NO Agent", EditorStyles.boldLabel);

                if (GUILayout.Button("Get Agent"))
                {
                    TryGetAgent();

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
            TrainingInfo();
            GraphInfo();
            ConditionsDataInfo();
            RewardInfo();

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
        }

        private void TrainingInfo()
        {
            BeginVerticalBoxArea();

            _trainingFoldoutValue = EditorGUILayout.Foldout(_trainingFoldoutValue, "-Training Info-");

            if (_trainingFoldoutValue)
            {
                var index = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                CreateIndentedLabel("Phase : ", _agentExpose.PhaseType.ToString());
                CreateIndentedLabel("Training : ", _agentExpose.TrainingType.ToString());

                if (_agentExpose.TrainingStateMachine.GetType() == typeof(SimpleTraining))
                {
                    CreateIndentedLabel("State Machine Type : ", "Simple");
                }
                else if(_agentExpose.TrainingStateMachine.GetType() == typeof(AdvancedTraining))
                {
                    CreateIndentedLabel("State Machine Type : ", "Advanced");

                }

                EditorGUILayout.Space();
                EditorGUI.indentLevel = index;
            }
            EndVerticalBoxArea();
        }

        private void GraphInfo()
        {
            BeginVerticalBoxArea();

            _nodesFoldoutValue = EditorGUILayout.Foldout(_nodesFoldoutValue, "-Graph Info-");

            if (_nodesFoldoutValue)
            {
                var index = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                CreateObjectFieldWithLabel("Graph ",_agentExpose.Graph);
                EditorGUILayout.Space();

                CreateIndentedLabel("Target Index : ", _agentExpose.TargetIndex.ToString());
                CreateObjectFieldWithLabel("Current Target ",_agentExpose.CurrentTarget);
                EditorGUILayout.Space();

                for (var i = 0; i < _agentExpose.NodesToFind.Length; i++)
                {
                    var node = _agentExpose.NodesToFind[i];
                    CreateObjectFieldWithLabel($"Target-{i+1}", node);
                }

                EditorGUILayout.Space();

                CreateObjectFieldWithLabel("StartNode ",_agentExpose.Graph.StartNode);
                CreateObjectFieldWithLabel("CheckPointNode ",_agentExpose.Graph.CheckPointNode);
                CreateObjectFieldWithLabel("EndNode ",_agentExpose.Graph.EndNode);
                EditorGUILayout.Space();

                var halfPath = _agentExpose.Graph.GetShortestPath(_agentExpose.Graph.StartNode,_agentExpose.Graph.CheckPointNode);
                var fullPath = _agentExpose.Graph.GetShortestPath(_agentExpose.Graph.CheckPointNode,_agentExpose.Graph.EndNode);
                CreateIntendedBigTextField("Half Path", halfPath.ToString());
                CreateIntendedBigTextField("Full Path", fullPath.ToString());

                EditorGUI.indentLevel = index;
            }
            EndVerticalBoxArea();
        }

        private void ConditionsDataInfo()
        {
            BeginVerticalBoxArea();

            _conditionsFoldoutValue = EditorGUILayout.Foldout(_conditionsFoldoutValue, "-Conditions Info-");
            var conditionsData = _agentExpose.ConditionsData;

            if (_conditionsFoldoutValue)
            {
                var index = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();
                CreateIndentedLabel("Max Step : ", conditionsData.MaxStep.ToString());
                CreateIndentedLabel($"Step Count ", $"{conditionsData.StepCount.ToString()}/{conditionsData.MaxStep.ToString()}");
                CreateIndentedLabel("Step Factor : ", conditionsData.StepFactor.ToString("f2"));

                EditorGUILayout.Space();
                CreateIndentedLabel("Half Length : ", conditionsData.CheckPointPathLength.ToString());
                CreateIndentedLabel("Total Length : ", conditionsData.FullPathLength.ToString());

                var tmp = (int)_agentExpose.PhaseType >= 3 ? conditionsData.FullPathLength.ToString() : conditionsData.CheckPointPathLength.ToString();
                CreateIndentedLabel($"Traveled Distance :", $"{conditionsData.TraveledDistance.ToString()}/{tmp}");

                EditorGUILayout.Space();
                CreateIndentedLabel("Found Check point : ", conditionsData.HasFoundCheckpoint.ToString());
                CreateIndentedLabel("Found Goal : ", conditionsData.HasFoundGoal.ToString());
                CreateIndentedLabel("Touched Wall: ", conditionsData.HasTouchedWall.ToString());

                EditorGUI.indentLevel = index;
            }
            EndVerticalBoxArea();
        }

        private void RewardInfo()
        {
            BeginVerticalBoxArea();

            _rewardFoldoutValue = EditorGUILayout.Foldout(_rewardFoldoutValue, "-Reward Data Info-");
            var rewardCondition = _agentExpose.RewardDataStruct.Conditions;

            if (_rewardFoldoutValue)
            {
                var index = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;


                EditorGUILayout.Space();

                if (rewardCondition == null)
                {
                    EditorGUILayout.LabelField("No reward conditions found", EditorStyles.boldLabel);
                }
                else
                {
                    CreateIndentedLabel("Conditions Sizes : ", rewardCondition.Length.ToString());

                    for (int i = 0; i < rewardCondition.Length; i++)
                    {
                        CreateIndentedLabel($"Conditions {i + 1}: ", rewardCondition[i].ToString());
                    }
                }

                EditorGUI.indentLevel = index;
            }
            EndVerticalBoxArea();
        }


    #endregion

    #region GUI Methods

        private void CreateObjectFieldWithLabel(string label, Object obj)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(label, EditorStyles.boldLabel);
            EditorGUILayout.ObjectField(obj, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();

        }

        private void CreateIntendedBigTextField(string label, string path)
        {
            CreateIndentedLabel($"{label} : ", "");
            EditorGUI.indentLevel++;
            var strList = path.Split(',');
            foreach (var str in strList)
            {
                EditorGUILayout.TextField(str, EditorStyles.boldLabel);
            }
            EditorGUI.indentLevel--;
        }

        private void CreateIndentedLabel(string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(label, value, EditorStyles.boldLabel, GUILayout.Width(_inputFieldWidth + 48));
            EditorGUILayout.EndHorizontal();
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

    #endregion

    }
}

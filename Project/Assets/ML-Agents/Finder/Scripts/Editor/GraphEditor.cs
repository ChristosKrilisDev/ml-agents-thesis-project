using UnityEngine;
using UnityEditor;

namespace Dijstra.path
{
    [CustomEditor(typeof(Graph))]
    public class GraphEditor : Editor
    {

        private Graph _graph;
        private Path _startPath = new Path();
        private Path _endPath = new Path();
        private int _length = 0;

        private int _inputFieldWidth = 260;
        private bool _hasValue = false;

        private void OnEnable()
        {
            _graph = target as Graph;
        }

        private void OnSceneGUI()
        {
            //if(!_liveUpdate) return;
            if (_graph == null) return;

            for (var i = 0; i < _graph.nodes.Count; i++)
            {
                var node = _graph.nodes[i];

                for (var j = 0; j < node.connections.Count; j++)
                {
                    var connection = node.connections[j];

                    if (connection == null)
                    {
                        continue;
                    }
                    var distance = Vector3.Distance(node.transform.position, connection.transform.position);
                    //var distance = 1;
                    var centeredTextDiff = connection.transform.position - node.transform.position;

                    // Handles.Label(node.transform.position + diff / 2, distance.ToString("f2"), EditorStyles.whiteBoldLabel);
                    //TODO : Get the distance from path values
                    Handles.Label(node.transform.position + centeredTextDiff / 2, distance.ToString("f2"), EditorStyles.whiteBoldLabel);

                    if (_startPath.PathNodes.Contains(node) && _startPath.PathNodes.Contains(connection))
                        DrawPath(node, connection, 3f, 2.5f, Color.yellow, Color.green);
                    else if (_endPath.PathNodes.Contains(node) && _endPath.PathNodes.Contains(connection))
                        DrawPath(node, connection, 3.5f, 3f, Color.cyan, Color.red);
                    else
                        Handles.DrawLine(node.transform.position, connection.transform.position);
                }
            }
            Repaint();
        }

        private void DrawPath(Node node, Node connection, float lineThickness, float radius, Color circleColor, Color lineColor)
        {
            var nodeTransform = node.transform;

            var tempColor = Handles.color;
            Handles.color = circleColor;
            Handles.DrawWireArc(nodeTransform.position, nodeTransform.up * 5f, -nodeTransform.right, 360, radius, 2.5f);
            Handles.color = tempColor;

            tempColor = Handles.color;
            Handles.color = lineColor;
            Handles.DrawLine(nodeTransform.position, connection.transform.position, lineThickness);
            Handles.color = tempColor;
        }

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
            var hasChanged = EditorGUI.EndChangeCheck();

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

        public override void OnInspectorGUI()
        {
            _graph.nodes.Clear();

            foreach (Transform child in _graph.transform)
            {
                var node = child.GetComponent<Node>();
                if (node != null) _graph.nodes.Add(node);
            }
            base.OnInspectorGUI();

            EditorGUILayout.Separator();

            EditorGUILayout.ObjectField("Start Node", _graph.m_Start, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));
            EditorGUILayout.ObjectField("Check point Node", _graph.m_CheckPoint, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));
            EditorGUILayout.ObjectField("End Node ", _graph.m_End, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));

            if (_hasValue)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.IntField("Start-Check Length", (int)_startPath.length, GUILayout.Width(_inputFieldWidth));
                EditorGUILayout.IntField("Check-End Length", (int)_endPath.length, GUILayout.Width(_inputFieldWidth));
                EditorGUILayout.IntField("Total Length", (int)_length, GUILayout.Width(_inputFieldWidth));
                EditorGUILayout.Space(10);
            }

            if (GUILayout.Button("Show Shortest Path"))
            {
                if (_graph.m_Start == null || _graph.m_End == null)
                {
                    Debug.LogError("From/To nodes are null");
                    _hasValue = false;

                    return;
                }

                _startPath = _graph.GetShortestPath(_graph.m_Start, _graph.m_CheckPoint);
                _endPath = _graph.GetShortestPath(_graph.m_CheckPoint, _graph.m_End);
                _length = (int)_startPath.length + (int)_endPath.length;

                _hasValue = true;

                Debug.Log($" Lengths => Start - Check point : {_startPath.length} | Check point - End : {_endPath.length} | total : {_length}");
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Clear"))
            {
                _startPath = new Path();
                _endPath = new Path();
                _length = 0;
                _hasValue = false;
            }
        }

    }
}

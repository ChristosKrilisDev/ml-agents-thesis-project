using UnityEditor;
using UnityEngine;

namespace Dijkstra.Scripts.Editor
{
    [CustomEditor(typeof(Graph))]
    public class GraphEditor : UnityEditor.Editor
    {
        private Graph _graph;
        private Path _startPath = new Path();
        private Path _endPath = new Path();
        private int _length;

        private readonly int _inputFieldWidth = 260;
        private bool _hasValue = false;

        private void OnEnable()
        {
            _graph = target as Graph;
        }

        private void OnSceneGUI()
        {
            //if(!_liveUpdate) return;
            if (_graph == null) return;

            for (var i = 0; i < _graph.Nodes.Count; i++)
            {
                var node = _graph.Nodes[i];

                for (var j = 0; j < node.Connections.Count; j++)
                {
                    var connection = node.Connections[j];

                    if (connection == null) continue;

                    var distance = Vector3.Distance(node.transform.position, connection.transform.position);
                    var centeredTextDiff = connection.transform.position - node.transform.position;

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
            var nTransform = nodeTransform.position;

            Handles.DrawWireArc(nTransform, nodeTransform.up * 5f, -nodeTransform.right, 360, radius, 2.5f);
            Handles.color = tempColor;

            tempColor = Handles.color;
            Handles.color = lineColor;
            Handles.DrawLine(nTransform, connection.transform.position, lineThickness);
            Handles.color = tempColor;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_graph.IsAbleToVisualizePath)
            {
                VisualizeBakedPath();
                SceneView.RepaintAll();
                _graph.IsAbleToVisualizePath = false;
            }

            if (_hasValue) DrawNodeFields();

            if (GUILayout.Button("Clear"))
            {
                _startPath = new Path();
                _endPath = new Path();
                _length = 0;
                _hasValue = false;
            }
        }

        private void DrawNodeFields()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.ObjectField("Start Node", _graph.StartNode, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));
            EditorGUILayout.ObjectField("Check point Node", _graph.CheckPointNode, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));
            EditorGUILayout.ObjectField("End Node ", _graph.EndNode, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));
            EditorGUILayout.Space(10);
            EditorGUILayout.IntField("Start-Check Length", (int)_startPath.Length, GUILayout.Width(_inputFieldWidth));
            EditorGUILayout.IntField("Check-End Length", (int)_endPath.Length, GUILayout.Width(_inputFieldWidth));
            EditorGUILayout.IntField("Total Length", _length, GUILayout.Width(_inputFieldWidth));
            EditorGUILayout.Space(10);
        }

        public void VisualizeBakedPath()
        {
            if(!Application.isPlaying) return;
            
            if (_graph.StartNode == null || _graph.EndNode == null)
            {
                Debug.LogError("From/To nodes are null");
                _hasValue = false;
                return;
            }

            _startPath = _graph.GetShortestPath(_graph.StartNode, _graph.CheckPointNode);
            _endPath = _graph.GetShortestPath(_graph.CheckPointNode, _graph.EndNode);
            _length = (int)_startPath.Length + (int)_endPath.Length;
            _hasValue = true;
            // Debug.Log($" Lengths => Start - Check point : {_startPath.Length} | Check point - End : {_endPath.Length} | total : {_length}");
        }
    }
}

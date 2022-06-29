using UnityEngine;
using UnityEditor;


namespace Dijstra.path
{
    [CustomEditor(typeof(Graph))]
    public class GraphEditor : Editor
    {

        private Graph m_Graph;
        private Path m_startPath = new Path();
        private Path m_endPath = new Path();
        private int length = 0;


        private int _inputFieldWidth = 260;
        private bool _hasValue = false;


        private void OnEnable()
        {
            m_Graph = target as Graph;
        }


        private void OnSceneGUI()
        {
            //if(!_liveUpdate) return;
            if (m_Graph == null) return;

            for (var i = 0; i < m_Graph.nodes.Count; i++)
            {
                var node = m_Graph.nodes[i];
                for (var j = 0; j < node.connections.Count; j++)
                {
                    var connection = node.connections[j];
                    if (connection == null)
                    {
                        continue;
                    }
                    // var distance = Vector3.Distance(node.transform.position, connection.transform.position);
                    var distance = 1;
                    var centeredTextDiff = connection.transform.position - node.transform.position;

                    // Handles.Label(node.transform.position + diff / 2, distance.ToString("f2"), EditorStyles.whiteBoldLabel);
                    //TODO : Get the distance from path values
                    Handles.Label(node.transform.position + centeredTextDiff / 2, distance.ToString("f2"), EditorStyles.whiteBoldLabel);


                    if (m_startPath.nodes.Contains(node) && m_startPath.nodes.Contains(connection))
                        DrawPath(node, connection, 3f,2.5f , Color.yellow, Color.green);
                    else if (m_endPath.nodes.Contains(node) && m_endPath.nodes.Contains(connection))
                        DrawPath(node, connection, 3.5f ,3f , Color.cyan, Color.red);
                    else
                        Handles.DrawLine(node.transform.position, connection.transform.position);
                }
            }
            Repaint();
        }
        
        private void DrawPath(Node node, Node connection, float lineThickness , float radius, Color circleColor , Color lineColor)
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


        public override void OnInspectorGUI()
        {
            m_Graph.nodes.Clear();
            foreach (Transform child in m_Graph.transform)
            {
                var node = child.GetComponent<Node>();
                if (node != null) m_Graph.nodes.Add(node);
            }
            base.OnInspectorGUI();

            EditorGUILayout.Separator();

            EditorGUILayout.ObjectField("Start Node", m_Graph.m_Start, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));
            EditorGUILayout.ObjectField("Check point Node", m_Graph.m_CheckPoint, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));
            EditorGUILayout.ObjectField("End Node ", m_Graph.m_End, typeof(Node), false, GUILayout.Width(_inputFieldWidth * 1.2f));
            if (_hasValue)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.IntField("Start-Check Length", (int)m_startPath.length, GUILayout.Width(_inputFieldWidth));
                EditorGUILayout.IntField("Check-End Length", (int)m_endPath.length, GUILayout.Width(_inputFieldWidth));
                EditorGUILayout.IntField("Total Length", (int)length, GUILayout.Width(_inputFieldWidth));
                EditorGUILayout.Space(10);
            }

            if (GUILayout.Button("Show Shortest Path"))
            {
                if (m_Graph.m_Start == null || m_Graph.m_End == null)
                {
                    Debug.LogError("From/To nodes are null");
                    _hasValue = false;
                    return;
                }

                m_startPath = m_Graph.GetShortestPath(m_Graph.m_Start, m_Graph.m_CheckPoint);
                m_endPath = m_Graph.GetShortestPath(m_Graph.m_CheckPoint, m_Graph.m_End);
                length = (int)m_startPath.length + (int)m_endPath.length;

                _hasValue = true;

                Debug.Log($" Lengths => Start - Check point : {m_startPath.length} | Check point - End : {m_endPath.length} | total : {length}");
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Clear"))
            {
                m_startPath = new Path();
                m_endPath = new Path();
                length = 0;
                _hasValue = false;
            }
        }

    }
}
